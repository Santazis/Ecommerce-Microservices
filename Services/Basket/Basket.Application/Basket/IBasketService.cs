using SharedKernel.Common;

namespace Basket.Application.Basket;

public interface IBasketService
{
    Task<Result> CreateBasketAsync(Guid userId, CancellationToken cancellation);
    Task<Result> DeleteProductFromBasketsAsync(Guid productId, CancellationToken cancellation);
    Task<Result> AddProductToBasketAsync(Guid basketId, AddProductRequest request, CancellationToken cancellation);
    Task<Result<BasketDto>> GetBasketAsync(Guid basketId, CancellationToken cancellation);
    Task<Result> DeleteProduct(Guid basketId, DeleteBasketItemRequest request, CancellationToken cancellation);
}

public record AddProductRequest(Guid ProductId, int Quantity);