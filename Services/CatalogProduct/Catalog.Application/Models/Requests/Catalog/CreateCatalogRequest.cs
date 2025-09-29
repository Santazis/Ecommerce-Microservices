namespace Catalog.Application.Models.Requests.Catalog;

public record CreateCatalogRequest(string Name,string? Description,string Slug,Guid? ParentId);