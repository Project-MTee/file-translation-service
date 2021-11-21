using Newtonsoft.Json;

namespace Tilde.MT.FileTranslationService.Models.RabbitMQ
{
    public record FileTranslationRequest
    {
        [JsonProperty("task")]
        public string Task { get; init; }
    }
}
