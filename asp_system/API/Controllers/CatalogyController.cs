using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Model;
using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogyController : ControllerBase
    {
        private readonly ICatalogyService _catalogyService;

        public CatalogyController(ICatalogyService catalogyService)
        {
            _catalogyService = catalogyService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _catalogyService.GetAllCatalogy();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddCatalogy(CatalogyAddDTO catalogy)
        {
            try
            {
                var result = await _catalogyService.AddCatalogy(catalogy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalogyById(int id)
        {
            var result = await _catalogyService.GetCatalogyById(id);
            return Ok(result);
        }
        [HttpPut("Delete")]
        public async Task<IActionResult> DeleteCatalogy(int id)
        {

            try
            {
                var result = await _catalogyService.DeteleCatalogy(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCatalogy(int id,CatalogyDTO catalogy)
        {

            try
            {
                var result = await _catalogyService.UpdateCatalogy(id,catalogy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
