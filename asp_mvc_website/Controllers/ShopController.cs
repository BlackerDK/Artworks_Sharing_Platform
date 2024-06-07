using asp_mvc_website.Enums;
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net;
using System.Text;




namespace asp_mvc_website.Controllers
{
    public class ShopController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;
        private int pageSize = 15;

        public ShopController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }

        private static string ApiKey = "AIzaSyB8sC0Z0tEfdI-1z-KHp7N25OJYBw8d1XU";
        private static string Bucket = "asp-system-57914.appspot.com";
        private static string AuthEmail = "thaieve@gmail.com";
        private static string AuthPassword = "thaieve@123";

        [HttpGet]
        public IActionResult Index()
        {
            //get list artwork
            List<ArtworkModel> artworkList = new List<ArtworkModel>();
            int page = 1;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/2").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);

                ////get 12 first artwork
                //artworkList = artworkList.Take(15).ToList();
                int totalItems = artworkList.Count;
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page; ViewBag.IsPreviousDisabled = (page == 1);
                ViewBag.IsNextDisabled = (page == totalPages);

                artworkList = artworkList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            }
            List<CategoryModel> categoryList = new List<CategoryModel>();
            HttpResponseMessage responseCategory = _client.GetAsync(_client.BaseAddress + "Catalogy").Result;
            if (responseCategory.IsSuccessStatusCode)
            {
                string data = responseCategory.Content.ReadAsStringAsync().Result;
                categoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
            }
            HomeModel homeModel = new HomeModel();
            homeModel.ArtworkList = artworkList;
            homeModel.CategoryList = categoryList;

            if (TempData["DupplicateId"] != null)
            {
                ViewBag.DupplicateId = TempData["DupplicateId"];
            }

            var user = _currentUserService.User().Result;
            if (user != null)
            {
                HttpResponseMessage checkPoster = _client.GetAsync(_client.BaseAddress + "Poster/" + user.Id).Result;
                if (checkPoster.IsSuccessStatusCode)
                {
                    homeModel.IsPoster = "IsPoster";
                }
            }


            return View(homeModel);
        }


        public IActionResult _searchArtwork(string txtSearch, int idCategory, int sortBy , int page)
        {
            // Get list of artwork
            List<ArtworkModel> artworkList = new List<ArtworkModel>();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/2").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                artworkList = JsonConvert.DeserializeObject<List<ArtworkModel>>(data);

                if (!string.IsNullOrEmpty(txtSearch))
                {
                    artworkList = artworkList.Where(x => x.Description.ToLower().Contains(txtSearch.ToLower()) ||
                                                         x.Title.ToLower().Contains(txtSearch.ToLower())).ToList();
                }
                if (idCategory > 0)
                {
                    artworkList = artworkList.Where(x => x.categories.Contains(idCategory)).ToList();
                }

                if (sortBy == 1)
                {
                    artworkList = artworkList.OrderBy(x => x.Title).ToList();
                }
                else if (sortBy == 2)
                {
                    artworkList = artworkList.OrderByDescending(x => x.Price).ToList();
                }
                else if (sortBy == 3)
                {
                    artworkList = artworkList.OrderBy(x => x.CreateOn).ToList();
                }
            }

            int totalItems = artworkList.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.TotalItems = totalPages;
            ViewBag.CurrentPage = page;

            if (artworkList.Count == 0)
            {
                ViewBag.Message = "No artwork found";
            }

            artworkList = artworkList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(artworkList);
        }


        [HttpPost]
        public  async Task<IActionResult> Post(PostArtworkModel model)
        {

            var user = await _currentUserService.User();
            var userId = user.Id.ToString();

            var categoryPostResult = await PostCategoryIfNew(model);
            if (!categoryPostResult.IsSuccess)
                return StatusCode(500, "Cannot create new Category");

            var artworkPostResult = await PostArtwork(model, userId, categoryPostResult.CategoryId);
            if (!artworkPostResult.IsSuccess)
                return StatusCode(500, "Cannot post new Artwork");

            var decsResult = await DecsQuantityPoster(userId);
            if (!decsResult.IsSuccessStatusCode)
                return StatusCode(500, "Error when decs quantity post of poster");

            var notiResult = await PostNotification(model);
            if (!notiResult.IsSuccess)
                return StatusCode(500, "Error when create new notification");

            var userNotiResult = await PostUserNotification(artworkPostResult.artworkId, notiResult.NotificationID);
			if (!userNotiResult.IsSuccess)
                return StatusCode(500, "Error when create new user notification");

            //ViewBag.PostSuccess = true;
            return Ok(new { success = true });
        }

        private async Task<(bool IsSuccess, int CategoryId)> PostCategoryIfNew(PostArtworkModel model)
        {
            if (!model.IsNewCategory)
                return (true, int.Parse(model.Category));

            var caDto = new CategoryAddDTO
            {
                Name = model.Category,
                Status = false
            };

            var response = await _client.PostAsync(
                _client.BaseAddress + "Catalogy",
                new StringContent(
                    JsonConvert.SerializeObject(caDto),
                    Encoding.UTF8,
                    "application/json"));

            if (!response.IsSuccessStatusCode)
                return (false, 0);

            var data = response.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<ResponseCategoryDTO>(data);

            return (true, dto.Data.id);
        }
        private async Task<(bool IsSuccess, int artworkId)> PostArtwork(PostArtworkModel model, string userId, int categoryId)
        {
            var imageUrl = ImageURL(model.File);

            var PDto = new PostArtworkDTO
            {
                Title = model.Title,
                Description = model.Description,
                Price = (double)model.Price,
                UserId = userId,
                ReOrderQuantity = 0,
                ImagesUrl = new List<string> { imageUrl },
                CategoryIds = new List<int> { categoryId }
            };

            var postArtwork = await _client.PostAsync(
                _client.BaseAddress + "Artwork/AddArtwork",
                new StringContent(
                    JsonConvert.SerializeObject(PDto),
                    Encoding.UTF8,
                    "application/json"));

            var data = postArtwork.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<ResponseArtworkDTO>(data);
            return (postArtwork.IsSuccessStatusCode, dto.Data.artworkId);
        }
        private async Task<HttpResponseMessage> DecsQuantityPoster(string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "Poster/DecreasePost?userId=" + userId);
            return await _client.SendAsync(request);
        }

        private async Task<(bool IsSuccess, int NotificationID)> PostNotification(PostArtworkModel model)
        {
            string title = "New Artwork";
            if (model.IsNewCategory)
            {
                title = "New Artwork with Custom New Categogy";
            }
             

            var notiDto = new createNotificationModel
            {
                Title = title,
                Description = "New Artwork has been posted",
                notiStatus = NotiStatus.ConfirmPost
            };
            var response = await _client.PostAsync(
                _client.BaseAddress + "Notification/CreateNotification",
                new StringContent(
                    JsonConvert.SerializeObject(notiDto),
                    Encoding.UTF8,
                    "application/json"));
            if(!response.IsSuccessStatusCode)
                return (false, 0);
            var data = response.Content.ReadAsStringAsync().Result;
            var dto = JsonConvert.DeserializeObject<ResponseNotificationDTO>(data);
            return (true, dto.Data.Id);
        }
        private async Task<(bool IsSuccess, HttpResponseMessage Response)> PostUserNotification(int artworkId, int notiId)
        {
            CreateAdminNotificationDTO dto = new CreateAdminNotificationDTO
            {
                ArtworkId = artworkId,
                NotificationId = notiId
            };
            var response = await _client.PostAsync(
                _client.BaseAddress + "UserNotifcation/AddNotiForadmin",
                new StringContent(
                    JsonConvert.SerializeObject(dto),
                    Encoding.UTF8,
                    "application/json"));
            
            return (response.IsSuccessStatusCode, response);

        }

        private async Task<string> UploadImageToFirebase(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                    var cancellation = new CancellationTokenSource();

                    var task = new FirebaseStorage(
                        Bucket,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                            ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                        })
                        .Child("assets")
                        .Child($"{file.FileName}")
                        .PutAsync(ms, cancellation.Token);

                    try
                    {
                        return await task;
                    }
                    catch (Exception ex)
                    {
                        // Handle exception if necessary
                        ViewBag.error = $"Exception was thrown: {ex}";
                        return null;
                    }
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please select a file to upload.");
                return null;
            }
        }

        private string ImageURL(IFormFile file)
        {
            return UploadImageToFirebase(file).GetAwaiter().GetResult();
        }


    }
}
