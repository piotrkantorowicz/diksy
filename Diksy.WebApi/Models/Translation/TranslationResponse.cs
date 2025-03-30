namespace Diksy.WebApi.Models.Translation
{
    /// <summary>
    ///     Response object for translation requests
    /// </summary>
    public sealed class TranslationResponse
    {
        /// <summary>
        ///     Default constructor for TranslationResponse
        /// </summary>
        public TranslationResponse()
        {
        }

        /// <summary>
        ///     Constructor for TranslationResponse with parameters
        /// </summary>
        /// <param name="success">Indicates if the translation was successful</param>
        /// <param name="response">Contains the translation details if successful</param>
        /// <param name="errors">List of error messages if any occurred during translation</param>
        public TranslationResponse(bool success, TranslationInfo? response = null, IEnumerable<string>? errors = null)
        {
            Success = success;
            Response = response;
            Errors = errors;
        }

        /// <summary>Indicates if the translation was successful</summary>
        public bool Success { get; init; }

        /// <summary>Contains the translation details if successful</summary>
        public TranslationInfo? Response { get; init; }

        /// <summary>List of error messages if any occurred during translation</summary>
        public IEnumerable<string>? Errors { get; init; }

        /// <summary>
        ///     Creates a successful translation response with the provided translation information
        /// </summary>
        /// <param name="translationInfo">The translation details to include in the response</param>
        /// <returns>A TranslationResponse indicating success with the provided translation</returns>
        public static TranslationResponse SuccessResponse(TranslationInfo translationInfo)
        {
            return new TranslationResponse { Success = true, Response = translationInfo };
        }

        /// <summary>
        ///     Creates an error translation response with the provided error message and optional exception
        /// </summary>
        /// <param name="errorMessage">The error message describing what went wrong</param>
        /// <returns>A TranslationResponse indicating failure with the error details</returns>
        public static TranslationResponse ErrorResponse(string errorMessage)
        {
            return new TranslationResponse { Success = false, Errors = [errorMessage] };
        }
    }
}