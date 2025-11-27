using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Slider
{
    public class SliderDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? MobileImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public string? ButtonText { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public SliderType SliderType { get; set; }
        public string SliderTypeName { get; set; } = string.Empty;
        public SliderLinkType LinkType { get; set; }
        public string LinkTypeName { get; set; } = string.Empty;
        public bool OpenInNewTab { get; set; }
        public string? TargetLocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ClickCount { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatorName { get; set; }
    }
}