using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Interfaces.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.Models.DTO.File;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Facades
{
    public class FileTranslationFacade: IFileTranslationFacade
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ITaskService _taskService;

        public FileTranslationFacade(
            IFileStorageService fileStorageService,
            ITaskService metadataService
        )
        {
            _fileStorageService = fileStorageService;
            _taskService = metadataService;
        }
        
        public async Task RemoveTask(Guid task)
        {
            if (!await _taskService.Exists(task))
            {
                throw new TaskNotFoundException(task);
            }

            var taskFound = await _taskService.Get(task);
            foreach (var file in taskFound.Files)
            {
                _fileStorageService.Delete(task, file.Category, new TaskFileExtension(file.Extension));
            }

            await _taskService.Remove(task);
        }

        public async Task<Models.DTO.Task.Task> AddTask(Models.DTO.Task.NewTask createTask)
        {
            using var fileStream = createTask.File.OpenReadStream();

            var metadata = await _taskService.Add(createTask);

            var extension = await _fileStorageService.Save(
                metadata.Id,
                FileCategory.Source,
                fileStream,
                createTask.File.FileName
            );
                
            await _taskService.AddFileToTask(metadata.Id, extension, new Models.DTO.File.NewFile()
            {
                Category = FileCategory.Source,
                Size = fileStream.Length
            });

            return await _taskService.Get(metadata.Id);
        }

        public async Task<Models.DTO.Task.Task> GetTask(Guid task)
        {
            if (!await _taskService.Exists(task))
            {
                throw new TaskNotFoundException(task);
            }

            var metadata = await _taskService.Get(task);

            return metadata;
        }

        public async Task<Models.DTO.Task.Task> UpdateTask(Guid task, Models.DTO.Task.TaskUpdate editTask)
        {
            if (!await _taskService.Exists(task))
            {
                throw new TaskNotFoundException(task);
            }

            var metadata = await _taskService.Update(task, editTask);

            return metadata;
        }

        public async Task RemoveExpiredTasks(TimeSpan ttl)
        {
            var expiredMetadata = await _taskService.GetExpired(ttl);

            foreach (var task in expiredMetadata)
            {
                await RemoveTask(task.Id);
            }
        }

        public async Task AddFile(Guid task, FileCategory category, IFormFile file)
        {
            if (!await _taskService.Exists(task))
            {
                throw new TaskNotFoundException(task);
            }

            using var fileStream = file.OpenReadStream();

            var extension = await _fileStorageService.Save(task, category, fileStream, file.FileName);

            await _taskService.AddFileToTask(
                task,
                extension,
                new NewFile()
                {
                    Category = category,
                    Size = fileStream.Length
                }
            );
        }

        public async Task<File> GetFile(Guid task, Guid file)
        {
            var foundFile = (await _taskService.Get(task))?.Files.Where(item => item.Id == file).FirstOrDefault();

            if (foundFile == null)
            {
                throw new TaskFileNotFoundException(task, file);
            }

            return foundFile;
        }

        public string GetFileStoragePath(Guid task, Enums.FileCategory category, TaskFileExtension extension)
        {
            return _fileStorageService.GetPath(task, category, extension);
        }
    }
}
