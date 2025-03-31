using System.Linq.Expressions;
using Mongo.Database;
using MongoDB.Driver;

namespace Mongo.Repositories
{
    /// <summary>
    /// Generic repository implementation for MongoDB operations.
    /// </summary>
    /// <typeparam name="TDocument">The document type stored in the collection.</typeparam>
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : class
    {
        private readonly IMongoCollection<TDocument> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRepository{TDocument}"/> class.
        /// </summary>
        /// <param name="context">The MongoDB context.</param>
        /// <param name="collectionName">The name of the collection.</param>
        public MongoRepository(MongoDbContext context, string collectionName)
        {
            _collection = context.GetCollection<TDocument>(collectionName);
        }

        /// <summary>
        /// Gets the MongoDB collection for this repository.
        /// </summary>
        public IMongoCollection<TDocument> Collection => _collection;

        /// <inheritdoc />
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task InsertOneAsync(TDocument document, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task InsertManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
        {
            await _collection.InsertManyAsync(documents, new InsertManyOptions(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument document, ReplaceOptions? options = null, CancellationToken cancellationToken = default)
        {
            return await _collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null, CancellationToken cancellationToken = default)
        {
            return await _collection.UpdateOneAsync(filter, update, options, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.DeleteOneAsync(filter, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _collection.DeleteManyAsync(filter, cancellationToken);
        }
    }
} 