using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ProductCode { get; set; }
        public ProductCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? MainImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Currency { get; set; } = "TRY";
        public bool ShowPrice { get; set; }
        public string? Dimensions { get; set; }
        public string? Material { get; set; }
        public string? Colors { get; set; }
        public Dictionary<string, object>? Features { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public ProductStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public bool IsNewProduct { get; set; }
        public int Order { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatorName { get; set; }
    }
}