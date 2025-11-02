using Basket.Application.Basket;
using Contracts.IntegrationEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Basket.Infrastructure.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly IBasketService _basketService;
    private readonly ILogger<UserCreatedConsumer> _logger;
    public UserCreatedConsumer(IBasketService basketService, ILogger<UserCreatedConsumer> logger)
    {
        _basketService = basketService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        _logger.LogInformation("User Created Integration Event consumed");
        await  _basketService.CreateBasketAsync(context.Message.UserId,context.CancellationToken);
        _logger.LogInformation("Event processed successfully for user {userId}",context.Message.UserId);
    }
}