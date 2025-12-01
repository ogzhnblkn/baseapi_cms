using MediatR;

namespace BaseApi.Application.Features.Pages.Queries.GetPageById
{
    public class GetPageByIdQuery : IRequest<GetPageByIdResponse>
    {
        public int Id { get; set; }
    }
}