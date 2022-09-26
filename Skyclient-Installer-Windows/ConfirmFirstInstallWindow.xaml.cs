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
    }
}
