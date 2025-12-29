using BaseApi.Application.Common;
using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<ApiResult<Contact?>> GetByIdAsync(int id);
        Task<ApiResult<IEnumerable<Contact>>> GetAllAsync();
        Task<ApiResult<Contact>> CreateAsync(Contact Contact);
    }
}
