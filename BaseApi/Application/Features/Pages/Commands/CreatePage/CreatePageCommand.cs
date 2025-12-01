using MediatR;

namespace BaseApi.Application.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommand : IRequest<CreatePageResponse>
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? FeaturedImageUrl { get; set; }
        public int Template { get; set; } = 1;
        public int Status { get; set; } = 1;
        public bool IsHomePage { get; set; } = false;
        public int Visibility { get; set; } = 1;
        public bool AllowComments { get; set; } = false;
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? CustomCss { get; set; }
        public string? CustomJs { get; set; }
        public int Order { get; set; } = 0;
        public DateTime? PublishedAt { get; set; }
        public int CreatedBy { get; set; }
    }
}