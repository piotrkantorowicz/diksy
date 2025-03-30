using Diksy.Translation.OpenAI.Factories;
using Diksy.Translation.OpenAI.Schema;
using Diksy.Translation.OpenAI.Services;
using Diksy.Translation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Diksy.Translation.OpenAI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenAiTranslator(this IServiceCollection services, OpenAiSettings settings)
        {
            services.AddSingleton(settings);
            services.AddSingleton<IOpenAiFactory, OpenAiFactory>();
            services.AddSingleton<ISchemaGenerator, SchemaGenerator>();
            services.AddScoped<ITranslator, OpenAiTranslator>();
            services.AddScoped<IClientTranslationService, ChatTranslationService>();

            return services;
        }
    }
}