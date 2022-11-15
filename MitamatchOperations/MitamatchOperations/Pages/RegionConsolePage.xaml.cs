using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using mitama.Algorithm.IR;
using mitama.Pages.RegionConsole;
using WinRT;
using mitama.Pages.OrderConsole;

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
    }
}
