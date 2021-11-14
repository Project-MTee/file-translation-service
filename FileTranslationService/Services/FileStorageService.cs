using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Models.Configuration;

namespace Tilde.MT.FileTranslationService.Services
{
    /// <summary>
    /// Store files in Hard drive storage
    /// </summary>
    public class FileStorageService
    {
        private readonly ConfigurationSettings _configurationSettings;
        private readonly ILogger _logger;

        /// <summary>
        ///
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configurationSettings"></param>
        public FileStorageService(
            ILogger<FileStorageService> logger,
            IOptions<ConfigurationSettings> configurationSettings
        )
        {
            _logger = logger;
            _configurationSettings = configurationSettings.Value;
        }

        /// <summary>
        /// Get path of stored file
        /// </summary>
        /// <param name="task"></param>
        /// <param name="linkedFile"></param>
        /// <returns></returns>
        public string GetPath(Guid task, Models.Database.File linkedFile)
        {
            var storageName = GetFileStorageName(linkedFile.Category.Category, linkedFile.Extension);
            return Path.Combine(GetFileTranslationDirectory(task), storageName);
        }

        /// <summary>
        /// Store single file
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="FileExtensionUnsupportedException">File extension is not supported</exception>
        /// <exception cref="FileConflictException">File already exists</exception>
        public async Task<string> Save(Guid task, FileCategory category, Stream file, string fileName)
        {
            Directory.CreateDirectory(GetFileTranslationDirectory(task));

            var extension = Path.GetExtension(fileName);

            if (!_configurationSettings.AllowedFileExtensions.Contains(extension))
            {
                throw new FileExtensionUnsupportedException(extension);
            }

            var storageName = GetFileStorageName(category, extension);

            var filePath = $"{GetFileTranslationDirectory(task)}/{storageName}";

            if (File.Exists(filePath))
            {
                throw new FileConflictException(filePath);
            }
            else
            {
                using var translationFile = File.Create(filePath);
                await file.CopyToAsync(translationFile);

                return extension;
            }
        }

        /// <summary>
        /// Delete current file or delete whole file translation directory if nothing is left
        /// </summary>
        /// <param name="task"></param>
        /// <param name="fileNameId"></param>
        /// <returns></returns>
        public void Delete(Guid task, Models.Database.File fileNameId)
        {
            var storageName = GetFileStorageName(fileNameId.Category.Category, fileNameId.Extension);
            var filePath = Path.Combine(GetFileTranslationDirectory(task), storageName);
            var translationDirectory = GetFileTranslationDirectory(task);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                _logger.LogWarning($"File marked for deletion is missing: {filePath}");
            }

            if (Directory.Exists(translationDirectory))
            {
                if (!Directory.EnumerateFileSystemEntries(translationDirectory).Any())
                {
                    Directory.Delete(GetFileTranslationDirectory(task));
                }
            }
            else
            {
                _logger.LogWarning($"Directory marked for deletion is missing: {translationDirectory}");
            }
        }

        private string GetFileTranslationDirectory(Guid task)
        {
            return $"{_configurationSettings.FileSystemStoragePath}/{task}";
        }
        private static string GetFileStorageName(FileCategory fileCategory, string safeExtension)
        {
            // Create unique file name identificator to prevent:
            //      - Directory travel exploit
            //      - File name injections
            var storageName = $"{fileCategory}.{safeExtension}";

            return storageName;
        }
    }
}
