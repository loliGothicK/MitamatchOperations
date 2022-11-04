using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RecogniseDialogContent
{
    private List<Memoria> memoriaList;
    public RecogniseDialogContent(IEnumerable<Memoria> memorias)
    {
        InitializeComponent();
        ResultView.ItemsSource = memoriaList = memorias.ToList();
    }
}