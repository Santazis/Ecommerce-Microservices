using System.Text;
using Identity.Api.Interfaces.Jwt;
using Identity.Api.Options;
using Identity.Api.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
}