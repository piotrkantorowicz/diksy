namespace Diksy.WebApi.Models.Translation
{
    /// <summary>
    ///     Contains details about a translation
    /// </summary>
    public sealed class TranslationInfo
    {
        /// <summary>
        ///     Default constructor for TranslationInfo
        /// </summary>
        public TranslationInfo()
        {
        }

        /// <summary>
        ///     Constructor for TranslationInfo with parameters
        /// </summary>
        /// <param name="phrase">The original phrase that was translated</param>
        /// <param name="translation">The translated text in the target language</param>
        /// <param name="transcription">Phonetic transcription of the translated text</param>
        /// <param name="example">An example usage of the translated phrase in context</param>
        public TranslationInfo(string phrase, string translation, string transcription, string example)
        {
            Phrase = phrase;
            Translation = translation;
            Transcription = transcription;
            Example = example;
        }

        /// <summary>The original phrase that was translated</summary>
        public required string Phrase { get; init; }

        /// <summary>The translated text in the target language</summary>
        public required string Translation { get; init; }

        /// <summary>Phonetic transcription of the translated text</summary>
        public required string Transcription { get; init; }

        /// <summary>An example usage of the translated phrase in context</summary>
        public required string Example { get; init; }
    }
}