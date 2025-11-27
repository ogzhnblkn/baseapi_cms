using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _context.Products
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(p => p.Slug.ToLower() == slug.ToLower());
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Creator)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.Status == ProductStatus.Active)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsByCategoryAsync(ProductCategory category)
        {
            return await _context.Products
                .Where(p => p.Category == category && p.Status == ProductStatus.Active)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsFeatured && p.Status == ProductStatus.Active)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetNewProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsNewProduct && p.Status == ProductStatus.Active)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsOnSaleAsync()
        {
            return await _context.Products
                .Where(p => p.DiscountPrice.HasValue && p.Price.HasValue &&
                           p.DiscountPrice < p.Price && p.Status == ProductStatus.Active)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _context.Products
                .Where(p => p.Status == ProductStatus.Active &&
                           (p.Name.ToLower().Contains(term) ||
                            p.Description!.ToLower().Contains(term) ||
                            p.ShortDescription!.ToLower().Contains(term) ||
                            p.ProductCode!.ToLower().Contains(term)))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                if (string.IsNullOrEmpty(product.Slug))
                {
                    product.Slug = GenerateSlug(product.Name);
                }

                // Ensure unique slug
                var originalSlug = product.Slug;
                var counter = 1;
                while (await ExistsAsync(product.Slug))
                {
                    product.Slug = $"{originalSlug}-{counter}";
                    counter++;
                }

                // Set order if not provided
                if (product.Order == 0)
                {
                    product.Order = await GetMaxOrderAsync(product.Category) + 1;
                }

                // Auto-generate product code if not provided
                if (string.IsNullOrEmpty(product.ProductCode))
                {
                    product.ProductCode = await GenerateProductCode(product.Category);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return product;
            }
            // Auto-generate slug if not provided

            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string slug, int? excludeId = null)
        {
            var query = _context.Products.Where(p => p.Slug.ToLower() == slug.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetMaxOrderAsync(ProductCategory? category = null)
        {
            var query = _context.Products.AsQueryable();

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            if (!await query.AnyAsync())
                return 0;

            return await query.MaxAsync(p => p.Order);
        }

        public async Task ReorderProductsAsync(IEnumerable<(int Id, int Order)> productOrders)
        {
            foreach (var (id, order) in productOrders)
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    product.Order = order;
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByStatusAsync(ProductStatus status)
        {
            return await _context.Products
                .Where(p => p.Status == status)
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal? minPrice, decimal? maxPrice, ProductCategory? category = null)
        {
            var query = _context.Products
                .Where(p => p.Status == ProductStatus.Active && p.Price.HasValue);

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category);
            }

            return await query
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Name)
                .ToListAsync();
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

        private async Task<string> GenerateProductCode(ProductCategory category)
        {
            var categoryCode = GetCategoryCode(category);
            var date = DateTime.Now.ToString("yyMM");

            // Get next sequence number for this category and month
            var lastProduct = await _context.Products
                .Where(p => p.ProductCode!.StartsWith($"{categoryCode}{date}"))
                .OrderByDescending(p => p.ProductCode)
                .FirstOrDefaultAsync();

            var sequenceNumber = 1;
            if (lastProduct != null && lastProduct.ProductCode!.Length >= 8)
            {
                var lastSequence = lastProduct.ProductCode.Substring(6);
                if (int.TryParse(lastSequence, out int parsed))
                {
                    sequenceNumber = parsed + 1;
                }
            }

            return $"{categoryCode}{date}{sequenceNumber:D4}";
        }

        private static string GetCategoryCode(ProductCategory category)
        {
            return category switch
            {
                ProductCategory.LivingRoom => "OG",
                ProductCategory.Bedroom => "YO",
                ProductCategory.DiningRoom => "YD",
                ProductCategory.ChildRoom => "CO",
                ProductCategory.Office => "OF",
                ProductCategory.Kitchen => "MU",
                ProductCategory.Bathroom => "BA",
                ProductCategory.Garden => "BB",
                ProductCategory.Armchair => "KL",
                ProductCategory.Sofa => "KA",
                ProductCategory.Chair => "SA",
                ProductCategory.Table => "MS",
                ProductCategory.Wardrobe => "DL",
                ProductCategory.Bed => "YT",
                ProductCategory.Dresser => "SF",
                ProductCategory.Nightstand => "KM",
                ProductCategory.Bookshelf => "KT",
                ProductCategory.TvUnit => "TV",
                ProductCategory.ShoeRack => "AD",
                ProductCategory.CoatRack => "VS",
                ProductCategory.Mirror => "AY",
                ProductCategory.Ottoman => "PF",
                ProductCategory.CoffeeTable => "SH",
                ProductCategory.Console => "KS",
                ProductCategory.Showcase => "VT",
                ProductCategory.Accessory => "AK",
                ProductCategory.Lighting => "IS",
                ProductCategory.Carpet => "HL",
                ProductCategory.Curtain => "PR",
                _ => "DG"
            };
        }
    }
}