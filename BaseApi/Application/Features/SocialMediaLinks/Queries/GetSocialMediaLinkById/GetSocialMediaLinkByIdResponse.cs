using BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinks;

namespace BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinkById
{
    public class GetSocialMediaLinkByIdResponse
    {
        public SocialMediaLinkDto? SocialMediaLink { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}