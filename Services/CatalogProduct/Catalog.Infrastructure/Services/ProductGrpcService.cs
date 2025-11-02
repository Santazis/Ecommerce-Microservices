using Catalog.Application.Interfaces;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProductGrpc;

namespace Catalog.Infrastructure.Services;

public class ProductGrpcService : ProductGrpc.ProductService.ProductServiceBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductGrpcService> _logger;

    public ProductGrpcService(IProductService productService, ILogger<ProductGrpcService> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public override async Task<IsProductAvailableResponseGrpc> IsProductAvailable(IsProductAvailableRequestGrpc request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received IsProductAvailable request: ProductId={ProductId}, Quantity={Quantity}",
            request.Id, request.Quantity);

        if (!Guid.TryParse(request.Id, out var guid))
        {
            _logger.LogWarning("Invalid product id: {ProductId}", request.Id);

            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid product id"));
        }

        var result = await _productService.IsAvailableAsync(guid, request.Quantity, context.CancellationToken);
        if (result.IsFailure)
        {
            var errorMessage = result.Error.Message ?? "Unknown error";
            _logger.LogInformation("Product {ProductId} unavailable: {Error}", request.Id, result.Error.Message);

            throw new RpcException(new Status(StatusCode.FailedPrecondition, errorMessage));
        }

        _logger.LogInformation("Product {ProductId} is available", request.Id);

        return new IsProductAvailableResponseGrpc { IsAvailable = result.Value };
    }

    public override async Task<GetProductsResponseGrpc> GetProducts(GetProductsRequestGrpc request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received GetProducts request for {Count} products", request.Items.Count);

        var items = request.Items.ToDictionary(i => Guid.Parse(i.Id), i => i.Qty);
        var products = await _productService.GetProductsByIds(items.Keys.ToHashSet(), context.CancellationToken);

        var response = new GetProductsResponseGrpc();
        response.Products.AddRange(products.Select(p => new Product()
        {
            Id = p.Id.ToString(),
            Price = (long)(p.Price * 100),
            Currency = p.Currency,
            Name = p.Name,
            ImageUrl = p.Images.Select(i => i.Url).FirstOrDefault() ?? string.Empty,
            Qty = p.Qty
        }));

        _logger.LogInformation("Returning {Count} products", response.Products.Count);

        return response;
    }
}