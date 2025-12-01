using System.ComponentModel.DataAnnotations;

namespace BaseApi.Domain.Entities
{
    public class SocialMediaLink
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Icon { get; set; }

        [StringLength(1000)]
        public string? ImageUrl { get; set; }


        public bool IsActive { get; set; } = true;

        public bool OpenInNewTab { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gösterilme sýrasý
        /// </summary>
        public int Order { get; set; } = 0;



        /// <summary>
        /// Týklanma sayýsý
        /// </summary>
        public int ClickCount { get; set; } = 0;

        /// <summary>
        /// Hex renk kodu (örn: #1DA1F2 for Twitter)
        /// </summary>
        [StringLength(7)]
        public string? ColorCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        // Navigation Properties
        public User? Creator { get; set; }
        public User? Updater { get; set; }
    }


}