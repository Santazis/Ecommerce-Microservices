using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Application.Services.Catalog;
using Catalog.Application.Services.Product;
using Catalog.Infrastructure.Services;

namespace Catalog.Api.Extensions
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageProcessingService, ImageProcessingService>();
            services.AddScoped<ITempStorageService, TempStorageService>();
            return services;
        }
    }
}