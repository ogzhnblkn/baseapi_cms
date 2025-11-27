using BaseApi.Domain.Entities;

namespace BaseApi.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetBySlugAsync(string slug);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> GetActiveProductsByCategoryAsync(ProductCategory category);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        Task<IEnumerable<Product>> GetNewProductsAsync();
        Task<IEnumerable<Product>> GetProductsOnSaleAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(string slug, int? excludeId = null);
        Task<int> GetMaxOrderAsync(ProductCategory? category = null);
        Task ReorderProductsAsync(IEnumerable<(int Id, int Order)> productOrders);
        Task IncrementViewCountAsync(int id);
        Task<IEnumerable<Product>> GetProductsByStatusAsync(ProductStatus status);
        Task<IEnumerable<Product>> GetProductsWithPriceRangeAsync(decimal? minPrice, decimal? maxPrice, ProductCategory? category = null);
    }
}