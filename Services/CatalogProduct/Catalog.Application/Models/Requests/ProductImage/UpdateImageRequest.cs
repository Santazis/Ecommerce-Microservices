namespace Catalog.Application.Models.Requests.ProductImage;

public record UpdateImageRequest(Guid Id,string Url,int? SortOrder)
{
    
}