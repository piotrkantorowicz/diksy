using Diksy.Translation.History.Filters;
using Diksy.Translation.History.Models;
using Diksy.Translation.Models;

namespace Diksy.Translation.History.Services
{
    public interface ITranslationHistoryService
    {
        Task<TranslationHistoryEntry?> GetTranslationByIdAsync(string id, CancellationToken cancellationToken);

        Task<IEnumerable<TranslationHistoryEntry>> GetTranslationHistoryAsync(TranslationHistoryFilter filter,
            CancellationToken cancellationToken);

        Task SaveTranslationAsync(TranslationInfo translation, string userId, string sourceLanguage,
            string targetLanguage, CancellationToken cancellationToken);

        Task DeleteTranslationAsync(string id, CancellationToken cancellationToken);
    }
}