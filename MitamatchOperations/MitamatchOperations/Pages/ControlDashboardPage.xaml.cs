using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Algorithm;
using mitama.Domain;
using mitama.Pages.Capture;
using mitama.Pages.Common;
using mitama.Pages.OrderConsole;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinRT;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ControlDashboardPage
{
    private readonly WindowCapture _capture = new(Search.WindowHandleFromCaption("Assaultlily"));
    private readonly DispatcherTimer _timer = new()
    {
        Interval = new TimeSpan(0, 0, 0, 0, 500)
    };

    private int _cursor = 0;
    private List<TimeTableItem> _deck = new();
    private readonly ObservableCollection<TimeTableItem> _reminds = new();
    private readonly ObservableCollection<ResultItem> _results = new();

    public ControlDashboardPage()
    {
        InitializeComponent();
        _timer.Tick += async (_, _) =>
        {
            switch (await Analyze(await _capture.TryCaptureOrderInfo()))
            {
                case SuccessResult(var user, var order):
                    {
                        if (_deck.Count == 0) break;
                        var ordered = Order.List.MinBy(o => Algo.LevenshteinRate(o.Name, order));
                        if (ordered == _reminds.First().Order)
                        {
                            _cursor++;
                            var popped = _reminds.First();
                            _reminds.RemoveAt(0);
                            _reminds.Add(_deck.First());
                            _deck.RemoveAt(0);
                            _results.Add(new ResultItem(popped.Pic, popped.Order, popped.Deviation));
                        }
                        break;
                    }
                case FailureResult(var raw) when raw != string.Empty: break;
            };
        };
        _timer.Start();
    }

    private static Task<AnalyzeResult> Analyze(string raw)
    {
        var orderedRegex = new Regex("(.+)がオーダー(.+)を準備");
        var match = orderedRegex.Match(raw);

        return Task.FromResult<AnalyzeResult>(match.Success
            ? new SuccessResult(match.Groups[1].Value, match.Groups[2].Value)
            : new FailureResult(raw));
    }

    private abstract record AnalyzeResult;

    private record SuccessResult(string User, string Order) : AnalyzeResult;

    private record FailureResult(string Raw) : AnalyzeResult;

    private void LoadComboBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox box) return;

        if (!Directory.Exists(Director.DeckDir()))
        {
            Director.CreateDirectory(Director.DeckDir());
        }
        var decks = Directory.GetFiles(Director.DeckDir(), "*.json").Select(path =>
        {
            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            var json = sr.ReadToEnd();
            return JsonSerializer.Deserialize<DeckJson>(json);
        }).ToList();

        box.ItemsSource = decks;
    }

    private void LoadButton_OnClick(object sender, RoutedEventArgs e)
    {
        var deck = DeckLoadBox.SelectedItem.As<DeckJson>();

        _deck = deck.Items.Select(item => (TimeTableItem)item).ToList();
        _reminds.Clear();
        _results.Clear();

        foreach (var item in _deck.GetRange(_cursor = 0, 4))
        {
            _reminds.Add(item);
        }

        RemainderBoard.SelectedIndex = 1;

        if (((Button)sender).Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }
    }
}

internal record struct ResultItem(string Pic, Order Order, int Deviation)
{
    public string DeviationFmt => $"({DeviationFmt})";
}
