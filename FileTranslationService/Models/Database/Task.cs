using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.Database
{
    public class Task : IDbEntityTimestamps
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(300)]
        public string FileName { get; set; }

        [MaxLength(2)]
        public string SourceLanguage { get; set; }

        [MaxLength(2)]
        public string TargetLanguage { get; set; }

        [MaxLength(200)]
        public string Domain { get; set; }

        #region Timestamps

        public DateTime? DbCreatedAt { get; set; }
        public DateTime? DbUpdatedAt { get; set; }

        #endregion

        #region Statistics

        public long Segments { get; set; }
        public long SegmentsTranslated { get; set; }

        #endregion

        #region Relations

        [JsonIgnore]
        public virtual TranslationStatus TranslationStatus { get; set; }

        [JsonIgnore]
        public virtual ICollection<File> Files { get; set; } = new Collection<File>();

        #endregion
    }
}
