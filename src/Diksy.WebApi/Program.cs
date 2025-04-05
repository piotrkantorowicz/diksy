using Diksy.Translation.OpenAI.Extensions;
using Diksy.WebApi.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using NSwag;
using System.Threading.RateLimiting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Version = "v1";
        document.Info.Title = "Diksy Translation API";
        document.Info.Description = "API for translating phrases using AI";
        document.Info.Contact = new OpenApiContact { Name = "Support", Email = "support@diksy.com" };
    };
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "translation", configureOptions: config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.PermitLimit = 20;
        config.QueueLimit = 10;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddOpenAiTranslator(builder.Configuration);
builder.Services.AddApiDependencies();

IConfigurationSection section = builder.Configuration.GetSection("OpenAI");

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.UseReDoc(config =>
    {
        config.Path = "/redoc";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();