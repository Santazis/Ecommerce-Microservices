using Catalog.Api.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
    ;
});

builder.Services.AddControllers();
builder.Services.AddObservability("catalog-api");
builder.Services.AddSwaggerGen();
builder.Services.ConfigureSwaggerWithJwt();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();