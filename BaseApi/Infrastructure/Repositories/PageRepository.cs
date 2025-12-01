using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly ApplicationDbContext _context;

        public PageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Page?> GetByIdAsync(int id)
        {
            return await _context.Pages
                .Include(p => p.Creator)
                .Include(p => p.Updater)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Page?> GetBySlugAsync(string slug)
        {
            return await _context.Pages
                .Include(p => p.Creator)
                .Include(p => p.Updater)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == PageStatus.Published);
        }

        public async Task<IEnumerable<Page>> GetAllAsync()
        {
            return await _context.Pages
                .Include(p => p.Creator)
                .Include(p => p.Updater)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Page>> GetByStatusAsync(PageStatus status)
        {
            return await _context.Pages
                .Include(p => p.Creator)
                .Where(p => p.Status == status)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Page>> GetByTemplateAsync(PageTemplate template)
        {
            return await _context.Pages
                .Include(p => p.Creator)
                .Where(p => p.Template == template && p.Status == PageStatus.Published)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Page>> GetPublishedAsync()
        {
            return await _context.Pages
                .Where(p => p.Status == PageStatus.Published)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<Page?> GetHomePageAsync()
        {
            return await _context.Pages
                .FirstOrDefaultAsync(p => p.IsHomePage && p.Status == PageStatus.Published);
        }

        public async Task<Page> CreateAsync(Page page)
        {
            _context.Pages.Add(page);
            await _context.SaveChangesAsync();
            return page;
        }

        public async Task UpdateAsync(Page page)
        {
            page.UpdatedAt = DateTime.UtcNow;
            _context.Pages.Update(page);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                _context.Pages.Remove(page);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Pages.Where(p => p.Slug == slug);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Page>> SearchAsync(string searchTerm)
        {
            return await _context.Pages
                .Where(p => p.Title.Contains(searchTerm) ||
                           p.Content.Contains(searchTerm) ||
                           p.Summary!.Contains(searchTerm))
                .Where(p => p.Status == PageStatus.Published)
                .OrderBy(p => p.Title)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                page.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}