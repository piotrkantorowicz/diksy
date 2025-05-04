using Diksy.Translation.History.Models;
using Diksy.Translation.History.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Extensions;
using Mongo.Options;

namespace Diksy.Translation.History.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string DefaultDatabaseName = "diksy";

        public static IServiceCollection AddTranslationHistory(this IServiceCollection services,
            IConfiguration configuration, string mongoDbSectionName = "MongoDb")
        {
            services.AddScoped<ITranslationHistoryService, TranslationHistoryService>();

            services.AddMongo(configuration, mongoDbSectionName);

            IConfigurationSection mongoDbOptionsSection = configuration.GetSection(mongoDbSectionName);
            MongoDbOptions mongoDbOptions = mongoDbOptionsSection.Get<MongoDbOptions>() ?? new MongoDbOptions();

            services.AddMongoRepository<TranslationHistoryEntry>(
                database: mongoDbOptions.Databases.FirstOrDefault() ?? DefaultDatabaseName,
                collectionName: CollectionNames.TranslationHistoryCollection);

            return services;
        }
    }
}