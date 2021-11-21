using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Exceptions.LanguageDirection;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Facades;
using Tilde.MT.FileTranslationService.Models.DTO.Task;
using Tilde.MT.FileTranslationService.Models.Errors;
using Tilde.MT.FileTranslationService.Services;

namespace Tilde.MT.FileTranslationService.Controllers
{
    /// <summary>
    /// File translation manipulation [start, stop, update]
    /// </summary>
    [ApiController]
    [Route("file")]
    public class TaskController : BaseController
    {
        private readonly FileTranslationFacade _fileTranslationFacade;
        private readonly LanguageDirectionService _languageDirectionService;
        private readonly ILogger<TaskController> _logger;
        private readonly TaskTranslationService _taskTranslationService;

        public TaskController(
            TaskTranslationService taskTranslationService,
            FileTranslationFacade fileTranslationFacade,
            LanguageDirectionService languageDirectionService,
            ILogger<TaskController> logger
        )
        {
            _fileTranslationFacade = fileTranslationFacade;
            _languageDirectionService = languageDirectionService;
            _taskTranslationService = taskTranslationService;
            _logger = logger;
        }

        /// <summary>
        /// Details of the file translation task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{task}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "", Type = typeof(Models.DTO.Task.Task))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult<Models.DTO.Task.Task>> GetAsync(Guid task)
        {
            try
            {
                var result = await _fileTranslationFacade.GetTask(task);
                return Ok(result);
            }
            catch (TaskNotFoundException ex)
            {
                _logger.LogError(ex, "Task not found");

                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }
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
            try
            {
                await _languageDirectionService.Validate(createTask.Domain, createTask.SourceLanguage, createTask.TargetLanguage);
            }
            catch (LanguageDirectionNotFoundException ex)
            {
                _logger.LogError(ex, "Language direction not found");

                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayLanguageDirectionNotFound);
            }
            catch (LanguageDirectionReadException ex)
            {
                _logger.LogError(ex, "Failed to load language directions");

                return FormatAPIError(HttpStatusCode.InternalServerError, ErrorSubCode.GatewayLanguageDirectionGeneric);
            }

            try
            {
                var result = await _fileTranslationFacade.AddTask(createTask);

                await _taskTranslationService.Send(result.Id);

                return Ok(result);
            }
            catch (FileExtensionUnsupportedException ex)
            {
                _logger.LogError(ex, $"File extension is not supported");

                return FormatAPIError(HttpStatusCode.UnsupportedMediaType, ErrorSubCode.GatewayMediaTypeNotValid);
            }
            catch (FileConflictException ex)
            {
                _logger.LogError(ex, $"File already exists");

                return FormatAPIError(HttpStatusCode.Conflict, ErrorSubCode.GatewayTaskFileConflict);
            }
        }

        /// <summary>
        /// Update file translation info
        /// </summary>
        /// <param name="task"></param>
        /// <param name="editTask"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = AuthenticationScheme.FileTranslationWorkflow)]
        [HttpPut]
        [Route("{task}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "", Type = typeof(Models.DTO.Task.Task))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Task is not found", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Missing or incorrect parameters", Type = typeof(APIError))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal translation error occured", Type = typeof(APIError))]
        public async Task<ActionResult> Update(Guid task, TaskUpdate editTask)
        {
            try
            {
                var updatedTask = await _fileTranslationFacade.UpdateTask(task, editTask);

                return Ok(updatedTask);
            }
            catch (TaskNotFoundException ex)
            {
                _logger.LogError(ex, "Task not found");

                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }
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
            try
            {
                await _fileTranslationFacade.RemoveTask(task);

                return Ok();
            }
            catch (TaskNotFoundException ex)
            {
                _logger.LogError(ex, "Task not found");

                return FormatAPIError(HttpStatusCode.NotFound, ErrorSubCode.GatewayTaskNotFound);
            }
        }
    }
}
