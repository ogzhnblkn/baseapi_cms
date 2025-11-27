using BaseApi.Application.DTOs.Product;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ProductCode { get; set; }
        public ProductCategory Category { get; set; }
        public string? MainImageUrl { get; set; }
        public List<string>? ImageUrls { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Currency { get; set; } = "TRY";
        public bool ShowPrice { get; set; } = true;
        public string? Dimensions { get; set; }
        public string? Material { get; set; }
        public string? Colors { get; set; }
        public Dictionary<string, object>? Features { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public bool IsFeatured { get; set; } = false;
        public bool IsNewProduct { get; set; } = false;
        public int Order { get; set; } = 0;
        public int CreatedBy { get; set; }
    }
}