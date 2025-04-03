using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Database;
using Mongo.Repositories;
using Mongo.Settings;

namespace Mongo.Extensions
{
    /// <summary>
    ///     Extension methods for setting up MongoDB services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds MongoDB services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="sectionName">The configuration section name for MongoDB settings. Default is "MongoDb".</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = "MongoDb")
        {
            services.Configure<MongoDbSettings>(configuration.GetSection(sectionName));
            services.AddSingleton<MongoDbContext>();

            return services;
        }

        /// <summary>
        ///     Adds a MongoDB repository for the specified document type to the service collection.
        /// </summary>
        /// <typeparam name="TDocument">The document type stored in the collection.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the repository to.</param>
        /// <param name="collectionName">The name of the MongoDB collection.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddMongoRepository<TDocument>(
            this IServiceCollection services,
            string collectionName)
            where TDocument : class
        {
            services.AddScoped<IMongoRepository<TDocument>>(provider =>
            {
                MongoDbContext context = provider.GetRequiredService<MongoDbContext>();
                return new MongoRepository<TDocument>(context, collectionName);
            });

            return services;
        }
    }
}