using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.OrderConsole;

namespace mitama.Pages;

/// <summary>
/// Order Console Page navigated to within a Main Page.
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
