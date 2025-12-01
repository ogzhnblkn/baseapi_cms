namespace BaseApi.Application.Features.SocialMediaLinks.Commands.CreateSocialMediaLink
{
    public class CreateSocialMediaLinkResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}