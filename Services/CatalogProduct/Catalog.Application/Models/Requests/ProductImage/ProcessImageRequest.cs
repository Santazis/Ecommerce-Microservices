namespace Catalog.Application.Models.Requests.ProductImage;

public record ProcessImageRequest(Guid ProductId,string TempUrl, int SortOrder);