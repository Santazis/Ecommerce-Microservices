using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Application.Services.Catalog;
using Catalog.Application.Services.Product;
using Catalog.Infrastructure.Consumers;
using MassTransit;

namespace Catalog.Api.Extensions
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICatalogService, CatalogService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITempStorageService, TempStorageService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            return services;
        }

        public static IServiceCollection AddMessageBroker(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddMassTransit(conf =>
            {
                conf.AddConsumer<ImageProcessedConsumer>();
                conf.SetKebabCaseEndpointNameFormatter();
                conf.UsingRabbitMq((context, opt) =>
                {
                    opt.Host(new Uri(configuration["MessageBroker:Host"]!), cred =>
                    {
                        cred.Username(configuration["MessageBroker:Username"]!);
                        cred.Password(configuration["MessageBroker:Password"]!);
                    });
                    opt.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}