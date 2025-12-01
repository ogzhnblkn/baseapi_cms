using BaseApi.Application.Common;
using BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinks;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinkById
{
    public class GetSocialMediaLinkByIdHandler : IRequestHandler<GetSocialMediaLinkByIdQuery, GetSocialMediaLinkByIdResponse>
    {
        private readonly ISocialMediaLinkRepository _socialMediaLinkRepository;

        public GetSocialMediaLinkByIdHandler(ISocialMediaLinkRepository socialMediaLinkRepository)
        {
            _socialMediaLinkRepository = socialMediaLinkRepository;
        }

        public async Task<GetSocialMediaLinkByIdResponse> Handle(GetSocialMediaLinkByIdQuery request, CancellationToken cancellationToken)
        {
            var socialMediaLink = await _socialMediaLinkRepository.GetByIdAsync(request.Id);

            if (socialMediaLink == null)
            {
                return new GetSocialMediaLinkByIdResponse
                {
                    Success = false,
                    Message = Messages.SocialMediaLink.NotFound
                };
            }

            var socialMediaLinkDto = new SocialMediaLinkDto
            {
                Id = socialMediaLink.Id,
                Name = socialMediaLink.Name,
                Url = socialMediaLink.Url,
                Icon = socialMediaLink.Icon,
                ImageUrl = socialMediaLink.ImageUrl,
                IsActive = socialMediaLink.IsActive,
                OpenInNewTab = socialMediaLink.OpenInNewTab,
                Description = socialMediaLink.Description,
                Order = socialMediaLink.Order,
                ClickCount = socialMediaLink.ClickCount,
                ColorCode = socialMediaLink.ColorCode,
                CreatedAt = socialMediaLink.CreatedAt,
                CreatedBy = socialMediaLink.Creator?.Username ?? "Sistem"
            };

            return new GetSocialMediaLinkByIdResponse
            {
                SocialMediaLink = socialMediaLinkDto,
                Success = true,
                Message = Messages.SocialMediaLink.Retrieved
            };
        }
    }
}