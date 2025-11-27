namespace BaseApi.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadResult> UploadImageAsync(IFormFile file, string folder = "images");
        Task<FileUploadResult> UploadFileAsync(IFormFile file, string folder = "files");
        Task<bool> DeleteFileAsync(string filePath);
        Task<FileValidationResult> ValidateImageAsync(IFormFile file);
        Task<FileValidationResult> ValidateFileAsync(IFormFile file);
        string GetFileUrl(string filePath);
        Task<byte[]?> GetFileAsync(string filePath);
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? Error { get; set; }
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
    }

    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string? Error { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}