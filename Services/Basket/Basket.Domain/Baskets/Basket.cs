
using SharedKernel.Primitives;

namespace Basket.Domain.Baskets;

public sealed class Basket : AggregateRoot
{
    private Basket(Guid id) : base(id)
    {
    }

    private Basket()
    {
        _items = new HashSet<BasketItem>();

    }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    private HashSet<BasketItem> _items = new HashSet<BasketItem>();
    public IReadOnlyCollection<BasketItem> Items => _items.ToList();
    public static Basket Create(Guid userId)
    {
        return new Basket(Guid.NewGuid())
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
        };
    }
    public void AddItem(Guid productId,int quantity)
    {
        var existingItem = _items.FirstOrDefault(i=> i.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            return;
        }
        var item = BasketItem.Create(productId,Id,quantity);
        _items.Add(item);
    }
    public void RemoveItem(Guid productId)
    {
        var existingItem = _items.FirstOrDefault(i=> i.ProductId == productId);
        if (existingItem != null)
        {
            _items.Remove(existingItem);
        }
        UpdateCart();
    }
    public void Clear()
    {
        _items.Clear();
    }
    public void IncreaseQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i=> i.ProductId == productId);
        if (item != null)
        {
            item.IncreaseQuantity(quantity);
        }
        UpdateCart();
    }
    public void DecreaseQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i=> i.ProductId == productId);
        if (item != null)
        {
          var result = item.DecreaseQuantity(quantity);
          if (result == 0)
          {
              _items.Remove(item);
          }
        }
        UpdateCart();
    }
    private void UpdateCart()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}