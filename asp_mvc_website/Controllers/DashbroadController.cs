
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using asp_mvc_website.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace asp_mvc_website.Controllers
{
    [Route("admin")]
    public class DashbroadController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public DashbroadController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<OrderModel> order = new List<OrderModel>();
            HttpResponseMessage responseOrder = _client.GetAsync(_client.BaseAddress + "Order/GetAll").Result;

            List<UserModel> user = new List<UserModel>();
            HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/getAllUser").Result;

            List<NotificationUserModel> noti = new List<NotificationUserModel>();
            HttpResponseMessage responseNoti = _client.GetAsync(_client.BaseAddress + "Notification").Result;

            List<ArtworkModel> artworkStatus1 = new List<ArtworkModel>();
            HttpResponseMessage responseStatus1 = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/1").Result;

            List<ArtworkModel> artworkStatus2 = new List<ArtworkModel>();
            HttpResponseMessage responseStatus2 = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/2").Result;

            List<ArtworkModel> artworkStatus3 = new List<ArtworkModel>();
            HttpResponseMessage responseStatus3 = _client.GetAsync(_client.BaseAddress + "Artwork/GetAllArtworkByStatus/3").Result;

            List<PosterModel> post = new List<PosterModel>();
            HttpResponseMessage responsePost = _client.GetAsync(_client.BaseAddress + "Poster").Result;

            if (responseOrder.IsSuccessStatusCode && responseUser.IsSuccessStatusCode && responseNoti.IsSuccessStatusCode
                && responseStatus1.IsSuccessStatusCode && responseStatus2.IsSuccessStatusCode && responseStatus3.IsSuccessStatusCode
                && responsePost.IsSuccessStatusCode
                )
            {
                string dataOrder = responseOrder.Content.ReadAsStringAsync().Result;
                order = JsonConvert.DeserializeObject<List<OrderModel>>(dataOrder);

                string dataUser = responseUser.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<List<UserModel>>(dataUser);

                string dataNoti = responseNoti.Content.ReadAsStringAsync().Result;
                noti = JsonConvert.DeserializeObject<List<NotificationUserModel>>(dataNoti);

                string dataChartStatus1 = responseStatus1.Content.ReadAsStringAsync().Result;
                artworkStatus1 = JsonConvert.DeserializeObject<List<ArtworkModel>>(dataChartStatus1);

                string dataChartStatus2 = responseStatus2.Content.ReadAsStringAsync().Result;
                artworkStatus2 = JsonConvert.DeserializeObject<List<ArtworkModel>>(dataChartStatus2);

                string dataChartStatus3 = responseStatus3.Content.ReadAsStringAsync().Result;
                artworkStatus3 = JsonConvert.DeserializeObject<List<ArtworkModel>>(dataChartStatus3);

                string dataPost = responsePost.Content.ReadAsStringAsync().Result;
                post = JsonConvert.DeserializeObject<List<PosterModel>>(dataPost);

                AdminModel AdminPage = new AdminModel
                {
                    orderModel = order,
                    userModels = user,
                    notificationUserModels = noti,
                    artModelStatus1 = artworkStatus1,
                    artModelStatus2 = artworkStatus2,
                    artModelStatus3 = artworkStatus3,
                    posterModels = post
                };
                return View(AdminPage);
            }
            return View();
        }
        [HttpGet("getUserRoles")]
        public async Task<IActionResult> UserListRole(Helpers.DefaultSearch defaultSearch)
        {
            HttpResponseMessage response = null;
            if (defaultSearch.currentPage != 0 || defaultSearch.perPage != 10)
            {
                response = await _client.GetAsync(_client.BaseAddress + $"admin/getUserRole?perPage={defaultSearch.perPage}&currentPage={defaultSearch.currentPage}");
            }
            else
            {
                response = await _client.GetAsync(_client.BaseAddress + $"admin/getUserRole?");
            }

            string data = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserRolseResponse>(data);

            var responseRoleList = await _client.GetAsync(_client.BaseAddress + "admin/getRole");
            var roles = await responseRoleList.Content.ReadAsStringAsync() ;
            var rolesList = JsonConvert.DeserializeObject<List<RolesVM>>(roles);

            List<string> roleNames = new List<string>();
            foreach(var role in rolesList)
            {
                roleNames.Add(role.name);
            }
            var rolesNameList = roleNames.ToArray<string>();
            var roleName = new RolesNameVM();
            roleName.ListRole = new SelectList(rolesNameList);

            var page = result.total / 10;
            if (result.total % 10 != 0) page++;
            ViewData["listRole"] = roleName;
            ViewData["totalPage"] = page;
            ViewData["currentPage"] = result.page;
            return View(result);
        }

        [HttpPost("AddUserRole")]
        public async Task<IActionResult> AddUserRole([FromBody] UserRolesCM model)
        {
            var response = await _client.PostAsync(_client.BaseAddress + $"admin/addUserRole?userId={model.userId}",
                new StringContent(JsonConvert.SerializeObject(model.roleName), Encoding.UTF8, "application/json"));
            string data = response.Content.ReadAsStringAsync().Result;
            //var result = JsonConvert.DeserializeObject<UserRolseResponse>(data); 
            return Ok(data);
        }


        [HttpPost("ChangeStatusUser")]
        public async Task<IActionResult> ChangeStatusUser([FromBody] UserCM model)
        {
            var response = await _client.PostAsync(_client.BaseAddress + $"admin/changeUserStatus?userId={model.userId}", null);
      
            string data = response.Content.ReadAsStringAsync().Result;
            //var result = JsonConvert.DeserializeObject<UserRolseResponse>(data); 
            return Ok(data);
        }

    }
}
