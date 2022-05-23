using System.Text.Json.Serialization;

namespace Tilde.MT.FileTranslationService.Models.RabbitMQ
{
    public record FileTranslationRequest
    {
        [JsonPropertyName("task")]
        public string Task { get; init; }
    }
}
