using Diksy.WebApi.Services;
using Diksy.WebApi.Settings;
using Microsoft.AspNetCore.RateLimiting;
using NSwag;

namespace Diksy.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<ITranslationService, TranslationService>();

            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Diksy Translation API";
                    document.Info.Description = "API for translating phrases using AI";
                    document.Info.Contact = new OpenApiContact { Name = "Support", Email = "support@diksy.com" };
                };
            });

            services.AddOptions<RateLimitingOptions>()
                .Bind(configuration.GetSection(ConfigurationSections.RateLimiting))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            RateLimitingOptions rateLimitingSettings =
                configuration.GetSection(ConfigurationSections.RateLimiting).Get<RateLimitingOptions>()
                ?? new RateLimitingOptions();

            services.AddRateLimiter(options =>
            {
                if (rateLimitingSettings.Enabled)
                {
                    options.AddFixedWindowLimiter(policyName: "default", configureOptions: config =>
                    {
                        config.Window = TimeSpan.FromMinutes(rateLimitingSettings.WindowInMinutes);
                        config.PermitLimit = rateLimitingSettings.PermitLimit;
                        config.QueueLimit = rateLimitingSettings.QueueLimit;
                        config.QueueProcessingOrder = rateLimitingSettings.QueueProcessingOrder;
                    });
                }

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
            
            return services;
        }
    }
}