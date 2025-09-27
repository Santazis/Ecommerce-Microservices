using ImageProcessing.Interfaces;
using ImageProcessing.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(conf =>
{
    conf.SetKebabCaseEndpointNameFormatter();
    conf.UsingRabbitMq((context, opt) =>
    {
        opt.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), cred =>
        {
            cred.Username(builder.Configuration["MessageBroker:Username"]!);
            cred.Password(builder.Configuration["MessageBroker:Password"]!);
        });
        opt.ConfigureEndpoints(context);
    });
});
builder.Services.AddSingleton<IImageProcessingService, ImageProcessingService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();