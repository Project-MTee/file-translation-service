using System.ComponentModel.DataAnnotations;
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

        [JsonPropertyName("substatus")]
        public TranslationSubstatus TranslationSubstatus { get; set; }

        #endregion

        /// <summary>
        /// Translation system domain to use for the file translation. Domain is detected automatically if not provided.
        /// </summary>
        /// <example>general</example>
        [MaxLength(200)]
        [JsonPropertyName("domain")]
        public string Domain { get; init; }
    }
}
