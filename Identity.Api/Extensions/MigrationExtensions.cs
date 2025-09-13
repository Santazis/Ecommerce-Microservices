using Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    public static async Task AddRoles(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
        }
        if (!await roleManager.RoleExistsAsync(Roles.Customer))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Customer));
        }
        if (!await roleManager.RoleExistsAsync(Roles.Merchant))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Merchant));
        }
    }
}