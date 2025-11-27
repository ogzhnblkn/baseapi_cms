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
            var productsResult = await GetProductsAsync(request);

            // If the result failed, return empty collection
            if (!productsResult.Success)
            {
                return Enumerable.Empty<ProductDto>();
            }

            var products = productsResult.Data ?? Enumerable.Empty<Domain.Entities.Product>();

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

        private async Task<BaseApi.Application.Common.ApiResult<IEnumerable<Domain.Entities.Product>>> GetProductsAsync(GetAllProductsQuery request)
        {
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                return await _productRepository.SearchProductsAsync(request.SearchTerm);
            }
            else if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
            {
                return await _productRepository.GetProductsWithPriceRangeAsync(request.MinPrice, request.MaxPrice, request.Category);
            }
            else if (request.Category.HasValue && request.ActiveOnly)
            {
                return await _productRepository.GetActiveProductsByCategoryAsync(request.Category.Value);
            }
            else if (request.Category.HasValue)
            {
                return await _productRepository.GetByCategoryAsync(request.Category.Value);
            }
            else if (request.ActiveOnly)
            {
                return await _productRepository.GetActiveProductsAsync();
            }
            else if (request.Status.HasValue)
            {
                return await _productRepository.GetProductsByStatusAsync(request.Status.Value);
            }
            else
            {
                return await _productRepository.GetAllAsync();
            }
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