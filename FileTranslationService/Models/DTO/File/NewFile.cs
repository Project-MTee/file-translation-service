using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.DTO.File
{
    public class NewFile
    {
        public FileCategory Type { get; set; }
        public long Size { get; set; }
    }
}
