using System.Collections.Concurrent;
using System.Net;

namespace BaseApi.Infrastructure.Middleware
{
    public class ContactRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ContactRateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, ContactRateLimit> _clients = new();
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(15); // 15 dakikalýk pencere
        private readonly int _requestLimit = 3; // 15 dakikada maksimum 3 istek

        public ContactRateLimitingMiddleware(RequestDelegate next, ILogger<ContactRateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Sadece contact POST isteði için kontrol et
            if (context.Request.Path.StartsWithSegments("/api/contact") &&
                context.Request.Method == "POST")
            {
                var clientId = GetClientIdentifier(context);
                var now = DateTime.UtcNow;

                var rateLimit = _clients.AddOrUpdate(clientId,
                    new ContactRateLimit { FirstRequest = now, RequestCount = 1 },
                    (key, existingLimit) =>
                    {
                        if (now - existingLimit.FirstRequest > _timeWindow)
                        {
                            // Zaman penceresi geçti, sýfýrla
                            return new ContactRateLimit { FirstRequest = now, RequestCount = 1 };
                        }
                        else
                        {
                            existingLimit.RequestCount++;
                            return existingLimit;
                        }
                    });

                if (rateLimit.RequestCount > _requestLimit)
                {
                    _logger.LogWarning("Rate limit exceeded for client {ClientId}. Requests: {RequestCount}",
                        clientId, rateLimit.RequestCount);

                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Çok fazla istek gönderdiniz. Lütfen 15 dakika bekleyin.");
                    return;
                }

                _logger.LogInformation("Contact form request from {ClientId}. Count: {RequestCount}/{RequestLimit}",
                    clientId, rateLimit.RequestCount, _requestLimit);
            }

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            // IP adresi + User-Agent kombinasyonu
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = context.Request.Headers.UserAgent.ToString();
            return $"{ip}-{userAgent.GetHashCode()}";
        }

        // Temizleme task'i (eski kayýtlarý sil)
        public static void CleanupOldEntries()
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            var keysToRemove = _clients
                .Where(kvp => kvp.Value.FirstRequest < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _clients.TryRemove(key, out _);
            }
        }
    }

    public class ContactRateLimit
    {
        public DateTime FirstRequest { get; set; }
        public int RequestCount { get; set; }
    }
}