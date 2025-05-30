using Diksy.Translation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diksy.WebApi.Models.Translation
{
    /// <summary>
    ///     Request object for translation requests
    /// </summary>
    public sealed class TranslationRequest
    {
        /// <summary>
        ///     Default constructor for TranslationRequest
        /// </summary>
        public TranslationRequest()
        {
        }

        /// <summary>
        ///     Constructor for TranslationRequest with parameters
        /// </summary>
        /// <param name="phrase">The phrase to translate (3-30 characters)</param>
        /// <param name="model">The AI model to use for translation. Defaults to GPT-4</param>
        /// <param name="targetLanguage">The target language for translation. Must be one of the supported languages</param>
        /// <param name="sourceLanguage">The source language for translation. Must be one of the supported languages</param>
        public TranslationRequest(string phrase, string? model = AllowedModels.Gpt4O,
            string? targetLanguage = AllowedLanguages.English, string? sourceLanguage = AllowedLanguages.Polish)
        {
            Phrase = phrase;
            Model = model;
            TargetLanguage = targetLanguage;
            SourceLanguage = sourceLanguage;
        }

        /// <summary>The phrase to translate (3-30 characters)</summary>
        [StringLength(30, MinimumLength = 3)]
        public required string Phrase { get; init; }

        /// <summary>The AI model to use for translation. Defaults to GPT-4</summary>
        [RegularExpression(AllowedModels.ModelRegex, ErrorMessage = "Invalid model")]
        [DefaultValue(AllowedModels.Gpt4O)]
        public string? Model { get; init; } = AllowedModels.Gpt4O;

        /// <summary>The target language for translation. Must be one of the supported languages</summary>
        [RegularExpression(AllowedLanguages.LanguageRegex, ErrorMessage = "Invalid language")]
        [DefaultValue(AllowedLanguages.English)]
        public string? TargetLanguage { get; init; } = AllowedLanguages.English;

        /// <summary>The source language for translation. Must be one of the supported languages</summary>
        [RegularExpression(AllowedLanguages.LanguageRegex, ErrorMessage = "Invalid language")]
        [DefaultValue(AllowedLanguages.Polish)]
        public string? SourceLanguage { get; init; } = AllowedLanguages.Polish;
    }
}