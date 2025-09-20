using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Database.Configurations;

public sealed class CatalogConfiguration : IEntityTypeConfiguration<Domain.Catalogs.Catalog>
{
    public void Configure(EntityTypeBuilder<Domain.Catalogs.Catalog> builder)
    {
        builder.ToTable("Catalogs", "public")
            .HasKey(c=> c.Id);
        
        builder.Property(c => c.Name).HasMaxLength(128);
        builder.Property(c => c.Description).HasMaxLength(256);
        
        builder.HasOne(c=> c.Parent)
            .WithMany()
            .HasForeignKey(c=> c.ParentId)
            .OnDelete(deleteBehavior: DeleteBehavior.Restrict);
        
        
    }
}