using SharedKernel.Primitives;

namespace Catalog.Domain.DomainEvents;

public record ProductCreatedDomainEvent(Guid ProductId) : IDomainEvent;
