using System.Linq.Expressions;
using MongoDB.Driver;

namespace Mongo.Repositories
{
    /// <summary>
    /// Defines a generic repository interface for MongoDB operations.
    /// </summary>
    /// <typeparam name="TDocument">The document type stored in the collection.</typeparam>
    public interface IMongoRepository<TDocument> where TDocument : class
    {
        /// <summary>
        /// Gets a queryable collection for LINQ operations.
        /// </summary>
        /// <returns>A queryable collection of documents.</returns>
        IMongoCollection<TDocument> Collection { get; }
    
        /// <summary>
        /// Finds documents that match the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of documents that match the filter criteria.</returns>
        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Finds a single document that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A document that matches the filter criteria, or null if no document is found.</returns>
        Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Inserts a document into the collection.
        /// </summary>
        /// <param name="document">The document to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous insert operation.</returns>
        Task InsertOneAsync(TDocument document, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Inserts multiple documents into the collection.
        /// </summary>
        /// <param name="documents">The documents to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous insert operation.</returns>
        Task InsertManyAsync(IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Replaces a document that matches the specified filter with a new document.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="document">The replacement document.</param>
        /// <param name="options">The update options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous replace operation.</returns>
        Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TDocument, bool>> filter, TDocument document, ReplaceOptions? options = null, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Updates a document that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="update">The update definition.</param>
        /// <param name="options">The update options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        Task<UpdateResult> UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, UpdateOptions? options = null, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Deletes a document that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task<DeleteResult> DeleteOneAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default);
    
        /// <summary>
        /// Deletes multiple documents that match the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> filter, CancellationToken cancellationToken = default);
    }
} 