using Catalog.Application.Interfaces;
using Catalog.Application.Models.Requests.Product;
using Catalog.Database;
using Catalog.Domain.Common.Errors;
using Catalog.Domain.Products;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace Catalog.Application.Services.Product;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICatalogService _catalogService;
    private readonly ILogger<ProductService> _logger;
    public ProductService(ApplicationDbContext dbContext, ICatalogService catalogService, ILogger<ProductService> logger)
    {
        _dbContext = dbContext;
        _catalogService = catalogService;
        _logger = logger;
    }

    public async Task<Result<Guid>> CreateAsync(CreateProductRequest request, CancellationToken cancellation)
    {
        _logger.LogInformation("Creating product {name} for catalog {catalogId}",request.Name,request.CatalogId);
        var catalogExists = await _catalogService.CatalogExistsAsync(request.CatalogId, cancellation);
        if (catalogExists.IsFailure)
        {
            _logger.LogError("Catalog {catalogId} not found",request.CatalogId);
            return Result<Guid>.Failure(catalogExists.Error);
        }
        var amount = Money.Create(request.Price, request.Currency);
        var product = Domain.Products.Product.Create(request.Name, request.Description, amount, request.CatalogId,
            request.StockQuantity,
            request.MerchantId);
        await _dbContext.Products.AddAsync(product,cancellation);
        await _dbContext.SaveChangesAsync(cancellation);
        _logger.LogInformation("Product created {productId}",product.Id);
        return Result<Guid>.Success(product.Id);
    }
}