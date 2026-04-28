using Application.Features.Products.Dtos;
using Application.Features.Products.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrendyApi.Services;

namespace TrendyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ImageService _imageService;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductController(IProductService productService, ImageService imageService)
        {
            _productService = productService;
            _imageService = imageService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct(
            [FromForm] CreateUpdateProductDto dto,
            [FromForm] List<IFormFile> Images)
        {
            // Deserialize Properties from JSON string if provided
            if (!string.IsNullOrWhiteSpace(dto.PropertiesJson))
            {
                try
                {
                    dto.Properties = JsonSerializer.Deserialize<IList<CreatePropertyDto>>(dto.PropertiesJson, _jsonOptions);
                }
                catch (JsonException ex)
                {
                    return BadRequest(new { Success = false, Message = "Invalid Properties JSON format.", Error = ex.Message });
                }
            }

            // Upload main product images
            var imageFiles = Images?.Where(f => f != null && f.Length > 0).ToList() ?? new List<IFormFile>();
            if (imageFiles.Any())
            {
                var uploadResult = await _imageService.UploadImagesAsync(imageFiles);
                if (!uploadResult.Success)
                    return BadRequest(uploadResult);
                dto.Image = string.Join(",", uploadResult.Data);
            }

            var result = await _productService.CreateProductAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] CreateUpdateProductDto dto, [FromForm] List<IFormFile> Images)
        {
            // Deserialize Properties from JSON string if provided
            if (!string.IsNullOrWhiteSpace(dto.PropertiesJson))
            {
                try
                {
                    dto.Properties = JsonSerializer.Deserialize<IList<CreatePropertyDto>>(dto.PropertiesJson, _jsonOptions);
                }
                catch (JsonException ex)
                {
                    return BadRequest(new { Success = false, Message = "Invalid Properties JSON format.", Error = ex.Message });
                }
            }

            // Upload new images if provided
            var imageFiles = Images?.Where(f => f != null && f.Length > 0).ToList() ?? new List<IFormFile>();

            if (imageFiles.Any())
            {
                var uploadResult = await _imageService.UploadImagesAsync(imageFiles);
                if (!uploadResult.Success)
                    return BadRequest(uploadResult);
                dto.Image = string.Join(",", uploadResult.Data);
            }

            var result = await _productService.UpdateProductAsync(id, dto);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
