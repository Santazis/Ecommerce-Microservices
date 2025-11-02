using Catalog.Application.Models.Requests.ProductImage;
using SharedKernel.Common;

namespace Catalog.Application.Interfaces;

public interface IProductImageService
{
    Task<Result<Dictionary<Guid,string>>> CreateImagesAsync(Guid productId,IEnumerable<ProcessImageRequest> requests, CancellationToken cancellation);
    Task<Result> UpdateImagesAsync(Guid productId,IEnumerable<UpdateImageRequest> requests, CancellationToken cancellation);
}