using Microsoft.UI.Xaml.Navigation;
using Mitama.Pages.LegionConsole;

namespace Mitama.Pages;

/// <summary>
/// Legion Console Page navigated to within a Main Page.
/// </summary>
public sealed partial class LegionConsolePage
{

    public LegionConsolePage()
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
