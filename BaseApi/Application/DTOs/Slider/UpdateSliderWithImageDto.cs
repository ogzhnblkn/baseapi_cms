using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Slider
{
    public class UpdateSliderWithImageDto
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public IFormFile? MobileImageFile { get; set; }
        public string? LinkUrl { get; set; }
        public string? ButtonText { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public SliderType SliderType { get; set; }
        public SliderLinkType LinkType { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? TargetLocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}