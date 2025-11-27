using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Commands.UpdateSlider
{
    public class UpdateSliderCommand : IRequest<SliderDto>
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
        public SliderLinkType LinkType { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? TargetLocation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}