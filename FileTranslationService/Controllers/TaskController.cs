using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Facades;
using Tilde.MT.FileTranslationService.Models;
using Tilde.MT.FileTranslationService.Models.DTO.Task;
using Tilde.MT.FileTranslationService.Services;

namespace Tilde.MT.FileTranslationService.Controllers
{
    /// <summary>
    /// File translation manipulation [start, stop, update]
    /// </summary>
    [ApiController]
    [Route("file")]
    public class TaskController : APIResponseBaseController
    {
        private readonly FileStorageService _fileStorageService;
        private readonly TaskService _metadataService;
        private readonly FileTranslationFacade _fileTranslationFacade;
        private readonly IBus _bus;
        private readonly LanguageDirectionService _languageDirectionService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            FileStorageService fileStorageService,
            TaskService metadataService,
            FileTranslationFacade fileTranslationFacade,
            IBus bus,
            LanguageDirectionService languageDirectionService,
            ILogger<TaskController> logger
        )
        {
            _metadataService = metadataService;
            _fileStorageService = fileStorageService;
            _fileTranslationFacade = fileTranslationFacade;
            _bus = bus;
            _languageDirectionService = languageDirectionService;
            _logger = logger;
        }

        /// <summary>
        /// Details of the file translation task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{task}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description ="", Type = typeof(Models.DTO.Task.Task))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult<Models.DTO.Task.Task>> DetailsAsync(Guid task)
        {
            if (!await _metadataService.Exists(task))
            {
                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }

            var metadata = await _metadataService.Get(task);

            return Ok(metadata);
        }

        /// <summary>
        /// Upload file for translation, creating file translation task
        /// </summary>
        /// <param name="createTask"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(52_428_800)] // 50MB
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "", Type = typeof(Models.DTO.Task.Task))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Language direction not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, Description = "File type is not supported", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult<Models.DTO.Task.Task>> Create([FromForm] NewTask createTask)
        {
            var languageDirections = await _languageDirectionService.Read();

            if (languageDirections == null)
            {
                return FormatAPIError(HttpStatusCode.InternalServerError, ErrorSubCode.GatewayLanguageDirectionGeneric);
            }

            // check if language direction exists.
            var languageDirectionInSettings = languageDirections.Where(item =>
            {
                var languageMatches = item.SourceLanguage == createTask.SourceLanguage &&
                    item.TargetLanguage == createTask.TargetLanguage;

                var domainMatches = string.IsNullOrEmpty(createTask.Domain) || item.Domain == createTask.Domain;

                return domainMatches && languageMatches;
            });

            if (!languageDirectionInSettings.Any())
            {
                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayLanguageDirectionNotFound);
            }

            using var fileStream = createTask.File.OpenReadStream();

            var metadata = await _metadataService.Create(createTask);
            string extension = null;

            try
            {
                extension = await _fileStorageService.Save(
                    metadata.Id,
                    FileCategory.Source,
                    fileStream,
                    createTask.File.FileName
                );
            }
            catch (FileExtensionUnsupportedException ex)
            {
                _logger.LogError(ex, $"File extension: {createTask.File.FileName} is not supported");

                return FormatAPIError(HttpStatusCode.UnsupportedMediaType, ErrorSubCode.GatewayMediaTypeNotValid);
            }

            await _metadataService.AddLinkedFile(metadata.Id, extension, new Models.DTO.File.NewFile()
            {
                Type = FileCategory.Source,
                Size = fileStream.Length
            });

            var endpoint = await _bus.GetSendEndpoint(new Uri("queue:file-translation?durable=true"));
            await endpoint.Send(new Models.RabbitMQ.FileTranslationRequest()
            {
                Task = metadata.Id.ToString()
            });

            return Ok(await _metadataService.Get(metadata.Id));
        }

        /// <summary>
        /// Update file translation info
        /// </summary>
        /// <param name="task"></param>
        /// <param name="editTask"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = AuthenticationSchemeType.FileTranslationWorkflow)]
        [HttpPut]
        [Route("{task}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "", Type = typeof(Models.DTO.Task.Task))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult<Models.DTO.Task.Task>> Update(Guid task, TaskUpdate editTask)
        {
            if (!await _metadataService.Exists(task))
            {
                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }

            var metadata = await _metadataService.Update(task, editTask);

            return Ok(metadata);
        }

        /// <summary>
        /// Delete the file stopping the file translation task if running
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{task}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult> Delete(Guid task)
        {
            if (!await _metadataService.Exists(task))
            {
                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }

            await _fileTranslationFacade.RemoveMetadata(task);

            return Ok();
        }
    }
}
