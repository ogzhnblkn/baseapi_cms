using BaseApi.Application.DTOs.Slider;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Queries.GetSlider
{
    public class GetSliderQuery : IRequest<SliderDto?>
    {
        public int Id { get; set; }
    }
}