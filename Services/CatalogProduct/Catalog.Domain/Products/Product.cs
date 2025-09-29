using SharedKernel.Primitives;

namespace Catalog.Domain.Products;

public sealed class Product : AggregateRoot
{
    private Product(Guid id) : base(id)
    {
        _images = new List<ProductImage>();
    }

    private Product()
    {
    }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public Guid CatalogId { get; private set; }
    public Catalogs.Catalog Catalog { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public Guid? MerchantId { get; private set; }
    
    private List<ProductImage> _images;
    public IReadOnlyCollection<ProductImage> Images => _images;

    public static Product Create(string name, string description, Money price, Guid catalogId, int stockQuantity,
        Guid? merchantId)
    {
        return new Product(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            Price = price,
            CatalogId = catalogId,
            StockQuantity = stockQuantity,
            MerchantId = merchantId,
        };
    } 
    
    public Guid AddImage(string url, int sortOrder)
    {
        var image = ProductImage.Create(url, sortOrder, Id);
        _images.Add(image);
        return image.Id;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is not null)
        {
            _images.Remove(image);
        }
    }
    
    public void UpdateImage(Guid imageId, string url, int? sortOrder)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is null) return;
        if (!sortOrder.HasValue)
        {
            image?.UpdateUrl(url);
            return;
        }
        image?.Update(url, sortOrder.Value);
    }
    
}