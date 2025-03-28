using System.Text.Json.Serialization;

namespace Diksy.Translation.Models
{
    public sealed record TranslationInfo(
        [property: JsonPropertyName("phrase")] string Phrase,
        [property: JsonPropertyName("translation")]
        string Translation,
        [property: JsonPropertyName("transcription")]
        string Transcription,
        [property: JsonPropertyName("example")]
        string Example);
}