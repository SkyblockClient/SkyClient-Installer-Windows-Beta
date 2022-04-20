using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyclient.JsonParts
{
    public class Mod : RepoItem
    {
        public override string LocalFolderName => "mods";
        public override string RepoFolderName => "mods";

        [JsonProperty("forge_id")]
        public string ForgeId { get; set; }
    }
}
