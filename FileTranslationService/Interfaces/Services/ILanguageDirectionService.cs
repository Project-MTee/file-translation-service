using System.Threading.Tasks;
using Tilde.MT.FileTranslationService.Exceptions.LanguageDirection;

namespace Tilde.MT.FileTranslationService.Interfaces.Services
{
    public interface ILanguageDirectionService
    {
        /// <summary>
        /// Check if language direction is available
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="sourceLanguage"></param>
        /// <param name="targetLanguage"></param>
        /// <returns></returns>
        /// <exception cref="LanguageDirectionReadException">Failed to load language directions</exception>
        /// <exception cref="LanguageDirectionNotFoundException">Language direction not found</exception>
        Task Validate(string domain, string sourceLanguage, string targetLanguage);
    }
}
