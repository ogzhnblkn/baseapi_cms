using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Queries.GetSlidersByType
{
    public class GetSlidersByTypeQueryHandler : IRequestHandler<GetSlidersByTypeQuery, IEnumerable<SliderDto>>
    {
        private readonly ISliderRepository _sliderRepository;

        public GetSlidersByTypeQueryHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }

        public async Task<IEnumerable<SliderDto>> Handle(GetSlidersByTypeQuery request, CancellationToken cancellationToken)
        {
            var slidersResult = request.ActiveOnly
                ? await _sliderRepository.GetActiveSlidersByTypeAsync(request.SliderType)
                : await _sliderRepository.GetBySliderTypeAsync(request.SliderType);

            if (!slidersResult.Success)
            {
                return Enumerable.Empty<SliderDto>();
            }

            var sliders = slidersResult.Data ?? Enumerable.Empty<Domain.Entities.Slider>();

            return sliders.Select(slider => new SliderDto
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
            });
        }
    }
}