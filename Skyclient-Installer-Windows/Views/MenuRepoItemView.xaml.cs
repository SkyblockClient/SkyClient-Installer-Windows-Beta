using Skyclient.JsonParts;
using Skyclient.Utilities;
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

namespace Skyclient.Views
{
    /// <summary>
    /// Interaktionslogik für MenuRepoItemView.xaml
    /// </summary>
    public partial class MenuRepoItemView : UserControl
    {
        public RepoItem Item;
        public MenuRepoItemView(RepoItem item)
        {
            this.Item = item;

            InitializeComponent();
            ViewUtilities.SetImage(ItemIcon, item);


            ItemDescription.Text = item.Description;
            ItemEnabledCheckbox.IsChecked = item.Enabled;

            item.DownloadStatusChanged += Item_DownloadStatusChanged;
            EventUtils.RepoItemSelectedStateChange += EventUtils_RepoItemSelectedStateChange;

            SetDownloadStatus(DownloadableFileStatus.Idle);

        }

        private void Item_DownloadStatusChanged(object sender, EventArgs e)
        {
            SetDownloadStatus(Item.DownloadStatus);
        }

        private void SetDownloadStatus(DownloadableFileStatus status)
        {
            if (status == DownloadableFileStatus.Downloading)
            {
                ItemDisplay.Content = "Downloading...";
                ItemAuthor.Content = "";
            }
            else
            {
                ItemDisplay.Content = Item.Display;
                ItemAuthor.Content = Item.IsSetCreator() ? $"by {Item.Creator}" : "";
            }

        }

        private void EventUtils_RepoItemSelectedStateChange(RepoItem item, EventArgs e)
        {
            if (this.Item == item)
            {
                ItemEnabledCheckbox.IsChecked = item.Enabled;
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance?.ShowRepoItemDetails(Item);
        }

        private void ItemEnabledCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetDownloadStatus(DownloadableFileStatus.Downloading);
            Item.SetEnabledStatus(true);
        }

        private void ItemEnabledCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Item.SetEnabledStatus(false);
        }
    }
}
