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
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ControlDashboardPage
{
    private WindowCapture? _capture;
    private readonly ObservableCollection<TimeTableItem> _reminds = new();
    private readonly ObservableCollection<ResultItem> _results = new();
    private readonly DispatcherTimer _timer = new()
    {
        Interval = new TimeSpan(0, 0, 0, 0, 200)
    };

    private int _cursor = 4;
    private List<TimeTableItem> _deck = new();
    private DateTime _nextTimePoint;
    private DateTime? _firstTimePoint;

    public bool isCtrlKeyPressed { get; set; }

    private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) isCtrlKeyPressed = true;
        else if (isCtrlKeyPressed && e.Key == VirtualKey.Q)
        {
            ManualTrigger();
        }
    }

    private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) isCtrlKeyPressed = false;
    }

    public ControlDashboardPage()
    {
        InitializeComponent();

        _timer.Tick += async (_, _) =>
        {
            if (_capture == null)
            {
                try
                {
                    _capture = new WindowCapture(Search.WindowHandleFromCaption("メディア"));
                    if (InfoBar.AccessKey == "ERROR")
                    {
                        InfoBar.AccessKey = "SUCCESS";
                        InfoBar.IsOpen = false;
                    }
                }
                catch
                {
                    InfoBar.IsOpen = true;
                    InfoBar.AccessKey = "ERROR";
                    InfoBar.Severity = InfoBarSeverity.Error;
                    InfoBar.Title = "ラスバレのウィンドウが見つかりませんでした、起動して最前面の状態にしてください";
                }
                return;
            }

            switch (await Analyze(await _capture!.TryCaptureOrderInfo()))
            {
                case SuccessResult(_, var order):
                    {
                        if (_reminds.Count == 0) break;
                        var ordered = Order.List.MinBy(o => Algo.LevenshteinRate(o.Name, order));
                        if (ordered == _reminds.First().Order)
                        {
                            var popped = _reminds.First();
                            var now = DateTime.Now;
                            _firstTimePoint ??= now;
                            var totalTime = (int)(popped.Order.PrepareTime + popped.Order.ActiveTime);
                            _nextTimePoint = now + new TimeSpan(0, 0, totalTime / 60, totalTime % 60);

                            _reminds.RemoveAt(0);
                            if (_deck.Count > ++_cursor) _reminds.Add(_deck[_cursor]);

                            var span = now - _firstTimePoint;
                            var deviation = span.Value.Minutes * 60 + span.Value.Seconds - (15 * 60 - popped.Start);
                            _results.Add(new ResultItem(popped.Pic, popped.Order, (int)deviation));

                            RemainderBoard.ItemsSource = _reminds;
                            ResultBoard.ItemsSource = _results;
                            RemainderBoard.SelectedIndex = 0;
                        }
                        break;
                    }
                case FailureResult(var raw) when raw != string.Empty:
                    {
                        InfoBar.IsOpen = true;
                        InfoBar.Severity = InfoBarSeverity.Informational;
                        InfoBar.Title = raw;
                        break;
                    }
            };
            if (_reminds.Count > 0 && _nextTimePoint - DateTime.Now <= new TimeSpan(0, 0, 0, 10))
            {
                if (_reminds.First().Start == 15u * 60u) return;
                InfoBar.IsOpen = true;
                InfoBar.Severity = InfoBarSeverity.Warning;
                InfoBar.Title =
                    $"{_reminds.First().Pic} さんの {_reminds.First().Order.Name} 発動まであと {(_nextTimePoint - DateTime.Now).Seconds} 秒";
            }
            else
            {
                InfoBar.IsOpen = false;
            }
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
        _cursor = 4;
        _firstTimePoint = null;

        foreach (var item in _deck.GetRange(0, 4))
        {
            _reminds.Add(item);
        }

        RemainderBoard.SelectedIndex = 0;

        if (((Button)sender).Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }
    }

    private void ManualTriggerButton_OnClick(object sender, RoutedEventArgs e)
    {
        ManualTrigger();
    }

    private void ManualTrigger()
    {
        if (_reminds.Count == 0) return;
        var popped = _reminds.First();
        var now = DateTime.Now;
        _firstTimePoint ??= now;
        var totalTime = (int)(popped.Order.PrepareTime + popped.Order.ActiveTime);
        _nextTimePoint = now + new TimeSpan(0, 0, totalTime / 60, totalTime % 60);

        _reminds.RemoveAt(0);
        if (_deck.Count > ++_cursor) _reminds.Add(_deck[_cursor]);

        var span = now - _firstTimePoint;
        var deviation = span.Value.Minutes * 60 + span.Value.Seconds - (15 * 60 - popped.Start);
        _results.Add(new ResultItem(popped.Pic, popped.Order, (int)deviation));

        RemainderBoard.ItemsSource = _reminds;
        ResultBoard.ItemsSource = _results;
        RemainderBoard.SelectedIndex = 0;

    }
}

internal record struct ResultItem(string Pic, Order Order, int Deviation)
{
    public readonly string DeviationFmt => $"({Deviation})";
}
