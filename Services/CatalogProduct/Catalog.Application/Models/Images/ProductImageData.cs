namespace Catalog.Application.Models.Images;

public record ProductImageData(
    string? Url,
    int SortOrder,
    Guid ProductId,
    string Status);