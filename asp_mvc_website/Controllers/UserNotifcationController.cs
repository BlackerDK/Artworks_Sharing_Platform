using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace asp_mvc_website.Controllers
{
    public class UserNotifcationController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public UserNotifcationController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }
        public async Task<GetUserNoti> GetNotify(string id)
        {
            var response = await _client.GetAsync(_client.BaseAddress + "UserNotifcation/getNotiUser?userId=" + id);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var userNoti = JsonConvert.DeserializeObject<GetUserNoti>(data);
                return userNoti;

            }
            return null;
        }



    }
}
