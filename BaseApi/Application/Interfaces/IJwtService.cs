using BaseApi.Domain.Entities;

namespace BaseApi.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        int GetUserIdFromToken(string token);
        DateTime GetTokenExpirationDate(string token);
        string GetTokenFromAuthorizationHeader(string authorizationHeader);
    }
}