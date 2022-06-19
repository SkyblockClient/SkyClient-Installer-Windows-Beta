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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
