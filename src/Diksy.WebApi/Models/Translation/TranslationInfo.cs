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
        /// <param name="translationOfExample">Translation of the example sentence</param>
        /// <param name="sourceLanguage">The source language of the phrase</param>
        /// <param name="targetLanguage">The target language of the translation</param>
        public TranslationInfo(string phrase, string translation, string transcription, string example,
            string translationOfExample, string sourceLanguage, string targetLanguage)
            : this()
        {
            Phrase = phrase;
            Translation = translation;
            Transcription = transcription;
            Example = example;
            TranslationOfExample = translationOfExample;
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;
        }

        /// <summary>The original phrase that was translated</summary>
        /// <example>Jak się masz?</example>
        /// <remarks>Used to provide context for the translation</remarks>
        public required string Phrase { get; init; }

        /// <summary>The translated text in the target language</summary>
        /// <example>How are you?</example>
        /// <remarks>Used to provide context for the translation</remarks>
        public required string Translation { get; init; }

        /// <summary>Phonetic transcription of the translated text</summary>
        /// <example>haʊ ɑːr juː</example>
        /// <remarks>Used to provide pronunciation guidance</remarks>
        public required string Transcription { get; init; }

        /// <summary>An example usage of the translated phrase in context</summary>
        /// <example>Jak się masz? Mam się dobrze.</example>
        /// <remarks>Used to provide context for the translation</remarks>
        public required string Example { get; init; }

        /// <summary>Translation of the example sentence</summary>
        /// <example>How are you? I'm fine.</example>
        /// <remarks>Used to provide context for the translation</remarks>
        public required string TranslationOfExample { get; init; }
 
        /// <summary>The source language of the phrase</summary>
        /// <example>Polish</example>
        /// <remarks>Used to provide context for the translation</remarks>
        public required string SourceLanguage { get; init; }

        /// <summary>The target language of the translation</summary>
        /// <example>English</example>
        /// <remarks>Used to provide context for the translation</remarks>
        public required string TargetLanguage { get; init; }
    }
}