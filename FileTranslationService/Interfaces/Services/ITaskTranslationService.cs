using System;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Interfaces.Services
{
    public interface ITaskTranslationService
    {
        Task Send(Guid task);
    }
}
