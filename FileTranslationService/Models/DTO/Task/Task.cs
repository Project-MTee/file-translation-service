
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Models.DTO.File;

namespace Tilde.MT.FileTranslationService.Models.DTO.Task
{
    public class Task
    {
        /// <summary>
        /// Identifier of the file translation task
        /// </summary>
        /// <example>08d989a1-d15e-419f-8b83-b4ce18dfd9d4</example>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Language code of the source file
        /// </summary>
        /// <example>en</example>
        [Required]
        [MaxLength(2)]
        [JsonPropertyName("srcLang")]
        public string SourceLanguage { get; set; }

        /// <summary>
        /// Language code of the translation
        /// </summary>
        /// <example>et</example>
        [Required]
        [MaxLength(2)]
        [JsonPropertyName("trgLang")]
        public string TargetLanguage { get; set; }

        /// <summary>
        /// Translation system domain to use for the file translation. Domain is detected automatically if not provided.
        /// </summary>
        /// <example>general</example>
        [MaxLength(200)]
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// Name of a source file
        /// </summary>
        /// <example>Welcome to Word.docx</example>
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Date and time of file upload
        /// </summary>
        /// <example>2021-10-07T14:50:34.2520337Z</example>
        [JsonPropertyName("createdAt")]
        public DateTime DbCreatedAt { get; set; }

        /// <summary>
        /// Status of the file translation task
        /// </summary>
        /// <example>Queuing</example>
        [JsonPropertyName("status")]
        public TranslationStatus TranslationStatus { get; set; }

        /// <summary>
        /// Count of text segments in the source file
        /// </summary>
        /// <example>0</example>
        [JsonPropertyName("segments")]
        public int Segments { get; set; }

        /// <summary>
        /// Count of translated text segmets for translation progress tracking
        /// </summary>
        /// <example>0</example>
        [JsonPropertyName("translatedSegments")]
        public int SegmentsTranslated { get; set; }

        [JsonPropertyName("files")]
        public List<File.File> Files { get; set; }
    }
}
