namespace BaseApi.Application.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}