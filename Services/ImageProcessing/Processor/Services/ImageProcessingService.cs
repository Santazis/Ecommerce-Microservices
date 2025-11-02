using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessing.Services;

public class ImageProcessingService : IImageProcessingService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3Settings> _s3Settings;
    private const int MaxWidth = 800;
    private const int MaxHeight = 800;
    private const int ThumbnailWidth = 200;
    private const int ThumbnailHeight = 200;
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings, ILogger<ImageProcessingService> logger)
    {
        _s3Client = s3Client;
        _s3Settings = s3Settings;
        _logger = logger;
    }

    public async Task<ProductImageProcessedResult> SaveProductImagesAsync(Guid productId,
        Dictionary<Guid, string> images)
    {
        _logger.LogInformation("Processing {imagesCount} images for product {productId}", images.Count, productId);
        var semaphore = new SemaphoreSlim(2);
        var tasks = images.Select(async x =>
        {
            await semaphore.WaitAsync();
            _logger.LogInformation("Processing image {imageName} for product {productId}", x.Value, productId);
            try
            {
                var bucketResponse = await _s3Client.GetObjectAsync(_s3Settings.Value.TempBucket, x.Value);
                await using var stream = bucketResponse.ResponseStream;
                using var image = await Image.LoadAsync(stream);
                image.Mutate(i => i.Resize(new ResizeOptions()
                {
                    Size = new Size(MaxWidth, MaxHeight),
                    Mode = ResizeMode.Max
                }));
                await using var ms = new MemoryStream();
                await image.SaveAsWebpAsync(ms);
                ms.Position = 0;
                var putRequest = new PutObjectRequest()
                {
                    Key = $"products/{productId}/{x.Key}",
                    BucketName = _s3Settings.Value.BucketName,
                    InputStream = ms,
                    ContentType = MediaTypeNames.Image.Webp,
                    Metadata = { ["file-name"] = x.Value }
                };
                await _s3Client.PutObjectAsync(putRequest);
                var deleteRequest = new DeleteObjectRequest()
                {
                    Key = x.Value,
                    BucketName = _s3Settings.Value.TempBucket,
                };
                var url = $"{_s3Settings.Value.ServiceUrl}/{_s3Settings.Value.BucketName}/{putRequest.Key}";

                await _s3Client.DeleteObjectAsync(deleteRequest);
                return new KeyValuePair<Guid, ProductImageData>(x.Key,
                    new ProductImageData(url, ImageProcessingResultStatus.Success));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing image {imageName} for product {productId}", x.Value, productId);
                return new KeyValuePair<Guid, ProductImageData>(x.Key,
                    new ProductImageData(null, ImageProcessingResultStatus.Failed));
            }
            finally
            {
                semaphore.Release();
            }
        });
        var processedImages = await Task.WhenAll(tasks);
        int successCount = processedImages.Count(x => x.Value.Status == ImageProcessingResultStatus.Success);
        int failedCount = processedImages.Length - successCount;
        var result = processedImages.ToDictionary(x => x.Key, x => x.Value);
        _logger.LogInformation("Processing {imagesCount} images for product {productId} completed", images.Count, productId);
        return new ProductImageProcessedResult(productId, result, successCount, failedCount);
    }

    public async Task DeleteProductImagesAsync(Guid productId)
    {
        _logger.LogInformation("Deleting images for product {productId}", productId);
        var prefix = $"products/{productId}/";
        string continuationToken = null;
        do
        {
            var listObjectsRequest = new ListObjectsV2Request()
            {
                BucketName = _s3Settings.Value.BucketName,
                Prefix = prefix,
                ContinuationToken = continuationToken
            };
            var listObjectsResponse = await _s3Client.ListObjectsV2Async(listObjectsRequest);
            if (listObjectsResponse.S3Objects.Any())
            {
                var deleteRequest = new DeleteObjectsRequest()
                {
                    BucketName = _s3Settings.Value.BucketName,
                    Objects = listObjectsResponse.S3Objects.Select(x => new KeyVersion()
                    {
                        Key = x.Key
                    }).ToList()
                };
                await _s3Client.DeleteObjectsAsync(deleteRequest);
            }
            continuationToken = listObjectsResponse.NextContinuationToken;
        } while (continuationToken != null);
        _logger.LogInformation("Images for product {productId} deleted", productId);
    }
}