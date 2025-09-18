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
}