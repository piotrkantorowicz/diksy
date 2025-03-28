using Diksy.Translation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Diksy.WebApi.Models.Translation
{
    /// <summary>
    ///     Request object for translation requests
    /// </summary>
    public sealed record TranslationRequest
    {
        /// <summary>The phrase to translate (3-30 characters)</summary>
        [StringLength(30, MinimumLength = 3)]
        public required string Phrase { get; set; }

        /// <summary>The AI model to use for translation. Defaults to GPT-4</summary>
        [RegularExpression(AllowedModels.ModelRegex, ErrorMessage = "Invalid model")]
        [DefaultValue(AllowedModels.Gpt4O)]
        public string? Model { get; set; }

        /// <summary>The target language for translation. Must be one of the supported languages</summary>
        [RegularExpression(AllowedLanguages.LanguageRegex, ErrorMessage = "Invalid language")]
        [DefaultValue(AllowedLanguages.English)]
        public string? Language { get; set; }
    }
}