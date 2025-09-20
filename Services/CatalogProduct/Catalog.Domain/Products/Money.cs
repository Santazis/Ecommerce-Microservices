using User.Domain.Models.Primitives;

namespace Catalog.Domain.Products;

public sealed class Money : ValueObject
{
    public decimal Amount { get;}
    public string Currency { get;}
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    private Money()
    {
    }

    public static Money Create(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }
}