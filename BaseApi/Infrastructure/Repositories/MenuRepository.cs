using BaseApi.Application.Common;
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

        public async Task<ApiResult<Menu?>> GetByIdAsync(int id)
        {
            try
            {
                var menu = await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.SubMenus.Where(s => s.IsActive))
                    .Include(m => m.Creator)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (menu == null)
                    return ApiResult<Menu?>.NotFoundResult(Messages.Menu.NotFound);

                return ApiResult<Menu?>.SuccessResult(menu, Messages.Menu.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<Menu?>.FailureResult($"Error retrieving menu: {ex.Message}");
            }
        }

        public async Task<ApiResult<Menu?>> GetBySlugAsync(string slug)
        {
            try
            {
                var menu = await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.SubMenus.Where(s => s.IsActive))
                    .FirstOrDefaultAsync(m => m.Slug.ToLower() == slug.ToLower());

                if (menu == null)
                    return ApiResult<Menu?>.NotFoundResult(Messages.Menu.NotFound);

                return ApiResult<Menu?>.SuccessResult(menu, Messages.Menu.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<Menu?>.FailureResult($"Error retrieving menu by slug: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Menu>>> GetAllAsync()
        {
            try
            {
                var menus = await _context.Menus
                    .Include(m => m.Parent)
                    .Include(m => m.Creator)
                    .OrderBy(m => m.MenuType)
                    .ThenBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Menu>>.SuccessResult(menus, Messages.Menu.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Menu>>.FailureResult($"Error retrieving menus: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Menu>>> GetByMenuTypeAsync(MenuType menuType)
        {
            try
            {
                var menus = await _context.Menus
                    .Include(m => m.SubMenus.Where(s => s.IsActive))
                    .Where(m => m.MenuType == menuType)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Menu>>.SuccessResult(menus, Messages.Menu.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Menu>>.FailureResult($"Error retrieving menus by type: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Menu>>> GetActiveMenusByTypeAsync(MenuType menuType)
        {
            try
            {
                var menus = await _context.Menus
                    .Include(m => m.SubMenus.Where(s => s.IsActive))
                    .Where(m => m.MenuType == menuType && m.IsActive)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Menu>>.SuccessResult(menus, Messages.Menu.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Menu>>.FailureResult($"Error retrieving active menus: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Menu>>> GetSubMenusAsync(int parentId)
        {
            try
            {
                var menus = await _context.Menus
                    .Where(m => m.ParentId == parentId)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Menu>>.SuccessResult(menus, Messages.Menu.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Menu>>.FailureResult($"Error retrieving sub menus: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Menu>>> GetRootMenusAsync(MenuType menuType)
        {
            try
            {
                var menus = await _context.Menus
                    .Include(m => m.SubMenus.Where(s => s.IsActive))
                    .Where(m => m.MenuType == menuType && m.ParentId == null && m.IsActive)
                    .OrderBy(m => m.Order)
                    .ThenBy(m => m.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Menu>>.SuccessResult(menus, Messages.Menu.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Menu>>.FailureResult($"Error retrieving root menus: {ex.Message}");
            }
        }

        public async Task<ApiResult<Menu>> CreateAsync(Menu menu)
        {
            try
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

                return ApiResult<Menu>.SuccessResult(menu, Messages.Menu.Created);
            }
            catch (Exception ex)
            {
                return ApiResult<Menu>.FailureResult($"Error creating menu: {ex.Message}");
            }
        }

        public async Task<ApiResult<Menu>> UpdateAsync(Menu menu)
        {
            try
            {
                menu.UpdatedAt = DateTime.UtcNow;
                _context.Menus.Update(menu);
                await _context.SaveChangesAsync();

                return ApiResult<Menu>.SuccessResult(menu, Messages.Menu.Updated);
            }
            catch (Exception ex)
            {
                return ApiResult<Menu>.FailureResult($"Error updating menu: {ex.Message}");
            }
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            try
            {
                var menu = await _context.Menus.Include(m => m.SubMenus).FirstOrDefaultAsync(m => m.Id == id);
                if (menu == null)
                    return ApiResult.FailureResult(Messages.Menu.NotFound);

                // Check if menu has children
                if (menu.SubMenus.Any())
                {
                    return ApiResult.FailureResult(Messages.Menu.HasChildren);
                }

                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult(Messages.Menu.Deleted);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error deleting menu: {ex.Message}");
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

        public async Task<ApiResult> ReorderMenusAsync(IEnumerable<(int Id, int Order)> menuOrders)
        {
            try
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

                return ApiResult.SuccessResult("Menus reordered successfully");
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error reordering menus: {ex.Message}");
            }
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