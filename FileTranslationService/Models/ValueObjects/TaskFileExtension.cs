using System.Text.RegularExpressions;
using Tilde.MT.FileTranslationService.Exceptions.File;

namespace Tilde.MT.FileTranslationService.Models.ValueObjects
{
    public record TaskFileExtension
    {
        private readonly Regex validExtensionTest = new(@"^[\.a-zA-Z0-9]+$");
        public string Value { get; }

        public TaskFileExtension(string extension)
        {
            if (validExtensionTest.IsMatch(extension))
            {
                throw new FileExtensionUnsafeException(extension);
            }
            
            Value = extension;
        }
    }
}
