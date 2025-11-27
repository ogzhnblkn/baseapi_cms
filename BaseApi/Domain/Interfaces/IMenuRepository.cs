using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IMenuRepository
    {
        Task<Menu?> GetByIdAsync(int id);
        Task<Menu?> GetBySlugAsync(string slug);
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<IEnumerable<Menu>> GetByMenuTypeAsync(MenuType menuType);
        Task<IEnumerable<Menu>> GetActiveMenusByTypeAsync(MenuType menuType);
        Task<IEnumerable<Menu>> GetSubMenusAsync(int parentId);
        Task<IEnumerable<Menu>> GetRootMenusAsync(MenuType menuType);
        Task<Menu> CreateAsync(Menu menu);
        Task<Menu> UpdateAsync(Menu menu);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string slug, int? excludeId = null);
        Task<int> GetMaxOrderAsync(MenuType menuType, int? parentId = null);
        Task ReorderMenusAsync(IEnumerable<(int Id, int Order)> menuOrders);
    }
}