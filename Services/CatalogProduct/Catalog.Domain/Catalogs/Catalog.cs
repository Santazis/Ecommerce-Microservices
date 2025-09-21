using System.Text;
using SharedKernel.Primitives;

namespace Catalog.Domain.Catalogs;

public sealed class Catalog : AggregateRoot
{
    private Catalog(Guid id) : base(id)
    {
    }

    private Catalog()
    {
    }
    
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public Catalog? Parent { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? UpdatedDate { get; private set; }
    public string Slug { get; private set; }
    public static Catalog Create(string name, string? description, Catalog? parent,string slug)
    {
        return new Catalog(Guid.NewGuid()) 
        {
            Name = name,
            Description = description,
            ParentId = parent?.Id,
            CreatedDate = DateTime.UtcNow,
            Slug = BuildSlug(parent?.Slug,slug)
        };
    }
    
    public void SetParent(Catalog parent)
    {
        ParentId = parent.Id;
        if (!string.IsNullOrWhiteSpace(parent.Slug))
        {
            Slug = BuildSlug(parent.Slug, Slug);
        }
    }

    public void RemoveParent(Catalog parent)
    {
        ParentId = null;
        if (!string.IsNullOrWhiteSpace(parent.Slug))
        {
            Slug = RemoveSlug(parent.Slug, Slug);
        }
    }

    private static string BuildSlug(string? parentSlug,string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug cannot be null or empty");
        if (!string.IsNullOrWhiteSpace(parentSlug))
        {
            return $"{parentSlug}/{slug}";
        }
        return $"catalog" + "/" +slug;
    }

    private static string RemoveSlug(string parentSlug, string slug)
    {
        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(parentSlug)) throw new ArgumentException("Slug cannot be null or empty");
        string prefix = parentSlug + "/";
        if (slug.StartsWith(prefix))
        {
            var replaced =  slug[prefix.Length..];
            return "catalog" + "/" + replaced;
        }
        return slug;
    }
    
}