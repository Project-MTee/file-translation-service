using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Tilde.MT.FileTranslationService.Models.Errors;

namespace FileTranslationService.Tests.Extensions
{
    public static class ValidationExtensions
    {
        public static void ValidateAPIErrorResult(this ActionResult result, int statusCode)
        {
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            var apiError = objectResult.Value.Should().BeOfType<APIError>().Subject;
            apiError.Error.Should().NotBeNull();
            apiError.Error.Message.Should().NotBeNullOrEmpty();
            apiError.Error.Code.Should().Be(statusCode);
        }

        public static void ValidateAPIErrorResult<Tresult>(this ActionResult<Tresult> result, int statusCode)
        {
            var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            var apiError = objectResult.Value.Should().BeOfType<APIError>().Subject;
            apiError.Error.Should().NotBeNull();
            apiError.Error.Message.Should().NotBeNullOrEmpty();
            apiError.Error.Code.Should().Be(statusCode);
        }
    }
}
