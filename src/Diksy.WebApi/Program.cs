using Diksy.Translation.OpenAI.Extensions;
using Diksy.WebApi.Extensions;
using Diksy.Translation.History.Extensions;
using Diksy.WebApi.Settings;
using Mongo.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using NSwag;
using System.Threading.RateLimiting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenAiTranslator(builder.Configuration, ConfigurationSections.OpenAi);
builder.Services.AddTranslationHistory(builder.Configuration, ConfigurationSections.Mongo);
builder.Services.AddApiDependencies(builder.Configuration);

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

IOptions<RateLimitingOptions> rateLimiterOptions = app.Services.GetRequiredService<IOptions<RateLimitingOptions>>();

if (rateLimiterOptions.Value.Enabled)
{
    app.UseRateLimiter();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();