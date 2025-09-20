using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Database.Configurations;

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages", "public").HasKey(i => i.Id);
        builder.Property(i => i.Url).IsRequired();
        builder.HasOne(i=> i.Product)
            .WithMany(p=> p.Images)
            .HasForeignKey(i=> i.ProductId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
    }
}