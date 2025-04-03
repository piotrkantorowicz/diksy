using System.ComponentModel.DataAnnotations;

namespace Diksy.Translation.OpenAI
{
    public sealed record OpenAiOptions
    {
        [Required]
        [Length(12, 1000, ErrorMessage = "API key must be between 12 and 1000 characters.")]
        public required string? ApiKey { get; init; }
        
        [Length(3, 100, ErrorMessage = "Default model must be between 3 and 100 characters.")]
        public string? DefaultModel { get; init; }
    }
}