using Catalog.Application.Models.Images;

namespace Catalog.Application.Models.Product;

public record ProductDto(Guid Id,Guid CatalogId, string Name, string Description, decimal Price,string Currency,List<ImageDto> Images,bool InStock);