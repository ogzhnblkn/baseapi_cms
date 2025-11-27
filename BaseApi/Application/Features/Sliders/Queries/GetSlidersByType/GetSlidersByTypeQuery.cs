using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Queries.GetSlidersByType
{
    public class GetSlidersByTypeQuery : IRequest<IEnumerable<SliderDto>>
    {
        public SliderType SliderType { get; set; }
        public bool ActiveOnly { get; set; } = true;
    }
}