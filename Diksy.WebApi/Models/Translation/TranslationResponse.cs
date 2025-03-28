namespace Diksy.WebApi.Models.Translation
{
    /// <summary>
    ///     Response object for translation requests
    /// </summary>
    public sealed record TranslationResponse
    {
        /// <summary>Indicates if the translation was successful</summary>
        public bool Success { get; set; }

        /// <summary>Contains the translation details if successful</summary>
        public TranslationInfo? Response { get; set; }

        /// <summary>List of error messages if any occurred during translation</summary>
        public IEnumerable<string>? Errors { get; set; }

        /// <summary>Exception that occurred during translation</summary>
        public Exception? Exception { get; set; }
    }
}