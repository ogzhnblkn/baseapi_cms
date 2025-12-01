namespace BaseApi.Application.Features.Pages.Queries.GetPageById
{
    public class GetPageByIdResponse
    {
        public PageDetailDto? Page { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class PageDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? FeaturedImageUrl { get; set; }
        public int Template { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public bool IsHomePage { get; set; }
        public int Visibility { get; set; }
        public string VisibilityName { get; set; } = string.Empty;
        public bool AllowComments { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Keywords { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? CustomCss { get; set; }
        public string? CustomJs { get; set; }
        public int Order { get; set; }
        public int ViewCount { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}