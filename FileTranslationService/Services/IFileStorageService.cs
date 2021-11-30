using System;
using System.IO;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Models.ValueObjects;

namespace Tilde.MT.FileTranslationService.Services
{
    public interface IFileStorageService
    {
        Task<TaskFileExtension> Save(Guid task, FileCategory category, Stream file, string fileName);
        void Delete(Guid task, FileCategory category, TaskFileExtension extension);
        string GetPath(Guid task, FileCategory category, TaskFileExtension extension);
    }
}
