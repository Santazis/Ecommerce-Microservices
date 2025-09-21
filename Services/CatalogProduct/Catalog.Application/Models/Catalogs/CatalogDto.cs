namespace Catalog.Application.Models.Catalogs;

public record CatalogDto(Guid Id, string Name, string? Description,Guid? ParentId,string Slug)
{
 
}