using Diksy.WebApi.Services;

namespace Diksy.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiDependencies(this IServiceCollection services)
        {
            services.AddScoped<ITranslationService, TranslationService>();
            return services;
        }
    }
}