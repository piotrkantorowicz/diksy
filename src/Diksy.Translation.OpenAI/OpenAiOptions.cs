using Diksy.Translation.Validation;
using System.ComponentModel.DataAnnotations;

namespace Diksy.Translation.OpenAI
{
    public sealed record OpenAiOptions
    {
        [Required]
        [Length(minimumLength: 12, maximumLength: 1000,
            ErrorMessage = "API key must be between 12 and 1000 characters.")]
        public required string ApiKey { get; init; }

        [MinLengthIfNotNull(minLength: 3, maxLength: 100,
            ErrorMessage = "When provided, default model must be between 3 and 100 characters.")]
        public string? DefaultModel { get; init; }
    }
}