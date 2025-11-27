using BaseApi.Application.DTOs.Product;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BaseApi.Application.Features.Products.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;

        public GetProductQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product? product = null;

            if (request.Id.HasValue)
            {
                product = await _productRepository.GetByIdAsync(request.Id.Value);
            }
            else if (!string.IsNullOrEmpty(request.Slug))
            {
                product = await _productRepository.GetBySlugAsync(request.Slug);
            }

            if (product == null)
                return null;

            // Increment view count if requested
            if (request.IncrementViewCount)
            {
                await _productRepository.IncrementViewCountAsync(product.Id);
                product.ViewCount++;
            }

            return new ProductDto
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
            };
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