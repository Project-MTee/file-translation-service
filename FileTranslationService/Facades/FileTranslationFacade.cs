using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Services;

namespace Tilde.MT.FileTranslationService.Facades
{
    public class FileTranslationFacade
    {
        private readonly FileStorageService _fileStorageService;
        private readonly TaskService _metadataService;

        public FileTranslationFacade(
            FileStorageService fileStorageService,
            TaskService metadataService
        )
        {
            _fileStorageService = fileStorageService;
            _metadataService = metadataService;
        }

        public async Task<IEnumerable<Models.DTO.Task.Task>> GetExpiredMetadata(TimeSpan ttl)
        {
            return await _metadataService.GetExpired(ttl);
        }

        public async System.Threading.Tasks.Task RemoveMetadata(Guid task)
        {
            var files = await _metadataService.GetLinkedFiles(task);
            foreach (var file in files)
            {
                _fileStorageService.Delete(task, file);
            }

            await _metadataService.Remove(task);
        }
    }
}
