using Amazon.Runtime;
using Amazon.S3;
using ImageProcessing.Consumers;
using ImageProcessing.Interfaces;
using ImageProcessing.Options;
using ImageProcessing.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using Observability;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();
builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();
// Add services to the container.
builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<ProcessImagesConsumer>();
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
builder.Services.AddObservability("image-processor-api", conf =>
{
    conf.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
});
builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3Settings"));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
    var cred = new BasicAWSCredentials(settings.AccessKey, settings.SecretKey);
    var config = new AmazonS3Config()
    {
        ServiceURL = settings.ServiceUrl,
        ForcePathStyle = true
    };
    return new AmazonS3Client(cred, config);
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