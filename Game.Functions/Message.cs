
using Newtonsoft.Json;

namespace Game.Functions
{
    [JsonObject]
    public class Message
    {
        [JsonProperty("message")]
        public string message { get; set; }
    }
}
