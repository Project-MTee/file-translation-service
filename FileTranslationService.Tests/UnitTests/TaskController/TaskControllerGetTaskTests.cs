using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Facades;
using Tilde.MT.FileTranslationService.Interfaces.Services;
using Tilde.MT.FileTranslationService.Models.Errors;
using Xunit;

namespace FileTranslationService.Tests.UnitTests.TaskController
{
    public class TaskControllerGetTaskTests
    {
        [Fact]
        public async Task TaskIsReturned_WhenTaskExists()
        {
            // --- Arrange
            var getTaskId = Guid.NewGuid();
            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.GetTask(It.IsAny<Guid>()))
                .Returns((Guid task) =>
                {
                    return Task.FromResult(new Tilde.MT.FileTranslationService.Models.DTO.Task.Task()
                    {
                        Id = task
                    });
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                Mock.Of<ITaskTranslationService>(),
                fileTranslationFacade.Object,
                Mock.Of<ILanguageDirectionService>(),
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.GetTask(getTaskId);

            // --- Assert

            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var taskResult = okResult.Value.Should().BeOfType<Tilde.MT.FileTranslationService.Models.DTO.Task.Task>().Subject;
            taskResult.Id.Should().Be(getTaskId);
        }


        [Fact]
        public async Task ErrorIsReturned_WhenTaskDoesNotExist()
        {
            // --- Arrange
            var getTaskId = Guid.NewGuid();
            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.GetTask(It.IsAny<Guid>()))
                .Returns((Guid task) =>
                {
                    throw new TaskNotFoundException(task);
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.TaskController(
                Mock.Of<ITaskTranslationService>(),
                fileTranslationFacade.Object,
                Mock.Of<ILanguageDirectionService>(),
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.TaskController>>()
            );

            // --- Act

            var result = await controller.GetTask(getTaskId);

            // --- Assert

            var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            var apiResult = objectResult.Value.Should().BeOfType<APIError>().Subject;
            apiResult.Error.Should().NotBeNull();
            apiResult.Error.Message.Should().NotBeNullOrEmpty();
            apiResult.Error.Code.Should().Be(404002);
        }
    }
}
