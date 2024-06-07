using Application.Interfaces.Services;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArtworkImageController : ControllerBase
    {
        private readonly IArtworkImageService _artworkImageService;

        public ArtworkImageController(IArtworkImageService artworkImageService)
        {
            _artworkImageService = artworkImageService;
        }

        //add artwork images
        [HttpPost("AddArtworkImages")]
        public async Task<IActionResult> AddArtworkImages([FromForm] ArtworkImageDTO artworkImages)
        {
            try
            {
                var result = await _artworkImageService.AddArtworkImages(artworkImages);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //delete artwork image
        [HttpDelete("DeleteArtworkImage/{artworkImageId}")]
        public async Task<IActionResult> DeleteArtworkImage(int artworkImageId)
        {
            var reponse = await _artworkImageService.DeleteArtworkImage(artworkImageId);
            if (reponse.IsSuccess)
            {
                return Ok(reponse);
            }
            return BadRequest(reponse);
        }
    }
}
