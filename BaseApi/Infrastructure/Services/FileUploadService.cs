using BaseApi.Application.Interfaces;

namespace BaseApi.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;

        // Allowed image formats
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        private readonly string[] _allowedImageMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp" };

        // Allowed file formats (for documents, etc.)
        private readonly string[] _allowedFileExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip", ".rar" };

        public FileUploadService(
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<FileUploadResult> UploadImageAsync(IFormFile file, string folder = "images")
        {
            var validation = await ValidateImageAsync(file);
            if (!validation.IsValid)
            {
                return new FileUploadResult
                {
                    Success = false,
                    Error = validation.Error ?? string.Join(", ", validation.Errors)
                };
            }

            return await UploadFileInternalAsync(file, folder);
        }

        public async Task<FileUploadResult> UploadFileAsync(IFormFile file, string folder = "files")
        {
            var validation = await ValidateFileAsync(file);
            if (!validation.IsValid)
            {
                return new FileUploadResult
                {
                    Success = false,
                    Error = validation.Error ?? string.Join(", ", validation.Errors)
                };
            }

            return await UploadFileInternalAsync(file, folder);
        }

        private async Task<FileUploadResult> UploadFileInternalAsync(IFormFile file, string folder)
        {
            try
            {
                // Get upload directory from configuration or use default
                var baseUploadPath = _configuration["FileUpload:UploadPath"] ?? "wwwroot/uploads";

                // Create absolute upload directory path
                var uploadPath = Path.Combine(_environment.ContentRootPath, baseUploadPath, folder);
                Directory.CreateDirectory(uploadPath);

                // Generate unique filename
                var fileName = GenerateUniqueFileName(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Generate relative path for database storage (web accessible path)
                var relativePath = Path.Combine("uploads", folder, fileName).Replace('\\', '/');
                var fileUrl = GetFileUrl(relativePath);

                _logger.LogInformation($"File uploaded successfully: {fileName} to {uploadPath}");

                return new FileUploadResult
                {
                    Success = true,
                    FilePath = relativePath,
                    FileName = fileName,
                    FileUrl = fileUrl,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
                return new FileUploadResult
                {
                    Success = false,
                    Error = "File upload failed"
                };
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                // Get upload directory from configuration or use default
                var baseUploadPath = _configuration["FileUpload:UploadPath"] ?? "wwwroot/uploads";

                // Remove 'uploads/' prefix if exists to avoid duplication
                var normalizedPath = filePath.StartsWith("uploads/") ? filePath.Substring(8) : filePath;

                var fullPath = Path.Combine(_environment.ContentRootPath, baseUploadPath, normalizedPath.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation($"File deleted successfully: {filePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public async Task<FileValidationResult> ValidateImageAsync(IFormFile file)
        {
            var result = new FileValidationResult { IsValid = true };

            if (file == null || file.Length == 0)
            {
                result.IsValid = false;
                result.Error = "No file uploaded";
                return result;
            }

            // Check file size (default: 5MB for images)
            var maxSize = _configuration.GetValue<long>("FileUpload:MaxImageSizeBytes", 5 * 1024 * 1024);
            if (file.Length > maxSize)
            {
                result.IsValid = false;
                result.Errors.Add($"File size must be less than {maxSize / (1024 * 1024)}MB");
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(extension))
            {
                result.IsValid = false;
                result.Errors.Add($"Invalid file type. Allowed types: {string.Join(", ", _allowedImageExtensions)}");
            }

            // Check mime type
            if (!_allowedImageMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                result.IsValid = false;
                result.Errors.Add("Invalid file format");
            }

            if (result.Errors.Any())
            {
                result.Error = string.Join(", ", result.Errors);
            }

            return result;
        }

        public async Task<FileValidationResult> ValidateFileAsync(IFormFile file)
        {
            var result = new FileValidationResult { IsValid = true };

            if (file == null || file.Length == 0)
            {
                result.IsValid = false;
                result.Error = "No file uploaded";
                return result;
            }

            // Check file size (default: 10MB for documents)
            var maxSize = _configuration.GetValue<long>("FileUpload:MaxFileSizeBytes", 10 * 1024 * 1024);
            if (file.Length > maxSize)
            {
                result.IsValid = false;
                result.Errors.Add($"File size must be less than {maxSize / (1024 * 1024)}MB");
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedFileExtensions.Contains(extension))
            {
                result.IsValid = false;
                result.Errors.Add($"Invalid file type. Allowed types: {string.Join(", ", _allowedFileExtensions)}");
            }

            if (result.Errors.Any())
            {
                result.Error = string.Join(", ", result.Errors);
            }

            return result;
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            var baseUrl = _configuration["FileUpload:BaseUrl"] ?? "";
            return $"{baseUrl.TrimEnd('/')}/{filePath.TrimStart('/')}";
        }

        public async Task<byte[]?> GetFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return null;

                // Get upload directory from configuration or use default
                var baseUploadPath = _configuration["FileUpload:UploadPath"] ?? "wwwroot/uploads";

                // Remove 'uploads/' prefix if exists to avoid duplication
                var normalizedPath = filePath.StartsWith("uploads/") ? filePath.Substring(8) : filePath;

                var fullPath = Path.Combine(_environment.ContentRootPath, baseUploadPath, normalizedPath.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(fullPath))
                {
                    return await File.ReadAllBytesAsync(fullPath);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
                return null;
            }
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

            // Sanitize filename
            var sanitizedName = string.Join("_", nameWithoutExtension.Split(Path.GetInvalidFileNameChars()));

            // Add timestamp and random string for uniqueness
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var randomString = Guid.NewGuid().ToString("N")[..8];

            return $"{sanitizedName}_{timestamp}_{randomString}{extension}";
        }
    }
}