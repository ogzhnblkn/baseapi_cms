using System.ComponentModel.DataAnnotations;

namespace BaseApi.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Url { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public int? ParentId { get; set; }

        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public MenuType MenuType { get; set; } = MenuType.Header;

        public MenuLinkType LinkType { get; set; } = MenuLinkType.Internal;

        public bool OpenInNewTab { get; set; } = false;

        public bool ShowForGuests { get; set; } = true;

        public bool ShowForAuthenticated { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int CreatedBy { get; set; }

        // Navigation Properties
        public Menu? Parent { get; set; }

        public ICollection<Menu> SubMenus { get; set; } = new List<Menu>();

        public User? Creator { get; set; }
    }

    public enum MenuType
    {
        Header = 1,
        Footer = 2,
        Sidebar = 3,
        Mobile = 4
    }

    public enum MenuLinkType
    {
        Internal = 1,    // Site içi link
        External = 2,    // Dýþ link
        Page = 3,        // Sayfa linki
        Category = 4,    // Kategori linki
        Product = 5,     // Ürün linki
        Custom = 6       // Özel link
    }
}