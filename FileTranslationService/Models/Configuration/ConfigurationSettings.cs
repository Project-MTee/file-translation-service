using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Models.Configuration
{
    public class ConfigurationSettings
    {
        /// <summary>
        /// Where to store file translation files
        /// </summary>
        public string FileSystemStoragePath { get; set; }

        public Dictionary<string, TimeSpan> UserGroupMetadataExpiration { get; set; }

        /// <summary>
        /// Allowed extensions for file to upload.
        /// Not using mime types, because not all types are declared for example tmx
        /// </summary>
        public HashSet<string> AllowedFileExtensions { get; set; }
    }
}
