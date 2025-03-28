using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Diksy.WebApi.Models
{
    /// <summary>
    ///     Represents a problem details object that follows the RFC 7807 specification.
    /// </summary>
    public class ApiProblemDetails : ProblemDetails
    {
        /// <summary>
        ///     Creates a new instance of ApiProblemDetails
        /// </summary>
        public ApiProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        }

        /// <summary>
        ///     A URI reference that identifies the specific occurrence of the problem
        /// </summary>
        [JsonPropertyName("instance")]
        public new string? Instance { get; set; }

        /// <summary>
        ///     A URI reference that identifies the problem type
        /// </summary>
        [JsonPropertyName("type")]
        public new string? Type { get; set; }

        /// <summary>
        ///     A short, human-readable summary of the problem
        /// </summary>
        [JsonPropertyName("title")]
        public new string? Title { get; set; }

        /// <summary>
        ///     A human-readable explanation specific to this occurrence of the problem
        /// </summary>
        [JsonPropertyName("detail")]
        public new string? Detail { get; set; }

        /// <summary>
        ///     The HTTP status code for this occurrence of the problem
        /// </summary>
        [JsonPropertyName("status")]
        public new int? Status { get; set; }

        /// <summary>
        ///     Additional details about the error
        /// </summary>
        [JsonPropertyName("errors")]
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}