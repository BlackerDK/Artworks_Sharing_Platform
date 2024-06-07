using API.Service;
using Application.Interfaces.Services;
using Domain.Model;
using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentController : ControllerBase
	{
		private readonly ICommentService _commentService;
		private readonly ICurrentUserService _currentUserService;

		public CommentController(ICommentService commentService, ICurrentUserService currentUserService)
		{
			_commentService = commentService;
			_currentUserService = currentUserService;
		}
		[HttpGet("GetAllCommentByArtwork")]
		public async Task<IActionResult> GetAllCommentByArtwork(int artworkId)
		{
			var result = await _commentService.GetAllCommentByArtwork(artworkId);
			return Ok(result);
		}
		[HttpPost("CreateComment")]
		public async Task<IActionResult> CreateComment(CommentAddDTO cmt)
		{
			try
			{
				var result = await _commentService.CreateComment(cmt);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
		[HttpDelete("DeleteCommment")]
		public async Task<IActionResult> DeleteComent(CommentAddDTO CommentId)
		{
			try
			{
				var result = await _commentService.DeleteComent(CommentId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
		[HttpPut("UpdateComment")]
		public async Task<IActionResult> UpdateComment(CommentUpdateDTO cmt)
		{
			try
			{
				var result = await _commentService.UpdateComment(cmt);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
