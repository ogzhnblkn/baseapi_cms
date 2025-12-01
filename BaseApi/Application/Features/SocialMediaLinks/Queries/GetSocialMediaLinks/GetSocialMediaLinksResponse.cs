namespace BaseApi.Application.Features.SocialMediaLinks.Queries.GetSocialMediaLinks
{
    public class GetSocialMediaLinksResponse
    {
        public List<SocialMediaLinkDto> SocialMediaLinks { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class SocialMediaLinkDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public string Location { get; set; } = string.Empty;
        public int ClickCount { get; set; }
        public string? ColorCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}