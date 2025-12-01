using BaseApi.Application.Common;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Commands.DeleteSocialMediaLink
{
    public class DeleteSocialMediaLinkHandler : IRequestHandler<DeleteSocialMediaLinkCommand, DeleteSocialMediaLinkResponse>
    {
        private readonly ISocialMediaLinkRepository _socialMediaLinkRepository;

        public DeleteSocialMediaLinkHandler(ISocialMediaLinkRepository socialMediaLinkRepository)
        {
            _socialMediaLinkRepository = socialMediaLinkRepository;
        }

        public async Task<DeleteSocialMediaLinkResponse> Handle(DeleteSocialMediaLinkCommand request, CancellationToken cancellationToken)
        {
            var socialMediaLink = await _socialMediaLinkRepository.GetByIdAsync(request.Id);

            if (socialMediaLink == null)
            {
                return new DeleteSocialMediaLinkResponse
                {
                    Success = false,
                    Message = Messages.SocialMediaLink.NotFound
                };
            }

            await _socialMediaLinkRepository.DeleteAsync(request.Id);

            return new DeleteSocialMediaLinkResponse
            {
                Success = true,
                Message = Messages.SocialMediaLink.Deleted
            };
        }
    }
}