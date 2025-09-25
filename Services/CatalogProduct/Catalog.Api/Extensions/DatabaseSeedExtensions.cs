using Catalog.Database;
using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Extensions
{
    public static class DatabaseSeedExtensions
    {
        public static void SeedCatalogs(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Catalogs.Any())
            {
                var electronics = Catalog.Domain.Catalogs.Catalog.Create("Electronics", "All electronic items", null, "electronics");
                var phones = Catalog.Domain.Catalogs.Catalog.Create("Phones", "Smartphones and mobile phones", electronics, "phones");
                var laptops = Catalog.Domain.Catalogs.Catalog.Create("Laptops", "Portable computers", electronics, "laptops");

                var fashion = Catalog.Domain.Catalogs.Catalog.Create("Fashion", "Clothing and accessories", null, "fashion");
                var men = Catalog.Domain.Catalogs.Catalog.Create("Men", "Men's clothing", fashion, "men");
                var women = Catalog.Domain.Catalogs.Catalog.Create("Women", "Women's clothing", fashion, "women");

                var menShirts = Catalog.Domain.Catalogs.Catalog.Create("Shirts", "Men's shirts", men, "shirts");
                var menPants = Catalog.Domain.Catalogs.Catalog.Create("Pants", "Men's pants", men, "pants");

                var womenDresses = Catalog.Domain.Catalogs.Catalog.Create("Dresses", "Women's dresses", women, "dresses");
                var womenShoes = Catalog.Domain.Catalogs.Catalog.Create("Shoes", "Women's shoes", women, "shoes");

                var catalogs = new List<Catalog.Domain.Catalogs.Catalog>
                {
                    electronics, phones, laptops,
                    fashion, men, women,
                    menShirts, menPants,
                    womenDresses, womenShoes
                };

                context.Catalogs.AddRange(catalogs);
                context.SaveChanges();
            }
        }

        public static void SeedProducts(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Products.Any() && context.Catalogs.Any())
            {
                var electronics = context.Catalogs.First(c => c.Name == "Electronics");
                var phones = context.Catalogs.First(c => c.Name == "Phones");
                var laptops = context.Catalogs.First(c => c.Name == "Laptops");

                var menShirts = context.Catalogs.First(c => c.Name == "Shirts" && c.ParentId.HasValue);
                var menPants = context.Catalogs.First(c => c.Name == "Pants" && c.ParentId.HasValue);

                var womenDresses = context.Catalogs.First(c => c.Name == "Dresses" && c.ParentId.HasValue);
                var womenShoes = context.Catalogs.First(c => c.Name == "Shoes" && c.ParentId.HasValue);

                var products = new List<Product>
                {
                    Product.Create("iPhone 15", "Latest Apple smartphone", Money.Create(999.99m, "USD"), phones.Id, 50, null),
                    Product.Create("MacBook Pro 16\"", "Apple laptop for professionals", Money.Create(2499.99m, "USD"), laptops.Id, 20, null),
                    Product.Create("Men's T-Shirt", "Cotton T-shirt for men", Money.Create(29.99m, "USD"), menShirts.Id, 100, null),
                    Product.Create("Men's Jeans", "Comfortable jeans", Money.Create(49.99m, "USD"), menPants.Id, 80, null),
                    Product.Create("Women's Evening Dress", "Elegant evening dress", Money.Create(79.99m, "USD"), womenDresses.Id, 60, null),
                    Product.Create("Women's Sneakers", "Casual sneakers", Money.Create(59.99m, "USD"), womenShoes.Id, 70, null)
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }}
}