using SharedKernel.Primitives;

namespace Catalog.Domain.Products;

public sealed class ProductImage : Entity
{
    private ProductImage(Guid id) : base(id)
    {
    }

    private ProductImage()
    {
    }
    public string Url { get; private set; }
    public int SortOrder { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public static ProductImage Create(string url, int sortOrder,Guid productId)
    {
        return new ProductImage(Guid.NewGuid())
        {
            Url = url,
            ProductId = productId,
            SortOrder = sortOrder,
        };
    }
}