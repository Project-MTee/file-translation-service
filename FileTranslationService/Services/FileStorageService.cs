using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.Models.Configuration;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Services
{
    /// <summary>
    /// Store files in Hard drive storage
    /// </summary>
    public class FileStorageService : IFileStorageService
    {
        private readonly ConfigurationSettings _configurationSettings;
        private readonly ILogger _logger;

        public FileStorageService(
            ILogger<FileStorageService> logger,
            IOptions<ConfigurationSettings> configurationSettings
        )
        {
            _logger = logger;
            _configurationSettings = configurationSettings.Value;
        }

        public string GetPath(Guid task, Enums.FileCategory category, TaskFileExtension extension)
        {
            var storageName = GetTaskFileStorageName(category, extension);
            return Path.Combine(GetTaskFileTranslationDirectory(task), storageName);
        }

        public async Task Save(Guid task, FileCategory category, Stream file, TaskFileExtension extension)
        {
            Directory.CreateDirectory(GetTaskFileTranslationDirectory(task));

            if (!_configurationSettings.AllowedFileExtensions.Contains(extension.Value))
            {
                throw new FileExtensionUnsupportedException(extension.Value);
            }

            var storageName = GetTaskFileStorageName(category, extension);

            var filePath = $"{GetTaskFileTranslationDirectory(task)}/{storageName}";

            if (File.Exists(filePath))
            {
                throw new TaskFileConflictException(task, filePath);
            }
            else
            {
                using var translationFile = File.Create(filePath);
                await file.CopyToAsync(translationFile);
            }
        }

        public void Delete(Guid task, FileCategory category, TaskFileExtension extension)
        {
            var filePath = GetTaskFileLocation(task, category, extension);
            var translationDirectory = GetTaskFileTranslationDirectory(task);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                _logger.LogWarning("File marked for deletion is missing: {filePath}", filePath);
            }

            if (Directory.Exists(translationDirectory))
            {
                if (!Directory.EnumerateFileSystemEntries(translationDirectory).Any())
                {
                    Directory.Delete(GetTaskFileTranslationDirectory(task));
                }
            }
            else
            {
                _logger.LogWarning("Directory marked for deletion is missing: {translationDirectory}", translationDirectory);
            }
        }

        private string GetTaskFileLocation(Guid task, FileCategory category, TaskFileExtension extension)
        {
            var storageName = GetTaskFileStorageName(category, extension);
            return Path.Combine(GetTaskFileTranslationDirectory(task), storageName);
        }
        private string GetTaskFileTranslationDirectory(Guid task)
        {
            return $"{_configurationSettings.FileSystemStoragePath}/{task}";
        }
        private static string GetTaskFileStorageName(FileCategory fileCategory, TaskFileExtension safeExtension)
        {
            // Create unique file name identificator to prevent:
            //      - Directory travel exploit
            //      - File name injections
            var storageName = $"{fileCategory}.{safeExtension.Value}";

            return storageName;
        }
    }
}
