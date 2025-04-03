using Mongo.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Diksy.Translation.History.Models
{
    public class TranslationHistoryEntry : BaseDocument
    {
        [BsonElement("userId")] [BsonRequired] public required string UserId { get; init; }

        [BsonElement("phrase")] [BsonRequired] public required string Phrase { get; init; }

        [BsonElement("translation")]
        [BsonRequired]
        public required string Translation { get; init; }

        [BsonElement("transcription")]
        [BsonRequired]
        public required string Transcription { get; init; }

        [BsonElement("example")]
        [BsonRequired]
        public required string Example { get; init; }

        [BsonElement("translationOfExample")]
        [BsonRequired]
        public required string TranslationOfExample { get; init; }

        [BsonElement("sourceLanguage")]
        [BsonRequired]
        public required string SourceLanguage { get; init; }

        [BsonElement("targetLanguage")]
        [BsonRequired]
        public required string TargetLanguage { get; init; }
    }
}