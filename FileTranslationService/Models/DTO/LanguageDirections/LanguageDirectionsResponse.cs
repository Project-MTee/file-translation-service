using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Models.Errors;

namespace Tilde.MT.FileTranslationService.Models.DTO.LanguageDirections
{
    public class LanguageDirectionsResponse
    {
        [JsonPropertyName("languageDirections")]
        public IEnumerable<LanguageDirection> LanguageDirections { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}
