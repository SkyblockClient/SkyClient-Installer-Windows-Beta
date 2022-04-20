using Skyclient.JsonParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Skyclient.Utilities
{
    public class ZipUtils
    {

        public static string? GetModIdFromJar(string jar)
        {
            try
            {
                using (ZipArchive zip = ZipFile.Open(jar, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.Name == "mcmod.info")
                        {
                            StreamReader reader = new StreamReader(entry.Open());
                            var text = reader.ReadToEnd();

                            var mcmodinfo = JsonConvert.DeserializeObject<McModInfo[]>(text);
                            if (mcmodinfo == null || mcmodinfo.Length == 0)
                                return null;

                            return mcmodinfo[0].ModID;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to extract " + jar);
            }
            return null;
        }
    }
}
