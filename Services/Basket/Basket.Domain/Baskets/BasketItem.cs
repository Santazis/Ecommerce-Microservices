
using SharedKernel.Primitives;

namespace Basket.Domain.Baskets;

public sealed class BasketItem : Entity
{
    public Guid ProductId { get;private set; }
    public Guid BasketId { get;private set; }
    public int Quantity { get;private set; }
    public DateTime DateAdded { get;private set; }
    private BasketItem(Guid id,Guid productId, Guid basketId, int quantity) : base(id)
    {
        ProductId = productId;
        BasketId = basketId;
        Quantity = quantity;
        DateAdded = DateTime.UtcNow;
    }

    private BasketItem()
    {
    }

    internal static BasketItem Create(Guid productId, Guid basketId, int quantity)
    {
        return new BasketItem(Guid.NewGuid(),productId, basketId, quantity);
    }
    internal int IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
        return Quantity;
    }
    internal int DecreaseQuantity(int quantity)
    {
        Quantity -= quantity;
        return Quantity;
    }
}