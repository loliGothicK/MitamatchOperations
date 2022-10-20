using System;
using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.Common;
using mitama.Pages.OrderConsole;
using WinRT;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderConsolePage
{
    public OrderConsolePage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;

        EditFrame.Navigate(typeof(DeckEditorPage));
        ManageFrame.Navigate(typeof(OrderManagerPage));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string parameter && !string.IsNullOrWhiteSpace(parameter))
        {
            EditFrame.Navigate(typeof(DeckEditorPage), parameter);
        }
        ManageFrame.Navigate(typeof(OrderManagerPage), e);
    }
}
