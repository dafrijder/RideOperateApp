using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RideOperateApp
{
    public sealed partial class ServerPanelPage : Page
    {
        private Server currentServer;

        public ServerPanelPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Server srv)
            {
                currentServer = srv;
                ServerTitle.Text = $"Panels - {srv.Name}";
                _ = LoadPanelsAsync();
            }
        }

        private async Task LoadPanelsAsync()
        {
            PanelContainer.Children.Clear();

            var panels = await TcpApiClient.GetPanelsAsync(currentServer);

            if (panels.Length == 0 || panels[0].StartsWith("ERROR"))
            {
                PanelContainer.Children.Add(new TextBlock
                {
                    Text = panels.Length > 0 ? panels[0] : "No panels found.",
                    FontSize = 16
                });
                return;
            }

            foreach (var panelName in panels)
            {
                // Kaart voor panel
                var border = new Border
                {
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkSlateGray),
                    CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8),
                    Padding = new Thickness(12)
                };

                var stack = new StackPanel { Spacing = 6 };

                // Panel titel
                stack.Children.Add(new TextBlock
                {
                    Text = panelName,
                    FontSize = 20,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGreen),
                    FontWeight = Microsoft.UI.Text.FontWeights.Bold
                });

                // Knoppen (actie)
                var actions = await TcpApiClient.GetPanelActionsAsync(currentServer, panelName);
                var actionPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };

                foreach (var kv in actions)
                {
                    var btn = new Button
                    {
                        Content = kv.Key,
                        Tag = (panelName, kv.Key),
                        Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green),
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                        Padding = new Thickness(8, 4, 8, 4),
                        CornerRadius = new Microsoft.UI.Xaml.CornerRadius(6)
                    };
                    btn.Click += ActionButton_Click;
                    actionPanel.Children.Add(btn);
                }

                stack.Children.Add(actionPanel);
                border.Child = stack;
                PanelContainer.Children.Add(border);
            }
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<string, string> tag)
            {
                string panelName = tag.Item1;
                string action = tag.Item2;

                bool success = await TcpApiClient.ExecuteActionAsync(currentServer, panelName, action);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
