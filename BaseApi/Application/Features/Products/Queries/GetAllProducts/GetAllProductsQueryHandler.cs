using BaseApi.Application.DTOs.Product;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BaseApi.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Product> products;

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                products = await _productRepository.SearchProductsAsync(request.SearchTerm);
            }
            else if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
            {
                products = await _productRepository.GetProductsWithPriceRangeAsync(request.MinPrice, request.MaxPrice, request.Category);
            }
            else if (request.Category.HasValue && request.ActiveOnly)
            {
                products = await _productRepository.GetActiveProductsByCategoryAsync(request.Category.Value);
            }
            else if (request.Category.HasValue)
            {
                products = await _productRepository.GetByCategoryAsync(request.Category.Value);
            }
            else if (request.ActiveOnly)
            {
                products = await _productRepository.GetActiveProductsAsync();
            }
            else if (request.Status.HasValue)
            {
                products = await _productRepository.GetProductsByStatusAsync(request.Status.Value);
            }
            else
            {
                products = await _productRepository.GetAllAsync();
            }

            // Apply additional filters
            if (request.IsFeatured.HasValue)
            {
                products = products.Where(p => p.IsFeatured == request.IsFeatured.Value);
            }

            if (request.IsNewProduct.HasValue)
            {
                products = products.Where(p => p.IsNewProduct == request.IsNewProduct.Value);
            }

            return products.Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                ProductCode = product.ProductCode,
                Category = product.Category,
                CategoryName = GetCategoryDisplayName(product.Category),
                MainImageUrl = product.MainImageUrl,
                ImageUrls = !string.IsNullOrEmpty(product.ImageUrls)
                    ? JsonSerializer.Deserialize<List<string>>(product.ImageUrls) ?? new List<string>()
                    : new List<string>(),
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Currency = product.Currency,
                ShowPrice = product.ShowPrice,
                Dimensions = product.Dimensions,
                Material = product.Material,
                Colors = product.Colors,
                Features = !string.IsNullOrEmpty(product.Features)
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(product.Features)
                    : null,
                MetaTitle = product.MetaTitle,
                MetaDescription = product.MetaDescription,
                Keywords = product.Keywords,
                Status = product.Status,
                StatusName = GetStatusDisplayName(product.Status),
                IsFeatured = product.IsFeatured,
                IsNewProduct = product.IsNewProduct,
                Order = product.Order,
                ViewCount = product.ViewCount,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CreatorName = product.Creator != null ? $"{product.Creator.FirstName} {product.Creator.LastName}" : null
            });
        }

        private static string GetCategoryDisplayName(ProductCategory category)
        {
            var field = typeof(ProductCategory).GetField(category.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? category.ToString();
        }

        private static string GetStatusDisplayName(ProductStatus status)
        {
            var field = typeof(ProductStatus).GetField(status.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? status.ToString();
        }
    }
}