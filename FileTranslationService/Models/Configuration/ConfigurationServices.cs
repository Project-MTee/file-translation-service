using Tilde.MT.FileTranslationService.Models.Configuration.Services;

namespace Tilde.MT.FileTranslationService.Models.Configuration
{
    public class ConfigurationServices
    {
        public Services.FileTranslationService FileTranslation { get; set; }
        public DatabaseService Database { get; set; }
        public Services.RabbitMQ RabbitMQ { get; set; }
        public Services.TranslationSystem TranslationSystem { get; set; }
    }
}
