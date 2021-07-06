using System;

namespace Tilde.MT.FileTranslationService.Models.Database
{
    public interface IDbEntityTimestamps
    {
        /// <summary>
        /// Timestamp when entity is created in DB
        /// </summary>
        public DateTime? DbCreatedAt { get; set; }
        /// <summary>
        /// Timestamp when entity is updated in DB
        /// </summary>
        public DateTime? DbUpdatedAt { get; set; }
    }
}
