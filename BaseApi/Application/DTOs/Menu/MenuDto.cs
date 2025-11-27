using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public MenuType MenuType { get; set; }
        public string MenuTypeName { get; set; } = string.Empty;
        public MenuLinkType LinkType { get; set; }
        public string LinkTypeName { get; set; } = string.Empty;
        public bool OpenInNewTab { get; set; }
        public bool ShowForGuests { get; set; }
        public bool ShowForAuthenticated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<MenuDto> SubMenus { get; set; } = new();
    }
}