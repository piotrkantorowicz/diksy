using Diksy.Translation;
using Diksy.Translation.Exceptions;
using Diksy.Translation.OpenAI;
using Diksy.Translation.Services;
using Diksy.WebApi.Models.Translation;

namespace Diksy.WebApi.Services
{
    public class TranslationService(
        ITranslator translator,
        ILogger<TranslationService> logger,
        OpenAiSettings openAiSettings) : ITranslationService
    {
        private readonly ILogger<TranslationService>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private readonly OpenAiSettings _openAiSettings =
            openAiSettings ?? throw new ArgumentNullException(nameof(openAiSettings));

        private readonly ITranslator _translator = translator ?? throw new ArgumentNullException(nameof(translator));

        public async Task<TranslationResponse> TranslateAsync(string phrase, string? model, string? language)
        {
            try
            {
                string defaultModel = model ?? _openAiSettings.DefaultModel ?? AllowedModels.Gpt4O;
                string defaultLanguage = language ?? AllowedLanguages.English;

                _logger.LogInformation(message: "Translating phrase: {Phrase} to {Language} using model {Model}",
                    phrase, defaultLanguage, defaultModel);

                Translation.Models.TranslationInfo result =
                    await _translator.TranslateAsync(word: phrase, model: defaultModel, language: defaultLanguage);

                TranslationInfo resultModel = new()
                {
                    Phrase = result.Phrase,
                    Translation = result.Translation,
                    Transcription = result.Transcription,
                    Example = result.Example
                };

                _logger.LogInformation(message: "Successfully translated phrase: {Phrase} to {Translation}",
                    phrase, result.Translation);

                SanitizeTranslationResponse(phrase: phrase, translation: resultModel);

                return new TranslationResponse { Success = true, Response = resultModel };
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "Error translating phrase: {Phrase}", phrase);
                return new TranslationResponse
                {
                    Success = false, Response = null!, Errors = [$"Translation error: {ex.Message}"], Exception = ex
                };
            }
        }

        private static void SanitizeTranslationResponse(string phrase, TranslationInfo translation)
        {
            if (string.IsNullOrEmpty(translation.Phrase) ||
                !translation.Phrase.Equals(value: phrase, comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                throw new TranslationException("Phrase is null or empty or is different from the original phrase");
            }

            if (string.IsNullOrEmpty(translation.Translation))
            {
                throw new TranslationException("Translation is null or empty");
            }

            if (string.IsNullOrEmpty(translation.Transcription))
            {
                throw new TranslationException("Transcription is null or empty");
            }

            if (string.IsNullOrEmpty(translation.Example))
            {
                throw new TranslationException("Example is null or empty");
            }
        }
    }
}