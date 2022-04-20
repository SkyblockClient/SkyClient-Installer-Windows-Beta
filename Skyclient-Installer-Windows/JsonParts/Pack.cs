using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyclient.JsonParts
{
    public class Pack : RepoItem
    {
        public override string LocalFolderName => "resourcepacks";
        public override string RepoFolderName => "packs";
    }
}
