using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Product
{
    public class CreateProductWithImagesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ProductCode { get; set; }
        public ProductCategory Category { get; set; }
        public IFormFile? MainImageFile { get; set; }
        public string? MainImageUrl { get; set; } // Fallback if no file uploaded
        public IFormFileCollection? ImageFiles { get; set; }
        public List<string>? ImageUrls { get; set; } // Fallback URLs
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string Currency { get; set; } = "TRY";
        public bool ShowPrice { get; set; } = true;
        public string? Dimensions { get; set; }
        public string? Material { get; set; }
        public string? Colors { get; set; }
        public string? FeaturesJson { get; set; } // JSON string for features
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public bool IsFeatured { get; set; } = false;
        public bool IsNewProduct { get; set; } = false;
        public int Order { get; set; } = 0;
    }
}