namespace Catalog.Application.Models.Catalogs;

public sealed record CatalogsMenuDto(Guid Id, string Name, string? Description, Guid? ParentId, string Slug,List<CatalogsMenuDto> Childs)
{
    
}