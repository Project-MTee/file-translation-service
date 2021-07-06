using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.DTO.LanguageDirections
{
    public class GetLanguageDirections
    {
        [JsonPropertyName("languageDirections")]
        public IEnumerable<LanguageDirection> LanguageDirections { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}
