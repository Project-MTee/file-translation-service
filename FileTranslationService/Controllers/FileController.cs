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
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Interfaces.Facades;
using Tilde.MT.FileTranslationService.Models.Errors;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Controllers
{
    /// <summary>
    /// Manipulate translation files
    /// </summary>
    [ApiController]
    [Route("file")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class FileController : BaseController
    {
        private readonly IFileTranslationFacade _fileTranslationFacade;
        private readonly ILogger _logger;
        private readonly IContentTypeProvider _contentTypeProvider;

        public FileController(
            IFileTranslationFacade fileTranslationFacade,
            ILogger<FileController> logger,
            IContentTypeProvider contentTypeProvider
        )
        {
            _fileTranslationFacade = fileTranslationFacade;
            _logger = logger;
            _contentTypeProvider = contentTypeProvider;
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
        [Authorize(AuthenticationSchemes = AuthenticationScheme.FileTranslationWorkflow)]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.Forbidden, Description = "Source file is forbidden to download")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task or File is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult> GetFile(Guid task, Guid file)
        {
            try
            {
                var fileFound = await _fileTranslationFacade.GetFile(task, file);

                if (fileFound.Category == FileCategory.Source)
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        return FormatAPIError(HttpStatusCode.Forbidden, GatewayErrorSubcode.GatewaySourceFileDownloadForbidden);
                    }
                }
                var filePath = _fileTranslationFacade.GetFileStoragePath(task, fileFound.Category, new TaskFileExtension(fileFound.Extension));

                if (!_contentTypeProvider.TryGetContentType(filePath, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                return PhysicalFile(filePath, contentType);
            }
            catch (TaskFileNotFoundException ex)
            {
                _logger.LogError(ex, "Task file not found");

                return FormatAPIError(HttpStatusCode.NotFound, GatewayErrorSubcode.GatewayFileNotFound);
            }
        }

        /// <summary>
        /// Submit new file for existing fileTranslation
        /// </summary>
        /// <param name="task"></param>
        /// <param name="category"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = AuthenticationScheme.FileTranslationWorkflow)]
        [HttpPost]
        [Route("{task}")]
        [RequestSizeLimit(104_857_600)] // Puprously larger than user uploaded file 100MB
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Description = "File with same category is already uploaded", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task or File is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult> CreateFile(Guid task, FileCategory category, IFormFile file)
        {
            try
            {
                await _fileTranslationFacade.AddFile(task, category, file);
            }
            catch (TaskNotFoundException ex)
            {
                _logger.LogError(ex, "Task not found");

                return FormatAPIError(HttpStatusCode.NotFound, GatewayErrorSubcode.GatewayTaskNotFound);
            }
            catch (TaskFileConflictException ex)
            {
                _logger.LogError(ex, "File already exists");
                return FormatAPIError(HttpStatusCode.Conflict, GatewayErrorSubcode.GatewayTaskFileConflict);
            }
            catch (FileExtensionUnsupportedException ex)
            {
                _logger.LogError(ex, "File extension not supported");
                return FormatAPIError(HttpStatusCode.UnsupportedMediaType, GatewayErrorSubcode.GatewayMediaTypeNotValid);
            }

            return Ok();
        }
    }
}
