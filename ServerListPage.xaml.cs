using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RideOperateApp;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace RideOperateApp
{
    public sealed partial class ServerListPage : Page
    {
        private readonly string jsonFile;
        public ObservableCollection<Server> Servers { get; set; } = new();

        public ServerListPage()
        {
            this.InitializeComponent();

            jsonFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RideOperateApp",
                "servers.json"
            );
            Directory.CreateDirectory(Path.GetDirectoryName(jsonFile)!);

            LoadServers();
            ServerList.ItemsSource = Servers;
        }

        private void LoadServers()
        {
            if (File.Exists(jsonFile))
            {
                string json = File.ReadAllText(jsonFile);
                var servers = JsonSerializer.Deserialize<ObservableCollection<Server>>(json);
                if (servers != null)
                {
                    Servers.Clear();
                    foreach (var srv in servers) Servers.Add(srv);
                }
            }
        }

        private void SaveServers()
        {
            string json = JsonSerializer.Serialize(Servers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFile, json);
        }

        private async void AddServer_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new()
            {
                Title = "Nieuwe server toevoegen",
                PrimaryButtonText = "Opslaan",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            StackPanel panel = new() { Spacing = 10 };

            TextBox nameBox = new() { PlaceholderText = "Servernaam" };
            TextBox ipBox = new() { PlaceholderText = "IP-adres" };
            TextBox portBox = new() { PlaceholderText = "Port" };
            TextBox keyBox = new() { PlaceholderText = "API Key" };

            panel.Children.Add(nameBox);
            panel.Children.Add(ipBox);
            panel.Children.Add(portBox);
            panel.Children.Add(keyBox);

            dialog.Content = panel;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var server = new Server
                {
                    Name = nameBox.Text,
                    Ip = ipBox.Text,
                    Port = portBox.Text,
                    ApiKey = keyBox.Text
                };

                Servers.Add(server);
                SaveServers();
            }
        }

        private void ServerList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Server srv)
            {
                Frame.Navigate(typeof(ServerPanelPage), srv);
            }
        }
    }
}
