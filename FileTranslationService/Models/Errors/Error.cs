using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.Errors
{
    public record Error
    {
        /// <summary>
        /// Error code
        /// </summary>
        /// <example>500</example>
        [JsonPropertyName("code")]
        public int Code { get; init; }

        /// <summary>
        /// Textual message of error
        /// </summary>
        /// <example>Error message</example>
        [JsonPropertyName("message")]
        public string Message { get; init; }
    }
}
