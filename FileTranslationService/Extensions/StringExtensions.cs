using System.IO;
using Tilde.MT.FileTranslationService.ValueObjects;

namespace Tilde.MT.FileTranslationService.Extensions
{
    public static class StringExtensions
    {
        public static TaskFileExtension GetTaskFileExtension(this string filename)
        {
            return new TaskFileExtension(Path.GetExtension(filename));
        }
    }
}
