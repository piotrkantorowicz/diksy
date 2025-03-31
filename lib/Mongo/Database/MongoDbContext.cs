using Microsoft.Extensions.Options;
using Mongo.Settings;
using MongoDB.Driver;

namespace Mongo.Database
{
    /// <summary>
    /// Provides access to MongoDB database and collections.
    /// Similar to DbContext in Entity Framework.
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbContext"/> class.
        /// </summary>
        /// <param name="settings">The MongoDB connection settings.</param>
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var mongoSettings = settings.Value;
        
            _client = new MongoClient(mongoSettings.ConnectionString);
            _database = _client.GetDatabase(mongoSettings.DatabaseName);
        }

        /// <summary>
        /// Gets a MongoDB collection with the specified name and document type.
        /// </summary>
        /// <typeparam name="TDocument">The type of the documents stored in the collection.</typeparam>
        /// <param name="collectionName">The name of the collection.</param>
        /// <returns>An IMongoCollection instance representing the collection.</returns>
        public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return _database.GetCollection<TDocument>(collectionName);
        }
    
        /// <summary>
        /// Gets the MongoDB database.
        /// </summary>
        /// <returns>The MongoDB database instance.</returns>
        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    
        /// <summary>
        /// Gets the MongoDB client.
        /// </summary>
        /// <returns>The MongoDB client instance.</returns>
        public MongoClient GetClient()
        {
            return _client;
        }
    }
} 