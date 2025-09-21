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
    
    public static Catalog Create(string name, string? description, Guid? parentId)
    {
        return new Catalog(Guid.NewGuid()) 
        {
            Name = name,
            Description = description,
            ParentId = parentId,
            CreatedDate = DateTime.UtcNow
        };
    }
    
    public void SetParent(Guid parentId)
    {
        ParentId = parentId;
    }

    public void RemoveParent()
    {
        ParentId = null;   
    }
}