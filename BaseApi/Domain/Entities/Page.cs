using System.ComponentModel.DataAnnotations;

namespace BaseApi.Domain.Entities
{
    public class Page
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Summary { get; set; }

        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Sayfa görseli
        /// </summary>
        [StringLength(1000)]
        public string? FeaturedImageUrl { get; set; }

        /// <summary>
        /// Sayfa þablonu (default, contact, about, custom vb.)
        /// </summary>
        public PageTemplate Template { get; set; } = PageTemplate.Default;

        /// <summary>
        /// Sayfa durumu
        /// </summary>
        public PageStatus Status { get; set; } = PageStatus.Draft;

        /// <summary>
        /// Ana sayfa olarak iþaretle
        /// </summary>
        public bool IsHomePage { get; set; } = false;

        /// <summary>
        /// Sayfa görünürlüðü
        /// </summary>
        public PageVisibility Visibility { get; set; } = PageVisibility.Public;

        /// <summary>
        /// Yorum yapýlabilir mi?
        /// </summary>
        public bool AllowComments { get; set; } = false;

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

        /// <summary>
        /// Canonical URL
        /// </summary>
        [StringLength(500)]
        public string? CanonicalUrl { get; set; }

        /// <summary>
        /// Custom CSS
        /// </summary>
        public string? CustomCss { get; set; }

        /// <summary>
        /// Custom JavaScript
        /// </summary>
        public string? CustomJs { get; set; }

        /// <summary>
        /// Sayfa sýrasý
        /// </summary>
        public int Order { get; set; } = 0;

        /// <summary>
        /// Görüntülenme sayýsý
        /// </summary>
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// Yayýnlanma tarihi
        /// </summary>
        public DateTime? PublishedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        // Navigation Properties
        public User? Creator { get; set; }
        public User? Updater { get; set; }
    }

    public enum PageTemplate
    {
        [Display(Name = "Varsayýlan")]
        Default = 1,

        [Display(Name = "Ýletiþim")]
        Contact = 2,

        [Display(Name = "Hakkýmýzda")]
        About = 3,

        [Display(Name = "Gizlilik Politikasý")]
        Privacy = 4,

        [Display(Name = "Kullaným Koþullarý")]
        Terms = 5,

        [Display(Name = "SSS")]
        Faq = 6,

        [Display(Name = "Galeri")]
        Gallery = 7,

        [Display(Name = "Hizmetler")]
        Services = 8,

        [Display(Name = "Ana Sayfa")]
        HomePage = 9,

        [Display(Name = "Özel Þablon")]
        Custom = 99
    }

    public enum PageStatus
    {
        [Display(Name = "Taslak")]
        Draft = 1,

        [Display(Name = "Yayýnda")]
        Published = 2,

        [Display(Name = "Arþiv")]
        Archived = 3,

        [Display(Name = "Çöp")]
        Trash = 4,

        [Display(Name = "Planlanmýþ")]
        Scheduled = 5
    }

    public enum PageVisibility
    {
        [Display(Name = "Herkese Açýk")]
        Public = 1,

        [Display(Name = "Sadece Giriþ Yapanlar")]
        Private = 2,

        [Display(Name = "Parola Korumalý")]
        Protected = 3,

        [Display(Name = "Sadece Yöneticiler")]
        AdminOnly = 4
    }
}