using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.Common;
using mitama.Pages.Main;

namespace mitama.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage
{
    public string Project = "ちっちゃい娘FC";

    public UIElement GetAppTitleBar => AppTitleBar;

    public MainPage()
    {
        InitializeComponent();
        RootFrame.Navigate(typeof(HomePage));
        AppNavBar.SelectedIndex = 1;
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
            .WithTitle("ログインレギオンを変更")
            .WithBody(new ChangeProjectDialogContent(s => selected = s))
            .WithPrimary("Login")
            .WithSecondary("Create New")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            LoginRegion.Text = Project = selected;
        });

        await dialog.ShowAsync();
    }

}

public class NavigationRootPageArgs
{
    public MainPage? NavigationRootPage;
    public object? Parameter;
}
