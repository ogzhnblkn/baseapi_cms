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
                    bool isBlacklisted;
                    try
                    {
                        isBlacklisted = await tokenBlacklistRepository.IsTokenBlacklistedAsync(token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Token blacklist check failed");

                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            success = false,
                            message = "Token dogrulamasi tamamlanamadi. Lutfen daha sonra tekrar deneyin."
                        });
                        return;
                    }

                    if (isBlacklisted)
                    {
                        _logger.LogWarning("Attempt to use blacklisted token from IP: {IP}",
                            context.Connection.RemoteIpAddress?.ToString());

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            success = false,
                            message = "Token gecersiz kilinmis."
                        });
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
