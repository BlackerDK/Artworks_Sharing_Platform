using asp_mvc_website.Enums;
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _factory;
        private readonly ICurrentUserService _currentUserService;
        private CartService cartService;
        private readonly string _cartCookieName = "CartItems";
        public CartController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService, IConfiguration configuration)
        {
            _factory = httpClientFactory;
            _logger = logger;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            //_client.BaseAddress = new Uri("https://localhost:7021/api/");
            //_client.BaseAddress = new Uri("https://apiartwork.azurewebsites.net/api/");
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return Redirect("/User/Login");
            }

            var response = await _client.GetAsync(_client.BaseAddress + "Cart/getCart");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CartResponseVM>(data);
                return View(result);
            }
            //var orderItem = GetOrderByUser(userId);
            //var cartItems = GetCartItem();
            //cartItems = RefreshCartItem(cartItems, orderItem);

            //cartService = new CartService(cartItems);
            //ViewBag.TotalPrice =  cartService.CalculateTotalPrice();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCM addToCartCM)
        {
            var user = await _currentUserService.User();
            addToCartCM.UserId = user.Id.ToString();
            var response = await _client.PostAsync(_client.BaseAddress + "Cart/addToCart", new StringContent(
                    JsonConvert.SerializeObject(addToCartCM),
                    Encoding.UTF8,
                    "application/json"));
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                TempData["DupplicateId"] = "The artwork is already in the cart";
            }
            return Ok(response);
        }

        //     [HttpGet]
        //     public IActionResult AddToCart(string id)
        //     {
        //var userId = HttpContext.Session.GetString("UserId");
        //if (userId == null)
        //{
        //	return Redirect("/User/Login");
        //}
        //ArtworkModel artworkModel = new ArtworkModel();
        //         HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/" + id).Result;
        //         if (response.IsSuccessStatusCode)
        //         {
        //             string data = response.Content.ReadAsStringAsync().Result;
        //             artworkModel = JsonConvert.DeserializeObject<ArtworkModel>(data);

        //	var IsExisted = AddArtworkToCart(artworkModel);
        //             if (IsExisted)
        //             {
        //                 TempData["DupplicateId"] = "The artwork is already in the cart";
        //             }
        //         }
        //         return Redirect("/Shop");
        //     }

        private bool AddArtworkToCart(ArtworkModel artworkModel)
        {
            var cartItems = GetCartItem();

            cartService = new CartService(cartItems);

            bool IsExisted = cartService.IsDuplicateArtworkId(cartItems, artworkModel.artworkId);
            if (IsExisted)
            {
                return true;
            }
            // Add the new item to the cart
            cartItems.Add(new CartItemModel
            {
                artworkId = artworkModel.artworkId,
                Title = artworkModel.Title,
                Price = artworkModel.Price,
                Image = artworkModel.Image
            });

            SaveCartToCookie(cartItems);

            return false;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteArtwork(string Id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return Redirect("/User/Login");
            }
            
            var responseDelete = await _client.DeleteAsync(_client.BaseAddress + $"Cart/removeCartItem?artworkId={Id}");

			return RedirectToAction("Index");
		}

        [HttpGet]
        public async Task<IActionResult> CheckOut(int artworkId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return Redirect("/User/Login");
            }
            ArtworkModel artworkModel = new ArtworkModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetById/" + artworkId).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkModel = JsonConvert.DeserializeObject<ArtworkModel>(data);
                if (artworkModel != null)
                {
                    if (artworkModel.Status == ArtWorkStatus.SoldPPendingConfirm
                        || artworkModel.Status == ArtWorkStatus.Sold)
                    {

                        ViewData["ErrorMsg"] = "Cannot Buy";

                        return RedirectToAction("Index");
                    }
                }
                ArtworkUpdateDTO artworkUpdate = new ArtworkUpdateDTO
                {
                    ArtworkId = artworkId,
                    Status = ArtWorkStatus.SoldPPendingConfirm
                };

                // Send registration request to Web API
                //            var responseUpdateArtwork = await _client.PutAsync(
                //			_client.BaseAddress + "Artwork/UpdateArtwork",
                //			new StringContent(
                //				JsonConvert.SerializeObject(artworkUpdate),
                //				Encoding.UTF8,
                //				"application/json"
                //));

                var responseUpdateArtwork = await _client.PutAsJsonAsync<ArtworkUpdateDTO>(_client.BaseAddress + "Artwork/UpdateArtwork", artworkUpdate);
                if (responseUpdateArtwork.IsSuccessStatusCode)
                {
                        //Create notify to artist                       
                        string description = "Your artwork is being purchased by Customer!!!";
                        string title = "You have a order";

                        createNotificationModel model = new createNotificationModel
                        {
                            Title = title,
                            Description = description,
                            notiStatus = NotiStatus.Order
                        };
                        response = await _client.PostAsync(
                          _client.BaseAddress + "Notification/CreateNotification",
                          new StringContent(
                              JsonConvert.SerializeObject(model),
                              Encoding.UTF8,
                              "application/json"));
                        if (!response.IsSuccessStatusCode)
                            return BadRequest();
                        data = response.Content.ReadAsStringAsync().Result;
                        var notiResponse = JsonConvert.DeserializeObject<ResponseNotificationDTO>(data);
                        // NotiId was created
                        var notiId = notiResponse.Data.Id;

                        var userNoti = new CreateUserNotificationDTO
                        {
                            userId = artworkModel.UserId,
                            notificationId = notiId,
                            artworkId = artworkId,
                            userIdFor = userId
                        };
                        response = await _client.PostAsync(
                          _client.BaseAddress + "UserNotifcation/CreateNotification",
                          new StringContent(
                              JsonConvert.SerializeObject(userNoti),
                              Encoding.UTF8,
                              "application/json"));
                        if (!response.IsSuccessStatusCode)
                        {
                            return BadRequest();
                        }
                        // Return a success response or redirect to the cart pa
                        return RedirectToAction("Index");
                    }
            }
            return RedirectToAction("Index");
        }

        private List<CartItemModel> GetCartItem()
        {
            var settings = new JsonSerializerSettings
            {
                // Provide default value for nullable enum property
                DefaultValueHandling = DefaultValueHandling.Populate,
                NullValueHandling = NullValueHandling.Ignore
            };

            // Retrieve cart items from session or cookie
            var cartItemsJson = HttpContext.Request.Cookies.ContainsKey(_cartCookieName)
                ? HttpContext.Request.Cookies[_cartCookieName]
                : null;

            var cartItems = cartItemsJson != null
            ? JsonConvert.DeserializeObject<List<CartItemModel>>(cartItemsJson, settings)
    : new List<CartItemModel>();

            return cartItems;
        }

        private List<CartItemModel> RefreshCartItem(List<CartItemModel> listCartItems, List<OrderModel> listOrderItem)
        {
            //If have no item will return
            if (listCartItems.Count == 0 || listOrderItem.Count == 0)
            {
                return listCartItems;
            }

            //Remove item that have been ordered 
            foreach (var item in listCartItems)
            {
                var removeItem = listOrderItem.Where(a => a.ArtworkId == item.artworkId).FirstOrDefault();
                if (removeItem != null)
                {
                    listCartItems.Remove(item);
                }
            }


            return listCartItems;
        }

        private void SaveCartToCookie(List<CartItemModel> cartItems)
        {
            var cartJson = JsonConvert.SerializeObject(cartItems);
            HttpContext.Response.Cookies.Append(_cartCookieName, cartJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(7) // Adjust expiration as needed
            });
        }

        private List<OrderModel> GetOrderByUser(string userId)
        {
            List<OrderModel> order = new List<OrderModel>();
            HttpResponseMessage responseOrder = _client.GetAsync(_client.BaseAddress + "Order/GetOrderByUser/" + userId).Result;
            if (responseOrder.IsSuccessStatusCode)
            {
                string dataOrder = responseOrder.Content.ReadAsStringAsync().Result;
                order = JsonConvert.DeserializeObject<List<OrderModel>>(dataOrder);
            }
            return order;
        }


    }
}
