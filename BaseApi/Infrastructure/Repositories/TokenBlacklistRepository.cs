using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenBlacklistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _context.TokenBlacklists
                .AnyAsync(t => t.Token == token && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task BlacklistTokenAsync(string token, int userId, DateTime expiresAt, string? reason = null)
        {
            var blacklistEntry = new TokenBlacklist
            {
                Token = token,
                UserId = userId,
                ExpiresAt = expiresAt,
                Reason = reason,
                BlacklistedAt = DateTime.UtcNow
            };

            _context.TokenBlacklists.Add(blacklistEntry);
            await _context.SaveChangesAsync();
        }

        public async Task BlacklistAllUserTokensAsync(int userId)
        {
            // This would require tracking issued tokens per user
            // For now, we'll implement a simple approach
            var reason = $"All tokens invalidated for user {userId} at {DateTime.UtcNow}";

            // Note: In a real implementation, you'd track all issued tokens
            // For now, this method exists for future enhancement
            await Task.CompletedTask;
        }

        public async Task CleanExpiredTokensAsync()
        {
            var expiredTokens = await _context.TokenBlacklists
                .Where(t => t.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            _context.TokenBlacklists.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}