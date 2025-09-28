using Catalog.Application.Interfaces;
using Catalog.Application.Models.Images;
using Catalog.Application.Models.Requests.ProductImage;
using Contracts.IntegrationEvents;
using MassTransit;

namespace Catalog.Infrastructure.Consumers;

public class ImageProcessedConsumer : IConsumer<ImageProcessedIntegrationEvent>
{
    private readonly IProductImageService _productImageService;

    public ImageProcessedConsumer(IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    public async Task Consume(ConsumeContext<ImageProcessedIntegrationEvent> context)
    {
        var updateRequests = context.Message.Images.Where(r => r.Value.Status == ImageProcessingResultStatus.Success && r.Value.Url != null)
            .Select(r => new UpdateImageRequest(r.Key, r.Value.Url!,null));
        await _productImageService.UpdateImagesAsync(context.Message.ProductId, updateRequests,
            context.CancellationToken);
    }
}