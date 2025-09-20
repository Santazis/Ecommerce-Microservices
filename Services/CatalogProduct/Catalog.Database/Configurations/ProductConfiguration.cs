using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Database.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "public").HasKey(p=> p.Id);

        builder.ComplexProperty(p => p.Price, p =>
        {
            p.Property(p => p.Amount).HasColumnName("Amount");
            p.Property(p => p.Currency).HasColumnName("Currency");
        });
        
        builder.HasOne(p=> p.Catalog)
            .WithMany()
            .HasForeignKey(c=> c.CatalogId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

        builder.Property(p => p.Description).HasMaxLength(256);
        builder.Property(p => p.Name).HasMaxLength(128);
        builder.HasMany(p=> p.Images)
            .WithOne(i=> i.Product)
            .HasForeignKey(i=> i.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);
        
    }
}