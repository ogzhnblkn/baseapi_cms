using BaseApi.Domain.Entities;

namespace BaseApi.Application.DTOs.Slider
{
    public class CreateSliderWithImageDto
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; } // Fallback if no file uploaded
        public IFormFile? MobileImageFile { get; set; }
        public string? MobileImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public string? ButtonText { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public SliderType SliderType { get; set; } = SliderType.MainSlider;
        public SliderLinkType LinkType { get; set; } = SliderLinkType.None;
        public bool OpenInNewTab { get; set; } = false;
        public string? TargetLocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}