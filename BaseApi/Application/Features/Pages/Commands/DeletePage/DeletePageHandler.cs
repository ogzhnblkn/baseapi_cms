using BaseApi.Application.Common;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.Pages.Commands.DeletePage
{
    public class DeletePageHandler : IRequestHandler<DeletePageCommand, DeletePageResponse>
    {
        private readonly IPageRepository _pageRepository;

        public DeletePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<DeletePageResponse> Handle(DeletePageCommand request, CancellationToken cancellationToken)
        {
            var page = await _pageRepository.GetByIdAsync(request.Id);

            if (page == null)
            {
                return new DeletePageResponse
                {
                    Success = false,
                    Message = Messages.Page.NotFound
                };
            }

            await _pageRepository.DeleteAsync(request.Id);

            return new DeletePageResponse
            {
                Success = true,
                Message = Messages.Page.Deleted
            };
        }
    }
}