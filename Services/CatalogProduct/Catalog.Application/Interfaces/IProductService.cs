using Catalog.Application.Models.Requests.Product;
using SharedKernel.Common;

namespace Catalog.Application.Interfaces;

public interface IProductService
{
    Task<Result> CreateAsync(CreateProductRequest request,CancellationToken cancellation);
}