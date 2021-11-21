using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.DTO.Task
{
    public record TaskUpdate
    {
        #region Statistics

        [JsonPropertyName("segments")]
        public long Segments { get; init; }

        [JsonPropertyName("translatedSegments")]
        public long SegmentsTranslated { get; init; }

        #endregion

        #region Translation status

        [JsonPropertyName("status")]
        public TranslationStatus TranslationStatus { get; init; }

        #endregion
    }
}
