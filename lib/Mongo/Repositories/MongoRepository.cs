using Mongo.Database;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Mongo.Repositories
{
    /// <summary>
    ///     Generic repository implementation for MongoDB operations.
    /// </summary>
    /// <typeparam name="TDocument">The document type stored in the collection.</typeparam>
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : class
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MongoRepository{TDocument}" /> class.
        /// </summary>
        /// <param name="context">The MongoDB context.</param>
        /// <param name="collectionName">The name of the collection.</param>
        public MongoRepository(MongoDbContext context, string collectionName)
        {
            Collection = context.GetCollection<TDocument>(collectionName);
        }

        /// <summary>
        ///     Gets the MongoDB collection for this repository.
        /// </summary>
        public IMongoCollection<TDocument> Collection { get; }

        /// <inheritdoc />
        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            return await Collection.Find(filter).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task InsertOneAsync(TDocument document, CancellationToken cancellationToken = default)
        {
            await Collection.InsertOneAsync(document, new InsertOneOptions(),
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task InsertManyAsync(IEnumerable<TDocument> documents,
            CancellationToken cancellationToken = default)
        {
            await Collection.InsertManyAsync(documents, new InsertManyOptions(),
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter,
            TDocument document, ReplaceOptions? options = null, CancellationToken cancellationToken = default)
        {
            return await Collection.ReplaceOneAsync(filter, document, options,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter,
            UpdateDefinition<TDocument> update, UpdateOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return await Collection.UpdateOneAsync(filter, update, options,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            return await Collection.DeleteOneAsync(filter, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            return await Collection.DeleteManyAsync(filter, cancellationToken);
        }
    }
}