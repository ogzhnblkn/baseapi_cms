using BaseApi.Application.Common;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Commands.UpdateSocialMediaLink
{
    public class UpdateSocialMediaLinkHandler : IRequestHandler<UpdateSocialMediaLinkCommand, UpdateSocialMediaLinkResponse>
    {
        private readonly ISocialMediaLinkRepository _socialMediaLinkRepository;

        public UpdateSocialMediaLinkHandler(ISocialMediaLinkRepository socialMediaLinkRepository)
        {
            _socialMediaLinkRepository = socialMediaLinkRepository;
        }

        public async Task<UpdateSocialMediaLinkResponse> Handle(UpdateSocialMediaLinkCommand request, CancellationToken cancellationToken)
        {
            var socialMediaLink = await _socialMediaLinkRepository.GetByIdAsync(request.Id);

            if (socialMediaLink == null)
            {
                return new UpdateSocialMediaLinkResponse
                {
                    Success = false,
                    Message = Messages.SocialMediaLink.NotFound
                };
            }

            socialMediaLink.Name = request.Name;
            socialMediaLink.Url = request.Url;
            socialMediaLink.Icon = request.Icon;
            socialMediaLink.ImageUrl = request.ImageUrl;
            socialMediaLink.IsActive = request.IsActive;
            socialMediaLink.OpenInNewTab = request.OpenInNewTab;
            socialMediaLink.Description = request.Description;
            socialMediaLink.Order = request.Order;
            socialMediaLink.ColorCode = request.ColorCode;
            socialMediaLink.UpdatedBy = request.UpdatedBy;

            await _socialMediaLinkRepository.UpdateAsync(socialMediaLink);

            return new UpdateSocialMediaLinkResponse
            {
                Id = socialMediaLink.Id,
                Name = socialMediaLink.Name,
                Url = socialMediaLink.Url,
                Message = Messages.SocialMediaLink.Updated,
                Success = true
            };
        }
    }
}