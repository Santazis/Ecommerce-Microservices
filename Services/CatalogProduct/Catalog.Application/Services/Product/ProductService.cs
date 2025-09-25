using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.Product;
using Catalog.Database;
using Catalog.Domain.Common.Errors;
using Catalog.Domain.Products;
using SharedKernel.Common;

namespace Catalog.Application.Services.Product;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICatalogService _catalogService;

    public ProductService(ApplicationDbContext dbContext, ICatalogService catalogService)
    {
        _dbContext = dbContext;
        _catalogService = catalogService;
    }

    public async Task<Result<Guid>> CreateAsync(CreateProductRequest request, CancellationToken cancellation)
    {
        var catalogExists = await _catalogService.CatalogExistsAsync(request.CatalogId, cancellation);
        if (catalogExists.IsFailure)
        {
            return Result<Guid>.Failure(catalogExists.Error);
        }
        var amount = Money.Create(request.Price, request.Currency);
        var product = Domain.Products.Product.Create(request.Name, request.Description, amount, request.CatalogId,
            request.StockQuantity,
            request.MerchantId);
        await _dbContext.Products.AddAsync(product,cancellation);
        await _dbContext.SaveChangesAsync(cancellation);
        return Result<Guid>.Success(product.Id);
    }
}