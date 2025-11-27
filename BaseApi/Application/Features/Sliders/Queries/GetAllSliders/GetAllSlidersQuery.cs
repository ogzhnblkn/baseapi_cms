using BaseApi.Application.DTOs.Slider;
using BaseApi.Domain.Entities;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Queries.GetAllSliders
{
    public class GetAllSlidersQuery : IRequest<IEnumerable<SliderDto>>
    {
        public SliderType? SliderType { get; set; }
        public bool? IsActive { get; set; }
        public string? TargetLocation { get; set; }
    }
}