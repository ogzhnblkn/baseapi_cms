using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Queries.GetSlider
{
    public class GetSliderQueryHandler : IRequestHandler<GetSliderQuery, SliderDto?>
    {
        private readonly ISliderRepository _sliderRepository;

        public GetSliderQueryHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }

        public async Task<SliderDto?> Handle(GetSliderQuery request, CancellationToken cancellationToken)
        {
            var slider = await _sliderRepository.GetByIdAsync(request.Id);
            if (slider == null)
                return null;

            return new SliderDto
            {
                Id = slider.Id,
                Title = slider.Title,
                Subtitle = slider.Subtitle,
                Description = slider.Description,
                ImageUrl = slider.ImageUrl,
                MobileImageUrl = slider.MobileImageUrl,
                LinkUrl = slider.LinkUrl,
                ButtonText = slider.ButtonText,
                Order = slider.Order,
                IsActive = slider.IsActive,
                SliderType = slider.SliderType,
                SliderTypeName = slider.SliderType.ToString(),
                LinkType = slider.LinkType,
                LinkTypeName = slider.LinkType.ToString(),
                OpenInNewTab = slider.OpenInNewTab,
                TargetLocation = slider.TargetLocation,
                StartDate = slider.StartDate,
                EndDate = slider.EndDate,
                ClickCount = slider.ClickCount,
                ViewCount = slider.ViewCount,
                CreatedAt = slider.CreatedAt,
                UpdatedAt = slider.UpdatedAt,
                CreatorName = slider.Creator != null ? $"{slider.Creator.FirstName} {slider.Creator.LastName}" : null
            };
        }
    }
}