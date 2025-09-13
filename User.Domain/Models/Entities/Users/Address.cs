using User.Domain.Models.Primitives;

namespace User.Domain.Models.Entities.Users;

public sealed class Address : ValueObject
{
    private Address(string street, string city, string? state, string country, string zipCode)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state;
        Country = country ?? throw new ArgumentNullException(nameof(country));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));
    }
    
    private Address() {}
    public string Street { get; }
    public string City { get; }
    public string? State { get; }
    public string Country { get; }
    public string ZipCode { get; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }


    public static Address Create(string street, string city, string? state, string country, string zipCode)
    {
        return new Address(street, city, state, country, zipCode);
    }
}