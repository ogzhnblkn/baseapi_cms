using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Commands.DeleteSocialMediaLink
{
    public class DeleteSocialMediaLinkCommand : IRequest<DeleteSocialMediaLinkResponse>
    {
        public int Id { get; set; }
    }
}