using BaseApi.Application.DTOs.Product;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BaseApi.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Validate price logic
            if (request.Price.HasValue && request.DiscountPrice.HasValue && request.DiscountPrice >= request.Price)
            {
                throw new InvalidOperationException("Discount price must be less than regular price");
            }

            var product = new Product
            {
                Name = request.Name,
                Slug = request.Slug ?? string.Empty,
                ShortDescription = request.ShortDescription,
                Description = request.Description,
                ProductCode = request.ProductCode,
                Category = request.Category,
                MainImageUrl = request.MainImageUrl,
                ImageUrls = request.ImageUrls != null ? JsonSerializer.Serialize(request.ImageUrls) : null,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                Currency = request.Currency,
                ShowPrice = request.ShowPrice,
                Dimensions = request.Dimensions,
                Material = request.Material,
                Colors = request.Colors,
                Features = request.Features != null ? JsonSerializer.Serialize(request.Features) : null,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                Keywords = request.Keywords,
                Status = request.Status,
                IsFeatured = request.IsFeatured,
                IsNewProduct = request.IsNewProduct,
                Order = request.Order,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateAsync(product);

            return new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Slug = createdProduct.Slug,
                ShortDescription = createdProduct.ShortDescription,
                Description = createdProduct.Description,
                ProductCode = createdProduct.ProductCode,
                Category = createdProduct.Category,
                CategoryName = GetCategoryDisplayName(createdProduct.Category),
                MainImageUrl = createdProduct.MainImageUrl,
                ImageUrls = !string.IsNullOrEmpty(createdProduct.ImageUrls)
                    ? JsonSerializer.Deserialize<List<string>>(createdProduct.ImageUrls) ?? new List<string>()
                    : new List<string>(),
                Price = createdProduct.Price,
                DiscountPrice = createdProduct.DiscountPrice,
                Currency = createdProduct.Currency,
                ShowPrice = createdProduct.ShowPrice,
                Dimensions = createdProduct.Dimensions,
                Material = createdProduct.Material,
                Colors = createdProduct.Colors,
                Features = !string.IsNullOrEmpty(createdProduct.Features)
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(createdProduct.Features)
                    : null,
                MetaTitle = createdProduct.MetaTitle,
                MetaDescription = createdProduct.MetaDescription,
                Keywords = createdProduct.Keywords,
                Status = createdProduct.Status,
                StatusName = GetStatusDisplayName(createdProduct.Status),
                IsFeatured = createdProduct.IsFeatured,
                IsNewProduct = createdProduct.IsNewProduct,
                Order = createdProduct.Order,
                ViewCount = createdProduct.ViewCount,
                CreatedAt = createdProduct.CreatedAt,
                UpdatedAt = createdProduct.UpdatedAt
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