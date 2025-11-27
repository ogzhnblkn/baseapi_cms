using BaseApi.Application.DTOs.Slider;
using BaseApi.Application.Features.Sliders.Commands.CreateSlider;
using BaseApi.Application.Features.Sliders.Commands.UpdateSlider;
using BaseApi.Application.Features.Sliders.Queries.GetAllSliders;
using BaseApi.Application.Features.Sliders.Queries.GetSlider;
using BaseApi.Application.Features.Sliders.Queries.GetSlidersByType;
using BaseApi.Application.Interfaces;
using BaseApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SlidersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileUploadService _fileUploadService;

        public SlidersController(IMediator mediator, IFileUploadService fileUploadService)
        {
            _mediator = mediator;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SliderDto>>> GetAllSliders(
            [FromQuery] SliderType? sliderType = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? targetLocation = null)
        {
            try
            {
                var query = new GetAllSlidersQuery
                {
                    SliderType = sliderType,
                    IsActive = isActive,
                    TargetLocation = targetLocation
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SliderDto>> GetSlider(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetSliderQuery { Id = id });

                if (result == null)
                    return NotFound(new { message = $"Slider with ID {id} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-type/{sliderType}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SliderDto>>> GetSlidersByType(
            SliderType sliderType,
            [FromQuery] bool activeOnly = true)
        {
            try
            {
                var query = new GetSlidersByTypeQuery
                {
                    SliderType = sliderType,
                    ActiveOnly = activeOnly
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<SliderDto>> CreateSlider([FromBody] CreateSliderDto createSliderDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new CreateSliderCommand
                {
                    Title = createSliderDto.Title,
                    Subtitle = createSliderDto.Subtitle,
                    Description = createSliderDto.Description,
                    ImageUrl = createSliderDto.ImageUrl,
                    MobileImageUrl = createSliderDto.MobileImageUrl,
                    LinkUrl = createSliderDto.LinkUrl,
                    ButtonText = createSliderDto.ButtonText,
                    Order = createSliderDto.Order,
                    IsActive = createSliderDto.IsActive,
                    SliderType = createSliderDto.SliderType,
                    LinkType = createSliderDto.LinkType,
                    OpenInNewTab = createSliderDto.OpenInNewTab,
                    TargetLocation = createSliderDto.TargetLocation,
                    StartDate = createSliderDto.StartDate,
                    EndDate = createSliderDto.EndDate,
                    CreatedBy = userId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetSlider), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("with-image")]
        public async Task<ActionResult<SliderDto>> CreateSliderWithImage(
            [FromForm] CreateSliderWithImageDto createSliderDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "User not authenticated" });

                // Upload main image
                FileUploadResult? mainImageResult = null;
                if (createSliderDto.ImageFile != null)
                {
                    mainImageResult = await _fileUploadService.UploadImageAsync(createSliderDto.ImageFile, "sliders");
                    if (!mainImageResult.Success)
                    {
                        return BadRequest(new { message = $"Main image upload failed: {mainImageResult.Error}" });
                    }
                }

                // Upload mobile image if provided
                FileUploadResult? mobileImageResult = null;
                if (createSliderDto.MobileImageFile != null)
                {
                    mobileImageResult = await _fileUploadService.UploadImageAsync(createSliderDto.MobileImageFile, "sliders/mobile");
                    if (!mobileImageResult.Success)
                    {
                        // Clean up main image if mobile image upload fails
                        if (mainImageResult != null)
                        {
                            await _fileUploadService.DeleteFileAsync(mainImageResult.FilePath!);
                        }
                        return BadRequest(new { message = $"Mobile image upload failed: {mobileImageResult.Error}" });
                    }
                }

                var command = new CreateSliderCommand
                {
                    Title = createSliderDto.Title,
                    Subtitle = createSliderDto.Subtitle,
                    Description = createSliderDto.Description,
                    ImageUrl = mainImageResult?.FileUrl ?? createSliderDto.ImageUrl ?? string.Empty,
                    MobileImageUrl = mobileImageResult?.FileUrl ?? createSliderDto.MobileImageUrl,
                    LinkUrl = createSliderDto.LinkUrl,
                    ButtonText = createSliderDto.ButtonText,
                    Order = createSliderDto.Order,
                    IsActive = createSliderDto.IsActive,
                    SliderType = createSliderDto.SliderType,
                    LinkType = createSliderDto.LinkType,
                    OpenInNewTab = createSliderDto.OpenInNewTab,
                    TargetLocation = createSliderDto.TargetLocation,
                    StartDate = createSliderDto.StartDate,
                    EndDate = createSliderDto.EndDate,
                    CreatedBy = userId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetSlider), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/with-image")]
        public async Task<ActionResult<SliderDto>> UpdateSliderWithImage(
            int id,
            [FromForm] UpdateSliderWithImageDto updateSliderDto)
        {
            try
            {
                // Get existing slider for image cleanup
                var existingSlider = await _mediator.Send(new GetSliderQuery { Id = id });
                if (existingSlider == null)
                {
                    return NotFound(new { message = $"Slider with ID {id} not found" });
                }

                string? newImageUrl = existingSlider.ImageUrl;
                string? newMobileImageUrl = existingSlider.MobileImageUrl;

                // Upload new main image if provided
                if (updateSliderDto.ImageFile != null)
                {
                    var mainImageResult = await _fileUploadService.UploadImageAsync(updateSliderDto.ImageFile, "sliders");
                    if (!mainImageResult.Success)
                    {
                        return BadRequest(new { message = $"Main image upload failed: {mainImageResult.Error}" });
                    }

                    // Delete old main image
                    if (!string.IsNullOrEmpty(existingSlider.ImageUrl))
                    {
                        await _fileUploadService.DeleteFileAsync(existingSlider.ImageUrl);
                    }

                    newImageUrl = mainImageResult.FileUrl;
                }

                // Upload new mobile image if provided
                if (updateSliderDto.MobileImageFile != null)
                {
                    var mobileImageResult = await _fileUploadService.UploadImageAsync(updateSliderDto.MobileImageFile, "sliders/mobile");
                    if (!mobileImageResult.Success)
                    {
                        return BadRequest(new { message = $"Mobile image upload failed: {mobileImageResult.Error}" });
                    }

                    // Delete old mobile image
                    if (!string.IsNullOrEmpty(existingSlider.MobileImageUrl))
                    {
                        await _fileUploadService.DeleteFileAsync(existingSlider.MobileImageUrl);
                    }

                    newMobileImageUrl = mobileImageResult.FileUrl;
                }

                var command = new UpdateSliderCommand
                {
                    Id = id,
                    Title = updateSliderDto.Title,
                    Subtitle = updateSliderDto.Subtitle,
                    Description = updateSliderDto.Description,
                    ImageUrl = newImageUrl ?? string.Empty,
                    MobileImageUrl = newMobileImageUrl,
                    LinkUrl = updateSliderDto.LinkUrl,
                    ButtonText = updateSliderDto.ButtonText,
                    Order = updateSliderDto.Order,
                    IsActive = updateSliderDto.IsActive,
                    SliderType = updateSliderDto.SliderType,
                    LinkType = updateSliderDto.LinkType,
                    OpenInNewTab = updateSliderDto.OpenInNewTab,
                    TargetLocation = updateSliderDto.TargetLocation,
                    StartDate = updateSliderDto.StartDate,
                    EndDate = updateSliderDto.EndDate
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ... diðer standart CRUD methodlarý
    }
}