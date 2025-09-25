using Catalog.Application.Interfaces;
using Catalog.Application.Models.Images;
using Catalog.Application.Models.Requests.Product;
using Catalog.Application.Models.Requests.ProductImage;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductRequest> _validator;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ITempStorageService _tempStorageService;
        public ProductController(IProductService productService, IValidator<CreateProductRequest> validator, IImageProcessingService imageProcessingService, ITempStorageService tempStorageService)
        {
            _productService = productService;
            _validator = validator;
            _imageProcessingService = imageProcessingService;
            _tempStorageService = tempStorageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductRequest request,[FromForm] IEnumerable<ImageFileRequest> images,CancellationToken cancellation)
        {
            await _validator.ValidateAndThrowAsync(request,cancellation);
            var result = await _productService.CreateAsync(request, cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            var processImageRequests = images.Select(async i =>
            {
                var tempUrl = await _tempStorageService.SaveFilesToTempStorageAsync(i.Image.OpenReadStream());
                return new ProcessImageRequest(result.Value, tempUrl, i.SortOrder);
            });
            var imageRequests = await Task.WhenAll(processImageRequests);
            var res = await _imageProcessingService.SaveProductImagesAsync(imageRequests);
            return Ok(res);
        }
    }

    public record ImageFileRequest(IFormFile Image, int SortOrder);
}