using Diksy.Translation;
using Diksy.Translation.Exceptions;
using Diksy.Translation.OpenAI;
using Diksy.Translation.Services;
using Diksy.WebApi.Models.Translation;
using Diksy.WebApi.Models.Translation.Maps;
using TranslationInfoModel = Diksy.Translation.Models.TranslationInfo;
using TranslationInfoDto = Diksy.WebApi.Models.Translation.TranslationInfo;

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

                TranslationInfoModel translationInfo =
                    await _translator.TranslateAsync(word: phrase, model: defaultModel, language: defaultLanguage);

                TranslationInfoDto translationInfoDto = TranslationInfoMapper.MapFrom(translationInfo: translationInfo);

                _logger.LogInformation(message: "Successfully translated phrase: {Phrase} to {Translation}",
                    phrase, translationInfoDto.Translation);

                SanitizeTranslationResponse(phrase: phrase, translation: translationInfoDto);

                return TranslationResponse.SuccessResponse(translationInfoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "Error translating phrase: {Phrase}", phrase);
                return TranslationResponse.ErrorResponse($"Translation error: {ex.Message}");
            }
        }

        private static void SanitizeTranslationResponse(string phrase, TranslationInfoDto translation)
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