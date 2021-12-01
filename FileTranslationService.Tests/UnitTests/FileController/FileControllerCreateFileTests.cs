using FileTranslationService.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Exceptions.Task;
using Tilde.MT.FileTranslationService.Facades;
using Xunit;

namespace FileTranslationService.Tests.UnitTests.FileController
{
    public class FileControllerCreateFileTests
    {
        private readonly IContentTypeProvider contentTypeProvider;
        public FileControllerCreateFileTests()
        {
            var contentTypeProvider = new Mock<IContentTypeProvider>();
            var outString = "application/octet-stream";
            contentTypeProvider
                .Setup(foo => foo.TryGetContentType(It.IsAny<string>(), out outString))
                .Returns(true);
            this.contentTypeProvider = contentTypeProvider.Object;
        }

        [Fact]
        public async Task FileIsReturned_WhenUserInputValid()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.AddFile(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<IFormFile>()));

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider
            );

            // --- Act

            var result = await controller.CreateFile(
                Guid.NewGuid(),
                FileCategory.Translated,
                Mock.Of<IFormFile>()
            );

            // --- Assert

            result.Should().BeOfType<OkResult>();
        }


        [Fact]
        public async Task ErrorIsReturned_WhenTaskDoesNotExist()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.AddFile(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<IFormFile>()))
                .Returns((Guid task, FileCategory category, IFormFile file) =>
                {
                    throw new TaskNotFoundException(task);
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider
            );

            // --- Act

            var result = await controller.CreateFile(
                Guid.NewGuid(),
                FileCategory.Translated,
                Mock.Of<IFormFile>()
            );

            // --- Assert

            result.ValidateAPIErrorResult(404002);
        }

        [Fact]
        public async Task ErrorIsReturned_WhenTaskFileExists()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.AddFile(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<IFormFile>()))
                .Returns((Guid task, FileCategory category, IFormFile file) =>
                {
                    throw new TaskFileConflictException(task, "/some/path");
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider
            );

            // --- Act

            var result = await controller.CreateFile(
                Guid.NewGuid(),
                FileCategory.Translated,
                Mock.Of<IFormFile>()
            );

            // --- Assert

            result.ValidateAPIErrorResult(409004);
        }


        [Fact]
        public async Task ErrorIsReturned_WhenTaskFileExtensionNotSupported()
        {
            // --- Arrange

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.AddFile(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<IFormFile>()))
                .Returns((Guid task, FileCategory category, IFormFile file) =>
                {
                    throw new FileExtensionUnsupportedException(".exe");
                });

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider
            );

            // --- Act

            var result = await controller.CreateFile(
                Guid.NewGuid(),
                FileCategory.Translated,
                Mock.Of<IFormFile>()
            );

            // --- Assert

            result.ValidateAPIErrorResult(415010);
        }
    }
}
