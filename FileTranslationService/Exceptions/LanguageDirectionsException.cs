using System;

namespace Tilde.MT.FileTranslationService.Exceptions
{
    public class LanguageDirectionsException : Exception
    {
        public LanguageDirectionsException(string message) : base($"Language Direction exception: {message}")
        {

        }
    }
}
