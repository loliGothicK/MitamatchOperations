using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;
using mitama.Pages.Main;
using MitamatchOperations.Pages;
using WinRT;

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
            Project = Director.ReadCache().Region;
            LogInUser.Label = User = Director.ReadCache().User ?? "不明なユーザー";
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
            {"region console",typeof(RegionConsolePage)},
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
            LoginRegion.Text = Project = selected;
            Director.CacheWrite(new Cache(Project, User).ToJsonBytes());
            Navigate(typeof(RegionConsolePage), Project);
            await LoginInfo();
        });

        dialog.SecondaryButtonCommand = new Defer(async delegate {
            LoginRegion.Text = Project = selected;
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}");
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Decks");
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Members");
            Director.CacheWrite(new Cache(Project, User).ToJsonBytes());
            Navigate(typeof(RegionConsolePage), Project);
            RegionView.IsSelected = true;
            await LoginInfo();
        });

        await dialog.ShowAsync();
    }

    private async void AddMemberButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new DialogBuilder(XamlRoot)
            .WithTitle("レギオンメンバを追加します")
            .WithPrimary("Add")
            .WithCancel("Cancel")
            .Build();
        dialog.IsPrimaryButtonEnabled = false;

        string name = null;
        Position position = null;

        var body = new AddNewMemberDialogContent(fragment =>
        {
            switch (fragment)
            {
                case NewMemberName(var s):
                    name = s;
                    break;
                case NewMemberPosition(var p):
                    position = p;
                    break;
            }
            if (name != null && position != null) dialog.IsPrimaryButtonEnabled = true;
        });

        dialog.Content = body;
        dialog.PrimaryButtonCommand = new Defer(async delegate {
            if (!Directory.Exists($@"{Director.ProjectDir()}\{Project}"))
            {
                await FailureInfo($"{Project} は作成されていないレギオン名です、新規作成してください");
                return;
            }
            Director.CreateDirectory($@"{Director.ProjectDir()}\{Project}\Members\{name}");
            await using var fs = Director.CreateFile($@"{Director.ProjectDir()}\{Project}\Members\{name}\info.json");
            var memberJson = new MemberInfo(DateTime.Now, DateTime.Now, name!, position!, []).ToJson();
            var save = new UTF8Encoding(true).GetBytes(memberJson);
            fs.Write(save, 0, save.Length);
            await SuccessInfo("Successfully saved!");
        });

        Navigate(typeof(RegionConsolePage), Project);

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

    private async void RemoveMemberButton_OnClick(object sender, RoutedEventArgs e)
    {
        // dialog forward definition
        var dialog = new DialogBuilder(XamlRoot)
            .WithTitle("おわかれするユーザーを選択してください")
            .WithPrimary("おわかれする")
            .WithCancel("やっぱりやめる")
            .Build();
        dialog.IsPrimaryButtonEnabled = false;

        // dialog content forward definition
        var body = new StackPanel();

        // init flyout items
        foreach (var member in Util.LoadMemberNames(Project))
        {
            body.Children.Add(new CheckBox
            {
                AccessKey = member,
                Content = member,
                Command = new Defer(delegate
                {
                    if (body.Children.Select(box => box.As<CheckBox>()).Any(box => box.IsChecked ?? false))
                    {
                        dialog.IsPrimaryButtonEnabled = true;
                    }
                    return Task.CompletedTask;
                })
            });
        }

        // inject controls
        dialog.Content = body;

        // ReSharper disable once AsyncVoidLambda
        dialog.PrimaryButtonCommand = new Defer(async delegate
        {
            foreach (var member in body.Children.Select(box => box.As<CheckBox>()).Where(box => box.IsChecked ?? false).Select(box => box.AccessKey!))
            {
                new DirectoryInfo($@"{Director.ProjectDir()}\{Project}\Members\{member}").Delete(true);
            }
            await SuccessInfo("Successfully deleted!");
        });

        await dialog.ShowAsync();
    }
}

public class NavigationRootPageProps
{
    public MainPage NavigationRootPage;
    public object Parameter;
}
