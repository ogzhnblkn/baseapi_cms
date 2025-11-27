using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseApi.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ShortDescription { get; set; }

        [StringLength(5000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ProductCode { get; set; }

        public ProductCategory Category { get; set; }

        /// <summary>
        /// Ana ürün görseli
        /// </summary>
        [StringLength(1000)]
        public string? MainImageUrl { get; set; }

        /// <summary>
        /// Ürün galeri görselleri (JSON array olarak)
        /// </summary>
        [StringLength(5000)]
        public string? ImageUrls { get; set; }

        /// <summary>
        /// Opsiyonel fiyat bilgisi
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Ýndirimli fiyat (opsiyonel)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        /// <summary>
        /// Para birimi (TRY, USD, EUR)
        /// </summary>
        [StringLength(3)]
        public string Currency { get; set; } = "TRY";

        /// <summary>
        /// Fiyat görünürlüðü (fiyat girilmiþse gösterilsin mi?)
        /// </summary>
        public bool ShowPrice { get; set; } = true;

        /// <summary>
        /// Ürün boyutlarý (JSON olarak)
        /// </summary>
        [StringLength(1000)]
        public string? Dimensions { get; set; }

        /// <summary>
        /// Ürün materyali
        /// </summary>
        [StringLength(200)]
        public string? Material { get; set; }

        /// <summary>
        /// Ürün rengi/renkleri
        /// </summary>
        [StringLength(200)]
        public string? Colors { get; set; }

        /// <summary>
        /// Ürün özellikleri (JSON olarak)
        /// </summary>
        [StringLength(3000)]
        public string? Features { get; set; }

        /// <summary>
        /// SEO meta title
        /// </summary>
        [StringLength(200)]
        public string? MetaTitle { get; set; }

        /// <summary>
        /// SEO meta description
        /// </summary>
        [StringLength(500)]
        public string? MetaDescription { get; set; }

        /// <summary>
        /// SEO keywords
        /// </summary>
        [StringLength(500)]
        public string? Keywords { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Active;

        public bool IsFeatured { get; set; } = false;

        public bool IsNewProduct { get; set; } = false;

        public int Order { get; set; } = 0;

        /// <summary>
        /// Görüntülenme sayýsý
        /// </summary>
        public int ViewCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        // Navigation Properties
        public User? Creator { get; set; }
    }

    public enum ProductCategory
    {
        [Display(Name = "Oturma Grubu")]
        LivingRoom = 1,

        [Display(Name = "Yatak Odasý")]
        Bedroom = 2,

        [Display(Name = "Yemek Odasý")]
        DiningRoom = 3,

        [Display(Name = "Çocuk Odasý")]
        ChildRoom = 4,

        [Display(Name = "Çalýþma Odasý")]
        Office = 5,

        [Display(Name = "Mutfak")]
        Kitchen = 6,

        [Display(Name = "Banyo")]
        Bathroom = 7,

        [Display(Name = "Bahçe & Balkon")]
        Garden = 8,

        [Display(Name = "Koltuk")]
        Armchair = 9,

        [Display(Name = "Kanepe")]
        Sofa = 10,

        [Display(Name = "Sandalye")]
        Chair = 11,

        [Display(Name = "Masa")]
        Table = 12,

        [Display(Name = "Dolap")]
        Wardrobe = 13,

        [Display(Name = "Yatak")]
        Bed = 14,

        [Display(Name = "Þifonyer")]
        Dresser = 15,

        [Display(Name = "Komodin")]
        Nightstand = 16,

        [Display(Name = "Kitaplýk")]
        Bookshelf = 17,

        [Display(Name = "TV Ünitesi")]
        TvUnit = 18,

        [Display(Name = "Ayakkabý Dolabý")]
        ShoeRack = 19,

        [Display(Name = "Vestiyer")]
        CoatRack = 20,

        [Display(Name = "Aynalýk")]
        Mirror = 21,

        [Display(Name = "Puf")]
        Ottoman = 22,

        [Display(Name = "Sehpa")]
        CoffeeTable = 23,

        [Display(Name = "Konsol")]
        Console = 24,

        [Display(Name = "Vitrin")]
        Showcase = 25,

        [Display(Name = "Aksesuar")]
        Accessory = 26,

        [Display(Name = "Aydýnlatma")]
        Lighting = 27,

        [Display(Name = "Halý & Kilim")]
        Carpet = 28,

        [Display(Name = "Perde")]
        Curtain = 29,

        [Display(Name = "Diðer")]
        Other = 99
    }

    public enum ProductStatus
    {
        [Display(Name = "Aktif")]
        Active = 1,

        [Display(Name = "Pasif")]
        Inactive = 2,

        [Display(Name = "Taslak")]
        Draft = 3,

        [Display(Name = "Stokta Yok")]
        OutOfStock = 4,

        [Display(Name = "Üretimde")]
        InProduction = 5,

        [Display(Name = "Yeni")]
        New = 6,

        [Display(Name = "Ýndirimde")]
        OnSale = 7
    }
}