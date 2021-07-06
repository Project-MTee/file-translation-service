using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Models;
using Tilde.MT.FileTranslationService.Models.DTO.File;
using Tilde.MT.FileTranslationService.Services;

namespace Tilde.MT.FileTranslationService.Controllers
{
    /// <summary>
    /// Manipulate translation files
    /// </summary>
    [ApiController]
    [Route("file")]
    public class FileController : APIResponseBaseController
    {
        private readonly FileStorageService _fileStorageService;
        private readonly TaskService _metadataService;
        private readonly ILogger _logger;

        public FileController(
            FileStorageService fileStorageService,
            TaskService metadataService,
            ILogger<FileController> logger
        )
        {
            _fileStorageService = fileStorageService;
            _metadataService = metadataService;
            _logger = logger;
        }

        /// <summary>
        /// Download file associated to the file translation task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{task}/{file}")]
        [AllowAnonymous]
        [Authorize(AuthenticationSchemes = AuthenticationSchemeType.FileTranslationWorkflow)]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, Description = "Source file is forbidden to download")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task or File is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult> Details(Guid task, Guid file)
        {
            var linkedFile = await _metadataService.GetLinkedFile(task, file);
            if (linkedFile == null)
            {
                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayFileNotFound);
            }
            else if (linkedFile.Category.Category == FileCategory.Source)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return FormatAPIError(HttpStatusCode.Forbidden, ErrorSubCode.GatewaySourceFileDownloadForbidden);
                }
            }
            var filePath = _fileStorageService.GetPath(task, linkedFile);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return PhysicalFile(filePath, contentType);
        }

        /// <summary>
        /// Submit new file for existing fileTranslation
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = AuthenticationSchemeType.FileTranslationWorkflow)]
        [HttpPost]
        [Route("{task}")]
        [RequestSizeLimit(104_857_600)] // Puprously larger than user uploaded file 100MB
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Description = "File with same category is already uploaded", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task or File is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult> Create(Guid task, FileCategory category, IFormFile file)
        {
            if (!await _metadataService.Exists(task)) {
                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }

            using var fileStream = file.OpenReadStream();

            string extension;
            try
            {
                extension = await _fileStorageService.Save(
                    task,
                    category,
                    fileStream,
                    file.FileName
                );
            }
            catch(FileConflictException ex)
            {
                // This should normally not happen. 
                // If translation failed, user can initiate new file translation
                // This prevents overriding same file 

                _logger.LogError(ex, "File already exists");
                return FormatAPIError(HttpStatusCode.Conflict, ErrorSubCode.GatewayTaskFileConflict);
            }
            catch(FileExtensionUnsupportedException ex)
            {
                _logger.LogError(ex, "File extension not supported");
                return FormatAPIError(HttpStatusCode.UnsupportedMediaType, ErrorSubCode.GatewayMediaTypeNotValid);
            }

            await _metadataService.AddLinkedFile(
                task, 
                extension,
                new NewFile()
                {
                    Type = category,
                    Size = fileStream.Length
                }
            );

            return Ok();
        }
    }
}
