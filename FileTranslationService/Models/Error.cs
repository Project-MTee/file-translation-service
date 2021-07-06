using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Models
{
    public class Error
    {
        /// <summary>
        /// Error code
        /// </summary>
        /// <example>500</example>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Textual message of error
        /// </summary>
        /// <example>Error message</example>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
