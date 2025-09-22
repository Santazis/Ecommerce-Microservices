using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.Product;
using SharedKernel.Common;

namespace Catalog.Application.Services.Product;

public class ProductService : IProductService
{
    public Task<Result> CreateAsync(CreateProductRequest request, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}