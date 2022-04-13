using FileTranslationService.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Exceptions.LanguageDirection;
using Tilde.MT.FileTranslationService.Interfaces.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Xunit;

namespace FileTranslationService.Tests.UnitTests.TaskController
{
    public class TaskControllerCreateTaskTests
    {
        private readonly IFileTranslationFacade normalFileTranslationFacade;
        private readonly Tilde.MT.FileTranslationService.Models.DTO.Task.NewTask newTask;
        private readonly ILanguageDirectionService normalLanguageDirectionService;

        public TaskControllerCreateTaskTests()
        {
            newTask = new Tilde.MT.FileTranslationService.Models.DTO.Task.NewTask()
            {
                Domain = "finance",
                File = Mock.Of<IFormFile>(),
                SourceLanguage = "en",
                TargetLanguage = "ru"
            };

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
                        TargetLanguage = task.TargetLanguage
                    });
                });
            normalFileTranslationFacade = fileTranslationFacade.Object;

            var languageDirectionService = new Mock<ILanguageDirectionService>();
            languageDirectionService
                .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            normalLanguageDirectionService = languageDirectionService.Object;
        }
        [Fact]
        public async Task TaskIsReturned_WhenTaskExists()
        {
            // --- Arrange

            var taskTranslationService = new Mock<ITaskTranslationService>();
            taskTranslationService
                .Setup(m => m.Send(It.IsAny<Guid>()));

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                taskTranslationService.Object,
                normalFileTranslationFacade,
                normalLanguageDirectionService,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.CreateTask(newTask);

            // --- Assert

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var taskResult = okResult.Value.Should().BeOfType<Tilde.MT.FileTranslationService.Models.DTO.Task.Task>().Subject;
            taskResult.Should().BeEquivalentTo(newTask, options =>
            {
                return options.Excluding(item => item.File);
            });
        }

        [Fact]
        public async Task ErrorIsReturned_WhenLanguageDirectionNotFound()
        {
            // --- Arrange

            var languageDirectionService = new Mock<ILanguageDirectionService>();
            languageDirectionService
                .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string domain, string sourceLanguage, string targetLanguage) =>
                {
                    throw new LanguageDirectionNotFoundException(domain, sourceLanguage, targetLanguage);
                });

            var taskTranslationService = new Mock<ITaskTranslationService>();
            taskTranslationService
                .Setup(m => m.Send(It.IsAny<Guid>()));

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                taskTranslationService.Object,
                normalFileTranslationFacade,
                languageDirectionService.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.CreateTask(newTask);

            // --- Assert

            result.ValidateAPIErrorResult(404007);
        }

        [Fact]
        public async Task ErrorIsReturned_WhenLanguageDirectionReadError()
        {
            // --- Arrange

            var languageDirectionService = new Mock<ILanguageDirectionService>();
            languageDirectionService
                .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string domain, string sourceLanguage, string targetLanguage) =>
                {
                    throw new LanguageDirectionReadException();
                });

            var taskTranslationService = new Mock<ITaskTranslationService>();
            taskTranslationService
                .Setup(m => m.Send(It.IsAny<Guid>()));

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                taskTranslationService.Object,
                normalFileTranslationFacade,
                languageDirectionService.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.CreateTask(newTask);

            // --- Assert

            result.ValidateAPIErrorResult(500006);
        }

        [Fact]
        public async Task ErrorIsReturned_WhenFileExtensionUnsupported()
        {
            // --- Arrange

            var taskTranslationService = new Mock<ITaskTranslationService>();
            taskTranslationService
                .Setup(m => m.Send(It.IsAny<Guid>()));

            var languageDirectionService = new Mock<ILanguageDirectionService>();
            languageDirectionService
                .Setup(m => m.Validate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.AddTask(It.IsAny<Tilde.MT.FileTranslationService.Models.DTO.Task.NewTask>()))
                .Returns((Tilde.MT.FileTranslationService.Models.DTO.Task.NewTask task) =>
                {
                    throw new FileExtensionUnsupportedException(".exe");
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                taskTranslationService.Object,
                fileTranslationFacade.Object,
                normalLanguageDirectionService,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.CreateTask(newTask);

            // --- Assert

            result.ValidateAPIErrorResult(415010);
        }
    }
}
