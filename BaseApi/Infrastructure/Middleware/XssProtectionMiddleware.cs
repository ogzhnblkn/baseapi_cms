using BaseApi.Application.Common.Security;
using System.Text;
using System.Text.Json;

namespace BaseApi.Infrastructure.Middleware
{
    public class XssProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XssProtectionMiddleware> _logger;

        public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IXssProtectionService xssProtectionService)
        {
            // Check request body for XSS
            if (context.Request.ContentLength > 0 &&
                (context.Request.ContentType?.Contains("application/json") == true ||
                 context.Request.ContentType?.Contains("application/x-www-form-urlencoded") == true))
            {
                await CheckRequestBodyForXss(context, xssProtectionService);
            }

            // Check query string parameters
            CheckQueryStringForXss(context, xssProtectionService);

            // Check headers
            CheckHeadersForXss(context, xssProtectionService);

            await _next(context);
        }

        private async Task CheckRequestBodyForXss(HttpContext context, IXssProtectionService xssProtectionService)
        {
            context.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer);
            var bodyContent = Encoding.UTF8.GetString(buffer);

            context.Request.Body.Position = 0;

            if (xssProtectionService.ContainsXss(bodyContent))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers.UserAgent.ToString();

                _logger.LogWarning("XSS detected in request body from IP: {IP}, User-Agent: {UserAgent}, Path: {Path}",
                    ipAddress, userAgent, context.Request.Path);

                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "Request contains potentially dangerous content",
                    message = "XSS content detected"
                }));
                return;
            }
        }

        private void CheckQueryStringForXss(HttpContext context, IXssProtectionService xssProtectionService)
        {
            foreach (var param in context.Request.Query)
            {
                if (xssProtectionService.ContainsXss(param.Value))
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                    _logger.LogWarning("XSS detected in query parameter '{Parameter}' from IP: {IP}",
                        param.Key, ipAddress);
                }
            }
        }

        private void CheckHeadersForXss(HttpContext context, IXssProtectionService xssProtectionService)
        {
            var suspiciousHeaders = new[] { "User-Agent", "Referer", "X-Forwarded-For" };

            foreach (var headerName in suspiciousHeaders)
            {
                if (context.Request.Headers.ContainsKey(headerName))
                {
                    var headerValue = context.Request.Headers[headerName].ToString();
                    if (xssProtectionService.ContainsXss(headerValue))
                    {
                        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                        _logger.LogWarning("XSS detected in header '{Header}' from IP: {IP}",
                            headerName, ipAddress);
                    }
                }
            }
        }
    }
}