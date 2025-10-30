using Catalog.Application.Models.Product;
using Catalog.Application.Models.Requests.Pagination;
using Catalog.Application.Models.Requests.Product;
using Catalog.Application.Models.Responses;
using SharedKernel.Common;

namespace Catalog.Application.Interfaces;

public interface IProductService
{
    Task<Result<Guid>> CreateAsync(CreateProductRequest request,CancellationToken cancellation);
    Task<PaginatedResponse<ProductDto>> GetProductsAsync(PaginationRequest pagination,CancellationToken cancellation);
    Task<Result> DeleteAsync(Guid id,Guid userId, CancellationToken cancellation);
    Task<Result<ProductDto>> GetByIdAsync(Guid id,CancellationToken cancellation);
}