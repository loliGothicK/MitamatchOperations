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
/// Control Dashboard Page navigated to within a Main Page.
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
    private readonly string? _user = Director.ReadCache().User;
    private readonly string _project = Director.ReadCache().Region;
    private bool _picFlag = true;

    private bool _reFormation;

    public bool IsCtrlKeyPressed { get; set; }

    private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) IsCtrlKeyPressed = true;
        else if (IsCtrlKeyPressed && e.Key == VirtualKey.Q)
        {
            ManualTrigger();
        }
    }

    private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) IsCtrlKeyPressed = false;
    }

    public ControlDashboardPage()
    {
        InitializeComponent();

        _timer.Tick += async (_, _) =>
        {
            #region Window Capture Initialisation
            if (_capture == null)
            {
                try
                {
                    _capture = new WindowCapture(Search.WindowHandleFromCaption("Assaultlily"));
                    if (InfoBar.AccessKey == "ERROR")
                    {
                        InfoBar.AccessKey = "SUCCESS";
                        InfoBar.IsOpen = false;
                        InfoBar.Content = null;
                    }
                }
                catch
                {
                    if (InfoBar.AccessKey != "ERROR")
                    {
                        InfoBar.IsOpen = true;
                        InfoBar.AccessKey = "ERROR";
                        InfoBar.Severity = InfoBarSeverity.Error;
                        InfoBar.Title = "ラスバレのウィンドウが見つかりませんでした、起動して最前面の状態にしてください";

                        var menu = new MenuFlyout { Placement = FlyoutPlacementMode.Bottom };
                        foreach (var (caption, handle) in Search.GetWindowList())
                        {
                            if (caption == string.Empty) continue;
                            var item = new MenuFlyoutItem
                            {
                                Text = caption,
                                Command = new Defer(delegate
                                {
                                    _capture = new WindowCapture(handle);
                                    InfoBar.AccessKey = "SUCCESS";
                                    InfoBar.IsOpen = false;
                                    InfoBar.Content = null;
                                }),
                            };
                            menu.Items.Add(item);
                        }

                        InfoBar.Content = new DropDownButton
                        {
                            Content = "または画面を選択する",
                            Flyout = menu,
                        };
                    }
                }
                return;
            }
            #endregion

            #region Text Recognition
            switch (await Analyze(await _capture!.TryCaptureOrderInfo()))
            {
                case SuccessResult(_, var order):
                    {
                        if (_reminds.Count == 0) break;
                        var ordered = Order.List.MinBy(o => Algo.LevenshteinRate(o.Name, order));
                        if (ordered == _reminds.First().Order)
                        {
                            Update();
                        }
                        break;
                    }
                case FailureResult(var raw) when raw != string.Empty: break;
            }
            #endregion

            #region Next Reminder Checking
            if (_reminds.Count > 0 && _nextTimePoint - DateTime.Now <= new TimeSpan(0, 0, 0, 10))
            {
                if (_reminds.First().Start == 15u * 60u) return;
                InfoBar.IsOpen = true;
                var flag = InfoBar.Severity == InfoBarSeverity.Warning;
                InfoBar.Severity = _nextTimePoint - DateTime.Now >= new TimeSpan() ? InfoBarSeverity.Warning : InfoBarSeverity.Error;
                InfoBar.Title =
                    $"{_reminds.First().Pic} さんの {_reminds.First().Order.Name} 発動まであと {(_nextTimePoint - DateTime.Now).Seconds} 秒";
                if (_picFlag && _user == _reminds.First().Pic)
                {
                    _picFlag = false;
                    PlayAlert(ElementSoundKind.Hide, ElementSoundKind.Invoke, ElementSoundKind.Show, ElementSoundKind.Invoke);
                }
                if (flag && InfoBar.Severity == InfoBarSeverity.Error) PlayAlert(ElementSoundKind.Hide);
            }
            else
            {
                InfoBar.IsOpen = false;
            }
            #endregion

            #region Re-Formation Information
            if (_reminds.Count > 0 && _reminds.First().Order.Kind == Kinds.Formation)
            {
                FormationInfoBar.IsOpen = true;
                FormationInfoBar.Severity = InfoBarSeverity.Informational;
                FormationInfoBar.Title = $"次は {_reminds.First().Order.Name} です";

                if (_nextTimePoint - DateTime.Now > new TimeSpan(0, 0, 0, 15)) return;
                FormationInfoBar.Severity = InfoBarSeverity.Warning;
                FormationInfoBar.Title = $"{_reminds.First().Order.Name} 発動まであと {(_nextTimePoint - DateTime.Now).Seconds} 秒";
                if (_reFormation) return;
                PlayAlert(ElementSoundKind.Show);
                _reFormation = true;
            }
            else
            {
                FormationInfoBar.IsOpen = false;
                _reFormation = false;
            }
            #endregion
        };
        _timer.Start();
    }

    private void Update()
    {
        var popped = _reminds.First();
        var now = DateTime.Now;
        _firstTimePoint ??= now;
        var totalTime = (int)(popped.Order.PrepareTime + popped.Order.ActiveTime);
        _nextTimePoint = now + new TimeSpan(0, 0, totalTime / 60, totalTime % 60);
        var span = now - _firstTimePoint;
        var deviation = span.Value.Minutes * 60 + span.Value.Seconds - (15 * 60 - popped.Start);
        _reminds.RemoveAt(0);
        if (_deck.Count > _cursor) _reminds.Add(_deck[_cursor++]);

        if (_reminds.Count > 0 && _reminds.First().Conditional && deviation >= 10)
        {
            var skip = _reminds.First();
            _reminds.RemoveAt(0);
            if (_deck.Count > _cursor) _reminds.Add(_deck[_cursor++]);
            ConditionalOrderInfo.IsOpen = true;
            ConditionalOrderInfo.Severity = InfoBarSeverity.Warning;
            ConditionalOrderInfo.Title = $"{skip.Order.Name} はスキップしてください";
        }
        else
        {
            ConditionalOrderInfo.IsOpen = false;
        }

        _results.Insert(0, new ResultItem(popped.Pic, popped.Order, (int)deviation));

        RemainderBoard.ItemsSource = _reminds;
        ResultBoard.ItemsSource = _results;
        RemainderBoard.SelectedIndex = 0;
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

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record SuccessResult(string User, string Order) : AnalyzeResult;

    private record FailureResult(string Raw) : AnalyzeResult;

    private void LoadComboBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox box) return;

        if (!Directory.Exists(Director.DeckDir(_project)))
        {
            Director.CreateDirectory(Director.DeckDir(_project));
        }
        var decks = Directory.GetFiles(Director.DeckDir(_project), "*.json").Select(path =>
        {
            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            var json = sr.ReadToEnd();
            return JsonSerializer.Deserialize<DeckJson>(json);
        }).ToList();

        box.ItemsSource = decks;
    }

    private async void LoadButton_OnClick(object sender, RoutedEventArgs e)
    {
        var deck = DeckLoadBox.SelectedItem.As<DeckJson>();

        _deck = deck.Items.Select(item => (TimeTableItem)item).ToList();

        // Initialisation
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

        TeachingInfoBar.IsOpen = true;
        await Task.Delay(2000);
        TeachingInfoBar.IsOpen = false;
    }

    private void ManualTriggerButton_OnClick(object sender, RoutedEventArgs e)
    {
        ManualTrigger();
    }

    private void ManualTrigger()
    {
        if (_reminds.Count == 0) return;
        Update();
    }

    private static async void PlayAlert(ElementSoundKind soundKind)
    {
        ElementSoundPlayer.State = ElementSoundPlayerState.On;

        ElementSoundPlayer.Play(soundKind);
        await Task.Delay(400);
        ElementSoundPlayer.Play(soundKind);
        await Task.Delay(400);
        ElementSoundPlayer.Play(soundKind);
        await Task.Delay(800);

        ElementSoundPlayer.State = ElementSoundPlayerState.Off;
    }

    private static async void PlayAlert(params ElementSoundKind[] soundKinds)
    {
        ElementSoundPlayer.State = ElementSoundPlayerState.On;

        foreach (var soundKind in soundKinds)
        {
            ElementSoundPlayer.Play(soundKind);
            await Task.Delay(400);
        }
        await Task.Delay(400);

        ElementSoundPlayer.State = ElementSoundPlayerState.Off;
    }

    private void DeckLoadBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadButton.IsEnabled = true;
    }
}

internal record struct ResultItem(string Pic, Order Order, int Deviation)
{
    public readonly string DeviationFmt => $"({Deviation})";
}
