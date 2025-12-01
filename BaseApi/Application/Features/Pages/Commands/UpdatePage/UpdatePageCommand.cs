using MediatR;

namespace BaseApi.Application.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommand : IRequest<UpdatePageResponse>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? FeaturedImageUrl { get; set; }
        public int Template { get; set; }
        public int Status { get; set; }
        public bool IsHomePage { get; set; }
        public int Visibility { get; set; }
        public bool AllowComments { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? CustomCss { get; set; }
        public string? CustomJs { get; set; }
        public int Order { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int UpdatedBy { get; set; }
    }
}