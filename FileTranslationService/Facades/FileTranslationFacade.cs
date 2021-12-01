using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Interfaces.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.Models.DTO.File;
using Tilde.MT.FileTranslationService.Models.ValueObjects;

namespace Tilde.MT.FileTranslationService.Facades
{
    public class FileTranslationFacade: IFileTranslationFacade
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public FileTranslationFacade(
            IFileStorageService fileStorageService,
            ITaskService metadataService,
            IMapper mapper
        )
        {
            _fileStorageService = fileStorageService;
            _taskService = metadataService;
            _mapper = mapper;
        }

        /// <summary>
        /// Remove specific task and associated data
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException">Task not found</exception>
        public async Task RemoveTask(Guid task)
        {
            if (!await _taskService.Exists(task))
            {
                throw new TaskNotFoundException(task);
            }

            var files = await _taskService.GetLinkedFiles(task);
            foreach (var file in files)
            {
                _fileStorageService.Delete(task, file.Category, new TaskFileExtension(file.Extension));
            }

            await _taskService.Remove(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createTask"></param>
        /// <returns></returns>
        /// <exception cref="TaskFileConflictException"></exception>
        /// <exception cref="FileExtensionUnsupportedException"></exception>
        public async Task<Models.DTO.Task.Task> AddTask(Models.DTO.Task.NewTask createTask)
        {
            using var fileStream = createTask.File.OpenReadStream();

            var metadata = await _taskService.Create(createTask);

            var extension = await _fileStorageService.Save(
                metadata.Id,
                FileCategory.Source,
                fileStream,
                createTask.File.FileName
            );
                
            await _taskService.AddLinkedFile(metadata.Id, extension, new Models.DTO.File.NewFile()
            {
                Type = FileCategory.Source,
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

        /// <summary>
        /// Remove all tasks that are expired
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        public async Task RemoveExpiredTasks(TimeSpan ttl)
        {
            var expiredMetadata = await _taskService.GetExpired(ttl);

            foreach (var task in expiredMetadata)
            {
                await RemoveTask(task.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="TaskNotFoundException"></exception>
        /// <exception cref="TaskFileConflictException"></exception>
        /// <exception cref="FileExtensionUnsupportedException"></exception>
        public async Task AddFile(Guid task, FileCategory category, IFormFile file)
        {
            if (!await _taskService.Exists(task))
            {
                throw new TaskNotFoundException(task);
            }

            using var fileStream = file.OpenReadStream();

            var extension = await _fileStorageService.Save(task, category, fileStream, file.FileName);

            await _taskService.AddLinkedFile(
                task,
                extension,
                new NewFile()
                {
                    Type = category,
                    Size = fileStream.Length
                }
            );
        }

        public async Task<Models.DTO.File.File> GetFile(Guid task, Guid file)
        {
            var linkedFile = await _taskService.GetLinkedFile(task, file);
            if (linkedFile == null)
            {
                throw new TaskFileNotFoundException(task, file);
            }

            var fileFound = _mapper.Map<Models.DTO.File.File>(linkedFile);
            return fileFound;
        }

        public string GetFileStoragePath(Guid task, Enums.FileCategory category, TaskFileExtension extension)
        {
            return _fileStorageService.GetPath(task, category, extension);
        }
    }
}
