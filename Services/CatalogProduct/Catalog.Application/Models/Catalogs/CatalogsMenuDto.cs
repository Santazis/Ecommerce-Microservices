namespace Catalog.Application.Models.Catalogs;

public sealed record CatalogsMenuDto(Guid Id, string Name,string Description,Guid? ParentId,List<CatalogsMenuDto> Childs)
{
    
}