using Contracts.Dtos;
using Contracts.IntegrationEvents;
using ImageProcessing.Interfaces;
using MassTransit;

namespace ImageProcessing.Consumers;

public class ProcessImagesConsumer : IConsumer<ProcessImagesIntegrationEvent>
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProcessImagesConsumer(IImageProcessingService imageProcessingService, IPublishEndpoint publishEndpoint)
    {
        _imageProcessingService = imageProcessingService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<ProcessImagesIntegrationEvent> context)
    {
        var result =
            await _imageProcessingService.SaveProductImagesAsync(context.Message.ProductId, context.Message.Images);
        var imageContracts = result.Result.ToDictionary(
            k => k.Key,
            k => new ProductImageDataContract(k.Value?.Url, k.Value!.Status));
        await _publishEndpoint.Publish(new ImageProcessedIntegrationEvent(result.ProductId, imageContracts,
            result.SuccessCount, result.FailedCount));
    }
}