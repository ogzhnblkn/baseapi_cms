using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Commands.CreateSlider
{
    public class CreateSliderCommandHandler : IRequestHandler<CreateSliderCommand, SliderDto>
    {
        private readonly ISliderRepository _sliderRepository;

        public CreateSliderCommandHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }

        public async Task<SliderDto> Handle(CreateSliderCommand request, CancellationToken cancellationToken)
        {
            // Validate date range
            if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate > request.EndDate)
            {
                throw new InvalidOperationException("Start date cannot be later than end date");
            }

            var slider = new Slider
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                MobileImageUrl = request.MobileImageUrl,
                LinkUrl = request.LinkUrl,
                ButtonText = request.ButtonText,
                Order = request.Order,
                IsActive = request.IsActive,
                SliderType = request.SliderType,
                LinkType = request.LinkType,
                OpenInNewTab = request.OpenInNewTab,
                TargetLocation = request.TargetLocation,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdSlider = await _sliderRepository.CreateAsync(slider);

            return new SliderDto
            {
                Id = createdSlider.Id,
                Title = createdSlider.Title,
                Subtitle = createdSlider.Subtitle,
                Description = createdSlider.Description,
                ImageUrl = createdSlider.ImageUrl,
                MobileImageUrl = createdSlider.MobileImageUrl,
                LinkUrl = createdSlider.LinkUrl,
                ButtonText = createdSlider.ButtonText,
                Order = createdSlider.Order,
                IsActive = createdSlider.IsActive,
                SliderType = createdSlider.SliderType,
                SliderTypeName = createdSlider.SliderType.ToString(),
                LinkType = createdSlider.LinkType,
                LinkTypeName = createdSlider.LinkType.ToString(),
                OpenInNewTab = createdSlider.OpenInNewTab,
                TargetLocation = createdSlider.TargetLocation,
                StartDate = createdSlider.StartDate,
                EndDate = createdSlider.EndDate,
                ClickCount = createdSlider.ClickCount,
                ViewCount = createdSlider.ViewCount,
                CreatedAt = createdSlider.CreatedAt,
                UpdatedAt = createdSlider.UpdatedAt
            };
        }
    }
}