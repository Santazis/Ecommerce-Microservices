using Basket.Application.Basket;
using Basket.Application.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProductGrpc;
using SharedKernel.Common;
using ProductGrpc_BasketItemRequest = ProductGrpc.BasketItemRequest;

namespace Basket.Infrastructure.Grpc;

public class ProductServiceClient : IProductServiceClient
{
    private readonly ProductGrpc.ProductService.ProductServiceClient _client;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(ProductService.ProductServiceClient client, ILogger<ProductServiceClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Result<bool>> IsProductAvailableAsync(Guid productId, int quantity,
        CancellationToken cancellation)
    {
        try
        {
            var request = new IsProductAvailableRequestGrpc()
            {
                Id = productId.ToString(),
                Quantity = quantity
            };
            _logger.LogDebug("Checking availability of product {ProductId} with quantity {Quantity}", productId,
                quantity);

            var result = await _client.IsProductAvailableAsync(request, cancellationToken: cancellation);
            if (!result.IsAvailable)
            {
                _logger.LogInformation("Product not available");
                Result.Failure(new Error("Product.NotAvailable", "Product not available"));
            }

            return Result<bool>.Success(true);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Error while checking availability of product {ProductId} with quantity {Quantity}",
                productId, quantity);
            return Result<bool>.Failure(new Error("Product.NotAvailable", ex.Message));
        }
    }

    public async Task<IEnumerable<BasketItemDto>> GetProductsByIdsAsync(Dictionary<Guid, int> requests,
        CancellationToken cancellation)
    {
        _logger.LogDebug("Getting {requestsCount} products from product service",requests.Count);
        var request = new GetProductsRequestGrpc()
        {
            Items =
            {
                requests.Select(r => new ProductGrpc_BasketItemRequest()
                {
                    Id = r.Key.ToString(),
                    Qty = r.Value
                })
            }
        };
        var response = await _client.GetProductsAsync(request, cancellationToken: cancellation);
        _logger.LogDebug("Got {productsCount} products from product service",response.Products.Count);
        return response.Products.Select(p =>
        {
            var productId = Guid.Parse(p.Id);
            var quantity = requests[productId];
            return new BasketItemDto(productId, p.Name, quantity, p.Qty, p.Price / 100m, p.Currency, p.ImageUrl,
                p.Qty >= quantity);
        });
    }
}