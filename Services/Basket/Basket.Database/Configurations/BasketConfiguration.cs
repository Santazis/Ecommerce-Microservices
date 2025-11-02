using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Database.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Domain.Baskets.Basket>
{
    public void Configure(EntityTypeBuilder<Domain.Baskets.Basket> builder)
    {
        builder.ToTable("Baskets", "public").HasKey(b => b.Id);

        builder.HasIndex(b => b.UpdatedAt);
        
        builder.HasMany(b=> b.Items)
            .WithOne()
            .HasForeignKey(i=> i.BasketId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
    }
}