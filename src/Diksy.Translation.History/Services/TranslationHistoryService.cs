using Diksy.Translation.History.Filters;
using Diksy.Translation.History.Models;
using Diksy.Translation.Models;
using MongoDB.Driver;

namespace Diksy.Translation.History.Services
{
    public class TranslationHistoryService(IMongoCollection<TranslationHistoryEntry> collection)
        : ITranslationHistoryService
    {
        private readonly IMongoCollection<TranslationHistoryEntry> _collection =
            collection ?? throw new ArgumentNullException(nameof(collection));

        public async Task<TranslationHistoryEntry?> GetTranslationByIdAsync(string id,
            CancellationToken cancellationToken)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TranslationHistoryEntry>> GetTranslationHistoryAsync(
            TranslationHistoryFilter filter, CancellationToken cancellationToken)
        {
            FilterDefinition<TranslationHistoryEntry> filterDefinition = BuildFilter(filter);

            return await _collection.Find(filterDefinition)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveTranslationAsync(TranslationInfo translation, string userId, string sourceLanguage,
            string targetLanguage, CancellationToken cancellationToken)
        {
            TranslationHistoryEntry entry = new()
            {
                UserId = userId,
                Phrase = translation.Phrase,
                Translation = translation.Translation,
                Transcription = translation.Transcription,
                Example = translation.Example,
                TranslationOfExample = translation.TranslationOfExample,
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage
            };

            await _collection.InsertOneAsync(entry, new InsertOneOptions(), cancellationToken);
        }

        public async Task DeleteTranslationAsync(string id, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
        }

        private static FilterDefinition<TranslationHistoryEntry> BuildFilter(TranslationHistoryFilter filter)
        {
            FilterDefinitionBuilder<TranslationHistoryEntry>? builder = Builders<TranslationHistoryEntry>.Filter;
            List<FilterDefinition<TranslationHistoryEntry>> filters = [];

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                filters.Add(builder.Eq(x => x.UserId, filter.UserId));
            }

            if (!string.IsNullOrEmpty(filter.SourceLanguage))
            {
                filters.Add(builder.Eq(x => x.SourceLanguage, filter.SourceLanguage));
            }

            if (!string.IsNullOrEmpty(filter.TargetLanguage))
            {
                filters.Add(builder.Eq(x => x.TargetLanguage, filter.TargetLanguage));
            }

            if (filter.FromDate.HasValue)
            {
                filters.Add(builder.Gte(x => x.CreatedAt, filter.FromDate.Value));
            }

            if (filter.ToDate.HasValue)
            {
                filters.Add(builder.Lte(x => x.CreatedAt, filter.ToDate.Value));
            }

            return filters.Count > 0
                ? builder.And(filters)
                : builder.Empty;
        }
    }
}