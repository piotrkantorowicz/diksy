using Diksy.WebApi.Models;
using Diksy.WebApi.Models.Translation;
using Diksy.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Diksy.WebApi.Controllers
{
    /// <summary>
    ///     Controller for translating text using AI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("translation")]
    [ProducesResponseType(type: typeof(ApiProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(type: typeof(ApiProblemDetails), statusCode: StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(type: typeof(ApiProblemDetails), statusCode: StatusCodes.Status429TooManyRequests)]
    public class TranslationController(ITranslationService translationService, ILogger<TranslationController> logger)
        : ControllerBase
    {
        private readonly ILogger<TranslationController> _logger =
            logger ?? throw new ArgumentNullException(nameof(logger));

        private readonly ITranslationService _translationService =
            translationService ?? throw new ArgumentNullException(nameof(translationService));

        /// <summary>
        ///     Translates a phrase to the specified language
        /// </summary>
        /// <param name="request">The translation request containing phrase, model, and target language</param>
        /// <remarks>
        ///     Sample request:
        ///     POST /api/Translation
        ///     {
        ///     "phrase": "Hello world",
        ///     "model": "gpt-4o",
        ///     "language": "Spanish"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the translated text with pronunciation and example</response>
        /// <response code="400">If the request is invalid or translation fails</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(type: typeof(TranslationResponse), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ApiProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ApiProblemDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Translate([FromBody] TranslationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiProblemDetails
                {
                    Title = "Validation Failed",
                    Detail = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Errors = ModelState.ToDictionary(
                        keySelector: kvp => kvp.Key,
                        elementSelector: kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                    )
                });
            }

            TranslationResponse result = await _translationService.TranslateAsync(
                phrase: request.Phrase,
                model: request.Model,
                language: request.Language,
                cancellationToken: default);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(statusCode: StatusCodes.Status500InternalServerError,
                value: new ApiProblemDetails
                {
                    Title = "Translation Error",
                    Detail = "An unexpected error occurred during translation.",
                    Status = StatusCodes.Status500InternalServerError
                });
        }
    }
}