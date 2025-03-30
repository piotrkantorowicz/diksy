using System.Text.Json.Serialization;

namespace Diksy.Translation.Models
{
    public class TranslationInfo
    {
        [JsonPropertyName("phrase")] public required string Phrase { get; init; }

        [JsonPropertyName("translation")] public required string Translation { get; init; }

        [JsonPropertyName("transcription")] public required string Transcription { get; init; }

        [JsonPropertyName("example")] public required string Example { get; init; }

        [JsonPropertyName("translationOfExample")]
        public required string TranslationOfExample { get; init; }
    }
}