using ImageProcessing.Models;

namespace ImageProcessing.Interfaces;

public interface IImageProcessingService
{
    Task<ProductImageProcessedResult> SaveProductImagesAsync(Guid productId,Dictionary<Guid,string> images);
}