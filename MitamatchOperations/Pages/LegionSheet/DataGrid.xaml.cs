using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Models.DataGrid;
using mitama.Pages.LegionSheet.Views;
using Syncfusion.UI.Xaml.Editors;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.LegionSheet;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DataGrid : Page
{
    private MemberInfo Info { get; }

    public DataGrid(MemberInfo info)
    {
        InitializeComponent();
        Info = info;
    }

    private void SegCtrl_SelectionChanged(object sender, SegmentSelectionChangedEventArgs e)
    {
        if (sender is not SfSegmentedControl ctrl) return;
        switch (ctrl.SelectedItem.As<SegmentedModel>().Name)
        {
            case "オーダー":
                {
                    if (Info.Version is null)
                    {
                        var legacyToV2 = Order
                            .List
                            .Where(o => o.Payed)
                            .Reverse()
                            .Select((order, index) => (order, index))
                            .ToDictionary(pair => pair.index, pair => pair.order.Index);
                        sfDataGrid.ItemsSource = new ObservableCollection<OrderInfo>(Info.OrderIndices is null ? [] : [.. Info.OrderIndices.Select(idx => new OrderInfo(Order.Of(legacyToV2[idx])))]);
                    }
                    else
                    {
                        sfDataGrid.ItemsSource = new ObservableCollection<OrderInfo>(Info.OrderIndices is null ? [] : [.. Info.OrderIndices.Select(idx => new OrderInfo(Order.Of(idx)))]);
                    }
                    break;
                }
            case "衣装":
                sfDataGrid.ItemsSource = new ObservableCollection<CostumeInfo>(Info.Costumes is null ? [] : [.. Info.Costumes.Select(raw => new CostumeInfo(raw))]);
                break;
            case "メモリア":
                sfDataGrid.ItemsSource = new ObservableCollection<MemoriaInfo>(Info.Memorias is null ? [] : [.. Info.Memorias.Select(raw => new MemoriaInfo(raw))]);
                break;
        }
    }
}
