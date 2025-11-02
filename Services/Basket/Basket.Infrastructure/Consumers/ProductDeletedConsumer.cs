using Basket.Application.Basket;
using Contracts.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Basket.Infrastructure.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeletedIntegrationEvent>
{
    private readonly IBasketService _basketService;
    private readonly ILogger<ProductDeletedConsumer> _logger;
    public ProductDeletedConsumer(IBasketService basketService, ILogger<ProductDeletedConsumer> logger)
    {
        _basketService = basketService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductDeletedIntegrationEvent> context)
    {
       _logger.LogInformation("Product Deleted Integration Event consumed");
       await _basketService.DeleteProductFromBasketsAsync(context.Message.Id,context.CancellationToken);
       _logger.LogInformation("Event processed successfully for product {productId}",context.Message.Id);
    }
}