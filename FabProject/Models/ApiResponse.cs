using System.Text.Json.Serialization;

namespace FabProject.Models
{
    public class ApiResponse
    {
        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<Result>? Results { get; set; }
    }
}
