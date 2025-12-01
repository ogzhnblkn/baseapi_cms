using MediatR;

namespace BaseApi.Application.Features.Pages.Queries.GetPages
{
    public class GetPagesQuery : IRequest<GetPagesResponse>
    {
        public int? Status { get; set; }
        public int? Template { get; set; }
        public bool? PublishedOnly { get; set; } = false;
    }
}