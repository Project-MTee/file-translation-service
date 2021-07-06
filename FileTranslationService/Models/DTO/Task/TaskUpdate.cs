using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.DTO.Task
{
    public class TaskUpdate
    {
        #region Statistics

        [JsonPropertyName("segments")]
        public long Segments { get; set; }

        [JsonPropertyName("translatedSegments")]
        public long SegmentsTranslated { get; set; }

        #endregion

        #region Translation status

        [JsonPropertyName("status")]
        public TranslationStatusType TranslationStatus { get; set; }

        #endregion
    }
}
