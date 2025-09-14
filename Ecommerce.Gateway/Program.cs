using Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddObservability("ecommerce-gateway");
builder.Services.AddControllers();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapReverseProxy();


app.Run();