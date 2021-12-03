using System;
using System.IO;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Interfaces.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Save file in storage
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="TaskFileConflictException">File already exists</exception>
        /// <exception cref="FileExtensionUnsupportedException">File exception is not supported</exception>
        Task<TaskFileExtension> Save(Guid task, FileCategory category, Stream file, string fileName);
        
        /// <summary>
        /// Delete file from storage
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="extension"></param>
        void Delete(Guid task, FileCategory category, TaskFileExtension extension);
        
        /// <summary>
        /// Get stored file path
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        string GetPath(Guid task, FileCategory category, TaskFileExtension extension);
    }
}
