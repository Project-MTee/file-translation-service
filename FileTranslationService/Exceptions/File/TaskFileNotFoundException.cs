using System;

namespace Tilde.MT.FileTranslationService.Exceptions.File
{
    public class TaskFileNotFoundException: Exception
    {
        public TaskFileNotFoundException(Guid task, Guid file) : base($"File '{file}' for tas '{task}' cannot be found")
        {

        }
    }
}
