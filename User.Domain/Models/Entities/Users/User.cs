using User.Domain.DomainEvents;
using User.Domain.Models.Primitives;

namespace User.Domain.Models.Entities.Users;

public class User : AggregateRoot
{
    private User(Guid id) : base(id)
    {
    }
    protected User(){}
    public Name Name { get;private set; }
    public Email Email { get;private set; }
    public Address Address { get;private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }
    
    public static User Create(Name name,Email email,Address address)
    {
        var user = new User(Guid.NewGuid())
        {
            Name = name,
            Email = email,
            Address = address,
            CreatedAt = DateTime.UtcNow,
        };
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user));
        return user;
    }
}
