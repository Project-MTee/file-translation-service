using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Interfaces.Facades
{
    public interface IFileTranslationFacade
    {
        /// <summary>
        /// Remove specific task and associated data
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        Task RemoveTask(Guid task);

        /// <summary>
        /// Create new task
        /// </summary>
        /// <param name="createTask"></param>
        /// <returns></returns>
        /// <exception cref="TaskFileConflictException"></exception>
        /// <exception cref="FileExtensionUnsupportedException"></exception>
        Task<Models.DTO.Task.Task> AddTask(Models.DTO.Task.NewTask createTask);

        /// <summary>
        /// Get task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        Task<Models.DTO.Task.Task> GetTask(Guid task);

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="editTask"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        Task<Models.DTO.Task.Task> UpdateTask(Guid task, Models.DTO.Task.TaskUpdate editTask);
        
        /// <summary>
        /// Remove all tasks that are expired
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        Task RemoveExpiredTasks(TimeSpan ttl);

        /// <summary>
        /// Add file to task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        /// <exception cref="TaskFileConflictException"></exception>
        /// <exception cref="FileExtensionUnsupportedException"></exception>
        Task AddFile(Guid task, FileCategory category, IFormFile file);

        /// <summary>
        /// Get task file
        /// </summary>
        /// <param name="task"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<Models.DTO.File.File> GetFile(Guid task, Guid file);

        /// <summary>
        /// Get task file storage path where file is stored
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        string GetFileStoragePath(Guid task, FileCategory category, TaskFileExtension extension);
    }
}
