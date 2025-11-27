using BaseApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost("image")]
        public async Task<ActionResult<FileUploadResult>> UploadImage(IFormFile file, [FromQuery] string folder = "sliders")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                var result = await _fileUploadService.UploadImageAsync(file, folder);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Error });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("images/multiple")]
        public async Task<ActionResult<List<FileUploadResult>>> UploadMultipleImages(
            IFormFileCollection files,
            [FromQuery] string folder = "sliders")
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { message = "No files uploaded" });
                }

                var results = new List<FileUploadResult>();

                foreach (var file in files)
                {
                    var result = await _fileUploadService.UploadImageAsync(file, folder);
                    results.Add(result);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("file")]
        public async Task<ActionResult<FileUploadResult>> UploadFile(IFormFile file, [FromQuery] string folder = "documents")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                var result = await _fileUploadService.UploadFileAsync(file, folder);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Error });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> DeleteFile([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest(new { message = "File path is required" });
                }

                var result = await _fileUploadService.DeleteFileAsync(filePath);

                if (!result)
                {
                    return NotFound(new { message = "File not found or could not be deleted" });
                }

                return Ok(new { message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile([FromQuery] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest("File path is required");
                }

                var fileBytes = await _fileUploadService.GetFileAsync(filePath);

                if (fileBytes == null)
                {
                    return NotFound("File not found");
                }

                var fileName = Path.GetFileName(filePath);
                var contentType = GetContentType(filePath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }
    }
}