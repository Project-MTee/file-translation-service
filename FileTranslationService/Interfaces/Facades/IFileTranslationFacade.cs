using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Models.ValueObjects;

namespace Tilde.MT.FileTranslationService.Interfaces.Facades
{
    public interface IFileTranslationFacade
    {
        Task RemoveTask(Guid task);
        Task<Models.DTO.Task.Task> AddTask(Models.DTO.Task.NewTask createTask);
        Task<Models.DTO.Task.Task> GetTask(Guid task);
        Task<Models.DTO.Task.Task> UpdateTask(Guid task, Models.DTO.Task.TaskUpdate editTask);
        Task RemoveExpiredTasks(TimeSpan ttl);
        Task AddFile(Guid task, FileCategory category, IFormFile file);
        Task<Models.DTO.File.File> GetFile(Guid task, Guid file);
        string GetFileStoragePath(Guid task, FileCategory category, TaskFileExtension extension);
    }
}
