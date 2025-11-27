using BaseApi.Application.DTOs.Slider;
using BaseApi.Application.Exceptions;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Commands.UpdateSlider
{
    public class UpdateSliderCommandHandler : IRequestHandler<UpdateSliderCommand, SliderDto>
    {
        private readonly ISliderRepository _sliderRepository;

        public UpdateSliderCommandHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }

        public async Task<SliderDto> Handle(UpdateSliderCommand request, CancellationToken cancellationToken)
        {
            var sliderResult = await _sliderRepository.GetByIdAsync(request.Id);
            if (!sliderResult.Success || sliderResult.Data == null)
                throw new NotFoundException($"Slider with ID {request.Id} not found");

            var slider = sliderResult.Data;

            // Validate date range
            if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate > request.EndDate)
            {
                throw new InvalidOperationException("Start date cannot be later than end date");
            }

            slider.Title = request.Title;
            slider.Subtitle = request.Subtitle;
            slider.Description = request.Description;
            slider.ImageUrl = request.ImageUrl;
            slider.MobileImageUrl = request.MobileImageUrl;
            slider.LinkUrl = request.LinkUrl;
            slider.ButtonText = request.ButtonText;
            slider.Order = request.Order;
            slider.IsActive = request.IsActive;
            slider.SliderType = request.SliderType;
            slider.LinkType = request.LinkType;
            slider.OpenInNewTab = request.OpenInNewTab;
            slider.TargetLocation = request.TargetLocation;
            slider.StartDate = request.StartDate;
            slider.EndDate = request.EndDate;

            var updateResult = await _sliderRepository.UpdateAsync(slider);
            if (!updateResult.Success)
                throw new InvalidOperationException(updateResult.Message);

            var updatedSlider = updateResult.Data!;

            return new SliderDto
            {
                Id = updatedSlider.Id,
                Title = updatedSlider.Title,
                Subtitle = updatedSlider.Subtitle,
                Description = updatedSlider.Description,
                ImageUrl = updatedSlider.ImageUrl,
                MobileImageUrl = updatedSlider.MobileImageUrl,
                LinkUrl = updatedSlider.LinkUrl,
                ButtonText = updatedSlider.ButtonText,
                Order = updatedSlider.Order,
                IsActive = updatedSlider.IsActive,
                SliderType = updatedSlider.SliderType,
                SliderTypeName = updatedSlider.SliderType.ToString(),
                LinkType = updatedSlider.LinkType,
                LinkTypeName = updatedSlider.LinkType.ToString(),
                OpenInNewTab = updatedSlider.OpenInNewTab,
                TargetLocation = updatedSlider.TargetLocation,
                StartDate = updatedSlider.StartDate,
                EndDate = updatedSlider.EndDate,
                ClickCount = updatedSlider.ClickCount,
                ViewCount = updatedSlider.ViewCount,
                CreatedAt = updatedSlider.CreatedAt,
                UpdatedAt = updatedSlider.UpdatedAt,
                CreatorName = updatedSlider.Creator != null ? $"{updatedSlider.Creator.FirstName} {updatedSlider.Creator.LastName}" : null
            };
        }
    }
}