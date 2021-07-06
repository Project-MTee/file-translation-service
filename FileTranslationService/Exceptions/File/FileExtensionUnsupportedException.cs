using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Exceptions.File
{
    public class FileExtensionUnsupportedException : Exception
    {
        public FileExtensionUnsupportedException(string fileExtension): base($"File extension unsupported: {fileExtension}")
        {

        }
    }
}
