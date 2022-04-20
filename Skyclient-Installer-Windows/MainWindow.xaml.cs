using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Skyclient.JsonParts;
using Skyclient.Utilities;

namespace Skyclient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            RepoViewMods.RepoItems = Repository.Instance.GetVisibleMods();
            RepoViewPacks.RepoItems = Repository.Instance.GetVisiblePacks();

            EventUtils.RepoItemSelectedStateChange += EventUtils_RepoItemSelectedStateChange;
        }

        private void EventUtils_RepoItemSelectedStateChange(RepoItem item, EventArgs e)
        {
            //Console.WriteLine(item.FolderName);

            Console.WriteLine("Event Recieved: " + item.Display + " = " + item.Enabled);
            //TODO: add outdated files support
            if (item.Enabled)
            {
                RepoUtils.AddRepoItemToQueue(item);
            }
            else
            {
                RepoUtils.RemoveRepoItem(item);
            }
        }

        public void ShowRepoItemDetails(RepoItem item)
        {
            RepoItemDetailsViewInstance.SetItem(item);
            RepoItemDetailsViewInstance.Visibility = Visibility.Visible;
        }
        public void HideRepoItemDetails()
        {
            RepoItemDetailsViewInstance.Visibility = Visibility.Collapsed;
        }

        private void ButtonShowMods_Click(object sender, RoutedEventArgs e)
        {
            this.RepoViewPacks.Visibility = Visibility.Collapsed;
            this.RepoViewMods.Visibility = Visibility.Visible;
        }

        private void ButtonShowPacks_Click(object sender, RoutedEventArgs e)
        {
            this.RepoViewMods.Visibility = Visibility.Collapsed;
            this.RepoViewPacks.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Close = true;
            if (Repository.Instance.GetDownloadQueueSize() == 0)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
