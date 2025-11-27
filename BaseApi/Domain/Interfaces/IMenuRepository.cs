using BaseApi.Application.Common;
using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IMenuRepository
    {
        Task<ApiResult<Menu?>> GetByIdAsync(int id);
        Task<ApiResult<Menu?>> GetBySlugAsync(string slug);
        Task<ApiResult<IEnumerable<Menu>>> GetAllAsync();
        Task<ApiResult<IEnumerable<Menu>>> GetByMenuTypeAsync(MenuType menuType);
        Task<ApiResult<IEnumerable<Menu>>> GetActiveMenusByTypeAsync(MenuType menuType);
        Task<ApiResult<IEnumerable<Menu>>> GetSubMenusAsync(int parentId);
        Task<ApiResult<IEnumerable<Menu>>> GetRootMenusAsync(MenuType menuType);
        Task<ApiResult<Menu>> CreateAsync(Menu menu);
        Task<ApiResult<Menu>> UpdateAsync(Menu menu);
        Task<ApiResult> DeleteAsync(int id);
        Task<bool> ExistsAsync(string slug, int? excludeId = null);
        Task<int> GetMaxOrderAsync(MenuType menuType, int? parentId = null);
        Task<ApiResult> ReorderMenusAsync(IEnumerable<(int Id, int Order)> menuOrders);
    }
}