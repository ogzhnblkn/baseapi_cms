using BaseApi.Application.Common;
using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<ApiResult<User?>> GetByIdAsync(int id);
        Task<ApiResult<User?>> GetByUsernameAsync(string username);
        Task<ApiResult<User?>> GetByEmailAsync(string email);
        Task<ApiResult<IEnumerable<User>>> GetAllAsync();
        Task<ApiResult<User>> CreateAsync(User user);
        Task<ApiResult<User>> UpdateAsync(User user);
        Task<ApiResult> DeleteAsync(int id);
        Task<bool> ExistsAsync(string username, string email);
    }
}