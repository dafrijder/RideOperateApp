using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace RideOperateApp
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Start altijd in ServerListPage
            RootFrame.Navigate(typeof(ServerListPage));
        }
    }
}
