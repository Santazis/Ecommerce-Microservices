namespace ImageProcessing.Models;

public record ProductImageProcessedResult(
    Guid ProductId,
    Dictionary<Guid,ProductImageData> Result,
    int SuccessCount,
    int FailedCount
);