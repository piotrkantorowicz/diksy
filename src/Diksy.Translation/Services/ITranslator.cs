using Diksy.Translation.Models;

namespace Diksy.Translation.Services
{
    public interface ITranslator
    {
        Task<TranslationInfo> TranslateAsync(string phrase, string model, string language,
            CancellationToken cancellationToken);
    }
}