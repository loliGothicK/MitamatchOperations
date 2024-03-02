using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.System;

namespace Mitama.Pages;

/// <summary>
/// Settings Page navigated to within a Main Page.
/// </summary>
public sealed partial class SettingsPage
{
    private readonly string appVersion = string.Format("Version: {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        {
            string owner = "LoliGothick";
            string repo = "MitamatchOperations";

            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "request"); // GitHub APIへのリクエストにはUser-Agentヘッダーが必要

            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                string version = System.Text.Json.JsonDocument.Parse(json).RootElement.GetProperty("tag_name").GetString();

                if (version == $"v{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}")
                {
                    InfoBar.Title = "このバージョンは最新です";
                    InfoBar.Severity = InfoBarSeverity.Informational;
                    InfoBar.IsOpen = true;
                    await Task.Delay(3000);
                    InfoBar.IsOpen = false;
                }
                else
                {
                    InfoBar.Title = $"{version}が利用可能です";
                    InfoBar.Severity = InfoBarSeverity.Informational;
                    var link = new Button
                    {
                        Content = $"https://github.com/LoliGothick/MitamatchOperations/releases/tag/{version}",
                    };
                    link.Click += (_s, _e) => { _ = Launcher.LaunchUriAsync(new Uri(link.Content.ToString())); };
                    InfoBar.ActionButton = link;
                    InfoBar.IsOpen = true;
                }
            }
        }
    }
}
