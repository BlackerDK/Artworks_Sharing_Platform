using asp_mvc_website.Enums;
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace asp_mvc_website.Controllers
{
    public class CheckArtworkController : Controller
    {
		private readonly IHttpClientFactory _factory;
		private readonly HttpClient _client;
		private readonly ICurrentUserService _currentUserService;

		public CheckArtworkController(IConfiguration configuration,
			IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
		{
			_factory = httpClientFactory;
			_client = new HttpClient();
			_currentUserService = currentUserService;
			_client = _factory.CreateClient("ServerApi");
			_client.BaseAddress = new Uri(configuration["Cron:localhost"]);
		}

		[HttpGet]
		public async Task<IActionResult> Index()
        {
			int itemsPerPage = 10;
			var user = await _currentUserService.User();
			if (user == null)
			{
				return Redirect("/User/Login");
			}
			string userId = user.Id.ToString();
			Result result = new Result();
			List<GetUserNotificationDTO1> dto = new List<GetUserNotificationDTO1>();


			HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"UserNotifcation/getNotiUser?userId={userId}");

			if (response.IsSuccessStatusCode)
			{
                var data = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<Result>(data);
            }

			foreach(var item in result.data)
			{
				if (item.NotificationVM.notiStatus == NotiStatus.ConfirmPost)
				{
					dto.Add(item);

				}
			}

			int total = dto.Count;
			int totalPage = (int)Math.Ceiling((double)total / itemsPerPage);

			int page = 1;

			page = Math.Max(1, Math.Min(page, totalPage));


			int startIndex = (page - 1) * itemsPerPage;
			int endIndex = Math.Min(startIndex + itemsPerPage - 1, total - 1);

			List<GetUserNotificationDTO1> paginatedDto = dto.GetRange(startIndex, endIndex - startIndex + 1);

			ViewData["totalPage"] = totalPage;
			ViewData["currentPage"] = page;

			return View(paginatedDto);
        }
		[HttpGet]
		public async Task<IActionResult> Select (NotiStatus status, int page)
		{
			int itemsPerPage = 10;
			var user = await _currentUserService.User();
			string userId = user.Id.ToString();
			Result result = new Result();
			List<GetUserNotificationDTO1> dto = new List<GetUserNotificationDTO1>();

			HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"UserNotifcation/getNotiUser?userId={userId}");

			if (response.IsSuccessStatusCode)
			{
				var data = response.Content.ReadAsStringAsync().Result;
				result = JsonConvert.DeserializeObject<Result>(data);
			}
			foreach (var item in result.data)
			{
				if (item.NotificationVM.notiStatus == status)
				{
					dto.Add(item);

				}
			}

			int total = dto.Count;
			int totalPage = (int)Math.Ceiling((double)total / itemsPerPage);

			page = Math.Max(1, Math.Min(page, totalPage));

			int startIndex = (page - 1) * itemsPerPage;
			int endIndex = Math.Min(startIndex + itemsPerPage - 1, total - 1);

			List<GetUserNotificationDTO1> paginatedDto = dto.GetRange(startIndex, endIndex - startIndex + 1);

			ViewData["totalPage"] = totalPage;
			ViewData["currentPage"] = page;

			return View(paginatedDto);
		}



		[HttpPost]
		public async Task<IActionResult> CheckPost(int artworkId, bool isAccept, int notiId, string userId, string reason)
		{
			if (isAccept)
			{
				var updateStatusArtworkResult = await UpdateStatusArtwork(artworkId);
				if (!updateStatusArtworkResult.IsSuccess)
					return BadRequest("Update status artwork failed");	
			}
			else
			{
				var increaseQuantityPosterResult = await IncreaseQuantityPoster(userId);
				if (!increaseQuantityPosterResult.IsSuccessStatusCode)
					return BadRequest("Increase quantity poster failed");
			}
			var updateStatusNotiResult = await UpdateStatusNoti(notiId, isAccept);
			if (!updateStatusNotiResult.IsSuccess)
				return BadRequest("Post notification failed");
			var postNotificationResult = await PostNotification(isAccept,reason);
			if (!postNotificationResult.IsSuccess)
				return BadRequest("Post notification failed");
			var postUserNotificationResult = await PostUserNotification(artworkId, postNotificationResult.NotificationID, userId);
			if (!postUserNotificationResult.IsSuccess)
				return BadRequest("Post user notification failed");

            return Ok(new { success = true });
        }

        private async Task<(bool IsSuccess, HttpResponseMessage Response)> UpdateStatusArtwork(int artworkId)
		{
			ArtworkUpdateDTO dto = new ArtworkUpdateDTO
			{
				ArtworkId = artworkId,
				Status = ArtWorkStatus.InProgress
			};
			var response = await _client.PutAsJsonAsync<ArtworkUpdateDTO>(_client.BaseAddress + "Artwork/UpdateArtwork", dto);

			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> UpdateStatusNoti(int notiId, bool isAccept)
		{
			var status = isAccept ? NotiStatus.AcceptPost : NotiStatus.DenyPost;

			UpdateNotiStatusDTO dto = new UpdateNotiStatusDTO
			{
				Id = notiId,
				notiStatus = status
			};
			var response = await _client.PutAsJsonAsync<UpdateNotiStatusDTO>(_client.BaseAddress + "Notification/UpdateStatusNoti", dto);

			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, int NotificationID)> PostNotification(bool IsAccept, string reason)
		{
			string description = IsAccept ? "Your artwork has been accepted" : reason;
			string title = IsAccept ? "Accept post artwork" : "Deny post artwork";

			createNotificationModel model = new createNotificationModel
			{
				Title = title,
				Description = description,
				notiStatus = NotiStatus.Normal
			};
			var response = await _client.PostAsync(
			   _client.BaseAddress + "Notification/CreateNotification",
			   new StringContent(
				   JsonConvert.SerializeObject(model),
				   Encoding.UTF8,
				   "application/json"));
			if (!response.IsSuccessStatusCode)
				return (false, 0);
			var data = response.Content.ReadAsStringAsync().Result;
			var dto = JsonConvert.DeserializeObject<ResponseNotificationDTO>(data);
			return (true, dto.Data.Id);
		}
		private async Task<(bool IsSuccess, HttpResponseMessage Response)> PostUserNotification(int artworkId, int notiId, string userId)
		{

			var dto = new CreateUserNotificationDTO
			{
				userId = userId,
				notificationId = notiId,
				artworkId = artworkId
			};
			var response = await _client.PostAsync(
			   _client.BaseAddress + "UserNotifcation/CreateNotification",
			   new StringContent(
				   JsonConvert.SerializeObject(dto),
				   Encoding.UTF8,
				   "application/json"));

			if (!response.IsSuccessStatusCode)
				return (false, response);
			return (response.IsSuccessStatusCode, response);
		}

		private async Task<HttpResponseMessage> IncreaseQuantityPoster(string userId)
		{
			var request = new HttpRequestMessage(HttpMethod.Put, _client.BaseAddress + "Poster/IncreasePost?userId=" + userId);
			return await _client.SendAsync(request);
		}

	}
}
