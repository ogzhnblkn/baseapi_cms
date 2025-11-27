using BaseApi.Application.DTOs.Product;
using BaseApi.Application.Exceptions;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BaseApi.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new NotFoundException($"Product with ID {request.Id} not found");

            // Validate price logic
            if (request.Price.HasValue && request.DiscountPrice.HasValue && request.DiscountPrice >= request.Price)
            {
                throw new InvalidOperationException("Discount price must be less than regular price");
            }

            // Check slug uniqueness
            if (!string.IsNullOrEmpty(request.Slug) && request.Slug != product.Slug)
            {
                if (await _productRepository.ExistsAsync(request.Slug, request.Id))
                    throw new InvalidOperationException("Slug already exists");
            }

            // Check product code uniqueness
            if (!string.IsNullOrEmpty(request.ProductCode) && request.ProductCode != product.ProductCode)
            {
                var existingProduct = await _productRepository.GetAllAsync();
                if (existingProduct.Any(p => p.ProductCode == request.ProductCode && p.Id != request.Id))
                    throw new InvalidOperationException("Product code already exists");
            }

            product.Name = request.Name;
            product.Slug = request.Slug ?? product.Slug;
            product.ShortDescription = request.ShortDescription;
            product.Description = request.Description;
            product.ProductCode = request.ProductCode;
            product.Category = request.Category;
            product.MainImageUrl = request.MainImageUrl;
            product.ImageUrls = request.ImageUrls != null ? JsonSerializer.Serialize(request.ImageUrls) : null;
            product.Price = request.Price;
            product.DiscountPrice = request.DiscountPrice;
            product.Currency = request.Currency;
            product.ShowPrice = request.ShowPrice;
            product.Dimensions = request.Dimensions;
            product.Material = request.Material;
            product.Colors = request.Colors;
            product.Features = request.Features != null ? JsonSerializer.Serialize(request.Features) : null;
            product.MetaTitle = request.MetaTitle;
            product.MetaDescription = request.MetaDescription;
            product.Keywords = request.Keywords;
            product.Status = request.Status;
            product.IsFeatured = request.IsFeatured;
            product.IsNewProduct = request.IsNewProduct;
            product.Order = request.Order;

            var updatedProduct = await _productRepository.UpdateAsync(product);

            return new ProductDto
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Slug = updatedProduct.Slug,
                ShortDescription = updatedProduct.ShortDescription,
                Description = updatedProduct.Description,
                ProductCode = updatedProduct.ProductCode,
                Category = updatedProduct.Category,
                CategoryName = GetCategoryDisplayName(updatedProduct.Category),
                MainImageUrl = updatedProduct.MainImageUrl,
                ImageUrls = !string.IsNullOrEmpty(updatedProduct.ImageUrls)
                    ? JsonSerializer.Deserialize<List<string>>(updatedProduct.ImageUrls) ?? new List<string>()
                    : new List<string>(),
                Price = updatedProduct.Price,
                DiscountPrice = updatedProduct.DiscountPrice,
                Currency = updatedProduct.Currency,
                ShowPrice = updatedProduct.ShowPrice,
                Dimensions = updatedProduct.Dimensions,
                Material = updatedProduct.Material,
                Colors = updatedProduct.Colors,
                Features = !string.IsNullOrEmpty(updatedProduct.Features)
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(updatedProduct.Features)
                    : null,
                MetaTitle = updatedProduct.MetaTitle,
                MetaDescription = updatedProduct.MetaDescription,
                Keywords = updatedProduct.Keywords,
                Status = updatedProduct.Status,
                StatusName = GetStatusDisplayName(updatedProduct.Status),
                IsFeatured = updatedProduct.IsFeatured,
                IsNewProduct = updatedProduct.IsNewProduct,
                Order = updatedProduct.Order,
                ViewCount = updatedProduct.ViewCount,
                CreatedAt = updatedProduct.CreatedAt,
                UpdatedAt = updatedProduct.UpdatedAt,
                CreatorName = updatedProduct.Creator != null ? $"{updatedProduct.Creator.FirstName} {updatedProduct.Creator.LastName}" : null
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