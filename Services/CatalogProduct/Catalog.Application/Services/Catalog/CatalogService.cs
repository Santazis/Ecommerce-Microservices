using Catalog.Application.Interfaces;
using Catalog.Application.Models.Catalogs;
using Catalog.Application.Models.Requests.Catalog;
using Catalog.Database;
using Catalog.Domain.Common.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Services.Catalog;

public class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CatalogService> _logger;
    public CatalogService(ApplicationDbContext context, ILogger<CatalogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result> CreateAsync(CreateCatalogRequest request, CancellationToken cancellation)
    {
        _logger.LogInformation("Creating catalog {catalogName} for parent {parentId}",request.Name,request.ParentId);
        Domain.Catalogs.Catalog? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _context.Catalogs.FirstOrDefaultAsync(c => c.Id == request.ParentId,cancellation);
            if (parent is null)
            {
                _logger.LogError("Parent catalog {parentId} not found",request.ParentId);
                return Result.Failure(CatalogErrors.NotFound);
            }
        }
        var createdCatalog = Domain.Catalogs.Catalog.Create(request.Name, request.Description, parent,request.Slug);
        await _context.Catalogs.AddAsync(createdCatalog,cancellation);
        _logger.LogInformation("Catalog created");
        return Result.Success;
    }

    public async Task<Result> CatalogExistsAsync(Guid id, CancellationToken cancellation)
    {
        _logger.LogInformation("Checking if catalog {catalogId} exists",id);
        var exists  =  await _context.Catalogs.AnyAsync(c => c.Id == id,cancellation);
        if (exists)
        {
            _logger.LogInformation("Catalog {catalogId} exists",id);
            return Result.Success;
        }
        _logger.LogInformation("Catalog {catalogId} not found",id);
        return Result.Failure(CatalogErrors.NotFound);
    }

    public async Task<Result<CatalogDto>> GetByIdAsync(Guid id, CancellationToken cancellation)
    {
        _logger.LogInformation("Getting catalog {catalogId}",id);
        var catalog = await _context.Catalogs
            .Where(c => c.Id == id)
            .Select(c => new CatalogDto(c.Id, c.Name, c.Description, c.ParentId, c.Slug))
            .FirstOrDefaultAsync(cancellation);
        if (catalog is null)
        {
            _logger.LogError("Catalog {catalogId} not found",id);
            return Result<CatalogDto>.Failure(CatalogErrors.NotFound);
        }

        _logger.LogInformation("Catalog {catalogId} found",id);
        return Result<CatalogDto>.Success(catalog);
    }


    public async Task<Result<IEnumerable<CatalogsMenuDto>>> GetCatalogsMenuAsync(CancellationToken cancellation)
    {
        var catalogs = await _context.Catalogs.AsNoTracking()
            .Select(c => new CatalogsMenuDto(c.Id, c.Name, c.Description, c.ParentId,c.Slug, new List<CatalogsMenuDto>()))
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