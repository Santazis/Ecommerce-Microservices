using Catalog.Application.Models.Requests.ProductImage;

namespace Catalog.Application.Models.Requests.Product;

public record CreateProductRequest(string Name, string Description, decimal Price,string Currency,Guid CatalogId,int StockQuantity,Guid? MerchantId,IEnumerable<ProductImageData> Images);