using System;

namespace Tilde.MT.FileTranslationService.Exceptions.File
{
    public class FileExtensionUnsafeException : Exception
    {
        public FileExtensionUnsafeException(string fileExtension) : base($"File extension '{fileExtension}' is not safe")
        {

        }
    }
}
