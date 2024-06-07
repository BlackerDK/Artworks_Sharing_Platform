using API.Service;
using Application.Interfaces.Services;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly ICurrentUserService _currentUserService;

        public PackageController(IPackageService packageService , ICurrentUserService currentUserService)
        {
            _packageService = packageService;
            _currentUserService = currentUserService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _packageService.GetAllPackage();
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddPackage(PackageAddDTO package)
        {
            try
            {
                var result = await _packageService.AddPackage(package);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var result = await _packageService.GetPackageById(id);
            return Ok(result);
        }
        [HttpPut("Delete")]
        public async Task<IActionResult> DeletePackage(int id)
        {

            try
            {
                var result = await _packageService.DetelePackage(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdatePackage(int id, PackageDTO package)
        {

            try
            {
                var result = await _packageService.UpdatePackage(id,package);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}