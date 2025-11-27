using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Queries.GetAllSliders
{
    public class GetAllSlidersQueryHandler : IRequestHandler<GetAllSlidersQuery, IEnumerable<SliderDto>>
    {
        private readonly ISliderRepository _sliderRepository;

        public GetAllSlidersQueryHandler(ISliderRepository sliderRepository)
        {
            _sliderRepository = sliderRepository;
        }

        public async Task<IEnumerable<SliderDto>> Handle(GetAllSlidersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.Slider> sliders;

            if (!string.IsNullOrEmpty(request.TargetLocation))
            {
                if (request.IsActive == true)
                {
                    sliders = await _sliderRepository.GetActiveSlidersByLocationAsync(request.TargetLocation);
                }
                else
                {
                    sliders = await _sliderRepository.GetSlidersByLocationAsync(request.TargetLocation);
                }
            }
            else if (request.SliderType.HasValue && request.IsActive == true)
            {
                sliders = await _sliderRepository.GetActiveSlidersByTypeAsync(request.SliderType.Value);
            }
            else if (request.SliderType.HasValue)
            {
                sliders = await _sliderRepository.GetBySliderTypeAsync(request.SliderType.Value);
            }
            else
            {
                sliders = await _sliderRepository.GetAllAsync();
            }

            if (request.IsActive.HasValue && string.IsNullOrEmpty(request.TargetLocation))
            {
                sliders = sliders.Where(s => s.IsActive == request.IsActive.Value);
            }

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