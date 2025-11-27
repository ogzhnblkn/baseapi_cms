using BaseApi.Application.Interfaces;
using BaseApi.Domain.Interfaces;
using MediatR;
using System.Text.Json;

namespace BaseApi.Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileUploadService _fileUploadService;

        public DeleteProductCommandHandler(IProductRepository productRepository, IFileUploadService fileUploadService)
        {
            _productRepository = productRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
                return false;

            // Delete associated files
            if (!string.IsNullOrEmpty(product.Data.MainImageUrl))
            {
                await _fileUploadService.DeleteFileAsync(product.Data.MainImageUrl);
            }

            if (!string.IsNullOrEmpty(product.Data.ImageUrls))
            {
                var imageUrls = JsonSerializer.Deserialize<List<string>>(product.Data.ImageUrls);
                if (imageUrls != null)
                {
                    foreach (var imageUrl in imageUrls)
                    {
                        await _fileUploadService.DeleteFileAsync(imageUrl);
                    }
                }
            }

            await _productRepository.DeleteAsync(request.Id);
            return true;
        }
    }
}