using BaseApi.Application.Common;
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

        public async Task<ApiResult<Product?>> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Creator)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return ApiResult<Product?>.NotFoundResult(Messages.Product.NotFound);

                return ApiResult<Product?>.SuccessResult(product, Messages.Product.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<Product?>.FailureResult($"Error retrieving product: {ex.Message}");
            }
        }

        public async Task<ApiResult<Product?>> GetBySlugAsync(string slug)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Creator)
                    .FirstOrDefaultAsync(p => p.Slug.ToLower() == slug.ToLower());

                if (product == null)
                    return ApiResult<Product?>.NotFoundResult(Messages.Product.NotFound);

                return ApiResult<Product?>.SuccessResult(product, Messages.Product.Retrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<Product?>.FailureResult($"Error retrieving product by slug: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetAllAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Creator)
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving products: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetByCategoryAsync(ProductCategory category)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.Category == category)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving products by category: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetActiveProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.Status == ProductStatus.Active)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving active products: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetActiveProductsByCategoryAsync(ProductCategory category)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.Category == category && p.Status == ProductStatus.Active)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving active products by category: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetFeaturedProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsFeatured && p.Status == ProductStatus.Active)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving featured products: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetNewProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsNewProduct && p.Status == ProductStatus.Active)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving new products: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetProductsOnSaleAsync()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.DiscountPrice.HasValue && p.Price.HasValue &&
                               p.DiscountPrice < p.Price && p.Status == ProductStatus.Active)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving products on sale: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                var term = searchTerm.ToLower();
                var products = await _context.Products
                    .Where(p => p.Status == ProductStatus.Active &&
                               (p.Name.ToLower().Contains(term) ||
                                p.Description!.ToLower().Contains(term) ||
                                p.ShortDescription!.ToLower().Contains(term) ||
                                p.ProductCode!.ToLower().Contains(term)))
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error searching products: {ex.Message}");
            }
        }

        public async Task<ApiResult<Product>> CreateAsync(Product product)
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
                    var maxOrderResult = await GetMaxOrderAsync(product.Category);
                    product.Order = (maxOrderResult?.Data ?? 0) + 1;
                }

                // Auto-generate product code if not provided
                if (string.IsNullOrEmpty(product.ProductCode))
                {
                    product.ProductCode = await GenerateProductCode(product.Category);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return ApiResult<Product>.SuccessResult(product, Messages.Product.Created);
            }
            catch (Exception ex)
            {
                return ApiResult<Product>.FailureResult($"Error creating product: {ex.Message}");
            }
        }

        public async Task<ApiResult<Product>> UpdateAsync(Product product)
        {
            try
            {
                product.UpdatedAt = DateTime.UtcNow;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return ApiResult<Product>.SuccessResult(product, Messages.Product.Updated);
            }
            catch (Exception ex)
            {
                return ApiResult<Product>.FailureResult($"Error updating product: {ex.Message}");
            }
        }

        public async Task<ApiResult> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return ApiResult.FailureResult(Messages.Product.NotFound);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult(Messages.Product.Deleted);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error deleting product: {ex.Message}");
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

        public async Task<ApiResult<int>> GetMaxOrderAsync(ProductCategory? category = null)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                if (category.HasValue)
                {
                    query = query.Where(p => p.Category == category.Value);
                }

                if (!await query.AnyAsync())
                    return ApiResult<int>.SuccessResult(0);

                var maxOrder = await query.MaxAsync(p => p.Order);
                return ApiResult<int>.SuccessResult(maxOrder);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.FailureResult($"Error getting max order: {ex.Message}");
            }
        }

        public async Task<ApiResult> ReorderProductsAsync(IEnumerable<(int Id, int Order)> productOrders)
        {
            try
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

                return ApiResult.SuccessResult("Products reordered successfully");
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error reordering products: {ex.Message}");
            }
        }

        public async Task<ApiResult> IncrementViewCountAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return ApiResult.FailureResult(Messages.Product.NotFound);

                product.ViewCount++;
                await _context.SaveChangesAsync();

                return ApiResult.SuccessResult("View count incremented successfully");
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult($"Error incrementing view count: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetProductsByStatusAsync(ProductStatus status)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.Status == status)
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving products by status: {ex.Message}");
            }
        }

        public async Task<ApiResult<IEnumerable<Product>>> GetProductsWithPriceRangeAsync(decimal? minPrice, decimal? maxPrice, ProductCategory? category = null)
        {
            try
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

                var products = await query
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return ApiResult<IEnumerable<Product>>.SuccessResult(products, Messages.Product.ListRetrieved);
            }
            catch (Exception ex)
            {
                return ApiResult<IEnumerable<Product>>.FailureResult($"Error retrieving products with price range: {ex.Message}");
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