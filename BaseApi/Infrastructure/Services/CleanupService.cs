using BaseApi.Infrastructure.Middleware;

namespace BaseApi.Infrastructure.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly ILogger<CleanupService> _logger;

        public CleanupService(ILogger<CleanupService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ContactRateLimitingMiddleware.CleanupOldEntries();
                    _logger.LogInformation("Rate limit cleanup completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during cleanup");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}