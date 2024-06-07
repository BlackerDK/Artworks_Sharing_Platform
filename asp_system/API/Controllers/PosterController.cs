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
    public class PosterController : ControllerBase
    {
        private readonly IPosterService _posterService;
        private readonly ICurrentUserService _currentUserService;

        public PosterController(IPosterService posterService, ICurrentUserService currentUserService)
        {
            _posterService = posterService;
            _currentUserService = currentUserService;

        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _posterService.GetAllPoster();
            return Ok(result);
        }
        [HttpPost("AddPoster")]
        public async Task<IActionResult> AddPoster(PosterAddDTO post)
        {
            try
            {
                var result = await _posterService.AddPoster(post);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetPostByUserId(string UserId)
        {
            var result = await _posterService.GetPosterByUserId(UserId);
            if(result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }
        [HttpPut("DecreasePost")]
        public async Task<IActionResult> DecreasePost(string userId) // Khi artist post bài 
        {

            try
            {
                var result = await _posterService.DecreasePost(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
		[HttpPut("IncreasePost")]
		public async Task<IActionResult> IncreasePost(string userId) // Khi artist post bài 
		{

			try
			{
				var result = await _posterService.IncreasePost(userId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
