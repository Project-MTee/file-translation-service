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
    public class TaskControllerDeleteTaskTests
    {
        private readonly IFileTranslationFacade normalFileTranslationFacade;
        private readonly ILanguageDirectionService normalLanguageDirectionService;
        private readonly Tilde.MT.FileTranslationService.Models.DTO.Task.TaskUpdate updateTask;

        public TaskControllerDeleteTaskTests()
        {
            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.RemoveTask(It.IsAny<Guid>()));

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

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                Mock.Of<ITaskTranslationService>(),
                normalFileTranslationFacade,
                normalLanguageDirectionService,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.DeleteTask(Guid.NewGuid());

            // --- Assert

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task ErrorIsReturned_WhenTaskNotFound()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.RemoveTask(It.IsAny<Guid>()))
                .Returns((Guid task) =>
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

            var result = await controller.DeleteTask(Guid.NewGuid());

            // --- Assert

            result.ValidateAPIErrorResult(404002);
        }
    }
}
