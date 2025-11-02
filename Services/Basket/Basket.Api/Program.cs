using Basket.Api.ExceptionHandlers;
using Basket.Api.Extensions;
using Basket.Database;
using MassTransit.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Observability;
using ProductGrpc;


AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(o => { o.ListenAnyIP(8081, l => l.Protocols = HttpProtocols.Http2); });

//for http
builder.WebHost.ConfigureKestrel(o => { o.ListenAnyIP(8080, l => l.Protocols = HttpProtocols.Http1); });

builder.Configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();
builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseNpgsql(Environment.GetEnvironmentVariable("NpgsqlConnection"));
});
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddObservability("basket-api",
    conf => { conf.AddSource(DiagnosticHeaders.DefaultListenerName); });
builder.Services.AddControllers();
builder.Services.ConfigureSwaggerWithJwt();
var app = builder.Build();
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();