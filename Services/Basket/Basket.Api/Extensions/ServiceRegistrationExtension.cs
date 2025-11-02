using System.Reflection.Metadata;
using Basket.Application;
using Basket.Application.Basket;
using Basket.Application.Interfaces;
using Basket.Infrastructure;
using Basket.Infrastructure.Grpc;
using MassTransit;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;

namespace Basket.Api.Extensions
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IProductServiceClient, ProductServiceClient>();
            return services;
        }

        public static IServiceCollection AddMessageBroker(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddMassTransit(conf =>
            {
                conf.AddConsumers(typeof(InfrastructureReference).Assembly);
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
        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddGrpcClient<ProductGrpc.ProductService.ProductServiceClient>(opt =>
            {
                var address = config["GrpcServices:ProductServiceClient"];
                if (string.IsNullOrEmpty(address))
                {
                    throw new Exception("GrpcServices:ProductServiceClient is not set");
                }
                opt.Address = new Uri(address);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = 
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                return handler;
            })
            .AddResilienceHandler("custom", pipeline =>
            {
                pipeline.AddTimeout(new HttpTimeoutStrategyOptions()
                {
                    Timeout = TimeSpan.FromSeconds(10),
                });
                pipeline.AddRetry(new HttpRetryStrategyOptions()
                {
                    MaxRetryAttempts = 3,
                    UseJitter = true,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(500),
                });
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions()
                {
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.9,
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(5),
                    
                });
            })
                ;
            return services;
        }

    }
}