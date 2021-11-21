
using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.Errors
{
    public record APIError
    {
        [JsonPropertyName("error")]
        public Error Error { get; init; }
    }
}
