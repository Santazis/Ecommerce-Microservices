using System.Runtime;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProfileGrpc;
using User.Api.Extensions;
using User.Database;
using User.Infrastructure.Grpc;
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(8081, l => l.Protocols = HttpProtocols.Http2);
});
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(8080, l => l.Protocols = HttpProtocols.Http1);
});
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(Environment.GetEnvironmentVariable("NpgsqlConnection"));
});
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(conf =>
{
    conf.TokenValidationParameters= new TokenValidationParameters
    {
        ValidateIssuer = false,         // Skip issuer validation (gateway handles it)
        ValidateAudience = false,       // Skip audience validation
        ValidateLifetime = false,       // Skip lifetime validation
        ValidateIssuerSigningKey = false, // Skip signature validation
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("verystrongprivatekey11000awdkbsfsekjbfkafhwefewkafjvw")),
    };
    conf.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].ToString();
            Console.WriteLine($"OnMessageReceived - Token: {token}");
            context.Token = token.StartsWith("Bearer ") ? token.Substring("Bearer ".Length).Trim() : null;
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("OnTokenValidated - Claims: " + 
                              string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}")));
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"OnAuthenticationFailed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"OnChallenge: {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerWithJwt();
builder.Services.AddGrpc();
var app = builder.Build();

app.MapGrpcService<UserGrpcService>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();