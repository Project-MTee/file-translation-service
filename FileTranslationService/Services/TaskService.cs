using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Services
{
    public class TaskService: ITaskService
    {
        private readonly Models.Database.TaskDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskService(
            IMapper mapper,
            Models.Database.TaskDbContext dbContext
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

        public async Task Remove(Guid task)
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
                .Include(item => item.Files)
                .FirstAsync();

            return _mapper.Map<Models.DTO.Task.Task>(metadata);
        }

        public async Task<Models.DTO.Task.Task> Add(Models.DTO.Task.NewTask createTask)
        {
            var metadata = _mapper.Map<Models.Database.Task>(createTask);
            metadata.TranslationStatus = Enums.TranslationStatus.Queuing;
            _dbContext.Tasks.Add(metadata);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<Models.DTO.Task.Task>(metadata);
        }

        public async Task<Models.DTO.Task.Task> Update(Guid task, Models.DTO.Task.TaskUpdate editTask)
        {
            var metadata = await _dbContext.Tasks.FindAsync(task);

            _mapper.Map(editTask, metadata);

            _dbContext.Tasks.Update(metadata);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<Models.DTO.Task.Task>(metadata);
        }

        public async Task<bool> Exists(Guid task)
        {
            var metadata = await _dbContext.Tasks.FindAsync(task);

            return metadata != null;
        }

        public async System.Threading.Tasks.Task AddFileToTask(Guid task, TaskFileExtension extension, Models.DTO.File.NewFile createLinkedFileDTO)
        {
            var metadata = await _dbContext.Tasks.FindAsync(task);

            var linkedFile = _mapper.Map<Models.Database.File>(createLinkedFileDTO);
            linkedFile.Extension = extension.Value;

            metadata.Files.Add(linkedFile);

            await _dbContext.SaveChangesAsync();
        }
    }
}
