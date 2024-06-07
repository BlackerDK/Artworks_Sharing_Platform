using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace asp_mvc_website.Controllers
{
	public class ProfileController : Controller
	{
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly ICurrentUserService _currentUserService;

        public ProfileController(IConfiguration configuration,
            IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }

        public async Task<IActionResult> Index(string id)
		{
			var user = await _currentUserService.User();
			ProfileModel userModel = new ProfileModel();
			HttpResponseMessage responseUser = _client.GetAsync(_client.BaseAddress + "User/GetUserById/" + id).Result;
			if (responseUser.IsSuccessStatusCode)
			{
				string data = responseUser.Content.ReadAsStringAsync().Result;
				userModel = JsonConvert.DeserializeObject<ProfileModel>(data);
			}
			ViewData["currentUserId"] = user.Id.ToString();
			userModel.UserId = id;
			if(user.Id.ToString() == id)
			{
				HttpResponseMessage responseUserPoster = _client.GetAsync(_client.BaseAddress + "Poster/" + id).Result;
				if (responseUserPoster.IsSuccessStatusCode)
				{
					string dataPost = responseUserPoster.Content.ReadAsStringAsync().Result;
					userModel.poster = JsonConvert.DeserializeObject<PosterModel>(dataPost);
				}
			}
			return View(userModel);
		}
		[HttpPost]
		public async Task<IActionResult> LikeArtwork([FromBody]LikeModel like)
		{
			try
			{
				var userId = like.UserId;
				HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "Like/CreateLike", new StringContent(
					JsonConvert.SerializeObject(like),
					Encoding.UTF8,
					"application/json"));
				if (response.IsSuccessStatusCode)
				{
					return Ok(new {status = true});
				}
			}
			catch (Exception ex)
			{
				View(ex);
			}
			return View("Index");
		}
		[HttpPost]
		public async Task<IActionResult> CommentArt([FromBody] CommentModel cmt)
		{
			try
			{
				var userId = cmt.UserId;
				HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "Comment/CreateComment", new StringContent(
					JsonConvert.SerializeObject(cmt),
					Encoding.UTF8,
					"application/json"));
				if (response.IsSuccessStatusCode)
				{
					return Ok(new { status = true });
				}
			}
			catch (Exception ex)
			{
				View(ex);
			}
			return View("Index");
		}

	}
}
