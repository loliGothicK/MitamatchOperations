using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.DataGrid;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.LegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DataGrid : Page
{
    public DataGrid()
    {
        InitializeComponent();
        rootGrid.Children.Add(new SfDataGrid());
    }
}