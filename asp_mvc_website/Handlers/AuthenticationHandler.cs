using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net;
using asp_mvc_website.Services;

namespace asp_mvc_website.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;
        private bool _isExpired;

        public AuthenticationHandler(IConfiguration configuration, IJwtTokenService jwtTokenService)
        {
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var jwt = _jwtTokenService.getToken();
            var isToServer = request.RequestUri?.AbsoluteUri.StartsWith(_configuration["Cron:localhost"] ?? "") ?? false;

            if (isToServer && !string.IsNullOrEmpty(jwt))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await base.SendAsync(request, cancellationToken);

            if (!_isExpired && !string.IsNullOrEmpty(jwt) && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                try
                {
                    _isExpired = true;

                    if (await _jwtTokenService.refreshTokenAsync())
                    {
                        jwt =  _jwtTokenService.getToken();

                        if (isToServer && !string.IsNullOrEmpty(jwt))
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

                        response = await base.SendAsync(request, cancellationToken);
                    }
                }
                finally
                {
                    _isExpired = false;
                }
            }
            return response;
        }
    }
}
