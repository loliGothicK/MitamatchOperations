using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.RegionConsole;

namespace mitama.Pages;

/// <summary>
/// Region Console Page navigated to within a Main Page.
/// </summary>
public sealed partial class RegionConsolePage
{

    public RegionConsolePage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;

        ManageConsoleFrame.Navigate(typeof(MemberManageConsole));
        UnitViewerFrame.Navigate(typeof(UnitViewer));
        HistoriaViewerFrame.Navigate(typeof(HistoriaViewer));
        ResultInputFrame.Navigate(typeof(ResultInput));

        ManageConsoleFrame.Navigated += (_, _) => { ManageConsoleFrame.Navigate(typeof(MemberManageConsole)); };
    }
}
