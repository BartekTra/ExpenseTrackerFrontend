using System.Text.Json.Serialization;

namespace Frontend.Models
{
    public class IdentityError
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
} 