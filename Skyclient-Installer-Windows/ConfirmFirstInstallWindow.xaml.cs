using Newtonsoft.Json;
using Skyclient.JsonParts;
using Skyclient.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Skyclient
{
    /// <summary>
    /// Interaktionslogik für ConfirmFirstInstallWindow.xaml
    /// </summary>
    public partial class ConfirmFirstInstallWindow : Window
    {
        public ConfirmFirstInstallWindow()
        {
            InitializeComponent();
        }

        private void AcceptInstall(object sender, RoutedEventArgs e)
        {
            AddProfile();
            DownloadForge();

            this.DialogResult = true;
            this.Close();
        }

        private void DenyInstall(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void AddProfile()
        {
            try
            {
                var launcherProfilesJson = Path.Combine(RepoUtils.DotMinecraftDirectory, "launcher_profiles.json");
                var jsonText = File.ReadAllText(launcherProfilesJson);
                var json = JsonConvert.DeserializeObject<LauncherProfilesFileJson>(jsonText);

                var contains = json.profiles.ContainsKey("skyclient");
                while (!contains)
                {
                    Console.WriteLine("Creating profile...");
                    json.profiles.Add("skyclient", LauncherProfileJson.Create());
                    File.WriteAllText(launcherProfilesJson, JsonConvert.SerializeObject(json, Formatting.Indented));
                    Thread.Sleep(500);
                    jsonText = File.ReadAllText(launcherProfilesJson);
                    json = JsonConvert.DeserializeObject<LauncherProfilesFileJson>(jsonText);
                    contains = json.profiles.ContainsKey("skyclient");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void DownloadForge()
        {
            var versionName = "1.8.9-forge1.8.9-11.15.1.2318-1.8.9";
            var versionJsonName = versionName + ".json";
            var jsonDirectory = Path.Combine(RepoUtils.DotMinecraftDirectory, "versions", versionName);
            var jsonLocation = Path.Combine(jsonDirectory, versionJsonName);
            if (!(Directory.Exists(jsonDirectory) && File.Exists(jsonLocation)))
            {
                try
                {
                    var versionJson = RepoUtils.DownloadRepoFileString("forge/1.8.9-forge1.8.9-11.15.1.2318-1.8.9.json");
                    Directory.CreateDirectory(jsonDirectory);
                    File.WriteAllText(jsonLocation, versionJson);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
    }
}
