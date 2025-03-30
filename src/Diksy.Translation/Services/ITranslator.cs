using Diksy.Translation.Models;

namespace Diksy.Translation.Services
{
    public interface ITranslator
    {
        Task<TranslationInfo> TranslateAsync(string word, string model, string language,
            CancellationToken cancellationToken);
    }
}