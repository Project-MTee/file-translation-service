using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.Database
{
    public class File : IDbEntityTimestamps
    {
        [Key]
        public Guid Id { get; set; }

        public string Extension { get; set; }

        public FileCategory Category { get; set; }

        public long Size { get; set; }

        public DateTime? DbCreatedAt { get; set; }
        public DateTime? DbUpdatedAt { get; set; }

        #region Relations

        [JsonIgnore]
        public Task Task { get; set; }

        #endregion
    }
}
