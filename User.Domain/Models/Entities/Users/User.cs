using User.Domain.DomainEvents;
using User.Domain.Models.Primitives;

namespace User.Domain.Models.Entities.Users;

public class User : AggregateRoot
{
    private User(Guid id) : base(id)
    {
    }
    protected User(){}
    public Email Email { get;private set; }
    public Name Name { get;private set; }
    public Address Address { get;private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }
    
    public static User Create(Guid id,Email email)
    {
        var user = new User(id)
        {
            Name = new Name(null,null),
            Address = Address.Create(null,null,null,null,null),
            Email = email,
            CreatedAt = DateTime.UtcNow,
        };
        return user;
    }

    public void CreateProfile(Name name, Address address)
    {
        Name = name;
        Address = address;
    }
    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
    }
}
