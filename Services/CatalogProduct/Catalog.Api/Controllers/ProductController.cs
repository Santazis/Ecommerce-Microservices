using Amazon.S3;
using Amazon.S3.Model;
using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.Product;
using Catalog.Application.Models.Requests.ProductImage;
using Catalog.Infrastructure.Options;
using Contracts.IntegrationEvents;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        private readonly IAmazonS3 _s3Client;
        private readonly IOptions<S3Settings> _s3Settings;
        public ProductController(IProductService productService, IValidator<CreateProductRequest> validator,
            ITempStorageService tempStorageService, IProductImageService imageService, IPublishEndpoint publishEndpoint, IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
        {
            _productService = productService;
            _validator = validator;
            _tempStorageService = tempStorageService;
            _imageService = imageService;
            _publishEndpoint = publishEndpoint;
            _s3Client = s3Client;
            _s3Settings = s3Settings;
        }

        [HttpPost("test-bucket")]
        public async Task<IActionResult> UploadAsync(IFormFile file, CancellationToken cancellation)
        {
            var stream = file.OpenReadStream();
            var key = Guid.NewGuid().ToString();
            var putRequest = new PutObjectRequest()
            {
                Key = $"images/{key}",
                BucketName = _s3Settings.Value.BucketName,
                InputStream = stream,
                ContentType = file.ContentType,
                Metadata = { ["file-name"] = file.FileName }
            };
            var url = $"{_s3Settings.Value.ServiceUrl}/{_s3Settings.Value.BucketName}/{putRequest.Key}";
            await _s3Client.PutObjectAsync(putRequest, cancellation);
            return Ok(url);
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
                    var tempUrl = await _tempStorageService.SaveFilesToTempStorageAsync(i.Image.OpenReadStream(),i.Image.ContentType,i.Image.FileName);;
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