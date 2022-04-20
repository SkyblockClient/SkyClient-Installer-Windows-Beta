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
    /// Interaktionslogik für RepoItemDetailsView.xaml
    /// </summary>
    public partial class RepoItemDetailsView : UserControl
    {
        public RepoItem? Item;
        public static RepoItemDetailsView? Instance { get; private set; }
        public RepoItemDetailsView()
        {
            Instance = this;
            InitializeComponent();
            EventUtils.RepoItemSelectedStateChange += EventUtils_RepoItemSelectedStateChange;
        }

        private void EventUtils_RepoItemSelectedStateChange(RepoItem item, EventArgs e)
        {
            if (item == Item)
            {
                ItemEnabledCheckbox.IsChecked = item.Enabled;
            }
        }

        public void SetItem(RepoItem item)
        {
            Item = item;
            ItemDisplay.Content = item.Display;
            ItemDescription.Text = item.Description;
            ItemEnabledCheckbox.IsChecked = item.Enabled;
            ItemAuthor.Content = item.IsSetCreator() ? $"by {item.Creator}" : "";

            ItemActionsBox.Items.Clear();
            foreach (var action in item.Actions)
            {
                if (action.Method == "click")
                {
                    var boxitem = new ActionListBoxItem(action);
                    ItemActionsBox.Items.Add(boxitem);
                }
            }

            ViewUtilities.SetImage(RepoItemIcon, item);
        }

        private bool IgnoreNextEvent = false;
        private void CloseViewButton_Click(object sender, RoutedEventArgs e)
        {
            //IgnoreNextEvent = true;
            //ItemEnabledCheckbox.IsChecked = false;
            MainWindow.Instance?.HideRepoItemDetails();
        }

        private void ItemEnabledCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            
            if (IgnoreNextEvent)
            {
                Console.WriteLine("Ignored event on true");
                IgnoreNextEvent = false;
                return;
            }
            
            Item?.SetEnabledStatus(true);
        }

        private void ItemEnabledCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            
            if (IgnoreNextEvent)
            {
                Console.WriteLine("Ignored event on false");
                IgnoreNextEvent = false;
                return;
            }
            
            Item?.SetEnabledStatus(false);
        }

    }
}
