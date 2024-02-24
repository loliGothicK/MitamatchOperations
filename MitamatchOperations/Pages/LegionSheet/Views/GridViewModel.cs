using System.Collections.ObjectModel;
using System.Linq;
using mitama.Domain;
using mitama.Models.DataGrid;

namespace mitama.Pages.LegionSheet.Views;

public class GridViewModel(Order[] orders)
{
    public ObservableCollection<OrderInfo> Orders { get; set; } = [.. orders.Select(order => new OrderInfo(order))];

    public GridViewModel(): this([]) { }
}
