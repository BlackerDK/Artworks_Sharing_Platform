using asp_mvc_website.Helpers;
using asp_mvc_website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using asp_mvc_website.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace asp_mvc_website.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _client;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrentUserService _currentUserService;
        public UserController(ILogger<UserController> logger, IHttpClientFactory httpClientFactory,
            IConfiguration configuration, ICurrentUserService currentUserService)
        {
            _factory = httpClientFactory;
            _client = new HttpClient();
            _currentUserService = currentUserService;
            _client = _factory.CreateClient("ServerApi");
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }
        public IActionResult Index()
        {
			var userId = HttpContext.Session.GetString("UserId");
			if (userId != null)
			{
				return Redirect("/Home");
			}
			return View("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
			var userId = HttpContext.Session.GetString("UserId");
			if (userId != null)
			{
				return Redirect("/Home");
			}
			var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginDTO = new LoginDTO
            {
                Email = model.Email,
                Password = model.Password
            };

            // Send login request to Web API
            var response = await _client.PostAsync(
                _client.BaseAddress + "User/SignIn",
                new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Store token in session, cookie, or local storage
                HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
                HttpContext.Session.SetString("RefeshToken", tokenResponse.RefreshToken);
                HttpContext.Session.SetString("UserEmail", model.Email);
				// Redirect user to the home page or another appropriate page

				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
                var user = await _currentUserService.User();
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                }

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(tokenResponse.Token);

                // Extract role claims
                var roleClaims = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                foreach (var role in roleClaims)
                {
                    if (role.Equals(AppRole.Admin))
                    {
                        // Dashboard
                        return RedirectToAction("Index", "Dashbroad");
                    }
                }   

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid username or password";
                //ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
			var userId = HttpContext.Session.GetString("UserId");
			if (userId != null)
			{
				return Redirect("/Home");
			}
			return View();
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerDTO = new RegisterDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                IsAdmin = false
            };

            // Send registration request to Web API
            var response = await _client.PostAsync(
                _client.BaseAddress + "User/SignUp",
                new StringContent(
                    JsonConvert.SerializeObject(registerDTO),
                    Encoding.UTF8,
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Registration successful, redirect to login page or other appropriate action
                return RedirectToAction("Login", "User");
            }
            else
            {
                // Handle error response
                // Example: Display error message to user
                ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
                return View(model);
            }
        }

		[AllowAnonymous]
		public async Task<ActionResult> ExternalLogin()
		{
            var props = new AuthenticationProperties { RedirectUri = "/user/GoogleLogin" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
		}

		public async Task<ActionResult> GoogleLogin()
		{
            var responseGoogle = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (responseGoogle.Principal == null) return BadRequest();
            var name = responseGoogle.Principal.FindFirstValue(ClaimTypes.Name);
            var givenName = responseGoogle.Principal.FindFirstValue(ClaimTypes.GivenName);
            var email = responseGoogle.Principal.FindFirstValue(ClaimTypes.Email);
            //Do something with the claims
            // var user = await UserService.FindOrCreate(new { name, givenName, email});
            var user = new RegisterDTO { 
                FirstName = name,
                LastName = name,
                Email = email,
                Password = email
            };

            // Send login request to Web API
            var response = await _client.PostAsync(
                _client.BaseAddress + "User/GoogleLogin", 
                new StringContent(
                    JsonConvert.SerializeObject(user),
                    Encoding.UTF8, 
                    "application/json"));

            if (response.IsSuccessStatusCode)
            {
				// Read response content
				var responseContent = await response.Content.ReadAsStringAsync();
				var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

				// Store token in session, cookie, or local storage
				HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
				HttpContext.Session.SetString("RefeshToken", tokenResponse.RefreshToken);
				HttpContext.Session.SetString("UserEmail", email);
				HttpContext.Session.SetString("FirstName", name);
				// Redirect user to the home page or another appropriate page

				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
				var currentUser = await _currentUserService.User();
				if (user != null)
				{
					HttpContext.Session.SetString("UserId", currentUser.Id.ToString());
				}

				var handler = new JwtSecurityTokenHandler();
				var token = handler.ReadJwtToken(tokenResponse.Token);

				// Extract role claims
				var roleClaims = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
				foreach (var role in roleClaims)
				{
					if (role.Equals(AppRole.Admin))
					{
						// Dashboard
						return RedirectToAction("Index", "Dashbroad");
					}
				}

				return RedirectToAction("Index", "Home");
			}
            else
            {
                ViewData["ErrorMessage"] = "not validate";
				//ModelState.AddModelError(string.Empty, "Invalid username or password");
				return RedirectToAction("Index", "Home");
			}


            //var response = await _client.PostAsync(
            //    _client.BaseAddress + "User/SignUp",
            //    new StringContent(
            //        JsonConvert.SerializeObject(user),
            //        Encoding.UTF8,
            //        "application/json"));

            //if (response.IsSuccessStatusCode)
            //{   
            //    HttpContext.Session.SetString("UserEmail", email);
            //    // Redirect user to the home page or another appropriate page
            //    return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    // Handle error response
            //    // Example: Display error message to user
            //    ViewBag.ErrorMessage = "An error occurred during registration. Please try again.";
            //    return RedirectToAction("Login", "User");
            //}

            return RedirectToAction("Login", "User");
        }



        //public async Task<IActionResult> RefeshToken()
        //{
        //    var model = new TokenResponse
        //    {
        //        Token = HttpContext.Session?.GetString("AccessToken"),
        //        RefreshToken = HttpContext.Session?.GetString("RefeshToken"),
        //    };
        //    var response = await _client.PostAsync(_client.BaseAddress + "User/refresh-token", new StringContent(model.RefreshToken));
        //    if(!response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Logout");
        //    }
        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
        //    HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
        //    HttpContext.Session.SetString("RefeshToken", tokenResponse.RefreshToken);
        //}

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var response = await _client.DeleteAsync(_client.BaseAddress + "User/SignOut");
            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session?.Remove("AccessToken");
                HttpContext.Session?.Remove("RefeshToken");
                HttpContext.Session?.Remove("UserId");
                HttpContext.Session?.Remove("UserEmail");
                return RedirectToAction("Login");
            }
            return Unauthorized();
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}