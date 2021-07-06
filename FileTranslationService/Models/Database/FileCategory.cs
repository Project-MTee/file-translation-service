using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.Database
{
    public class FileCategory
    {
        /// <summary>
        /// Enum id
        /// </summary>
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        [NotMapped]
        [JsonIgnore]
        public Enums.FileCategory Category
        {
            get
            {
                return (Enums.FileCategory)Id;
            }
        }
    }
}
