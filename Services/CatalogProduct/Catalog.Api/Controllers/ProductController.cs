using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.Pagination;
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

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync([FromQuery] PaginationRequest pagination,
            CancellationToken cancellation)
        {
            return Ok(await _productService.GetProductsAsync(pagination,cancellation));
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid productId, CancellationToken cancellation)
        {
            //temporary allowing no auth operations pass userId in future
            var result =  await _productService.DeleteAsync(productId,Guid.NewGuid(),cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return NoContent();
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid productId, CancellationToken cancellation)
        {
            var result = await _productService.GetByIdAsync(productId,cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
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

            var productId = result.Value;
            if (!images.Any())
            {
                return Ok();
            }

            var imageRequests = await ProcessImageRequestsAsync(images, productId, cancellation);
            var imagesResponse = await _imageService.CreateImagesAsync(productId, imageRequests, cancellation);
            if (imagesResponse.IsFailure)
            {
                return BadRequest(imagesResponse.Error);
            }

            await _publishEndpoint.Publish(
                new ProcessImagesIntegrationEvent(productId, imagesResponse.Value), cancellation);
            return Ok();
        }

        [HttpGet("{productId:guid}/ava")]
        public async Task<IActionResult> IsAvailableAsync([FromRoute] Guid productId, [FromQuery] int quantity,
            CancellationToken cancellation)
        {
            var result = await _productService.IsAvailableAsync(productId, quantity, cancellation);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Value);
        }
        private async Task<IEnumerable<ProcessImageRequest>> ProcessImageRequestsAsync(
            IEnumerable<ImageFileRequest> images, Guid productId, CancellationToken cancellation)
        {
            var semaphore = new SemaphoreSlim(2);
            var processImageRequests = images.Select(async i =>
            {
                await semaphore.WaitAsync(cancellation);
                try
                {
                    var tempUrl = await _tempStorageService.SaveFilesToTempStorageAsync(i.Image.OpenReadStream(),
                        i.Image.ContentType, i.Image.FileName);
                    ;
                    return new ProcessImageRequest(productId, tempUrl, i.SortOrder);
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
            return imageRequests;
        }
    }

    public record ImageFileRequest(IFormFile Image, int SortOrder);
}