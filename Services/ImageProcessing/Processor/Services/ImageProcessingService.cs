using System.Net.Mime;
using ImageProcessing.Interfaces;
using ImageProcessing.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessing.Services;

public class ImageProcessingService : IImageProcessingService
{
    private const int MaxWidth = 800;
    private const int MaxHeight = 800;
    private const int ThumbnailWidth = 200;
    private const int ThumbnailHeight = 200;
    private  string Folder = $"{Environment.CurrentDirectory}/images";
    public async Task<ProductImageProcessedResult> SaveProductImagesAsync(IEnumerable<ProcessImageRequest> images)
    {
        var tasks = images.Select(async x =>
        {
            try
            {
                using var image = await Image.LoadAsync(x.TempUrl);
                image.Mutate(i => i.Resize(new ResizeOptions()
                {
                    Size = new Size(MaxWidth, MaxHeight),
                    Mode = ResizeMode.Max
                }));
                if (!Directory.Exists(Folder))
                {
                    Directory.CreateDirectory(Folder);
                }
        
                var path = Path.Combine(Folder, $"{Guid.NewGuid()}.webp");
                await image.SaveAsWebpAsync(path);
                File.Delete(x.TempUrl);
                return new ProductImageData(path, x.SortOrder, x.ProductId, ImageProcessingResultStatus.Success);
            }
            catch (Exception e)
            {
                return new ProductImageData(null, x.SortOrder, x.ProductId,
                    ImageProcessingResultStatus.Failed);
            }
        });
        var processedImages = await Task.WhenAll(tasks);
        
        int successCount = processedImages.Count(x => x.Status == ImageProcessingResultStatus.Success);
        int failedCount = processedImages.Length - successCount;
        return new ProductImageProcessedResult(processedImages, successCount, failedCount);
    }
}