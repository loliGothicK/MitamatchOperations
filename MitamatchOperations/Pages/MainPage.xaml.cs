using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Mitama.Domain;
using Mitama.Lib;
using Mitama.Pages.Common;
using Mitama.Pages.Main;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using Windows.System;
using WinRT;

namespace Mitama.Pages;

public enum Badge
{
    None,
    New,
    Updated,
    Preview,
}

public record struct Control(string Name, Badge Badge, Symbol Icon)
{
    public static Control Library => new("Library", Badge.Updated, Symbol.Bookmarks);
    public static Control Managements => new("Management", Badge.New, Symbol.Manage);
    public static Control LegionSheet => new("Legion Sheet", Badge.New, Symbol.Page2);
    public static Control DeckBuilder => new("Deck Builder", Badge.None, Symbol.Flag);
    public static Control OrderConsole => new("Order Console", Badge.None, Symbol.OpenWith);
    public static Control LegionConsole => new("Legion Console", Badge.None, Symbol.MapPin);
    public static Control ControlDashboard => new("Control Dashboard", Badge.Preview, Symbol.VideoChat);

    public static Control[] All => [
        LegionConsole,
        Managements,
        LegionSheet,
        OrderConsole,
        DeckBuilder,
        Library,
        ControlDashboard
    ];

    public readonly Visibility NewVisible => Badge == Badge.New ? Visibility.Visible : Visibility.Collapsed;
    public readonly Visibility UpdatedVisible => Badge == Badge.Updated ? Visibility.Visible : Visibility.Collapsed;
    public readonly Visibility PreviewVisible => Badge == Badge.Preview ? Visibility.Visible : Visibility.Collapsed;
}

public record ViewModel
{
    public static Control[] Controls => Control.All;
}

/// <summary>
/// Top Page
/// </summary>
public sealed partial class MainPage
{
    public string Project = "新規プロジェクト";
    public string User = "不明なユーザー";
    public DiscordUser DiscordUser { get; set; }

    public UIElement GetAppTitleBar => AppTitleBar;

    public MainPage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
        LoadCache();
        RootFrame.Navigate(typeof(LegionConsolePage));
        Loaded += async (_, _) => await UpdateCheck();
    }

    private async Task UpdateCheck()
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

            if (version != $"v{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}")
            {
                InfoBar.Title = $"{version}が利用可能です";
                InfoBar.Severity = InfoBarSeverity.Informational;
                var link = new Button
                {
                    Content = $"https://github.com/LoliGothick/MitamatchOperations/releases/tag/{version}",
                };
                link.Click += (_, _) => { _ = Launcher.LaunchUriAsync(new Uri(link.Content.ToString())); };
                InfoBar.ActionButton = link;
                InfoBar.IsOpen = true;
            }
        }
    }

    private void LoadCache()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var cache = $@"{desktop}\MitamatchOperations\Cache\cache.json";
        var exists = File.Exists(cache);
        if (exists)
        {
            Director.ReadCache().Deconstruct(out Project, out User, out var JWT);
            DiscordUser = JsonConvert.DeserializeObject<DiscordUser>(App.DecodeJwt(JWT).Unwrap());
            LoginLegion.Text = Project;
            UserDisplay.Text = User;
            if (DiscordUser.avatar is not null)
            {
                Avatar.ImageSource = new BitmapImage(new Uri(@$"https://cdn.discordapp.com/avatars/{DiscordUser.id}/{DiscordUser.avatar}"));
            }
        }
        else
        {
            Director.CreateDirectory($@"{desktop}\MitamatchOperations\Cache");
            using var fs = Director.CreateFile($@"{desktop}\MitamatchOperations\Cache\cache.json");
            var save = new Cache(Project).ToJsonBytes();
            fs.Write(save, 0, save.Length);
        }
    }

    public void Navigate(
        System.Type pageType,
        object targetPageArguments = null,
        NavigationTransitionInfo navigationTransitionInfo = null)
    {
        var args = new NavigationRootPageProps
        {
            NavigationRootPage = this,
            Parameter = targetPageArguments
        };
        args.NavigationRootPage = this;
        args.Parameter = targetPageArguments;
        RootFrame.Navigate(pageType, args, navigationTransitionInfo);
    }

    private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
    {
    }

    private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
    {
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            if (RootFrame.SourcePageType != typeof(SettingsPage))
            {
                Navigate(typeof(SettingsPage));
            }
        }
        else
        {
            NavView_Navigate(args.InvokedItem.As<StackPanel>().AccessKey);
        }
    }

    private void NavView_Navigate(string item)
    {
        var mapping = new Dictionary<string, System.Type>()
        {
            [Control.Library.Name] = typeof(LibraryPage),
            [Control.Managements.Name] = typeof(ManagementPage),
            [Control.LegionSheet.Name] = typeof(LegionSheetPage),
            [Control.DeckBuilder.Name] = typeof(DeckBuilderPage),
            [Control.OrderConsole.Name] = typeof(OrderConsolePage),
            [Control.LegionConsole.Name] = typeof(LegionConsolePage),
            [Control.ControlDashboard.Name] = typeof(ControlDashboardPage),
        };

        var pageType = mapping[item];
        if (RootFrame.CurrentSourcePageType == pageType) return;
        Navigate(pageType);
    }

    private async void ChangeProjectButton_Click(object sender, RoutedEventArgs e)
    {
        var builder = new DialogBuilder(XamlRoot);

        var selected = Project;

        var dialog = builder
            .WithTitle("ログインレギオンを変更または作成します")
            .WithPrimary("Login")
            .WithSecondary("Create New")
            .WithCancel("Cancel")
            .Build();

        dialog.IsPrimaryButtonEnabled = false;

        dialog.Content = new ChangeProjectDialogContent(s =>
        {
            selected = s;
            dialog.IsPrimaryButtonEnabled = true;
        });

        dialog.PrimaryButtonCommand = new Defer(async delegate
        {
            if (!Directory.Exists($@"{Director.ProjectDir()}\{selected}"))
            {
                await FailureInfo(selected);
                return;
            }
            LoginLegion.Text = Project = selected;
            Director.CacheWrite((Director.ReadCache() with { Legion = Project, User = User }).ToJsonBytes());
            Navigate(typeof(LegionConsolePage), Project);
            await LoginInfo();
        });

        dialog.SecondaryButtonCommand = new Defer(async delegate
        {
            LoginLegion.Text = Project = selected;
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}");
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Decks");
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Members");
            Director.CacheWrite((Director.ReadCache() with { Legion = Project, User = User }).ToJsonBytes());
            Navigate(typeof(LegionConsolePage), Project);
            await LoginInfo();
        });

        await dialog.ShowAsync();
    }

    private async Task FailureInfo(string msg)
    {
        InfoBar.IsOpen = true;
        InfoBar.Severity = InfoBarSeverity.Error;
        InfoBar.Title = msg;
        await Task.Delay(2000);
        InfoBar.IsOpen = false;
    }

    private async Task LoginInfo()
    {
        InfoBar.IsOpen = true;
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.Title = "Successfully logged in!";
        Director.CacheWrite((Director.ReadCache() with { Legion = Project, User = User }).ToJsonBytes());
        await Task.Delay(2000);
        InfoBar.IsOpen = false;
    }
}

public class NavigationRootPageProps
{
    public MainPage NavigationRootPage;
    public object Parameter;
}
