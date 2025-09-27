namespace ImageProcessing.Models;

public record ProductImageData(
    string? Url,
    int SortOrder,
    Guid ProductId,
    string Status);