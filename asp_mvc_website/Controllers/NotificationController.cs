using asp_mvc_website.Enums;
using asp_mvc_website.Models;
using asp_mvc_website.Services;
using Firebase.Auth;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace asp_mvc_website.Controllers
{
	public class NotificationController : Controller
	{
		private readonly ILogger<NotificationController> _logger;
		private readonly IHttpClientFactory _factory;
		private readonly HttpClient _client;
		private readonly ICurrentUserService _currentUserService;

		public NotificationController(ILogger<NotificationController> logger, IConfiguration configuration,
			IHttpClientFactory httpClientFactory, ICurrentUserService currentUserService)
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
		public async Task<IActionResult> Index(string id)
		{
			var user = await _currentUserService.User();
			if (user == null)
			{
				return Redirect("/User/Login");
			}
			string userId = user.Id.ToString();
			//id = "a88a4533-52da-4b30-b9c5-b259423f14b2";
			var response = await _client.GetAsync(_client.BaseAddress + $"UserNotifcation/getNotiUser?userId={userId}&perPage=10&currentPage=0&isAscending=false");
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				var userNoti = JsonConvert.DeserializeObject<GetUserNoti>(data);
				//userNoti = userNoti.OrderByDescending(noti => noti.dateTime).ToList();

				//    HttpContext.Session.SetString("MyListSessionKey", JsonConvert.SerializeObject(userNoti));
				return View(userNoti);
			}
			return View(null);
		}

		[HttpPut]
		public async Task<HttpResponseMessage> MarkReadNoti(int notificationId)
		{
			try
			{
				var requestUri = $"Notification/MarkReadNoti?id={notificationId}";
				HttpResponseMessage response = await _client.PutAsync(requestUri, null);
				return response;
			}
			catch (Exception ex)
			{
				// Log the exception
				_logger.LogError(ex, "An error occurred while marking notification as read.");
				// Return an error response
				return null;
			}
		}

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> UpdateStatusArtwork(int artworkId)
		{
			ArtworkUpdateDTO dto = new ArtworkUpdateDTO
			{
				ArtworkId = artworkId,
				Status = ArtWorkStatus.Sold
			};
			var response = await _client.PutAsJsonAsync<ArtworkUpdateDTO>(_client.BaseAddress + "Artwork/UpdateArtwork", dto);
			// response.
			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> DenyStatusArtwork(int artworkId)
		{
			ArtworkUpdateDTO dto = new ArtworkUpdateDTO
			{
				ArtworkId = artworkId,
				Status = ArtWorkStatus.SoldPPendingConfirm
			};
			var response = await _client.PutAsJsonAsync<ArtworkUpdateDTO>(_client.BaseAddress + "Artwork/UpdateArtwork", dto);

			return (response.IsSuccessStatusCode, response);
		}

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> UpdateStatusNoti(int notiId, bool isAccept)
		{
			var status = isAccept ? NotiStatus.AcceptOrder : NotiStatus.DenyOrder;

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
			string description = IsAccept ? "Your order has been accepted by the Artist!!" : reason;
			string title = IsAccept ? "Your order has been accepted" : "Your order has been denited";
			NotiStatus noti = IsAccept ? NotiStatus.AcceptOrder : NotiStatus.DenyOrder;

			createNotificationModel model = new createNotificationModel
			{
				Title = title,
				Description = description,
				notiStatus = noti
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

		private async Task<(bool IsSuccess, HttpResponseMessage Response)> PostUserNotification(int artworkId, int notiId, string userIdFor)
		{

			var dto = new CreateUserNotificationDTO
			{
				userId = userIdFor,
				notificationId = notiId,
				artworkId = artworkId,
				userIdFor = null
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
		[HttpPost]
		public async Task<IActionResult> CheckPost([FromBody] CheckPost checkPost)
		{
			if (checkPost.isAccept)
			{
				var updateStatusArtworkResult = await UpdateStatusArtwork(checkPost.artworkId);
				if (!updateStatusArtworkResult.IsSuccess)
					return BadRequest("Update status artwork failed");
				// Create order.
				var userId = checkPost.userId;
				var orderInfo = new OrderCM
				{
					ArtworkId = checkPost.artworkId,
					UserId = userId,
					ReOrderStatus = false,
					Code = ""
				};
				var response = await _client.PostAsync(
					_client.BaseAddress + "Order/CreateOrder",
				 new StringContent(
				 JsonConvert.SerializeObject(orderInfo),
				 Encoding.UTF8,
				 "application/json"));
				if(!response.IsSuccessStatusCode)
				{
					return BadRequest("Create order failed");
				}
			}
			else
			{
				var denyArtwork = await DenyStatusArtwork(checkPost.artworkId);
				if (!denyArtwork.IsSuccess)
					return BadRequest("Update status artwork failed");
			}
			var updateStatusNotiResult = await UpdateStatusNoti(checkPost.notiId, checkPost.isAccept);
			if (!updateStatusNotiResult.IsSuccess)
				return BadRequest("Post notification failed");
			var postNotificationResult = await PostNotification(checkPost.isAccept, checkPost.reason);
			if (!postNotificationResult.IsSuccess)
				return BadRequest("Post notification failed");
			var postUserNotificationResult = await PostUserNotification(checkPost.artworkId, postNotificationResult.NotificationID, checkPost.userId);
			if (!postUserNotificationResult.IsSuccess)
				return BadRequest("Post user notification failed");
			var markRead = await MarkReadNoti(checkPost.notiId);
			if (markRead.IsSuccessStatusCode)
			{
				return Json("true");
			}
			else {
				return BadRequest();
			}
		}
	}
}
