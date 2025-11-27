using BaseApi.Application.DTOs.Product;
using BaseApi.Application.Exceptions;
using BaseApi.Application.Features.Products.Commands.CreateProduct;
using BaseApi.Application.Features.Products.Commands.DeleteProduct;
using BaseApi.Application.Features.Products.Commands.UpdateProduct;
using BaseApi.Application.Features.Products.Queries.GetAllProducts;
using BaseApi.Application.Features.Products.Queries.GetProduct;
using BaseApi.Application.Features.Products.Queries.GetProductsByCategory;
using BaseApi.Application.Interfaces;
using BaseApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace BaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileUploadService _fileUploadService;

        public ProductsController(IMediator mediator, IFileUploadService fileUploadService)
        {
            _mediator = mediator;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(
            [FromQuery] ProductCategory? category = null,
            [FromQuery] ProductStatus? status = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] bool? isNewProduct = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool activeOnly = true)
        {
            try
            {
                var query = new GetAllProductsQuery
                {
                    Category = category,
                    Status = status,
                    IsFeatured = isFeatured,
                    IsNewProduct = isNewProduct,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SearchTerm = searchTerm,
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int id, [FromQuery] bool incrementViewCount = false)
        {
            try
            {
                var result = await _mediator.Send(new GetProductQuery { Id = id, IncrementViewCount = incrementViewCount });

                if (result == null)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug, [FromQuery] bool incrementViewCount = false)
        {
            try
            {
                var result = await _mediator.Send(new GetProductQuery { Slug = slug, IncrementViewCount = incrementViewCount });

                if (result == null)
                    return NotFound(new { message = $"Product with slug '{slug}' not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(
            ProductCategory category,
            [FromQuery] bool activeOnly = true)
        {
            try
            {
                var query = new GetProductsByCategoryQuery
                {
                    Category = category,
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
        [Authorize]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new CreateProductCommand
                {
                    Name = createProductDto.Name,
                    Slug = createProductDto.Slug,
                    ShortDescription = createProductDto.ShortDescription,
                    Description = createProductDto.Description,
                    ProductCode = createProductDto.ProductCode,
                    Category = createProductDto.Category,
                    MainImageUrl = createProductDto.MainImageUrl,
                    ImageUrls = createProductDto.ImageUrls,
                    Price = createProductDto.Price,
                    DiscountPrice = createProductDto.DiscountPrice,
                    Currency = createProductDto.Currency,
                    ShowPrice = createProductDto.ShowPrice,
                    Dimensions = createProductDto.Dimensions,
                    Material = createProductDto.Material,
                    Colors = createProductDto.Colors,
                    Features = createProductDto.Features,
                    MetaTitle = createProductDto.MetaTitle,
                    MetaDescription = createProductDto.MetaDescription,
                    Keywords = createProductDto.Keywords,
                    Status = createProductDto.Status,
                    IsFeatured = createProductDto.IsFeatured,
                    IsNewProduct = createProductDto.IsNewProduct,
                    Order = createProductDto.Order,
                    CreatedBy = userId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
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

        [HttpPost("with-images")]
        [Authorize]
        public async Task<ActionResult<ProductDto>> CreateProductWithImages([FromForm] CreateProductWithImagesDto createProductDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new { message = "User not authenticated" });

                var imageUrls = new List<string>();

                // Upload main image
                string? mainImageUrl = createProductDto.MainImageUrl;
                if (createProductDto.MainImageFile != null)
                {
                    var mainImageResult = await _fileUploadService.UploadImageAsync(createProductDto.MainImageFile, "products");
                    if (!mainImageResult.Success)
                    {
                        return BadRequest(new { message = $"Main image upload failed: {mainImageResult.Error}" });
                    }
                    mainImageUrl = mainImageResult.FileUrl;
                }

                // Upload gallery images
                if (createProductDto.ImageFiles != null && createProductDto.ImageFiles.Count > 0)
                {
                    foreach (var imageFile in createProductDto.ImageFiles)
                    {
                        var imageResult = await _fileUploadService.UploadImageAsync(imageFile, "products/gallery");
                        if (imageResult.Success)
                        {
                            imageUrls.Add(imageResult.FileUrl!);
                        }
                    }
                }

                // Add existing URLs if provided
                if (createProductDto.ImageUrls != null)
                {
                    imageUrls.AddRange(createProductDto.ImageUrls);
                }

                // Parse features JSON
                Dictionary<string, object>? features = null;
                if (!string.IsNullOrEmpty(createProductDto.FeaturesJson))
                {
                    try
                    {
                        features = JsonSerializer.Deserialize<Dictionary<string, object>>(createProductDto.FeaturesJson);
                    }
                    catch (JsonException)
                    {
                        return BadRequest(new { message = "Invalid features JSON format" });
                    }
                }

                var command = new CreateProductCommand
                {
                    Name = createProductDto.Name,
                    Slug = createProductDto.Slug,
                    ShortDescription = createProductDto.ShortDescription,
                    Description = createProductDto.Description,
                    ProductCode = createProductDto.ProductCode,
                    Category = createProductDto.Category,
                    MainImageUrl = mainImageUrl,
                    ImageUrls = imageUrls.Any() ? imageUrls : null,
                    Price = createProductDto.Price,
                    DiscountPrice = createProductDto.DiscountPrice,
                    Currency = createProductDto.Currency,
                    ShowPrice = createProductDto.ShowPrice,
                    Dimensions = createProductDto.Dimensions,
                    Material = createProductDto.Material,
                    Colors = createProductDto.Colors,
                    Features = features,
                    MetaTitle = createProductDto.MetaTitle,
                    MetaDescription = createProductDto.MetaDescription,
                    Keywords = createProductDto.Keywords,
                    Status = createProductDto.Status,
                    IsFeatured = createProductDto.IsFeatured,
                    IsNewProduct = createProductDto.IsNewProduct,
                    Order = createProductDto.Order,
                    CreatedBy = userId
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                var command = new UpdateProductCommand
                {
                    Id = id,
                    Name = updateProductDto.Name,
                    Slug = updateProductDto.Slug,
                    ShortDescription = updateProductDto.ShortDescription,
                    Description = updateProductDto.Description,
                    ProductCode = updateProductDto.ProductCode,
                    Category = updateProductDto.Category,
                    MainImageUrl = updateProductDto.MainImageUrl,
                    ImageUrls = updateProductDto.ImageUrls,
                    Price = updateProductDto.Price,
                    DiscountPrice = updateProductDto.DiscountPrice,
                    Currency = updateProductDto.Currency,
                    ShowPrice = updateProductDto.ShowPrice,
                    Dimensions = updateProductDto.Dimensions,
                    Material = updateProductDto.Material,
                    Colors = updateProductDto.Colors,
                    Features = updateProductDto.Features,
                    MetaTitle = updateProductDto.MetaTitle,
                    MetaDescription = updateProductDto.MetaDescription,
                    Keywords = updateProductDto.Keywords,
                    Status = updateProductDto.Status,
                    IsFeatured = updateProductDto.IsFeatured,
                    IsNewProduct = updateProductDto.IsNewProduct,
                    Order = updateProductDto.Order
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteProductCommand { Id = id });

                if (!result)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}