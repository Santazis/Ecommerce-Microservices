using User.Domain.Models.Entities.Users;
using User.Domain.Models.Primitives;

namespace User.Domain.DomainEvents;

public sealed record UserCreatedDomainEvent(Models.Entities.Users.User User) : IDomainEvent {}
  