using System.Text.Json.Serialization;

namespace FabProject.Models
{
    public class Result
    {
        [JsonPropertyName("uid")]
        public string? Id { get; set; }
    }
}
