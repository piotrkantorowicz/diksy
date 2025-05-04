using Diksy.Translation;
using Diksy.Translation.Exceptions;
using Diksy.Translation.OpenAI;
using Diksy.Translation.Services;
using Diksy.WebApi.Models.Translation;
using Diksy.WebApi.Models.Translation.Maps;
using Microsoft.Extensions.Options;
using TranslationInfoModel = Diksy.Translation.Models.TranslationInfo;
using TranslationInfoDto = Diksy.WebApi.Models.Translation.TranslationInfo;
using Diksy.Translation.History.Services;

namespace Diksy.WebApi.Services
{
    public class TranslationService(
        ITranslator translator,
        ILogger<TranslationService> logger,
        ITranslationHistoryService translationHistoryService,
        IOptions<OpenAiOptions> openAiOptions) : ITranslationService
    {
        private readonly ILogger<TranslationService>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        private readonly IOptions<OpenAiOptions> _openAiOptions =
            openAiOptions ?? throw new ArgumentNullException(nameof(openAiOptions));

        private readonly ITranslationHistoryService
            _translationHistoryService = translationHistoryService ??
                                         throw new ArgumentNullException(nameof(translationHistoryService));

        private readonly ITranslator _translator = translator ?? throw new ArgumentNullException(nameof(translator));

        public async Task<TranslationResponse> TranslateAsync(string phrase, string? model, string? sourceLanguage,
            string? targetLanguage, CancellationToken cancellationToken)
        {
            try
            {
                string defaultModel = model ?? _openAiOptions.Value.DefaultModel ?? AllowedModels.Gpt4O;
                string defaultLanguage = targetLanguage ?? AllowedLanguages.English;

                _logger.LogInformation(message: "Translating phrase: {Phrase} to {Language} using model {Model}",
                    phrase, defaultLanguage, defaultModel);

                TranslationInfoModel translationInfo =
                    await _translator.TranslateAsync(phrase: phrase, model: defaultModel,
                        sourceLanguage: sourceLanguage, targetLanguage: defaultLanguage,
                        cancellationToken: cancellationToken);

                TranslationInfoDto translationInfoDto = TranslationInfoMapper.MapFrom(translationInfo);

                _logger.LogInformation(message: "Successfully translated phrase: {Phrase} to {Translation}",
                    phrase, translationInfoDto.Translation);

                SanitizeTranslationResponse(phrase: phrase, translation: translationInfoDto);

                await _translationHistoryService.SaveTranslationAsync(translation: translationInfo,
                    userId: string.Empty, cancellationToken: cancellationToken);

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

            if (string.IsNullOrEmpty(translation.TranslationOfExample))
            {
                throw new TranslationException("Translation of example is null or empty");
            }

            if (string.IsNullOrEmpty(translation.TargetLanguage))
            {
                throw new TranslationException("Target language is null or empty");
            }

            if (string.IsNullOrEmpty(translation.SourceLanguage))
            {
                throw new TranslationException("Source language is null or empty");
            }
        }
    }
}