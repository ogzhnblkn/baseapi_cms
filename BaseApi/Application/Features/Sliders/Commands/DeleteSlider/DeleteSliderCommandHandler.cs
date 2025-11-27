using BaseApi.Application.Interfaces;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Sliders.Commands.DeleteSlider
{
    public class DeleteSliderCommandHandler : IRequestHandler<DeleteSliderCommand, bool>
    {
        private readonly ISliderRepository _sliderRepository;
        private readonly IFileUploadService _fileUploadService;

        public DeleteSliderCommandHandler(ISliderRepository sliderRepository, IFileUploadService fileUploadService)
        {
            _sliderRepository = sliderRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<bool> Handle(DeleteSliderCommand request, CancellationToken cancellationToken)
        {
            var slider = await _sliderRepository.GetByIdAsync(request.Id);
            if (slider == null)
                return false;

            // Delete associated files
            if (!string.IsNullOrEmpty(slider.ImageUrl))
            {
                await _fileUploadService.DeleteFileAsync(slider.ImageUrl);
            }

            if (!string.IsNullOrEmpty(slider.MobileImageUrl))
            {
                await _fileUploadService.DeleteFileAsync(slider.MobileImageUrl);
            }

            await _sliderRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}