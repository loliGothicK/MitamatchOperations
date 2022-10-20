using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;
using mitama.Pages.Main;

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
        NavigationCacheMode = NavigationCacheMode.Enabled;
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
        async void InvokeInfo(InfoProps props)
        {
            InfoBar.IsOpen = true;
            InfoBar.Severity = props.Severity;
            InfoBar.Title = props.Title;
            await Task.Delay(2000);
            InfoBar.IsOpen = false;
        }

        var props = new Props(Project) { InvokeInfo = InvokeInfo };
        var mapping = new Dictionary<string, (Type, object?)>()
        {
            {"home", (typeof(HomePage), null)},
            {"region console", (typeof(RegionConsolePage), props)},
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

        async void PrimaryAction()
        {
            LoginRegion.Text = Project = selected;
            Navigate(typeof(RegionConsolePage), new Props(Project));
            await LoginInfo();
        }

        dialog.PrimaryButtonCommand = new Defer(PrimaryAction);

        async void SecondaryAction()
        {
            LoginRegion.Text = Project = selected;
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Director.CreateDirectory($@"{desktop}\MitamatchOperations\Regions\{Project}");
            Navigate(typeof(RegionConsolePage), new Props(Project));
            await LoginInfo();
        }

        dialog.SecondaryButtonCommand = new Defer(SecondaryAction);

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

        async void Action()
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fs = Director.CreateFile($@"{desktop}\MitamatchOperations\Regions\{Project}\{name}.json");
            var memberJson = new Member(DateTime.Now, DateTime.Now, name!, position!, new ushort[] { }).ToJson();
            var save = new UTF8Encoding(true).GetBytes(memberJson);
            fs.Write(save, 0, save.Length);
            await SavedInfo();
        }

        dialog.PrimaryButtonCommand = new Defer(Action);

        await dialog.ShowAsync();
    }

    private async Task SavedInfo()
    {
        InfoBar.IsOpen = true;
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.Title = "successfully saved!";
        await Task.Delay(2000);
        InfoBar.IsOpen = false;
    }
    private async Task LoginInfo()
    {
        InfoBar.IsOpen = true;
        InfoBar.Severity = InfoBarSeverity.Success;
        InfoBar.Title = "successfully logged in!";
        Director.CacheWrite(new Cache(Project).ToJsonBytes());
        await Task.Delay(2000);
        InfoBar.IsOpen = false;
    }
}

public class NavigationRootPageProps
{
    public MainPage? NavigationRootPage;
    public object? Parameter;
}
