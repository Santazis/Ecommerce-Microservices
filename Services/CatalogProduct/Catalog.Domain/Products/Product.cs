using User.Domain.Models.Primitives;

namespace Catalog.Domain.Products;

public sealed class Product : AggregateRoot
{
    private Product(Guid id) : base(id)
    {
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
    
    private List<ProductImage> _images = [];
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
            MerchantId = merchantId
        };
    } 
    
    public void AddImage(ProductImage productImage)
    {
        _images.Add(productImage);
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(x => x.Id == imageId);
        if (image is not null)
        {
            _images.Remove(image);
        }
    }
    
    
}