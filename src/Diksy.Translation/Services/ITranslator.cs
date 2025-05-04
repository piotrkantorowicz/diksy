using Diksy.Translation.Models;

namespace Diksy.Translation.Services
{
    public interface ITranslator
    {
        Task<TranslationInfo> TranslateAsync(string phrase, string model, string? sourceLanguage,
            string targetLanguage, CancellationToken cancellationToken);
    }
}