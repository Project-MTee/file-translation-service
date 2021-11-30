using System;

namespace Tilde.MT.FileTranslationService.Models.Configuration.Services
{
    public record TranslationSystem
    {
        public string Url { get; init; }
        public TimeSpan CacheTTL { get; init; }
    }
}
