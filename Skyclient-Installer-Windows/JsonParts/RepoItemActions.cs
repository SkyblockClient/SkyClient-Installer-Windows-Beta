using Newtonsoft.Json;
using Skyclient.Utilities;
using System.ComponentModel;

namespace Skyclient.JsonParts
{
    public class RepoItemAction
    {

        [JsonProperty("icon"), DefaultValue("invalid.png")]
        public string IconName
        {
            set => _icon = value;
            get
            {
                if (_icon != null && _icon != "" && _icon != "invalid.png")
                    return _icon;
                if (Text != null && Text != "")
                {
                    switch (Text)
                    {
                        case "Guide": return "guide.png";
                        case "Forum": return "forum.png";
                        case "Github": return "github.png";
                        case "Curseforge": return "curseforge.png";
                    }
                }
                return "invalid.png";
            }
        }
        [JsonIgnore]
        private string _icon;

        [JsonProperty("text")]
        public string? Text { get; set; }

        [JsonProperty("creator")]
        public string? Creator { get; set; }

        [JsonProperty("link")]
        public string? Link { get; set; }

        [JsonProperty("document")]
        public string? Document { get; set; }

        [JsonProperty("method"), DefaultValue("click")]
        public string Method { get; set; } = "click";
    }
}
