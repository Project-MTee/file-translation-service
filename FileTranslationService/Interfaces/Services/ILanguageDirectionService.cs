using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Interfaces.Services
{
    public interface ILanguageDirectionService
    {
        Task Validate(string domain, string sourceLanguage, string targetLanguage);
    }
}
