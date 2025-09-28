using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.Product;
using Catalog.Application.Models.Requests.ProductImage;
using Contracts.IntegrationEvents;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<CreateProductRequest> _validator;
        private readonly ITempStorageService _tempStorageService;
        private readonly IProductImageService _imageService;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductController(IProductService productService, IValidator<CreateProductRequest> validator,
            ITempStorageService tempStorageService, IProductImageService imageService, IPublishEndpoint publishEndpoint)
        {
            _productService = productService;
            _validator = validator;
            _tempStorageService = tempStorageService;
            _imageService = imageService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductRequest request,
            [FromForm] IEnumerable<ImageFileRequest> images, CancellationToken cancellation)
        {
            await _validator.ValidateAndThrowAsync(request, cancellation);
            var result = await _productService.CreateAsync(request, cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            if (!images.Any())
            {
                return Ok();
            }
            var semaphore = new SemaphoreSlim(2);
            var processImageRequests = images.Select(async i =>
            {
                await semaphore.WaitAsync(cancellation);
                try
                {
                    var tempUrl = await _tempStorageService.SaveFilesToTempStorageAsync(i.Image.OpenReadStream());
                    return new ProcessImageRequest(result.Value, tempUrl, i.SortOrder);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    semaphore.Release();
                }
            });
            var imageRequests = await Task.WhenAll(processImageRequests);
            var imagesResponse = await _imageService.CreateImagesAsync(result.Value, imageRequests, cancellation);
            if (imagesResponse.IsFailure)
            {
                return BadRequest(imagesResponse.Error);
            }

            await _publishEndpoint.Publish(
                new ProcessImagesIntegrationEvent(result.Value, imagesResponse.Value), cancellation);
            return Ok();
        }
    }

    public record ImageFileRequest(IFormFile Image, int SortOrder);
}