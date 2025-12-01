namespace BaseApi.Application.Features.Pages.Queries.GetPages
{
    public class GetPagesResponse
    {
        public List<PageDto> Pages { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class PageDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string Template { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsHomePage { get; set; }
        public string Visibility { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int Order { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}