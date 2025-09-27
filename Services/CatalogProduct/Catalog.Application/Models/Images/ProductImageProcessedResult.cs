namespace Catalog.Application.Models.Images;

public record ProductImageProcessedResult(
    IEnumerable<ProductImageData> Result,
    int SuccessCount,
    int FailedCount
);