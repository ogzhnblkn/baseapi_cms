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
            // Sadece gerçek tehlikeli XSS pattern'leri kontrol et
            if (context.Request.ContentLength > 0 &&
                (context.Request.ContentType?.Contains("application/json") == true ||
                 context.Request.ContentType?.Contains("application/x-www-form-urlencoded") == true))
            {
                await CheckRequestBodyForDangerousXss(context, xssProtectionService);
            }

            // Query string parametrelerini kontrol et
            CheckQueryStringForDangerousXss(context, xssProtectionService);

            await _next(context);
        }

        private async Task CheckRequestBodyForDangerousXss(HttpContext context, IXssProtectionService xssProtectionService)
        {
            context.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer);
            var bodyContent = Encoding.UTF8.GetString(buffer);

            context.Request.Body.Position = 0;

            // Sadece gerçekten tehlikeli script pattern'leri kontrol et
            if (ContainsDangerousScript(bodyContent))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var userAgent = context.Request.Headers.UserAgent.ToString();

                _logger.LogWarning("Dangerous XSS detected in request body from IP: {IP}, User-Agent: {UserAgent}, Path: {Path}",
                    ipAddress, userAgent, context.Request.Path);

                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "Request contains dangerous script content",
                    message = "Potentially harmful script detected"
                }));
                return;
            }
        }

        private void CheckQueryStringForDangerousXss(HttpContext context, IXssProtectionService xssProtectionService)
        {
            foreach (var param in context.Request.Query)
            {
                if (ContainsDangerousScript(param.Value))
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString();

                    _logger.LogWarning("Dangerous XSS detected in query parameter '{Parameter}' from IP: {IP}",
                        param.Key, ipAddress);
                }
            }
        }

        private bool ContainsDangerousScript(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Sadece gerçekten tehlikeli pattern'ler
            var dangerousPatterns = new[]
            {
                "<script",
                "</script>",
                "javascript:",
                "vbscript:",
                "onload=",
                "onclick=",
                "onmouseover=",
                "onerror=",
                "onsubmit=",
                "alert(",
                "eval(",
                "setTimeout(",
                "setInterval(",
                "document.cookie",
                "window.location",
                "document.write"
            };

            return dangerousPatterns.Any(pattern =>
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
    }
}