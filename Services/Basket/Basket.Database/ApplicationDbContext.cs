using Microsoft.EntityFrameworkCore;

namespace Basket.Database;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Domain.Baskets.Basket> Baskets { get; set; }
    public DbSet<Basket.Domain.Baskets.BasketItem> BasketItems { get; set; }
    public ApplicationDbContext(DbContextOptions options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}