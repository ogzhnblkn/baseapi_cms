using System.ComponentModel.DataAnnotations;

namespace BaseApi.Domain.Entities
{
    public class Slider
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Subtitle { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(1000)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LinkUrl { get; set; }

        [StringLength(100)]
        public string? ButtonText { get; set; }

        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public SliderType SliderType { get; set; } = SliderType.MainSlider;

        public SliderLinkType LinkType { get; set; } = SliderLinkType.None;

        public bool OpenInNewTab { get; set; } = false;

        /// <summary>
        /// Slider'ýn hangi sayfada/konumda gösterileceði (örn: "homepage", "category-oturma-grubu")
        /// </summary>
        [StringLength(100)]
        public string? TargetLocation { get; set; }

        /// <summary>
        /// Responsive design için mobile görsel URL'i (opsiyonel)
        /// </summary>
        [StringLength(1000)]
        public string? MobileImageUrl { get; set; }

        /// <summary>
        /// Gösterilme baþlangýç tarihi (opsiyonel)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gösterilme bitiþ tarihi (opsiyonel)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Týklanma sayýsý (analytics için)
        /// </summary>
        public int ClickCount { get; set; } = 0;

        /// <summary>
        /// Görüntülenme sayýsý (analytics için)
        /// </summary>
        public int ViewCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        // Navigation Properties
        public User? Creator { get; set; }
    }

    public enum SliderType
    {
        MainSlider = 1,        // Ana sayfa büyük slider
        BannerSlider = 2,      // Banner tipinde küçük slider
        CategorySlider = 3,    // Kategori sayfasý slider'ý
        ProductSlider = 4,     // Ürün detay slider'ý
        PromoSlider = 5,       // Promosyon banner'larý
        SidebarSlider = 6,     // Yan panel slider'larý
        FooterSlider = 7,      // Footer bölümü slider'larý
        MobileSlider = 8       // Mobil özel slider'larý
    }

    public enum SliderLinkType
    {
        None = 1,           // Link yok
        Internal = 2,       // Site içi link
        External = 3,       // Dýþ link
        Product = 4,        // Ürün sayfasý
        Category = 5,       // Kategori sayfasý
        Page = 6,           // Statik sayfa
        Contact = 7,        // Ýletiþim formu
        WhatsApp = 8,       // WhatsApp link
        Phone = 9,          // Telefon link
        Email = 10          // Email link
    }
}