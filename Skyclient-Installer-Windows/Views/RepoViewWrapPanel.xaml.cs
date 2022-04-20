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
    /// Interaktionslogik für RepoViewWrapPanel.xaml
    /// </summary>
    public partial class RepoViewWrapPanel : UserControl
    {
        KeyValuePair<string, List<RepoItem>> entry;
        public override string ToString()
        {
            return entry.Key;
        }
        public RepoViewWrapPanel(KeyValuePair<string, List<RepoItem>> entry)
        {
            this.entry = entry;
            InitializeComponent();
            foreach (var item in entry.Value)
            {
                ListWrapPanel.Children.Add(new MenuRepoItemView(item));
            }
        }
    }
}
