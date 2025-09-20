using User.Domain.Models.Primitives;

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

    public static ProductImage Create(string url, int sortOrder)
    {
        return new ProductImage(Guid.NewGuid())
        {
            Url = url,
            SortOrder = sortOrder,
        };
    }
}