using Diksy.Translation.OpenAI.Factories;
using Diksy.Translation.OpenAI.Schema;
using Diksy.Translation.OpenAI.Services;
using Diksy.Translation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diksy.Translation.OpenAI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenAiTranslator(this IServiceCollection services,
            IConfiguration configuration, string sectionName)
        {
            services.AddOptions<OpenAiOptions>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IOpenAiFactory, OpenAiFactory>();
            services.AddSingleton<ISchemaGenerator, SchemaGenerator>();
            services.AddScoped<ITranslator, OpenAiTranslator>();
            services.AddScoped<IChatTranslationService, ChatTranslationService>();

            return services;
        }
    }
}