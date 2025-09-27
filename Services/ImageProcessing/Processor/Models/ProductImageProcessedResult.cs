namespace ImageProcessing.Models;

public record ProductImageProcessedResult(
    IEnumerable<ProductImageData> Result,
    int SuccessCount,
    int FailedCount
);