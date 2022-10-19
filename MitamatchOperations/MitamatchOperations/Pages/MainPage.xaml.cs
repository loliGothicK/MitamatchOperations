using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using mitama.Domain;
using mitama.Pages.Common;
using mitama.Pages.Main;
using mitama.Pages.OrderConsole;

namespace mitama.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage
{
    public string Project = "新規プロジェクト";

    public UIElement GetAppTitleBar => AppTitleBar;

    public MainPage()
    {
        InitializeComponent();
        AppNavBar.SelectedIndex = 1;
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
            Project = Director.ReadCache().LoggedIn;
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
        object? targetPageArguments = null,
        Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo? navigationTransitionInfo = null)
    {
        var args = new NavigationRootPageArgs
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
        var mapping = new Dictionary<string, (Type, object?)>()
        {
            {"home", (typeof(HomePage), null)},
            {"region console", (typeof(RegionConsolePage), Project)},
            {"order console", (typeof(OrderConsolePage), null)},
        };

        var (pageType, args) = mapping[(string)item.Tag];
        if (RootFrame.CurrentSourcePageType != pageType)
        {
            Navigate(pageType, args);
        }
    }

    private async void ChangeProjectButton_Click(object sender, RoutedEventArgs e)
    {
        var builder = new DialogBuilder(XamlRoot);

        var selected = Project;

        var dialog = builder
            .WithTitle("ログインレギオンを変更または作成します")
            .WithBody(new ChangeProjectDialogContent(s => selected = s))
            .WithPrimary("Login")
            .WithSecondary("Create New")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(async delegate
        {
            LoginRegion.Text = Project = selected;
            await LoginInfo();
        });

        dialog.SecondaryButtonCommand = new Defer(async delegate
        {
            LoginRegion.Text = Project = selected;
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Director.CreateDirectory($@"{desktop}\MitamatchOperations\Regions\{Project}");
            await LoginInfo();
        });

        await dialog.ShowAsync();
    }

    private async void AddMemberButton_OnClick(object sender, RoutedEventArgs e)
    {
        var builder = new DialogBuilder(XamlRoot);

        string? name = null;
        Position? position = null;

        var dialog = builder
            .WithTitle("レギオンメンバを追加します")
            .WithBody(new AddNewMemberDialogContent(fragment =>
            {
                switch (fragment)
                {
                    case NewMemberName(var s):
                        name = s; break;
                    case NewMemberPosition(var p):
                        position = p; break;
                }
            }))
            .WithPrimary("Add")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(async delegate
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fs = Director.CreateFile($@"{desktop}\MitamatchOperations\Regions\{Project}\{name}.json");
            var memberJson = new Domain.Member(DateTime.Now, DateTime.Now, name!, position!, new ushort[]{}).ToJson();
            var save = new UTF8Encoding(true).GetBytes(memberJson);
            fs.Write(save, 0, save.Length);
            await SavedInfo();
        });

        await dialog.ShowAsync();
    }

    private async Task SavedInfo()
    {
        SavedInfoBar.IsOpen = true;
        await Task.Delay(2000);
        SavedInfoBar.IsOpen = false;
    }
    private async Task LoginInfo()
    {
        LoginInfoBar.IsOpen = true;
        await Task.Delay(2000);
        Director.CacheWrite(new Cache(Project).ToJsonBytes());
        LoginInfoBar.IsOpen = false;
    }
}

public class NavigationRootPageArgs
{
    public MainPage? NavigationRootPage;
    public object? Parameter;
}
