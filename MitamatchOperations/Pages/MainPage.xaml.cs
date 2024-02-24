using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.Common;
using mitama.Pages.Main;
using MitamatchOperations.Lib;

namespace mitama.Pages;
/// <summary>
/// Top Page
/// </summary>
public sealed partial class MainPage
{
    public string Project = "新規プロジェクト";
    public string User = "不明なユーザー";

    public UIElement GetAppTitleBar => AppTitleBar;

    public MainPage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
        AppNavBar.SelectedIndex = 0;
        LoadCache();
        RootFrame.Navigate(typeof(HomePage));
    }

    private void LoadCache()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var cache = $@"{desktop}\MitamatchOperations\Cache\cache.json";
        var exists = File.Exists(cache);
        if (exists)
        {
            Project = Director.ReadCache().Legion;
            LogInUser.Label = Avatar.AvatarName = User = Director.ReadCache().User ?? "不明なユーザー";
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
        Type pageType,
        object targetPageArguments = null,
        Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo navigationTransitionInfo = null)
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
            // find NavigationViewItem with Content that equals InvokedItem
            var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
            NavView_Navigate(item);
        }
    }

    private void NavView_Navigate(FrameworkElement item)
    {
        var mapping = new Dictionary<string, System.Type>()
        {
            {"home", typeof(HomePage)},
            {"Legion console",typeof(LegionConsolePage)},
            {"Legion Sheet",typeof(LegionSheetPage)},
            {"order console", typeof(OrderConsolePage)},
            {"control dashboard", typeof(ControlDashboardPage)},
            {"deck builder", typeof(DeckBuilderPage)},
            {"library", typeof(LibraryPage)},
        };

        var pageType = mapping[(string)item.Tag];
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
            Director.CacheWrite(new Cache(Project, User).ToJsonBytes());
            Navigate(typeof(LegionConsolePage), Project);
            await LoginInfo();
        });

        dialog.SecondaryButtonCommand = new Defer(async delegate {
            LoginLegion.Text = Project = selected;
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}");
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Decks");
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Members");
            Director.CacheWrite(new Cache(Project, User).ToJsonBytes());
            Navigate(typeof(LegionConsolePage), Project);
            LegionView.IsSelected = true;
            await LoginInfo();
        });

        await dialog.ShowAsync();
    }

    private async Task SuccessInfo(string msg)
    {
        InfoBar.IsOpen = true;
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.Title = msg;
        await Task.Delay(2000);
        InfoBar.IsOpen = false;
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
        Director.CacheWrite(new Cache(Project, User).ToJsonBytes());
        await Task.Delay(2000);
        InfoBar.IsOpen = false;
    }

    private async void LogInUser_OnClick(object sender, RoutedEventArgs e)
    {
        // placeholder
        string loggedIn = null;

        // dialog forward definition
        var dialog = new DialogBuilder(XamlRoot)
            .WithTitle("ログインユーザーを選択してください")
            .WithPrimary("ログイン")
            .WithCancel("やっぱりやめる")
            .Build();
        dialog.IsPrimaryButtonEnabled = false;

        // dialog content forward definition
        var body = new DropDownButton
        {
            Content = Project,
        };

        // dialog content flyout
        var menu = new MenuFlyout { Placement = FlyoutPlacementMode.Bottom };

        // init flyout items
        foreach (var member in Directory.GetFiles($@"{Director.ProjectDir()}\{Project}", "*.json")
                     .Select(path => path.Split('\\').Last().Replace(".json", string.Empty)))
        {
            menu.Items.Add(new MenuFlyoutItem
            {
                Text = member,
                Command = new Defer(delegate
                {
                    // store
                    loggedIn = member;
                    // show selected member name for UX
                    body.Content = member;
                    // enable when member is selected
                    dialog.IsPrimaryButtonEnabled = true;
                    return Task.CompletedTask;
                })
            });
        }

        // inject controls
        body.Flyout = menu;
        dialog.Content = body;

        // ReSharper disable once AsyncVoidLambda
        dialog.PrimaryButtonCommand = new Defer(async delegate
        {
            LogInUser.Label = User = loggedIn!;
            await LoginInfo();
        });

        await dialog.ShowAsync();
    }
}

public class NavigationRootPageProps
{
    public MainPage NavigationRootPage;
    public object Parameter;
}
