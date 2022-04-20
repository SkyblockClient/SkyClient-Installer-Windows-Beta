using Skyclient.JsonParts;
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
    /// Interaktionslogik für ModView.xaml
    /// </summary>
    public partial class RepoView : UserControl
    {
        private SortedDictionary<string, List<RepoItem>> SortedUniqueCategories;
        public RepoItem[] RepoItems
        {
            get { return _repoItems; }
            set
            {
                //ListWrapPanel.BeginInit();

                Dictionary<string, List<RepoItem>> uniqueCategories = new Dictionary<string, List<RepoItem>>();
                _repoItems = value;
                uniqueCategories.Add("0;All", new List<RepoItem>());
                foreach (var item in RepoItems)
                {

                    uniqueCategories["0;All"].Add(item);

                    bool hasCategory = false;
                    foreach (var category in item.Categiories)
                    {
                        hasCategory = true;
                        if (uniqueCategories.ContainsKey(category))
                        {
                            uniqueCategories[category].Add(item);
                        }
                        else
                        {
                            uniqueCategories.Add(category, new List<RepoItem>() { item });
                        }
                    }
                    if (!hasCategory)
                    {
                        if (uniqueCategories.ContainsKey("99;Other"))
                        {
                            uniqueCategories["99;Other"].Add(item);
                        }
                        else
                        {
                            uniqueCategories.Add("99;Other", new List<RepoItem>() { item });
                        }
                    }
                }

                SortedUniqueCategories = new SortedDictionary<string, List<RepoItem>>(uniqueCategories);
                foreach (var entry in SortedUniqueCategories)
                {
                    TabItem item = new TabItem();
                    //item.Content = new RepoViewWrapPanel(entry);
                    item.Header = entry.Key.Split(';')[1];
                    item.Tag = entry;
                    TabList.Items.Add(item);
                }

                TabList.EndInit();

            }
        }
        private RepoItem[] _repoItems;

        public RepoView()
        {
            InitializeComponent();
        }

        private void TabList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var entry in SortedUniqueCategories)
            {
                // jank
                if (entry.Key == ((KeyValuePair<string, List<RepoItem>>)(((TabItem)TabList.SelectedItem).Tag)).Key)
                {
                    //Console.WriteLine(((KeyValuePair<string, List<RepoItem>>)(((TabItem)TabList.SelectedItem).Tag)).Key);
                    ListWrapPanel.Children.Clear();
                    foreach (var item in entry.Value)
                    {
                        ListWrapPanel.Children.Add(new MenuRepoItemView(item));
                    }
                }
            }
        }
    }
}
