using MediatR;

namespace BaseApi.Application.Features.SocialMediaLinks.Commands.UpdateSocialMediaLink
{
    public class UpdateSocialMediaLinkCommand : IRequest<UpdateSocialMediaLinkResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool OpenInNewTab { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public string? ColorCode { get; set; }
        public int UpdatedBy { get; set; }
    }
}