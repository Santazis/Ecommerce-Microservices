using Contracts.IntegrationEvents;
using ImageProcessing.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ImageProcessing.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeletedIntegrationEvent>
{
    private readonly ILogger<ProductDeletedConsumer> _logger;
    private readonly IImageProcessingService _imageProcessingService;

    public ProductDeletedConsumer(ILogger<ProductDeletedConsumer> logger, IImageProcessingService imageProcessingService)
    {
        _logger = logger;
        _imageProcessingService = imageProcessingService;
    }

    public async Task Consume(ConsumeContext<ProductDeletedIntegrationEvent> context)
    {
        _logger.LogInformation("Consuming ProductDeletedIntegrationEvent {productId}",context.Message.Id);
        await _imageProcessingService.DeleteProductImagesAsync(context.Message.Id);
        _logger.LogInformation("ProductDeletedIntegrationEvent consumed {productId}",context.Message.Id);
    }
}