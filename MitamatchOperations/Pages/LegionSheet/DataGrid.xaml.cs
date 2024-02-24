using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Models.DataGrid;
using Syncfusion.UI.Xaml.Editors;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.LegionSheet;

internal abstract class GridViewModel<T>
{
    internal virtual ObservableCollection<T> Data { get; }
}

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DataGrid : Page
{
    private MemberInfo Info { get; }
    private readonly string[] Controls = ["オーダー", "衣装", "メモリア"];

    public DataGrid(MemberInfo info)
    {
        InitializeComponent();
        Info = info;
        sfDataGrid.ItemsSource = new ObservableCollection<OrderInfo>([.. Info.OrderIndices.Select(idx => new OrderInfo(Order.Of(idx)))]);
    }

    private void SegCtrl_SelectionChanged(object sender, SegmentSelectionChangedEventArgs e)
    {
        if (sender is not SfSegmentedControl ctrl) return;
        switch (ctrl.SelectedItem.As<string>())
        {
            case "オーダー":
                sfDataGrid.ItemsSource = new ObservableCollection<OrderInfo>([.. Info.OrderIndices.Select(idx => new OrderInfo(Order.Of(idx)))]);
                break;
            case "衣装":
                sfDataGrid.ItemsSource = new ObservableCollection<MemoriaInfo>([.. Info.Memorias.Select(raw => new MemoriaInfo(raw))]);
                break;
            case "メモリア":
                sfDataGrid.ItemsSource = new ObservableCollection<OrderInfo>([.. Info.OrderIndices.Select(idx => new OrderInfo(Order.Of(idx)))]);
                break;
        }
    }
}
