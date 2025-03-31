using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.Models
{
    /// <summary>
    /// Base class for MongoDB documents, providing common properties.
    /// </summary>
    public abstract class BaseDocument
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
        /// <summary>
        /// Gets or sets the creation date and time of the document.
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
        /// <summary>
        /// Gets or sets the last update date and time of the document.
        /// </summary>
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 