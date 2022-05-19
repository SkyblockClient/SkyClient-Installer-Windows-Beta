using Newtonsoft.Json;
using Skyclient.JsonParts;
using Skyclient.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyclient
{
    public class Repository
    {
        public static Repository Instance { get; private set; }

        public bool IsFirstInstall = false;

        public static void Initialize() => Instance = new Repository();

        public Mod[] Mods { get; private set; } = new Mod[0];
        public Pack[] Packs { get; private set; } = new Pack[0]; // packwatch


        // this could be a list
        private Dictionary<int, DownloadableFile> DownloadQueue = new Dictionary<int, DownloadableFile>();
        // will be null when no files are downloading
        private DownloadableFile? CurrentlyDownloadingFile;

        private async void Download()
        {
            if (CurrentlyDownloadingFile is not null)
                return;

            var queuesize = GetDownloadQueueSize();
            if (queuesize == 0)
                return;

            DownloadableFile file = DownloadQueue[DownloadQueue.Keys.Last()];

            CurrentlyDownloadingFile = file;
            RemoveFromDownloadQueue(file, false);

            var temppath = await RepoUtils.DownloadTempFile(file);
            if (temppath is not null)
            {
                RepoUtils.MoveFile(temppath, file.FileDestination);
            }


            CurrentlyDownloadingFile = null;

            if (queuesize - 1 != 0)
            {
                Console.WriteLine("Items left: " + (queuesize - 1));
            }
            else
            {
                if (App.Close)
                {
                    App.Current.Shutdown();
                }
            }

            // this might cause problems
            // make it an event
            Download();
        }

        public int GetDownloadQueueSize() { return DownloadQueue.Count; }

        public void RemoveFile(DownloadableFile file)
        {
            if (File.Exists(file.FileDestination))
                RepoUtils.MoveFile(file.FileDestination, RepoUtils.GetTempFilePath(file));
        }

        public void RemoveFromDownloadQueue(DownloadableFile file, bool setdownloadingfile = true)
        {
            DownloadQueue.Remove(file.Guid);
            if (setdownloadingfile && CurrentlyDownloadingFile?.Guid == file.Guid)
            {
                CurrentlyDownloadingFile.CancelDownload = true;
                //CurrentlyDownloadingFile = null;
            }
        }

        public void AddToDownloadQueue(DownloadableFile file)
        {
            if (!DownloadQueue.ContainsKey(file.Guid))
            {
                var tmp = RepoUtils.GetTempFilePath(file);
                if (File.Exists(tmp))
                {
                    RepoUtils.MoveFile(tmp, file.FileDestination);
                    return;
                }

                DownloadQueue.Add(file.Guid, file);
                Download();
            }
        }

        // "local" as in "in the mods folder"
        // file name, forge id
        public Dictionary<string, string?> LocalModsAndTheirForgeIDs = new Dictionary<string, string?>();
        // file name, repo id
        public Dictionary<string, string> LocalModsAndTheirRepoIDs = new Dictionary<string, string>();
        // file name, repo id
        public Dictionary<string, string> LocalPacksAndTheirRepoIDs = new Dictionary<string, string>();

        // forge id, list of files
        // mods with no forge id (null) don't get added
        public Dictionary<string, List<string>> DuplicateMods = new Dictionary<string, List<string>>();

        private Repository()
        {
            var modsjson = RepoUtils.DownloadRepoFileString("mods.json");
            var packsjson = RepoUtils.DownloadRepoFileString("packs.json");

            Console.WriteLine("Deserializing...");

            List<Mod>? mods = JsonConvert.DeserializeObject<Mod[]>(modsjson)
                .Where(m => m.Display != "no")
                .ToList();
            List<Pack>? packs = JsonConvert.DeserializeObject<List<Pack>>(packsjson)
                .ToList();

            Mods = mods.ToArray();
            Packs = packs.ToArray();
        }

        public void CheckSelectedItems()
        {
            var userhome = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var scmodsfolder = Path.Combine(userhome, ".minecraft", "skyclient", "mods");
            var scpackssfolder = Path.Combine(userhome, ".minecraft", "skyclient", "resourcepacks");

            if (Directory.Exists(scmodsfolder))
            {
                Console.WriteLine("Checking for local mods...");
                var files = Directory.GetFiles(Path.Combine(userhome, ".minecraft", "skyclient", "mods"), "*.jar");
                if (files.Length == 0)
                {
                    Console.WriteLine("No mods found... Skipping.");
                    foreach (var mod in Mods)
                    {
                        mod.LocalFiles = new string[] { mod.File };
                        if (IsFirstInstall)
                        {
                            if (mod.Enabled)
                            {
                                RepoUtils.AddRepoItemToQueue(mod);
                            }
                        }
                        else
                        {
                            mod.Enabled = false;
                        }
                    }

                }
                else
                {
                    foreach (var file in files)
                    {
                        var filename = Path.GetFileName(file);
                        var modid = ZipUtils.GetModIdFromJar(file);
                        LocalModsAndTheirForgeIDs.Add(filename, modid);
                    }


                    // gets every mod with the same forge mod id
                    var dupemodforgeids = LocalModsAndTheirForgeIDs
                        .GroupBy(x => x.Value)
                        .Where(x => x.Count() > 1)
                        .ToList();

                    foreach (var dupemodforgeid in dupemodforgeids)
                    {
                        if (dupemodforgeid.Key is null)
                            continue;

                        // this code is horrendous 
                        // initialize list with forge id
                        DuplicateMods.Add(dupemodforgeid.Key, new List<string>());
                        // iterate through known mods
                        foreach (var localmod in LocalModsAndTheirForgeIDs)
                        {
                            // localmod.Value is forge id, dupemodforgeid.Key is forge id 
                            if (localmod.Value == dupemodforgeid.Key)
                            {
                                // add file name to dupemod list
                                DuplicateMods[dupemodforgeid.Key].Add(localmod.Key);
                            }
                        }
                    }

                    var checkedlocals = new List<string>();
                    var checkedrepoids = new List<string>();

                    // detect repo from forge id
                    foreach (var localmod in LocalModsAndTheirForgeIDs.Where(x => x.Value is not null))
                    {
                        var repomod = GetModByForgeID(localmod.Value);

                        if (repomod is null)
                            continue;

                        checkedlocals.Add(localmod.Key);
                        checkedrepoids.Add(repomod.Id);
                        LocalModsAndTheirRepoIDs.Add(localmod.Key, repomod.Id);
                    }

                    // file name based lookup
                    foreach (var localmod in LocalModsAndTheirForgeIDs.Where(x => !checkedlocals.Contains(x.Key)))
                    {
                        foreach (var repomod in Mods.Where(x => !checkedrepoids.Contains(x.Id)))
                        {
                            if (CompareFileName(localmod.Key, repomod.File))
                            {
                                checkedlocals.Add(localmod.Key);
                                checkedrepoids.Add(repomod.Id);
                                LocalModsAndTheirRepoIDs.Add(localmod.Key, repomod.Id);

                                break;
                            }
                        }
                    }

                    List<string> probablyInstalledBundles = new List<string>();
                    List<string> ignoredBecauseInBundle = new List<string>();
                    foreach (var bundle in Mods.Where(x => x.Packages.Length != 0))
                    {
                        bool printedheader = false;
                        foreach (var localmod in LocalModsAndTheirRepoIDs.Where(x => bundle.Packages.Contains(x.Value)))
                        {
                            if (!printedheader)
                            {
                                printedheader = true;
                                probablyInstalledBundles.Add(bundle.Id);
                                // Console.WriteLine(bundle.Display);
                                // Console.WriteLine(" - " + bundle.File);
                            }
                            ignoredBecauseInBundle.Add(localmod.Value);
                            // Console.WriteLine(" - " + localmod.Key);
                        }
                    }

                    foreach (var repomod in Mods)
                    {
                        if (ignoredBecauseInBundle.Contains(repomod.Id))
                            continue;

                        var enable = false;
                        if (probablyInstalledBundles.Contains(repomod.Id))
                        {
                            enable = true;
                        }
                        else if (LocalModsAndTheirRepoIDs.Values.Contains(repomod.Id))
                        {
                            enable = true;
                        }

                        repomod.Enabled = enable;
                    }

                    // this code can and should probably be included earlier in the code
                    // sets every mod to use localfiles
                    // if mod is not installed, set localfiles to file name
                    foreach (var mod in Mods)
                    {
                        mod.LocalFiles = new string[] { mod.File };
                    }

                    foreach (var localmod in LocalModsAndTheirRepoIDs)
                    {
                        if (localmod.Value is null)
                            continue;
                        var repomod = Repository.Instance.GetModByRepoID(localmod.Value);
                        if (repomod is null)
                            continue;
                        repomod.LocalFiles = new string[] { localmod.Key };
                    }

                    foreach (var dm in DuplicateMods)
                    {
                        var dmId = dm.Key;
                        var dmFiles = dm.Value;
                        var dmMod = Repository.Instance.GetModByForgeID(dmId);
                        if (dmMod is null)
                            continue;
                        dmMod.LocalFiles = dmFiles.ToArray();
                    }
                }
            }

            if (Directory.Exists(scpackssfolder))
            {
                Console.WriteLine("Checking for local packs...");
                var files = Directory.GetFiles(Path.Combine(userhome, ".minecraft", "skyclient", "resourcepacks"), "*.zip");
                Console.WriteLine(files.Length);
                if (files.Length == 0)
                {
                    Console.WriteLine("No packs found... Skipping.");

                    foreach (var pack in Packs)
                    {
                        pack.LocalFiles = new string[] { pack.File };
                        if (IsFirstInstall)
                        {
                            if (pack.Enabled)
                            {
                                RepoUtils.AddRepoItemToQueue(pack);
                            }
                        }
                        else
                        {
                            pack.Enabled = false;
                        }
                    }

                } 
                else
                {
                    // file name based lookup
                    foreach (var localpack in files)
                    {
                        var localfilename = Path.GetFileName(localpack);
                        var mightbes = new List<MightBeLookup>();

                        const int MAX_PERMISSIBLE_DIFF = 6;

                        foreach (var repopack in Packs)
                        {
                            var diff = StringUtils.CalcLevenshteinDistance(localfilename, repopack.File);
                            if (diff <= MAX_PERMISSIBLE_DIFF)
                            {
                                mightbes.Add(new MightBeLookup(repopack, diff));
                            }
                        }

                        switch (mightbes.Count())
                        {
                            case 0:
                                break;
                            case 1:
                                LocalPacksAndTheirRepoIDs.Add(localfilename, mightbes[0].Repopack.Id);
                                break;
                            default:
                                var ordered = mightbes.OrderBy(x => x.Difference).ToArray();
                                LocalPacksAndTheirRepoIDs.Add(localfilename, ordered[0].Repopack.Id);
                                break;
                        }
                    }

                    foreach (var repopack in Packs)
                    {
                        var enable = false;
                        if (LocalPacksAndTheirRepoIDs.Values.Contains(repopack.Id))
                        {
                            enable = true;
                        }
                        repopack.Enabled = enable;
                    }

                }
            }
        }

        //public record MightBeLookup(Pack Repopack, int Difference);

        public class MightBeLookup
        {
            public Pack Repopack;
            public int Difference;

            public MightBeLookup(Pack Repopack, int Difference)
            {
                this.Repopack = Repopack;
                this.Difference = Difference;
            }
        }

        private bool CompareFileName(string localfile, string repofile, int distance = 6)
        {
            return StringUtils.CalcLevenshteinDistance(repofile, localfile) <= distance;
        }
        public Mod? GetModByForgeID(string id)
        {
            return Mods.Where(mod => mod.ForgeId == id).FirstOrDefault();
        }

        public Mod? GetModByRepoID(string query)
        {
            return Mods.Where(mod => mod.Id == query).FirstOrDefault();
        }
        public Pack? GetPackByRepoID(string query)
        {
            return Packs.Where(pack => pack.Id == query).FirstOrDefault();
        }

        public Mod[] GetVisibleMods()
        {
            return Mods.Where(mod => !mod.Hidden).ToArray();
        }
        public Pack[] GetVisiblePacks()
        {
            return Packs.Where(pack => !pack.Hidden).ToArray();
        }
    }
}
