using Contracts.Dtos;
using Contracts.IntegrationEvents;
using ImageProcessing.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ImageProcessing.Consumers;

public class ProcessImagesConsumer : IConsumer<ProcessImagesIntegrationEvent>
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ProcessImagesConsumer> _logger;

    public ProcessImagesConsumer(IImageProcessingService imageProcessingService, IPublishEndpoint publishEndpoint,
        ILogger<ProcessImagesConsumer> logger)
    {
        _imageProcessingService = imageProcessingService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessImagesIntegrationEvent> context)
    {
        _logger.LogInformation("Consumed {imagesCount} images for product {productId}", context.Message.Images.Count,
            context.Message.ProductId);
        var result =
            await _imageProcessingService.SaveProductImagesAsync(context.Message.ProductId, context.Message.Images);
        var imageContracts = result.Result.ToDictionary(
            k => k.Key,
            k => new ProductImageDataContract(k.Value?.Url, k.Value!.Status));
        _logger.LogInformation(
            "Images saved for ProductId={ProductId}. SuccessCount={SuccessCount}, FailedCount={FailedCount}",
            context.Message.ProductId, result.SuccessCount, result.FailedCount);
        await _publishEndpoint.Publish(new ImageProcessedIntegrationEvent(result.ProductId, imageContracts,
            result.SuccessCount, result.FailedCount));
        _logger.LogInformation("ImageProcessedIntegrationEvent published for ProductId={ProductId}", context.Message.ProductId);
    }
}