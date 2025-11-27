using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Menu
{
    public class UpdateMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public MenuType MenuType { get; set; }
        public MenuLinkType LinkType { get; set; }
        public bool OpenInNewTab { get; set; }
        public bool ShowForGuests { get; set; }
        public bool ShowForAuthenticated { get; set; }
    }
}