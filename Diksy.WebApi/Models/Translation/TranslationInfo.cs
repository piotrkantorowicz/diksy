namespace Diksy.WebApi.Models.Translation
{
    /// <summary>
    ///     Contains details about a translation
    /// </summary>
    public sealed class TranslationInfo
    {
        /// <summary>The original phrase that was translated</summary>
        public required string Phrase { get; set; }

        /// <summary>The translated text in the target language</summary>
        public required string Translation { get; set; }

        /// <summary>Phonetic transcription of the translated text</summary>
        public required string Transcription { get; set; }

        /// <summary>An example usage of the translated phrase in context</summary>
        public required string Example { get; set; }
    }
}