using API.Helper;
using API.Service;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Model;
using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserNotifcationController : ControllerBase
    {
        private readonly IUserNotificationService _userNotificationService;
        private readonly ICurrentUserService _currentUserService;
        public UserNotifcationController(IUserNotificationService notiService, ICurrentUserService currentUserService)
        {
            _userNotificationService = notiService;
            _currentUserService = currentUserService;
        }

        [HttpPost("CreateNotification")]
        public async Task<IActionResult> CreateUserNotification( CreateUserNotificationDTO noti)
        {
            try
            {
				var result = await _userNotificationService.CreateUserNotification(noti);
				return Ok(result);
			}catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("getNotiUser")]
        public async Task<IActionResult> GetNotificationByUserId(string userId,[FromQuery] DefaultSearch defaultSearch)
        {
            var notifications = await _userNotificationService.GetNotiSortResultAsync(userId, defaultSearch);
            var total = _userNotificationService.totalGetNotiUserSortResult(userId);
            if (notifications == null)
            {
                return NotFound(); // or handle as needed
            }

            return Ok(new { total, data = notifications, page = defaultSearch.currentPage });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUserNotification(int id)
        {
            var response = await _userNotificationService.RemoveUserNotification(id);

            if (response.IsSuccess)
            {
                return Ok(response); // 200 OK
            }

            return BadRequest(response); // 400 Bad Request
        }
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> RemoveAllUserNotificationsByUserId(string userId)
        {
            var response = await _userNotificationService.RemoveAllUserNotificationsByUserId(userId);

            if (response.IsSuccess)
            {
                return Ok(response); // 200 OK
            }

            return BadRequest(response); // 400 Bad Request
        }
        [HttpPost("AddNotiForadmin")]
        public async Task<ResponseDTO> AddNoticationForAdmin(CreateAdminNotificationDTO noti)
        {
            try
            {
                var result = await _userNotificationService.AddNoticationForAdmin(noti);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
