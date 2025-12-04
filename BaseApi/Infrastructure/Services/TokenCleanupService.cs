using BaseApi.Domain.Interfaces;

namespace BaseApi.Infrastructure.Services
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6); // Clean up every 6 hours

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var tokenBlacklistRepository = scope.ServiceProvider.GetRequiredService<ITokenBlacklistRepository>();

                    await tokenBlacklistRepository.CleanExpiredTokensAsync();
                    _logger.LogInformation("Token cleanup completed at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during token cleanup");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}