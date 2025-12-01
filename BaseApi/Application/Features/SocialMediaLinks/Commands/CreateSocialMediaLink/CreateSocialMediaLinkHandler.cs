using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Commands.CreateSocialMediaLink
{
    public class CreateSocialMediaLinkHandler : IRequestHandler<CreateSocialMediaLinkCommand, CreateSocialMediaLinkResponse>
    {
        private readonly ISocialMediaLinkRepository _socialMediaLinkRepository;

        public CreateSocialMediaLinkHandler(ISocialMediaLinkRepository socialMediaLinkRepository)
        {
            _socialMediaLinkRepository = socialMediaLinkRepository;
        }

        public async Task<CreateSocialMediaLinkResponse> Handle(CreateSocialMediaLinkCommand request, CancellationToken cancellationToken)
        {
            var socialMediaLink = new SocialMediaLink
            {
                Name = request.Name,
                Url = request.Url,
                Icon = request.Icon,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive,
                OpenInNewTab = request.OpenInNewTab,
                Description = request.Description,
                Order = request.Order,
                ColorCode = request.ColorCode,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var createdSocialMediaLink = await _socialMediaLinkRepository.CreateAsync(socialMediaLink);

            return new CreateSocialMediaLinkResponse
            {
                Id = createdSocialMediaLink.Id,
                Name = createdSocialMediaLink.Name,
                Url = createdSocialMediaLink.Url,
                Message = Messages.SocialMediaLink.Created,
                Success = true
            };
        }
    }
}