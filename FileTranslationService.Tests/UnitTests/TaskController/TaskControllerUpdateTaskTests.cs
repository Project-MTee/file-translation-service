using FileTranslationService.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Xunit;

namespace FileTranslationService.Tests.UnitTests.TaskController
{
    public class TaskControllerUpdateTaskTests
    {
        private readonly IFileTranslationFacade normalFileTranslationFacade;
        private readonly ILanguageDirectionService normalLanguageDirectionService;
        private readonly Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate updateTask;
        public TaskControllerUpdateTaskTests()
        {
            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.AddTask(It.IsAny<Tilde.MT.FileTranslationService.Models.DTO.Task.NewTask>()))
                .Returns((Tilde.MT.FileTranslationService.Models.DTO.Task.NewTask task) =>
                {
                    return Task.FromResult(new Tilde.MT.FileTranslationService.Models.DTO.Task.Task()
                    {
                        Id = Guid.NewGuid(),
                        Domain = task.Domain,
                        SourceLanguage = task.SourceLanguage,
                        TargetLanguage = task.TargetLanguage,
                        FileName = task.File.FileName
                    });
                });
            normalFileTranslationFacade = fileTranslationFacade.Object;

            var languageDirectionService = new Mock<ILanguageDirectionService>();
            languageDirectionService
                .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            normalLanguageDirectionService = languageDirectionService.Object;

            updateTask = new Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate()
            {
                Segments = 30,
                SegmentsTranslated = 19,
                TranslationStatus = Tilde.MT.FileTranslationService.Enums.TranslationStatus.Translating
            };
        }

        [Fact]
        public async Task TaskIsReturned_WhenTaskIsFound()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.UpdateTask(It.IsAny<Guid>(), It.IsAny<Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate>()))
                .Returns((Guid task, Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate editTask) =>
                {
                    return Task.FromResult(new Tilde.MT.FileTranslationService.Models.DTO.Task.Task() with
                    {
                        Id = task,
                        TranslationStatus = editTask.TranslationStatus,
                        Segments = editTask.Segments,
                        SegmentsTranslated = editTask.SegmentsTranslated
                    });
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                Mock.Of<ITaskTranslationService>(),
                fileTranslationFacade.Object,
                normalLanguageDirectionService,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.UpdateTask(Guid.NewGuid(), updateTask);

            // --- Assert

            var objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var taskResult = objectResult.Value.Should().BeOfType<Tilde.MT.FileTranslationService.Models.DTO.Task.Task>().Subject;
            taskResult.Should().BeEquivalentTo(updateTask);
        }

        [Fact]
        public async Task ErrorIsReturned_WhenTaskNotFound()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.UpdateTask(It.IsAny<Guid>(), It.IsAny<Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate>()))
                .Returns((Guid task, Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate editTask) =>
                {
                    throw new TaskNotFoundException(task);
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                Mock.Of<ITaskTranslationService>(),
                fileTranslationFacade.Object,
                normalLanguageDirectionService,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.UpdateTask(Guid.NewGuid(), updateTask);

            // --- Assert
            result.ValidateAPIErrorResult(404002);
        }
    }
}
