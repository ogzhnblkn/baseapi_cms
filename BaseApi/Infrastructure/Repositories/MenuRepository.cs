using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.SubMenus.Where(s => s.IsActive))
                .Include(m => m.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Menu?> GetBySlugAsync(string slug)
        {
            return await _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.SubMenus.Where(s => s.IsActive))
                .FirstOrDefaultAsync(m => m.Slug.ToLower() == slug.ToLower());
        }

        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.Creator)
                .OrderBy(m => m.MenuType)
                .ThenBy(m => m.Order)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetByMenuTypeAsync(MenuType menuType)
        {
            return await _context.Menus
                .Include(m => m.SubMenus.Where(s => s.IsActive))
                .Where(m => m.MenuType == menuType)
                .OrderBy(m => m.Order)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetActiveMenusByTypeAsync(MenuType menuType)
        {
            return await _context.Menus
                .Include(m => m.SubMenus.Where(s => s.IsActive))
                .Where(m => m.MenuType == menuType && m.IsActive)
                .OrderBy(m => m.Order)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetSubMenusAsync(int parentId)
        {
            return await _context.Menus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetRootMenusAsync(MenuType menuType)
        {
            return await _context.Menus
                .Include(m => m.SubMenus.Where(s => s.IsActive))
                .Where(m => m.MenuType == menuType && m.ParentId == null && m.IsActive)
                .OrderBy(m => m.Order)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<Menu> CreateAsync(Menu menu)
        {
            // Auto-generate slug if not provided
            if (string.IsNullOrEmpty(menu.Slug))
            {
                menu.Slug = GenerateSlug(menu.Name);
            }

            // Ensure unique slug
            var originalSlug = menu.Slug;
            var counter = 1;
            while (await ExistsAsync(menu.Slug))
            {
                menu.Slug = $"{originalSlug}-{counter}";
                counter++;
            }

            // Set order if not provided
            if (menu.Order == 0)
            {
                menu.Order = await GetMaxOrderAsync(menu.MenuType, menu.ParentId) + 1;
            }

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task<Menu> UpdateAsync(Menu menu)
        {
            menu.UpdatedAt = DateTime.UtcNow;
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task DeleteAsync(int id)
        {
            var menu = await GetByIdAsync(id);
            if (menu != null)
            {
                // Also delete sub-menus
                var subMenus = await GetSubMenusAsync(id);
                _context.Menus.RemoveRange(subMenus);
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Menus.Where(m => m.Slug.ToLower() == slug.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetMaxOrderAsync(MenuType menuType, int? parentId = null)
        {
            var query = _context.Menus.Where(m => m.MenuType == menuType);

            if (parentId.HasValue)
            {
                query = query.Where(m => m.ParentId == parentId);
            }
            else
            {
                query = query.Where(m => m.ParentId == null);
            }

            if (!await query.AnyAsync())
                return 0;

            return await query.MaxAsync(m => m.Order);
        }

        public async Task ReorderMenusAsync(IEnumerable<(int Id, int Order)> menuOrders)
        {
            foreach (var (id, order) in menuOrders)
            {
                var menu = await _context.Menus.FindAsync(id);
                if (menu != null)
                {
                    menu.Order = order;
                    menu.UpdatedAt = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static string GenerateSlug(string name)
        {
            return name.ToLowerInvariant()
                      .Replace(" ", "-")
                      .Replace("þ", "s")
                      .Replace("ð", "g")
                      .Replace("ü", "u")
                      .Replace("ý", "i")
                      .Replace("ö", "o")
                      .Replace("ç", "c")
                      .Replace("Ý", "i")
                      .Replace("Þ", "s")
                      .Replace("Ð", "g")
                      .Replace("Ü", "u")
                      .Replace("Ö", "o")
                      .Replace("Ç", "c");
        }
    }
}