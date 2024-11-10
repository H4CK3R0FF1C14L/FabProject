using System.Text.Json.Serialization;

namespace FabProject.Models
{
    public class Item
    {
        [JsonPropertyName("licenses")]
        public List<License>? Licenses { get; set; }
    }

    public class License
    {
        [JsonPropertyName("offerId")]
        public string? OfferId { get; set; }
    }
}
