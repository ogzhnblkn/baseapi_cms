using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinks
{
    public class GetSocialMediaLinksQuery : IRequest<GetSocialMediaLinksResponse>
    {

        public bool? ActiveOnly { get; set; } = false;
    }
}