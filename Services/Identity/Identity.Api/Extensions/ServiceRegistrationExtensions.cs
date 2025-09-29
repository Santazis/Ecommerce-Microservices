using System.Text;
using Identity.Api.Grpc.Clients;
using Identity.Api.Interfaces;
using Identity.Api.Interfaces.Jwt;
using Identity.Api.Options;
using Identity.Api.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProfileGrpc;

namespace Identity.Api.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection ConfigureAuthJwt(this IServiceCollection services,IConfiguration config)
    {
        var jwtSettings = new JwtSettings();
        config.GetSection("JwtSettings").Bind(jwtSettings);
        services.AddSingleton(jwtSettings);
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(conf =>
        {
            conf.SaveToken = true;
            conf.RequireHttpsMetadata = true;
            conf.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGeneratorService, JwtTokenGeneratorService>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserServiceClient, GrpcUserServiceClient>();
        return services;
    }

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddGrpcClient<ProfileGrpc.UserService.UserServiceClient>(opt =>
        {
            var address = config["GrpcServices:UserServiceClient"];
            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("GrpcServices:UserServiceClient is not set");
            }
            opt.Address = new Uri(address);
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return handler;
        });
        return services;
    }
}