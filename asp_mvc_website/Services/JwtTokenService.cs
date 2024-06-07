using asp_mvc_website.Controllers;
using Newtonsoft.Json;

namespace asp_mvc_website.Services
{
    public interface IJwtTokenService
    {
        string getToken();
        Task<bool> refreshTokenAsync();
    }
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtTokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<UserController> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
            _client.BaseAddress = new Uri(configuration["Cron:localhost"]);
        }

        public string getToken()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
        }

        public async Task<bool> refreshTokenAsync()
        {
            var model = new TokenResponse
            {
                Token = _httpContextAccessor.HttpContext.Session?.GetString("AccessToken"),
                RefreshToken = _httpContextAccessor.HttpContext.Session?.GetString("RefeshToken"),
            };
            var response = await _client.PostAsync(_client.BaseAddress + "User/refresh-token", new StringContent(model.RefreshToken));
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            _httpContextAccessor.HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
            _httpContextAccessor.HttpContext.Session.SetString("RefeshToken", tokenResponse.RefreshToken);
            _logger.LogInformation("Call Refresh Token");
            return true;
        }

        public bool refreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
