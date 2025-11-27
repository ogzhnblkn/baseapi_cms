using MediatR;

namespace BaseApi.Application.Features.Sliders.Commands.DeleteSlider
{
    public class DeleteSliderCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}