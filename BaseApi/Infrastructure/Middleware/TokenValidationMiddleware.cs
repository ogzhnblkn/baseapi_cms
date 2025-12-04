using BaseApi.Application.Interfaces;
using BaseApi.Domain.Interfaces;

namespace BaseApi.Infrastructure.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenValidationMiddleware> _logger;

        public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService, ITokenBlacklistRepository tokenBlacklistRepository)
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = jwtService.GetTokenFromAuthorizationHeader(authHeader);

                if (!string.IsNullOrEmpty(token))
                {
                    // Check if token is blacklisted
                    if (await tokenBlacklistRepository.IsTokenBlacklistedAsync(token))
                    {
                        _logger.LogWarning("Attempt to use blacklisted token from IP: {IP}",
                            context.Connection.RemoteIpAddress?.ToString());

                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token has been invalidated");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}