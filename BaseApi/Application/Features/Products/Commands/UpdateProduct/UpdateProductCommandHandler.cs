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
            if (!string.IsNullOrEmpty(request.Slug) && request.Slug != product.Data.Slug)
            {
                if (await _productRepository.ExistsAsync(request.Slug, request.Id))
                    throw new InvalidOperationException("Slug already exists");
            }

            // Check product code uniqueness
            if (!string.IsNullOrEmpty(request.ProductCode) && request.ProductCode != product.Data.ProductCode)
            {
                var existingProduct = await _productRepository.GetAllAsync();
                if (existingProduct.Data.Any(p => p.ProductCode == request.ProductCode && p.Id != request.Id))
                    throw new InvalidOperationException("Product code already exists");
            }

            product.Data.Name = request.Name;
            product.Data.Slug = request.Slug ?? product.Data.Slug;
            product.Data.ShortDescription = request.ShortDescription;
            product.Data.Description = request.Description;
            product.Data.ProductCode = request.ProductCode;
            product.Data.Category = request.Category;
            product.Data.MainImageUrl = request.MainImageUrl;
            product.Data.ImageUrls = request.ImageUrls != null ? JsonSerializer.Serialize(request.ImageUrls) : null;
            product.Data.Price = request.Price;
            product.Data.DiscountPrice = request.DiscountPrice;
            product.Data.Currency = request.Currency;
            product.Data.ShowPrice = request.ShowPrice;
            product.Data.Dimensions = request.Dimensions;
            product.Data.Material = request.Material;
            product.Data.Colors = request.Colors;
            product.Data.Features = request.Features != null ? JsonSerializer.Serialize(request.Features) : null;
            product.Data.MetaTitle = request.MetaTitle;
            product.Data.MetaDescription = request.MetaDescription;
            product.Data.Keywords = request.Keywords;
            product.Data.Status = request.Status;
            product.Data.IsFeatured = request.IsFeatured;
            product.Data.IsNewProduct = request.IsNewProduct;
            product.Data.Order = request.Order;

            var updatedProduct = await _productRepository.UpdateAsync(product.Data);

            return new ProductDto
            {
                Id = updatedProduct.Data.Id,
                Name = updatedProduct.Data.Name,
                Slug = updatedProduct.Data.Slug,
                ShortDescription = updatedProduct.Data.ShortDescription,
                Description = updatedProduct.Data.Description,
                ProductCode = updatedProduct.Data.ProductCode,
                Category = updatedProduct.Data.Category,
                CategoryName = GetCategoryDisplayName(updatedProduct.Data.Category),
                MainImageUrl = updatedProduct.Data.MainImageUrl,
                ImageUrls = !string.IsNullOrEmpty(updatedProduct.Data.ImageUrls)
                    ? JsonSerializer.Deserialize<List<string>>(updatedProduct.Data.ImageUrls) ?? new List<string>()
                    : new List<string>(),
                Price = updatedProduct.Data.Price,
                DiscountPrice = updatedProduct.Data.DiscountPrice,
                Currency = updatedProduct.Data.Currency,
                ShowPrice = updatedProduct.Data.ShowPrice,
                Dimensions = updatedProduct.Data.Dimensions,
                Material = updatedProduct.Data.Material,
                Colors = updatedProduct.Data.Colors,
                Features = !string.IsNullOrEmpty(updatedProduct.Data.Features)
                    ? JsonSerializer.Deserialize<Dictionary<string, object>>(updatedProduct.Data.Features)
                    : null,
                MetaTitle = updatedProduct.Data.MetaTitle,
                MetaDescription = updatedProduct.Data.MetaDescription,
                Keywords = updatedProduct.Data.Keywords,
                Status = updatedProduct.Data.Status,
                StatusName = GetStatusDisplayName(updatedProduct.Data.Status),
                IsFeatured = updatedProduct.Data.IsFeatured,
                IsNewProduct = updatedProduct.Data.IsNewProduct,
                Order = updatedProduct.Data.Order,
                ViewCount = updatedProduct.Data.ViewCount,
                CreatedAt = updatedProduct.Data.CreatedAt,
                UpdatedAt = updatedProduct.Data.UpdatedAt,
                CreatorName = updatedProduct.Data.Creator != null ? $"{updatedProduct.Data.Creator.FirstName} {updatedProduct.Data.Creator.LastName}" : null
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