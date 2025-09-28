using Contracts.Dtos;

namespace Contracts.IntegrationEvents;

public record ImageProcessedIntegrationEvent(Guid ProductId,Dictionary<Guid,ProductImageDataContract> Images,int SuccessCount,int FailedCount) : IIntegrationEvent;