using System.Net.Mime;
using Amazon.S3;
using Amazon.S3.Model;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using ImageProcessing.Options;
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

    public ImageProcessingService(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
    {
        _s3Client = s3Client;
        _s3Settings = s3Settings;
    }

    public async Task<ProductImageProcessedResult> SaveProductImagesAsync(Guid productId,
        Dictionary<Guid, string> images)
    {
        var semaphore = new SemaphoreSlim(2);
        var tasks = images.Select(async x =>
        {
            await semaphore.WaitAsync();
            try
            {
                var buckerResponse = await _s3Client.GetObjectAsync(_s3Settings.Value.TempBucket, x.Value);
                await using var stream = buckerResponse.ResponseStream;
                using var image = await Image.LoadAsync(stream);
                image.Mutate(i => i.Resize(new ResizeOptions()
                {
                    Size = new Size(MaxWidth, MaxHeight),
                    Mode = ResizeMode.Max
                }));
                await using var ms = new MemoryStream();
                await image.SaveAsWebpAsync(ms);
                ms.Position = 0;
                var key = Guid.NewGuid().ToString();
                var putRequest = new PutObjectRequest()
                {
                    Key = $"products/{key}",
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
        return new ProductImageProcessedResult(productId, result, successCount, failedCount);
    }
}