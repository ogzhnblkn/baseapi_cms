using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IPageRepository
    {
        Task<Page?> GetByIdAsync(int id);
        Task<Page?> GetBySlugAsync(string slug);
        Task<IEnumerable<Page>> GetAllAsync();
        Task<IEnumerable<Page>> GetByStatusAsync(PageStatus status);
        Task<IEnumerable<Page>> GetByTemplateAsync(PageTemplate template);
        Task<IEnumerable<Page>> GetPublishedAsync();
        Task<Page?> GetHomePageAsync();
        Task<Page> CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<IEnumerable<Page>> SearchAsync(string searchTerm);
        Task IncrementViewCountAsync(int id);
    }
}