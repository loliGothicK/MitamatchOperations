using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Models.DataGrid;
using MitamatchOperations.Lib;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.Library;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MemoriaLibrary : Page
{
    public ObservableCollection<MemoriaData> MemoriaList { get; } = new([.. Memoria.List.Select(m => new MemoriaData(m))]);
    public MemoriaLibrary()
    {
        InitializeComponent();
        sfDataGrid.ItemsSource = MemoriaList;
    }
}
