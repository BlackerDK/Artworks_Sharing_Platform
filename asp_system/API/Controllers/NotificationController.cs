using API.Service;
using Application.Interfaces.Services;
using Domain.Model;
using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUserService;

        public NotificationController(INotificationService notiService, ICurrentUserService currentUserService)
        {
            _notificationService = notiService;
            _currentUserService = currentUserService;
        }


        //get all
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _notificationService.GetAllNotification();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _notificationService.GetNotificationById(id);
            return Ok(result);
        }
        [HttpPost("CreateNotification")]
        public async Task<IActionResult> CreateNotification(CreateNotificationDTO noti)
        {
            try
            {
                //var userid = _currentUserService.GetUserId();
                var result = await _notificationService.CreateNotification(noti);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("MarkReadNoti")]
        public async Task<IActionResult> MarkReadNoti(int id)
        {
            var reponse = await _notificationService.MarkReadNoti(id);
            if (reponse.IsSuccess)
            {
                return Ok(reponse);
            }
            return BadRequest(reponse);
        }

        [HttpDelete("RemoveNotification/{id}")]
        public async Task<IActionResult> RemoveNotification(int id)
        {
            try
            {
                var result = await _notificationService.RemoveNotification(id);
                if (!result.IsSuccess)
                {
                    return NotFound(); // 404 Not Found if the notification with the given id is not found
                }
                return NoContent(); // 204 No Content indicates successful removal
            }
            catch (Exception ex)
            {
                // Log the exception for further analysis
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut("UpdateStatusNoti")]
        public async Task<IActionResult> UpdateStatusNoti(UpdateNotiStatusDTO dto)
        {
			var reponse = await _notificationService.UpdateStatusNoti(dto);
			if (reponse.IsSuccess)
            {
				return Ok(reponse);
			}
			return BadRequest(reponse);
		}
    }
}
