using Diksy.Translation.History.Models;
using Diksy.Translation.History.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Diksy.Translation.History
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTranslationHistory(this IServiceCollection services)
        {
            services.AddScoped<ITranslationHistoryService, TranslationHistoryService>();
            services.AddScoped<IMongoCollection<TranslationHistoryEntry>>(sp =>
            {
                IMongoDatabase database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<TranslationHistoryEntry>(CollectionNames.TranslationHistory);
            });
            return services;
        }
    }
}