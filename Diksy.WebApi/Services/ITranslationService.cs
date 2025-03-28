using Diksy.WebApi.Models.Translation;

namespace Diksy.WebApi.Services
{
    public interface ITranslationService
    {
        Task<TranslationResponse> TranslateAsync(string phrase, string? model, string? language);
    }
}