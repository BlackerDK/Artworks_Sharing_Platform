using API.Service;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CartController : ControllerBase
	{
		private readonly ICartService _cartService;
		private readonly ICurrentUserService _currentUserService;
		private readonly IArtworkService _artworkService;

		public CartController(ICurrentUserService currentUserService, ICartService cartService, IArtworkService artworkService)
		{
			_cartService = cartService;
			_currentUserService = currentUserService;
			_artworkService = artworkService;

		}

		[HttpGet("getCart")]
		public async Task<IActionResult> getCart()
		{
			var user = await _currentUserService.User();
			var cart = await _cartService.GetCart(user.Id);
			if (cart.Count() == 0)
			{
				return BadRequest("This user not have any product in cart");
			}
			return Ok(new
			{
				total = cart.Count(),
				data = cart
			});
		}

		[HttpPost("addToCart")]
		public async Task<IActionResult> AddToCart(CartCM cartCM)
		{
			var artwork = await _artworkService.GetArtworkById(cartCM.ArtWorkId);
			var item = new CartDTO
			{
				Id = Guid.NewGuid(),
				ArtWorkId = cartCM.ArtWorkId,
				UserId = cartCM.UserId,
				ArtWorkImage = artwork.ImageUrl.FirstOrDefault(),
				Price = (double)artwork.Price,
				Title = artwork.Title,
			};
			var result = await _cartService.AddToCart(item);
			if(!result.IsSuccess)
			{
				return BadRequest("Artwork was in cart");
			}
			return Ok(result);
		}

		[HttpDelete("removeCartItem")]
		public async Task<IActionResult> RemoveCartItem(string artworkId)
		{
			var result = await _cartService.DeleteItemInCart(artworkId);
			if (result.IsSuccess)
			{
				return Ok(result);
			}
			return BadRequest();
		}


	}
}
