using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.Models.Entities.Users;

namespace User.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<Domain.Models.Entities.Users.User>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Entities.Users.User> builder)
    {
        builder.ToTable("Users", "public")
            .HasKey(u => u.Id);

        builder.ComplexProperty(u => u.Name, n =>
        {
            n.Property(n => n.FirstName).HasColumnName("FirstName").HasMaxLength(32);
            n.Property(n => n.LastName).HasColumnName("LastName").HasMaxLength(32);
        });
        builder.Property(u => u.Email).HasConversion(e => e.Value,
            value => Email.Create(value));
        builder.HasIndex(u => u.Email).IsUnique();

        builder.ComplexProperty(u => u.Address, a =>
        {
            a.Property(a => a.Street).HasColumnName("Street").HasMaxLength(128);
            a.Property(a => a.City).HasColumnName("City").HasMaxLength(32);
            a.Property(a => a.State).HasColumnName("State").HasMaxLength(32);
            a.Property(a => a.Country).HasColumnName("Country").HasMaxLength(32);
            a.Property(a => a.ZipCode).HasColumnName("ZipCode").HasMaxLength(32);
        });
    }
}