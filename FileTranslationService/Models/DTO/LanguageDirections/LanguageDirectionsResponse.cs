using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Models.Errors;

namespace Tilde.MT.FileTranslationService.Models.DTO.LanguageDirections
{
    public record LanguageDirectionsResponse
    {
        [JsonPropertyName("languageDirections")]
        public IEnumerable<LanguageDirection> LanguageDirections { get; init; }

        [JsonPropertyName("error")]
        public Error Error { get; init; }
    }
}
