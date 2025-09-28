namespace Contracts.IntegrationEvents;

public record ProcessImagesIntegrationEvent(Guid ProductId,Dictionary<Guid,string> Images) : IIntegrationEvent;
