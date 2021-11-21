using System;

namespace Tilde.MT.FileTranslationService.Exceptions.File
{
    public class FileExtensionUnsupportedException : Exception
    {
        public FileExtensionUnsupportedException(string fileExtension) : base($"File extension '{fileExtension}' is not supported")
        {

        }
    }
}
