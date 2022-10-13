using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace mitama;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage
{
    public UIElement GetAppTitleBar => AppTitleBar;

    public MainPage()
    {
        InitializeComponent();
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
        switch (item.Tag)
        {
            case "home":
            {
                if (RootFrame.CurrentSourcePageType != typeof(OrderComposerPage))
                {
                    Navigate(typeof(OrderComposerPage));
                }

                break;
            }
            case "order composer":
            {
                if (RootFrame.CurrentSourcePageType != typeof(OrderComposerPage))
                {
                    Navigate(typeof(OrderComposerPage));
                }

                break;
            }
        }
    }
}

public class NavigationRootPageArgs
{
    public MainPage? NavigationRootPage;
    public object? Parameter;
}
