using Catalog.Api.ExceptionHandlers;
using Catalog.Api.Extensions;
using Catalog.Application;
using Catalog.Database;
using FluentValidation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Observability;
//disabling tls for grpc to work without https
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);
//for grpc
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(8081, l => l.Protocols = HttpProtocols.Http2);
});

//for http
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(8080, l => l.Protocols = HttpProtocols.Http1);
});

builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseNpgsql(Environment.GetEnvironmentVariable("NpgsqlConnection"));
});
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly, includeInternalTypes: true);
builder.Services.AddControllers();
builder.Services.AddObservability("catalog-api");
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.ConfigureSwaggerWithJwt();
var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.SeedCatalogs();
    app.SeedProducts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();