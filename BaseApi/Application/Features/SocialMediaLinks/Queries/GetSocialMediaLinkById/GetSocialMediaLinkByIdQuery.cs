using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinkById
{
    public class GetSocialMediaLinkByIdQuery : IRequest<GetSocialMediaLinkByIdResponse>
    {
        public int Id { get; set; }
    }
}