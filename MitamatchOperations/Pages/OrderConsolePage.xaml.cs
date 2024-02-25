using Microsoft.UI.Xaml.Navigation;
using Mitama.Pages.OrderConsole;

namespace Mitama.Pages;

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
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string parameter && !string.IsNullOrWhiteSpace(parameter))
        {
            EditFrame.Navigate(typeof(DeckEditorPage), parameter);
        }
    }
}
