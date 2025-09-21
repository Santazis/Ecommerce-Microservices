using Catalog.Application.Interfaces;
using Catalog.Application.Services.Catalog;

namespace Catalog.Api.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogService, CatalogService>();
        return services;
    }
}