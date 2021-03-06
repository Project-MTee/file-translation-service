
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.DTO.Task
{
    public record NewTask
    {
        /// <summary>
        /// Translatable file
        /// </summary>
        [Required]
        [JsonPropertyName("file")]
        public IFormFile File { get; init; }

        /// <summary>
        /// Language code of the source file
        /// </summary>
        /// <example>en</example>
        [Required]
        [MaxLength(2)]
        [JsonPropertyName("srcLang")]
        public string SourceLanguage { get; init; }

        /// <summary>
        /// Language code of the translation
        /// </summary>
        /// <example>et</example>
        [Required]
        [MaxLength(2)]
        [JsonPropertyName("trgLang")]
        public string TargetLanguage { get; init; }

        /// <summary>
        /// Translation system domain to use for the file translation. Domain is detected automatically if not provided.
        /// </summary>
        /// <example>general</example>
        [MaxLength(200)]
        [JsonPropertyName("domain")]
        public string Domain { get; init; }
    }
}
