using System.IO;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Pages.Common;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HistoriaViewer : Page
{
    public HistoriaViewer()
    {
        InitializeComponent();
    }

    private void Load_Click(object _, RoutedEventArgs _e)
    {
        var logDir = @$"{Director.ProjectDir()}\{Director.ReadCache().Region}\BattleLog";
        var path = $@"{logDir}\{Calendar.Date:yyyy-MM-dd}\summary.json";
        if (!File.Exists(path))
        {
            return;
        }
        var summary = JsonSerializer.Deserialize<Summary>(File.ReadAllText(path));
        var result = summary.AllyPoints > summary.OpponentPoints ? "Win" : "Lose";
        Title.Text = $"{summary.Opponent}Åi{result}Åj, ÉmÉCÉì: {summary.NeunWelt}";
        foreach (var x in summary.AllyOrders)
        {
            AllyStackPanel.Children.Add(new TextBlock { Text = $"{x.Time}: {Order.List[x.Index].Name}" });
        }
        foreach (var x in summary.OpponentOrders)
        {
            OpponentStackPanel.Children.Add(new TextBlock { Text = $"{x.Time}: {Order.List[x.Index].Name}" });
        }
    }
}

internal record Summary(
    string Opponent,
    int AllyPoints,
    int OpponentPoints,
    string NeunWelt,
    OrderIndexAndTime[] AllyOrders,
    OrderIndexAndTime[] OpponentOrders
);
