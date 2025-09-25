using Catalog.Application.Models.Catalogs;
using Catalog.Application.Models.Requests.Catalog;
using SharedKernel.Common;

namespace Catalog.Application.Interfaces;

public interface ICatalogService
{
    Task<Result> CreateAsync(CreateCatalogRequest request,CancellationToken cancellation);
    Task<Result<CatalogDto>> GetByIdAsync(Guid id,CancellationToken cancellation);
    Task<Result<IEnumerable<CatalogsMenuDto>>> GetCatalogsMenuAsync(CancellationToken cancellation);
    Task<Result> CatalogExistsAsync(Guid id,CancellationToken cancellation);
}