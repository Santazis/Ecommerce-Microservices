using Catalog.Application.Interfaces;
using Catalog.Application.Models.Catalogs;
using Catalog.Application.Models.Requests.Catalog;
using Catalog.Database;
using Catalog.Domain.Common.Errors;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;

namespace Catalog.Application.Services.Catalog;

public class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _context;

    public CatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> CreateAsync(CreateCatalogRequest request, CancellationToken cancellation)
    {
        var createdCatalog = Domain.Catalogs.Catalog.Create(request.Name, request.Description, request.ParentId);
        await _context.Catalogs.AddAsync(createdCatalog,cancellation);
        return Result.Success;
    }

    public async Task<Result<CatalogDto>> GetByIdAsync(Guid id, CancellationToken cancellation)
    {
        var catalog = await _context.Catalogs
                .Select(c=> new CatalogDto(c.Id,c.Name,c.Description,c.ParentId))
                .FirstOrDefaultAsync(c => c.Id == id,cancellation);
        if (catalog is null) return Result<CatalogDto>.Failure(CatalogErrors.NotFound);
        return Result<CatalogDto>.Success(catalog);
    }


    public async Task<Result<IEnumerable<CatalogsMenuDto>>> GetCatalogsMenuAsync(CancellationToken cancellation)
    {
        var catalogs = await _context.Catalogs.AsNoTracking()
            .Select(c => new CatalogsMenuDto(c.Id, c.Name, c.Description, c.ParentId, new List<CatalogsMenuDto>()))
            .ToListAsync(cancellation);
        var catalogRoots = new List<CatalogsMenuDto>();
        var catalogDict = catalogs.ToDictionary(c=> c.Id);
        foreach (var catalog in catalogs)
        {
            if (catalog.ParentId.HasValue && catalogDict.TryGetValue(catalog.ParentId.Value,out var parent))
            {
                parent.Childs.Add(catalog);
            }
            else
            {
                catalogRoots.Add(catalog);
            }
        }
        return Result<IEnumerable<CatalogsMenuDto>>.Success(catalogRoots);
    }
    
}