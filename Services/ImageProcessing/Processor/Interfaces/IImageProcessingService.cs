using ImageProcessing.Models;

namespace ImageProcessing.Interfaces;

public interface IImageProcessingService
{
    Task<ProductImageProcessedResult> SaveProductImagesAsync(IEnumerable<ProcessImageRequest> images);
}