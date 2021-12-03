using System;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Interfaces.Services
{
    public interface ITaskTranslationService
    {
        /// <summary>
        /// Send translation job to translation worker
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task Send(Guid task);
    }
}
