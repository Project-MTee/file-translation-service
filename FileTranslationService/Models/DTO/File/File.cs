using System;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.DTO.File
{
    public class File
    {
        /// <summary>
        /// Identifier of the file
        /// </summary>
        /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// File type extention
        /// </summary>
        /// <example>docx</example>
        [JsonPropertyName("extension")]
        public string Extension { get; set; }

        /// <summary>
        /// File category
        /// </summary>
        /// <example>Source</example>
        [JsonPropertyName("category")]
        public FileCategory Category { get; set; }

        /// <summary>
        /// Size of a file in bytes
        /// </summary>
        /// <example>707382</example>
        [JsonPropertyName("size")]
        public long Size { get; set; }
    }
}
