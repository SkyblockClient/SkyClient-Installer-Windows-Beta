using Newtonsoft.Json;

namespace Skyclient.JsonParts
{
    public class CommitsAPI
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }
    }
}
