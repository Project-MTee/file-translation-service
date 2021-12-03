using Tilde.MT.FileTranslationService.Enums;

namespace Tilde.MT.FileTranslationService.Models.DTO.File
{
    public record NewFile
    {
        public FileCategory Category { get; init; }
        public long Size { get; init; }
    }
}
