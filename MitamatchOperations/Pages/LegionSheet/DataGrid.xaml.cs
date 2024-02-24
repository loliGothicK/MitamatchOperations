using System.Linq;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Pages.LegionSheet.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.LegionSheet;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DataGrid : Page
{
    private GridViewModel GridView { get; set; }
    private MemberInfo Info { get; }

    public DataGrid(MemberInfo info)
    {
        InitializeComponent();
        Info = info;
        GridView = new GridViewModel([.. Info.OrderIndices.Select(idx => Order.Of(idx))]);
    }
}
