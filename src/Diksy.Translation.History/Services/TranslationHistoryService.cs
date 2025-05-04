using Diksy.Translation.History.Filters;
using Diksy.Translation.History.Models;
using Diksy.Translation.Models;
using Mongo.Repositories;

namespace Diksy.Translation.History.Services
{
    public class TranslationHistoryService(IMongoRepository<TranslationHistoryEntry> translationHistoryRepository)
        : ITranslationHistoryService
    {
        private readonly IMongoRepository<TranslationHistoryEntry> _translationHistoryRepository =
            translationHistoryRepository ?? throw new ArgumentNullException(nameof(translationHistoryRepository));

        public async Task<TranslationHistoryEntry?> GetTranslationByIdAsync(string id,
            CancellationToken cancellationToken)
        {
            return await _translationHistoryRepository.FindOneAsync(x => x.Id == id,
                cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<TranslationHistoryEntry>> GetTranslationHistoryAsync(
            TranslationHistoryFilter filter, CancellationToken cancellationToken)
        {
            return await _translationHistoryRepository.FindAsync(filter: filter.BuildPredicate(),
                sortFields: filter.SortFields,
                cancellationToken: cancellationToken);
        }

        public async Task SaveTranslationAsync(TranslationInfo translation, string userId,
            CancellationToken cancellationToken)
        {
            TranslationHistoryEntry entry = new()
            {
                UserId = userId,
                Phrase = translation.Phrase,
                Translation = translation.Translation,
                Transcription = translation.Transcription,
                Example = translation.Example,
                TranslationOfExample = translation.TranslationOfExample,
                SourceLanguage = translation.SourceLanguage,
                TargetLanguage = translation.TargetLanguage
            };

            await _translationHistoryRepository.InsertOneAsync(document: entry, cancellationToken: cancellationToken);
        }

        public async Task DeleteTranslationAsync(string id, CancellationToken cancellationToken)
        {
            await _translationHistoryRepository.DeleteOneAsync(filter: x => x.Id == id,
                cancellationToken: cancellationToken);
        }
    }
}