using Newtonsoft.Json;

namespace Skyclient.JsonParts
{
    public class McModInfo
    {
        [JsonProperty("modid")]
        public string? ModID { get; set; }
    }
}
