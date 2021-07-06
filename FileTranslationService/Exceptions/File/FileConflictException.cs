using System;

namespace Tilde.MT.FileTranslationService.Exceptions.File
{
    public class FileConflictException:Exception
    {
        public FileConflictException(string filePath):base($"File already exists: {filePath}")
        {

        }
    }
}
