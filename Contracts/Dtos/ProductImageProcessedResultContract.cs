namespace Contracts.Dtos;

public record ProductImageProcessedResultContract(
    Guid ProductId,
    IEnumerable<ProductImageDataContract> Result,
    int SuccessCount,
    int FailedCount
);