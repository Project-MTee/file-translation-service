using FileTranslationService.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Enums;
using Tilde.MT.FileTranslationService.Exceptions.File;
using Tilde.MT.FileTranslationService.Facades;
using Tilde.MT.FileTranslationService.Models.ValueObjects;
using Xunit;

namespace FileTranslationService.Tests.UnitTests.FileController
{
    public class FileControllerGetFileTests
    {
        [Fact]
        public async Task FileIsReturned_WhenTranslatedFileIsRequested()
        {
            // --- Arrange

            string storageFilePath = "";

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.GetFile(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns((Guid task, Guid file) =>
                {
                    return Task.FromResult(new Tilde.MT.FileTranslationService.Models.DTO.File.File()
                    {
                        Id = file,
                        Category = FileCategory.Translated,
                        Extension = ".docx",
                        Size = 7789
                    });
                });

            fileTranslationFacade
                .Setup(m => m.GetFileStoragePath(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<TaskFileExtension>()))
                .Returns((Guid task, FileCategory category, TaskFileExtension extension) =>
                {
                    storageFilePath = $"/tmp/test/{task}/{category}{extension.Value}";
                    return storageFilePath;
                });

            var contentTypeProvider = new Mock<IContentTypeProvider>();
            var outString = "application/octet-stream";
            contentTypeProvider
                .Setup(foo => foo.TryGetContentType(storageFilePath, out outString))
                .Returns(true);

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider.Object
            );

            // --- Act

            var result = await controller.GetFile(Guid.NewGuid(), Guid.NewGuid());

            // --- Assert

            var fileResult = result.Should().BeOfType<PhysicalFileResult>().Subject;
            fileResult.FileName.Should().Be(storageFilePath);
        }

        [Fact]
        public async Task ErrorReturned_WhenTaskFileIsNotFound()
        {
            // --- Arrange

            string storageFilePath = "";

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.GetFile(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns((Guid task, Guid file) =>
                {
                    throw new TaskFileNotFoundException(task, file);
                });

            fileTranslationFacade
                .Setup(m => m.GetFileStoragePath(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<TaskFileExtension>()))
                .Returns((Guid task, FileCategory category, TaskFileExtension extension) =>
                {
                    storageFilePath = $"/tmp/test/{task}/{category}{extension.Value}";
                    return storageFilePath;
                });

            var contentTypeProvider = new Mock<IContentTypeProvider>();
            var outString = "application/octet-stream";
            contentTypeProvider
                .Setup(foo => foo.TryGetContentType(storageFilePath, out outString))
                .Returns(true);

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider.Object
            );

            // --- Act

            var result = await controller.GetFile(Guid.NewGuid(), Guid.NewGuid());

            // --- Assert

            result.ValidateAPIErrorResult(404003);
        }

        [Fact]
        public async Task ErrorReturned_WhenTaskSourceFileIsRequestedByAnonymousUser()
        {
            // --- Arrange

            string storageFilePath = "";

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.GetFile(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns((Guid task, Guid file) =>
                {
                    return Task.FromResult(new Tilde.MT.FileTranslationService.Models.DTO.File.File()
                    {
                        Id = file,
                        Category = FileCategory.Source,
                        Extension = ".docx",
                        Size = 7789
                    });
                });

            fileTranslationFacade
                .Setup(m => m.GetFileStoragePath(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<TaskFileExtension>()))
                .Returns((Guid task, FileCategory category, TaskFileExtension extension) =>
                {
                    storageFilePath = $"/tmp/test/{task}/{category}{extension.Value}";
                    return storageFilePath;
                });

            var contentTypeProvider = new Mock<IContentTypeProvider>();
            var outString = "application/octet-stream";
            contentTypeProvider
                .Setup(foo => foo.TryGetContentType(storageFilePath, out outString))
                .Returns(true);

            var userIdentity = new Mock<ClaimsIdentity>();
            userIdentity
                .Setup(m => m.IsAuthenticated)
                .Returns(false);

            var claimsPrincipal = new ClaimsPrincipal(userIdentity.Object);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider.Object
            )
            {
                ControllerContext = context
            };

            // --- Act

            var result = await controller.GetFile(Guid.NewGuid(), Guid.NewGuid());

            // --- Assert

            result.ValidateAPIErrorResult(403005);
        }

        [Fact]
        public async Task FileIsReturned_WhenTaskSourceFileIsRequestedByFileTranslationWorkflow()
        {
            // --- Arrange

            string storageFilePath = "";

            var fileTranslationFacade = new Mock<IFileTranslationFacade>();
            fileTranslationFacade
                .Setup(m => m.GetFile(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns((Guid task, Guid file) =>
                {
                    return Task.FromResult(new Tilde.MT.FileTranslationService.Models.DTO.File.File()
                    {
                        Id = file,
                        Category = FileCategory.Source,
                        Extension = ".docx",
                        Size = 7789
                    });
                });

            fileTranslationFacade
                .Setup(m => m.GetFileStoragePath(It.IsAny<Guid>(), It.IsAny<FileCategory>(), It.IsAny<TaskFileExtension>()))
                .Returns((Guid task, FileCategory category, TaskFileExtension extension) =>
                {
                    storageFilePath = $"/tmp/test/{task}/{category}{extension.Value}";
                    return storageFilePath;
                });

            var contentTypeProvider = new Mock<IContentTypeProvider>();
            var outString = "application/octet-stream";
            contentTypeProvider
                .Setup(foo => foo.TryGetContentType(storageFilePath, out outString))
                .Returns(true);

            var userIdentity = new Mock<ClaimsIdentity>();
            userIdentity
                .Setup(m => m.IsAuthenticated)
                .Returns(true);

            var claimsPrincipal = new ClaimsPrincipal(userIdentity.Object);
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            var controller = new Tilde.MT.FileTranslationService.Controllers.FileController(
                fileTranslationFacade.Object,
                Mock.Of<ILogger<Tilde.MT.FileTranslationService.Controllers.FileController>>(),
                contentTypeProvider.Object
            )
            {
                ControllerContext = context
            };

            // --- Act

            var result = await controller.GetFile(Guid.NewGuid(), Guid.NewGuid());

            // --- Assert

            var fileResult = result.Should().BeOfType<PhysicalFileResult>().Subject;
            fileResult.FileName.Should().Be(storageFilePath);
        }
    }
}
