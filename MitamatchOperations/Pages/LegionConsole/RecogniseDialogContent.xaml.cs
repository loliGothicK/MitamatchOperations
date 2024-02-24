using System.Collections.Generic;
using System.Linq;
using mitama.Domain;

namespace mitama.Pages.LegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RecogniseDialogContent
{
    public RecogniseDialogContent(IEnumerable<Memoria> memorias)
    {
        InitializeComponent();
        ResultView.ItemsSource = memorias.ToList();
    }
}