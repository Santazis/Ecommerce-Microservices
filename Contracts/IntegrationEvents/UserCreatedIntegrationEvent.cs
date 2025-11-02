namespace Contracts.IntegrationEvents;

public record UserCreatedIntegrationEvent(Guid UserId) : IIntegrationEvent
{
    
}