using System;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Services
{
    public interface ITaskTranslationService
    {
        Task Send(Guid task);
    }
}
