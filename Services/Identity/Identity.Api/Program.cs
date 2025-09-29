using Database;
using FluentValidation;
using Identity.Api.ExceptionHandlers;
using Identity.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Observability;
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

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly,includeInternalTypes:true);
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(Environment.GetEnvironmentVariable("NpgsqlConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddObservability("identity-api",null);
builder.Services.ConfigureAuthJwt(builder.Configuration);
builder.Services.AddAuthServices();
builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.ConfigureSwaggerWithJwt();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
var app = builder.Build();
app.UseExceptionHandler();
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