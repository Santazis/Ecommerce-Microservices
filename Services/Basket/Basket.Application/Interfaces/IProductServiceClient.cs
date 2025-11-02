using Basket.Application.Basket;
using SharedKernel.Common;

namespace Basket.Application.Interfaces;

public interface IProductServiceClient
{
    Task<Result<bool>> IsProductAvailableAsync(Guid productId,int quantity,CancellationToken cancellation);
    Task<IEnumerable<BasketItemDto>> GetProductsByIdsAsync(Dictionary<Guid,int> requests,CancellationToken cancellation);
}