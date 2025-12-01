using MediatR;

namespace BaseApi.Application.Features.Pages.Commands.DeletePage
{
    public class DeletePageCommand : IRequest<DeletePageResponse>
    {
        public int Id { get; set; }
    }
}