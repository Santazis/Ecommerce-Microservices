using Microsoft.EntityFrameworkCore;
using User.Database.Configurations;

namespace User.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<User.Domain.Models.Entities.Users.User> Users { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}