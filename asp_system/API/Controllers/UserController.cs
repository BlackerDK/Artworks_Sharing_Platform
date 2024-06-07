    using API.Service;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Model;
using Firebase.Auth;
using Infrastructure.Data;
using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICurrentUserService _currentUserSerivice;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUserServices userServices, IJwtTokenService jwtTokenService, ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userServices = userServices;
            _jwtTokenService = jwtTokenService;
            _context = context;
            _userManager = userManager;
            _currentUserSerivice = currentUserService;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(UserSignUpDTO signUpModel)
        {
            var result = await _userServices.SignUpAsync(signUpModel);
            if (result == null)
            {
                return BadRequest("Email is existed");
            }
            if (result.Succeeded)
            {
                return Ok(result.Succeeded);
            }

            return StatusCode(500);
        }

        [Authorize]
        [HttpGet("GetValue")]
        public async Task<IActionResult> GetValue()
        {
            return Ok("Oke");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserSignInDTO signInModel)
        {
            var user = await _userServices.SignInAsync(signInModel);
            if (user == null || !(user.IsActive))
            {
                return Unauthorized();
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.CreateToken(user, userRoles);
            var refreshToken = _jwtTokenService.CreateRefeshToken();
            user.RefreshToken = refreshToken;
            user.DateExpireRefreshToken = DateTime.Now.AddDays(7);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new { token = accessToken, refreshToken });
        }

        [Authorize]
        [HttpDelete("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            var user = await _currentUserSerivice.User();
            if (user is null)
                return Unauthorized();
            user.RefreshToken = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<IActionResult> getCurrentUserId()
        {
            var user = _currentUserSerivice.GetUserId();
            return Ok(new { userId = user });
        }
        [HttpGet("getAllUser")]
        public Task<IEnumerable<UserDTO>> getAllUsers()
        {
            var listUser = _userServices.GetAllUsers();
            return listUser;
        }
        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> refeshToken(string refreshToken)
        {
            var userId = _currentUserSerivice.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive || user.RefreshToken != refreshToken || user.DateExpireRefreshToken < DateTime.UtcNow)
            {
                return BadRequest(new { processStatus = ProcessStatus.NotPermission });
            } 
            var userRoles = await _userManager.GetRolesAsync(user);
            var newRefreshToken = _jwtTokenService.CreateRefeshToken();
            user.RefreshToken = newRefreshToken;
            user.DateExpireRefreshToken = DateTime.Now.AddDays(7);
            var token = _jwtTokenService.CreateToken(user, userRoles);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok(new { token = token, refreshToken = newRefreshToken });
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetAllUser(string id)
        {
            var result = await _userServices.GetUserByIDlAsync(id);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var result = await _currentUserSerivice.User();
            return Ok(result);
        }

        [HttpPost("GoogleLogin")]
        public async Task<ActionResult> GoogleLogin(UserSignUpDTO model)
        {
            var user = await _userServices.ExternalLoginAsync(model);
			if (user == null)
			{
				return Unauthorized();

			}
			var userRoles = await _userManager.GetRolesAsync(user);
			var accessToken = _jwtTokenService.CreateToken(user, userRoles);
			var refreshToken = _jwtTokenService.CreateRefeshToken();
			user.RefreshToken = refreshToken;
			user.DateExpireRefreshToken = DateTime.Now.AddDays(7);
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return Ok(new { token = accessToken, refreshToken });
			////new
			//var responseGoogle = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			//if (responseGoogle == null) {
			//    return Unauthorized();
			//}
			//if (responseGoogle.Principal == null) return BadRequest();

			//var name = responseGoogle.Principal.FindFirstValue(ClaimTypes.Name);
			//var givenName = responseGoogle.Principal.FindFirstValue(ClaimTypes.GivenName);
			//var email = responseGoogle.Principal.FindFirstValue(ClaimTypes.Email);
			////Do something with the claims
			//// var user = await UserService.FindOrCreate(new { name, givenName, email});

			//return Ok();


			//old
			//var info = await _signInManager.GetExternalLoginInfoAsync();
			//if (info == null)
			//{
			//    return Unauthorized();
			//} 
			//var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

			//if (signInResult.Succeeded)
			//{
			//    return Ok();
			//}
			//else
			//{
			//    var responseGoogle = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			//    if (responseGoogle.Principal == null) return BadRequest();

			//    var name = responseGoogle.Principal.FindFirstValue(ClaimTypes.Name);
			//    var givenName = responseGoogle.Principal.FindFirstValue(ClaimTypes.GivenName);
			//    var email = responseGoogle.Principal.FindFirstValue(ClaimTypes.Email);
			//    //Do something with the claims
			//    // var user = await UserService.FindOrCreate(new { name, givenName, email});
			//    var user = new User { Email = email, FirstName = name };

			//    var userLogin = await _userManager.FindByEmailAsync(email);
			//    IdentityResult result;
			//    if (user != null)
			//    {
			//        result = await _userManager.AddLoginAsync(userLogin, info);
			//        if (result.Succeeded)
			//        {
			//            await _signInManager.SignInAsync(userLogin, isPersistent: false);
			//            return Ok();
			//        }
			//    }
			//    else
			//    {
			//        ApplicationUser newUser = new ApplicationUser {
			//            Email = email,
			//            UserName = email,
			//            FirstName = name
			//        };
			//        result = await _userManager.CreateAsync(newUser);
			//        if (result.Succeeded)
			//        {
			//            result = await _userManager.AddLoginAsync(newUser, info);
			//            if (result.Succeeded)
			//            {
			//                //TODO: Send an emal for the email confirmation and add a default role as in the Register action
			//                await _signInManager.SignInAsync(newUser, isPersistent: false);
			//                return Ok();                        }
			//        }
			//    }
			return Unauthorized();
            }
    }
}
