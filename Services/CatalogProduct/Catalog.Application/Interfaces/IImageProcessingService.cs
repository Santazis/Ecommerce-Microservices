using Catalog.Application.Models.Images;
using Catalog.Application.Models.Requests.ProductImage;

namespace Catalog.Application.Interfaces;

public interface IImageProcessingService
{
    Task<ProductImageProcessedResult> SaveProductImagesAsync(IEnumerable<ProcessImageRequest> images);
}