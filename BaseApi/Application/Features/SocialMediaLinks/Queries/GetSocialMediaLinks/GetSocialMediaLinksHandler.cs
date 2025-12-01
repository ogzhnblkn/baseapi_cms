using BaseApi.Application.Common;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinks
{
    public class GetSocialMediaLinksHandler : IRequestHandler<GetSocialMediaLinksQuery, GetSocialMediaLinksResponse>
    {
        private readonly ISocialMediaLinkRepository _socialMediaLinkRepository;

        public GetSocialMediaLinksHandler(ISocialMediaLinkRepository socialMediaLinkRepository)
        {
            _socialMediaLinkRepository = socialMediaLinkRepository;
        }

        public async Task<GetSocialMediaLinksResponse> Handle(GetSocialMediaLinksQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<SocialMediaLink> socialMediaLinks;

            if (request.ActiveOnly == true)
            {
                socialMediaLinks = await _socialMediaLinkRepository.GetActiveAsync();
            }

            else
            {
                socialMediaLinks = await _socialMediaLinkRepository.GetAllAsync();
            }

            var socialMediaLinkDtos = socialMediaLinks.Select(s => new SocialMediaLinkDto
            {
                Id = s.Id,
                Name = s.Name,
                Url = s.Url,
                Icon = s.Icon,
                ImageUrl = s.ImageUrl,
                IsActive = s.IsActive,
                OpenInNewTab = s.OpenInNewTab,
                Description = s.Description,
                Order = s.Order,
                ClickCount = s.ClickCount,
                ColorCode = s.ColorCode,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.Creator?.Username ?? "Sistem"
            }).ToList();

            return new GetSocialMediaLinksResponse
            {
                SocialMediaLinks = socialMediaLinkDtos,
                Message = Messages.SocialMediaLink.ListRetrieved,
                Success = true
            };
        }
    }
}