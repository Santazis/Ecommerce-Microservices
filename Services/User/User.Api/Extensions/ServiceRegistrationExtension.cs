using MassTransit;
using User.Application.Interfaces;
using User.Application.Services;

namespace User.Api.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
    public static IServiceCollection AddMessageBroker(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddMassTransit(conf =>
        {
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