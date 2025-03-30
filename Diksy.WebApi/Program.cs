using Diksy.Translation.OpenAI;
using Diksy.Translation.OpenAI.Extensions;
using Diksy.WebApi.Services;
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

OpenAiSettings openAiSettings = builder.Configuration.GetSection("OpenAI").Get<OpenAiSettings>()
                                ?? new OpenAiSettings(
                                    ApiKey: builder.Configuration["OpenAI:ApiKey"] ??
                                            throw new InvalidOperationException("OpenAI API key is not configured."),
                                    DefaultModel: builder.Configuration["OpenAI:DefaultModel"] ?? "gpt-4o");

builder.Services.AddOpenAiTranslator(openAiSettings);
builder.Services.AddScoped<ITranslationService, TranslationService>();

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