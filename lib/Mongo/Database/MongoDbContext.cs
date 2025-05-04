using Microsoft.Extensions.Options;
using Mongo.Options;
using MongoDB.Driver;

namespace Mongo.Database
{
    /// <summary>
    ///     Provides access to MongoDB database and collections.
    ///     Similar to DbContext in Entity Framework.
    /// </summary>
    public class MongoDbContext
    {
        private readonly MongoClient _client;
        private readonly IDictionary<string, IMongoDatabase> _databases;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MongoDbContext" /> class.
        /// </summary>
        /// <param name="options">The MongoDB connection settings.</param>
        public MongoDbContext(IOptions<MongoDbOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            _client = new MongoClient(options.Value.ConnectionString);

            _databases = options.Value.Databases.ToDictionary(
                dbName => dbName,
                dbName => _client.GetDatabase(dbName));
        }

        /// <summary>
        ///     Gets a MongoDB collection with the specified name and document type.
        /// </summary>
        /// <typeparam name="TDocument">The type of the documents stored in the collection.</typeparam>
        /// <param name="database">The name of the database.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An IMongoCollection instance representing the collection.</returns>
        public IMongoCollection<TDocument> GetCollection<TDocument>(string database, string collectionName)
        {
            ArgumentNullException.ThrowIfNull(collectionName);

            return _databases[database].GetCollection<TDocument>(collectionName);
        }

        /// <summary>
        ///     Gets the MongoDB database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <returns>The MongoDB database instance.</returns>
        public IMongoDatabase GetDatabase(string database)
        {
            return _databases[database];
        }

        /// <summary>
        ///     Gets the MongoDB client.
        /// </summary>
        /// <returns>The MongoDB client instance.</returns>
        public MongoClient GetClient()
        {
            return _client;
        }
    }
}