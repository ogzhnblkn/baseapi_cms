namespace BaseApi.Domain.Interfaces
{
    public interface ITokenBlacklistRepository
    {
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task BlacklistTokenAsync(string token, int userId, DateTime expiresAt, string? reason = null);
        Task BlacklistAllUserTokensAsync(int userId);
        Task CleanExpiredTokensAsync();
    }
}