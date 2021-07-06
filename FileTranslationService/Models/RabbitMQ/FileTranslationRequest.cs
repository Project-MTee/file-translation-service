using Newtonsoft.Json;

namespace Tilde.MT.FileTranslationService.Models.RabbitMQ
{
    public class FileTranslationRequest
    {
        [JsonProperty("task")]
        public string Task { get; set; }
    }
}
