using System;

namespace Tilde.MT.FileTranslationService.Exceptions.File
{
    /// <summary>
    /// This should normally not happen, 
    /// if translation failed, user can initiate new file translation
    /// This prevents overriding same file 
    /// </summary>
    public class FileConflictException:Exception
    {
        public FileConflictException(string filePath):base($"Cannot add duplicate file: {filePath}")
        {

        }
    }
}
