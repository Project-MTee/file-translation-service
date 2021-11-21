using System;

namespace Tilde.MT.FileTranslationService.Exceptions.Task
{
    public class TaskNotFoundException: Exception
    {
        public TaskNotFoundException(Guid task): base($"Task '{task}' not found")
        {

        }
    }
}
