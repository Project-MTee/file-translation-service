using System;

namespace Tilde.MT.FileTranslationService.Exceptions.LanguageDirection
{
    public class LanguageDirectionReadException : Exception
    {
        public LanguageDirectionReadException() : base("Lanuage directions cannot be read")
        {

        }
    }
}
