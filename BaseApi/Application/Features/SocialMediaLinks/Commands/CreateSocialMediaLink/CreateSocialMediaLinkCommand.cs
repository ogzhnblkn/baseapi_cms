using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Commands.CreateSocialMediaLink
{
    public class CreateSocialMediaLinkCommand : IRequest<CreateSocialMediaLinkResponse>
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool OpenInNewTab { get; set; } = true;
        public string? Description { get; set; }
        public int Order { get; set; } = 0;
        public string? ColorCode { get; set; }
        public int CreatedBy { get; set; }
    }
}