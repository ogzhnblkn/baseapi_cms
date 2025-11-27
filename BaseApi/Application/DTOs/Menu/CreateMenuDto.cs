using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Menu
{
    public class CreateMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public MenuType MenuType { get; set; } = MenuType.Header;
        public MenuLinkType LinkType { get; set; } = MenuLinkType.Internal;
        public bool OpenInNewTab { get; set; } = false;
        public bool ShowForGuests { get; set; } = true;
        public bool ShowForAuthenticated { get; set; } = true;
    }
}