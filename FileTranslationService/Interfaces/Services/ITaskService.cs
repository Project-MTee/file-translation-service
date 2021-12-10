using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Interfaces.Services
{
    public interface ITaskService
    {
        /// <summary>
        /// Check if file exists
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<bool> Exists(Guid task);

        /// <summary>
        /// Remove task from storage
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task Remove(Guid task);

        /// <summary>
        /// Add file to task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="extension"></param>
        /// <param name="createLinkedFileDTO"></param>
        /// <returns></returns>
        Task AddFileToTask(Guid task, TaskFileExtension extension, Models.DTO.File.NewFile createLinkedFileDTO);

        /// <summary>
        /// Add new task
        /// </summary>
        /// <param name="createTask"></param>
        /// <returns></returns>
        Task<Models.DTO.Task.Task> Add(Models.DTO.Task.NewTask createTask);

        /// <summary>
        /// Get task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<Models.DTO.Task.Task> Get(Guid task);

        /// <summary>
        /// Update task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="editTask"></param>
        /// <returns></returns>
        Task<Models.DTO.Task.Task> Update(Guid task, Models.DTO.Task.TaskUpdate editTask);

        /// <summary>
        /// Get expired tasks
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task<IEnumerable<Models.DTO.Task.Task>> GetExpired(TimeSpan ttl);
    }
}
