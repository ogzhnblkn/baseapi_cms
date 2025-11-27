using BaseApi.Application.Common;
using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<ApiResult<Product?>> GetByIdAsync(int id);
        Task<ApiResult<Product?>> GetBySlugAsync(string slug);
        Task<ApiResult<IEnumerable<Product>>> GetAllAsync();
        Task<ApiResult<IEnumerable<Product>>> GetByCategoryAsync(ProductCategory category);
        Task<ApiResult<IEnumerable<Product>>> GetActiveProductsAsync();
        Task<ApiResult<IEnumerable<Product>>> GetActiveProductsByCategoryAsync(ProductCategory category);
        Task<ApiResult<IEnumerable<Product>>> GetFeaturedProductsAsync();
        Task<ApiResult<IEnumerable<Product>>> GetNewProductsAsync();
        Task<ApiResult<IEnumerable<Product>>> GetProductsOnSaleAsync();
        Task<ApiResult<IEnumerable<Product>>> SearchProductsAsync(string searchTerm);
        Task<ApiResult<Product>> CreateAsync(Product product);
        Task<ApiResult<Product>> UpdateAsync(Product product);
        Task<ApiResult> DeleteAsync(int id);
        Task<bool> ExistsAsync(string slug, int? excludeId = null);
        Task<ApiResult<int>> GetMaxOrderAsync(ProductCategory? category = null);
        Task<ApiResult> ReorderProductsAsync(IEnumerable<(int Id, int Order)> productOrders);
        Task<ApiResult> IncrementViewCountAsync(int id);
        Task<ApiResult<IEnumerable<Product>>> GetProductsByStatusAsync(ProductStatus status);
        Task<ApiResult<IEnumerable<Product>>> GetProductsWithPriceRangeAsync(decimal? minPrice, decimal? maxPrice, ProductCategory? category = null);
    }
}