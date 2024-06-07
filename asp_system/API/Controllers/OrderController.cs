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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;

        public OrderController(IOrderService orderService, ICurrentUserService currentUserService)
        {
            _orderService = orderService;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO order)
        {
            try
            {
                var currentUser = _currentUserService.GetUserId();
                var result = await _orderService.CreateOrder(order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [Authorize]
        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateDTO order)
        {
            var reponse = await _orderService.UpdateOrder(order);
            if (reponse.IsSuccess)
            {
                return Ok(reponse);
            }
            return BadRequest(reponse);
        }
        [HttpGet("GetOrder{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetOrder(id);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(OrderDeleteDTO order)
        {
            var reponse = await _orderService.DeleteOrder(order);
            if (reponse.IsSuccess)
            {
                return Ok(reponse);
            }
            return BadRequest(reponse);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllOrder()
        {
            var result = await _orderService.GetAllOrder();
            return Ok(result);
        }

		[HttpGet("GetOrderByUser/{userId}")]
		public async Task<IActionResult> GetOrderByUser(string userId)
		{
			var result = await _orderService.GetOrderByUser(userId);
			return Ok(result);
		}
	}
}
