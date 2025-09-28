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
    public async Task<ProductImageProcessedResult> SaveProductImagesAsync(Guid productId,Dictionary<Guid,string> images)
    {
        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);
        var semaphore = new SemaphoreSlim(2);
        var tasks = images.Select(async x =>
        {
            await semaphore.WaitAsync();
            try
            {
                using var image = await Image.LoadAsync(x.Value);
                image.Mutate(i => i.Resize(new ResizeOptions()
                {
                    Size = new Size(MaxWidth, MaxHeight),
                    Mode = ResizeMode.Max
                }));
        
                var path = Path.Combine(Folder, $"{Guid.NewGuid()}.webp");
                Console.WriteLine(path);
                await image.SaveAsWebpAsync(path);
                File.Delete(x.Value);
                return new KeyValuePair<Guid,ProductImageData>(x.Key,new ProductImageData(path,   ImageProcessingResultStatus.Success)) ;
            }
            catch (Exception e)
            {
                return new KeyValuePair<Guid,ProductImageData>(x.Key,new ProductImageData(null,   ImageProcessingResultStatus.Failed)) ;
            } finally{semaphore.Release();}
        }); 
        var processedImages = await Task.WhenAll(tasks);
        int successCount = processedImages.Count(x => x.Value.Status == ImageProcessingResultStatus.Success);
        int failedCount = processedImages.Length - successCount;
        var result = processedImages.ToDictionary(x=> x.Key,x=> x.Value);
        return new ProductImageProcessedResult(productId,result, successCount, failedCount);
    }
}