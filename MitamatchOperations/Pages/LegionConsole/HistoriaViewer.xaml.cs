using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Pages.Common;
using MitamatchOperations.Lib;
using mitama.Models;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.LegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HistoriaViewer : Page
{
    private Summary Summary;
    private readonly ObservableCollection<OrderLog> AllyOrders = [];
    private readonly ObservableCollection<OrderLog> OpponentOrders = [];
    private readonly ObservableCollection<Player> AllyMembers = [];
    private readonly ObservableCollection<Player> OpponentMembers = [];
    private readonly ObservableCollection<UnitChangeLog> unitChanges = [];
    private ChartViewModel chartView;

    public HistoriaViewer()
    {
        InitializeComponent();
    }

    private void Load_Click(object _, RoutedEventArgs _e)
    {
        var allyLegion = Director.ReadCache().Legion;
        var logDir = @$"{Director.ProjectDir()}\{allyLegion}\BattleLog";
        var path = $@"{logDir}\{Calendar.Date:yyyy-MM-dd}\summary.json";
        if (!File.Exists(path))
        {
            return;
        }
        
        var summary = Summary = JsonSerializer.Deserialize<Summary>(File.ReadAllText(path).Replace("\"Region\"", "\"Legion\""));
        var r1 = summary.AllyPoints > summary.OpponentPoints ? "Win" : "Lose";
        var r2 = summary.AllyPoints > summary.OpponentPoints ? "Lose" : "Win";
        
        Date.Text = $"{Calendar.Date:yyyy-MM-dd}";
        Title.Text = $"{r1}: {allyLegion}（{summary.AllyPoints:#,0}） - {r2}: {summary.Opponent}（{summary.OpponentPoints:#,0}）";
        NeunWelt.Text = $"ノイン: {summary.NeunWelt}";
        Comment.Text = summary.Comment;

        AllyMembers.Clear();
        foreach (var player in summary.Allies)
        {
            AllyMembers.Add(player);
        }
        OpponentMembers.Clear();
        foreach (var player in summary.Opponents)
        {
            OpponentMembers.Add(player);
        }
        AllyOrders.Clear();
        foreach (var x in summary.AllyOrders)
        {
            AllyOrders.Add(new OrderLog(Order.Of(x.Index), $"{x.Time.Minute:D2}:{x.Time.Second:D2}"));
        }
        OpponentOrders.Clear();
        foreach (var x in summary.OpponentOrders)
        {
            OpponentOrders.Add(new OrderLog(Order.Of(x.Index), $"{x.Time.Minute:D2}:{x.Time.Second:D2}"));
        }
        unitChanges.Clear();
        path = $@"{logDir}\{Calendar.Date:yyyy-MM-dd}\unitChanges.json";
        if (File.Exists(path))
        {
            var unitChangesData = JsonSerializer.Deserialize<UnitChanges>(File.ReadAllText(path));
            foreach (var (name, time) in unitChangesData.Data)
            {
                unitChanges.Add(new(name, time));
            }
        }
        foreach (var player in summary.Allies)
        {
            PlayerSelect.Items.Add(player);
        }

        var statusPath = $@"{logDir}\{Calendar.Date:yyyy-MM-dd}\Ally\[{summary.Allies[0].Name}]\status.json";
        var history = JsonSerializer.Deserialize<SortedDictionary<TimeOnly, AllStatus>>(File.ReadAllText(statusPath));
        chartView = new ChartViewModel(history);
    }

    private void MenuFlyout_Opening(object sender, object e)
    {
        var menu = sender as MenuFlyout;
        if (menu.Items.Any(item => item.Name == "Unit")) return;

        var playerName = menu.Target.AccessKey;
        var allyLegion = Director.ReadCache().Legion;
        var logDir = @$"{Director.ProjectDir()}\{allyLegion}\BattleLog";
        var date = $"{Calendar.Date:yyyy-MM-dd}";
        var dir = Summary.Allies.Select(p => p.Name).Contains(playerName)
            ? "Ally"
            : "Opponent";
        var unitPath = $@"{logDir}\{date}\{dir}\[{playerName}]\Units";
        var files = Directory.GetFiles(unitPath);
        foreach (var file in files)
        {
            var text = file.Split("\\").Last().Replace(".json", string.Empty);
            var (_, unit)  = Unit.FromJson(File.ReadAllText(file));
            var dialog = new DialogBuilder(XamlRoot)
                .WithTitle(text)
                .WithBody(new UnitViewDialog(unit))
                .WithCancel("閉じる")
                .Build();

            menu.Items.Add(new MenuFlyoutItem
            {
                Text = text,
                Command = new Defer(async () => await dialog.ShowAsync()),
            });
        }
    }

    private void PlayerSelect_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        if (sender is not ComboBox comboBox) return;
        var selected = comboBox.SelectedValue.As<Player>().Name;
        var logDir = @$"{Director.ProjectDir()}\{Director.ReadCache().Legion}\BattleLog";
        var statusPath = $@"{logDir}\{Calendar.Date:yyyy-MM-dd}\Ally\[{selected}]\status.json";
        var history = JsonSerializer.Deserialize<SortedDictionary<TimeOnly, AllStatus>>(File.ReadAllText(statusPath));
        chartView = new ChartViewModel(history);
        Line.ItemsSource = chartView.Data;
    }

    private void StautsSelect_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        chartView.SwithcTo(Line.Label = sender.As<ComboBox>().SelectedValue.As<ComboBoxItem>().Content.As<string>());
        Line.ItemsSource = chartView.Data;
    }
}

internal record struct OrderLog(Order Order, string Time);
internal record struct UnitChangeLog(string Name, TimeOnly Time)
{
    public readonly string Display => $@"{Time} => {Name}";
}

internal record UnitChanges(List<UnitChangePoint> Data);

internal record Summary(
    string Opponent,
    string Comment,
    int AllyPoints,
    int OpponentPoints,
    string NeunWelt,
    Player[] Allies,
    Player[] Opponents,
    OrderIndexAndTime[] AllyOrders,
    OrderIndexAndTime[] OpponentOrders
);
