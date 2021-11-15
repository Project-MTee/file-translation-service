using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.Database
{
    public class TranslationStatus
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        [NotMapped]
        [JsonIgnore]
        public Enums.TranslationStatus Status
        {
            get
            {
                return (Enums.TranslationStatus)Id;
            }
        }

        [JsonIgnore]
        public virtual ICollection<Task> FileTranslationMetadata { get; set; } = new Collection<Task>();
    }
}
