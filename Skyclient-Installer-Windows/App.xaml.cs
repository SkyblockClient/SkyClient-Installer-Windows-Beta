using Newtonsoft.Json;
using Skyclient.JsonParts;
using Skyclient.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Skyclient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool Close = false;
        public static string[] StartArgs { get; set; } = new string[0];

        void AppStartup(object sender, StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            StartArgs = e.Args;
            var count = 1;
            foreach (var arg in e.Args)
            {
                Console.Write("ARG" + (count++) + ": ");
                Console.WriteLine(arg);
            }

            Console.WriteLine("Loading Components");
            RepoUtils.Initialize();
            Repository.Initialize();

            // check if first install

            var userhome = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            try
            {
                Directory.CreateDirectory(RepoUtils.SkyclientTempData);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Source);
                Console.WriteLine(ee.Message);
                Console.WriteLine(ee.StackTrace);
            }

            var scmodsfolder =   Path.Combine(RepoUtils.SkyclientDirectory, "mods");
            var scpackssfolder = Path.Combine(RepoUtils.SkyclientDirectory, "resourcepacks");

            var modssize = 0;
            var packssize = 0;

            try
            {
                modssize = Directory.GetFiles(Path.Combine(RepoUtils.SkyclientDirectory, "mods"), "*.jar").Length;
            }
            catch (Exception) { }
            try
            {
                packssize = Directory.GetFiles(Path.Combine(RepoUtils.SkyclientDirectory, "resourcepacks"), "*.zip").Length;
            }
            catch (Exception) { }

            var containsSkyclient = false;
            try
            {
                var launcherProfilesJson = Path.Combine(RepoUtils.DotMinecraftDirectory, "launcher_profiles.json");
                var jsonText = File.ReadAllText(launcherProfilesJson);
                var json = JsonConvert.DeserializeObject<LauncherProfilesFileJson>(jsonText);
                containsSkyclient = json.profiles.ContainsKey("skyclient");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            var firstinstall = (packssize == 0 && modssize == 0) || !containsSkyclient;

            if (firstinstall)
            {
                var allowFirstinstall = new ConfirmFirstInstallWindow().ShowDialog();
                Console.WriteLine("Allow first install: ");
                Console.WriteLine(allowFirstinstall ?? false);
                Repository.Instance.IsFirstInstall = allowFirstinstall ?? false;
            }

            Repository.Instance.PopulatePackageParents();
            Repository.Instance.CheckSelectedItems();

            Console.WriteLine("Starting...");
            //new MainWindow().ShowDialog();
        }
    }
}
