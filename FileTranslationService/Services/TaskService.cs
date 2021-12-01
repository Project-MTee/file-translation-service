using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.Models.Database;
using Tilde.MT.FileTranslationService.Models.DTO.File;
using Tilde.MT.FileTranslationService.Models.DTO.Task;
using Tilde.MT.FileTranslationService.Models.ValueObjects;

namespace Tilde.MT.FileTranslationService.Services
{
    public class TaskService: ITaskService
    {
        private readonly TaskDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskService(
            IMapper mapper,
            TaskDbContext dbContext
        )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Models.DTO.Task.Task>> GetExpired(TimeSpan ttl)
        {
            var timeWhenExpires = DateTime.UtcNow - ttl;
            var metadata = await _dbContext.Tasks
                .Where(item => item.DbUpdatedAt < timeWhenExpires)
                .ToListAsync();

            return metadata.Select(item => _mapper.Map<Models.DTO.Task.Task>(item)).ToList();
        }

        public async Task<IEnumerable<Models.DTO.File.File>> GetLinkedFiles(Guid task)
        {
            var metadata = await _dbContext.Files
                .Where(item => item.Task.Id == task)
                .Include(item => item.Category)
                .ToListAsync();

            return metadata.Select(item => _mapper.Map<Models.DTO.File.File>(item));
        }

        public async System.Threading.Tasks.Task Remove(Guid task)
        {
            var metadata = await _dbContext.Tasks
                .Where(item => item.Id == task)
                .Include(item => item.Files)
                .FirstAsync();

            _dbContext.Files.RemoveRange(metadata.Files);
            _dbContext.Tasks.Remove(metadata);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Models.DTO.Task.Task> Get(Guid task)
        {
            var metadata = await _dbContext.Tasks
                .Where(item => item.Id == task)
                .Include(item => item.TranslationStatus)
                .Include(item => item.Files)
                    .ThenInclude(item => item.Category)
                .FirstAsync();

            return _mapper.Map<Models.DTO.Task.Task>(metadata);
        }

        public async Task<Models.DTO.Task.Task> Create(NewTask createTask)
        {
            var metadata = _mapper.Map<Models.Database.Task>(createTask);
            metadata.TranslationStatus = await _dbContext.TranslationStatuses.FindAsync((int)Enums.TranslationStatus.Queuing);
            _dbContext.Tasks.Add(metadata);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<Models.DTO.Task.Task>(metadata);
        }

        public async Task<Models.DTO.Task.Task> Update(Guid task, TaskUpdate editTask)
        {
            var metadata = await _dbContext.Tasks.FindAsync(task);
            await _dbContext.Entry(metadata).Reference(item => item.TranslationStatus).LoadAsync();

            /*if(
                metadata.TranslationStatus.Status == TranslationStatusType.Completed ||
                metadata.TranslationStatus.Status == TranslationStatusType.Error
            )
            {
                return null;
            }*/

            _mapper.Map(editTask, metadata);
            metadata.TranslationStatus = await _dbContext.TranslationStatuses.FindAsync((int)editTask.TranslationStatus);

            _dbContext.Tasks.Update(metadata);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<Models.DTO.Task.Task>(metadata);
        }

        /// <summary>
        /// Check if metadata exists
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> Exists(Guid task)
        {
            var metadata = await _dbContext.Tasks.FindAsync(task);

            return metadata != null;
        }

        public async System.Threading.Tasks.Task AddLinkedFile(Guid task, TaskFileExtension extension, NewFile createLinkedFileDTO)
        {
            var metadata = await _dbContext.Tasks.FindAsync(task);

            var linkedFile = _mapper.Map<Models.Database.File>(createLinkedFileDTO);
            linkedFile.Extension = extension.Value;
            linkedFile.Category = await _dbContext.FileCategories.FindAsync((int)createLinkedFileDTO.Type);

            metadata.Files.Add(linkedFile);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Models.DTO.File.File> GetLinkedFile(Guid task, Guid fileId)
        {
            var metadata = await _dbContext.Files
                .Where(item => item.Id == fileId)
                .Where(item => item.Task.Id == task)
                .Include(item => item.Category)
                .FirstOrDefaultAsync();

            return _mapper.Map<Models.DTO.File.File>(metadata);
        }
    }
}
