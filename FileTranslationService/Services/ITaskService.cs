using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Models.ValueObjects;

namespace Tilde.MT.FileTranslationService.Services
{
    public interface ITaskService
    {
        Task<bool> Exists(Guid task);
        Task<IEnumerable<Models.DTO.File.File>> GetLinkedFiles(Guid task);
        Task<Models.DTO.File.File> GetLinkedFile(Guid task, Guid fileId);
        Task Remove(Guid task);
        Task AddLinkedFile(Guid task, TaskFileExtension extension, Models.DTO.File.NewFile createLinkedFileDTO);
        Task<Models.DTO.Task.Task> Create(Models.DTO.Task.NewTask createTask);
        Task<Models.DTO.Task.Task> Get(Guid task);
        Task<Models.DTO.Task.Task> Update(Guid task, Models.DTO.Task.TaskUpdate editTask);
        Task<IEnumerable<Models.DTO.Task.Task>> GetExpired(TimeSpan ttl);
    }
}
