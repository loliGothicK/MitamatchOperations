using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using mitama.Domain.OrderKinds;
using SimdLinq;
using MitamatchOperations;

namespace mitama.Pages;

/// <summary>
/// Control Dashboard Page navigated to within a Main Page.
/// </summary>
public sealed partial class ControlDashboardPage
{
    private WindowCapture? _capture;

    private readonly HistoricalScheduler[] _schedulers = { new(), new(), new(), new(), new () };
    private readonly ObservableCollection<TimeTableItem> _reminds = new();
    private readonly ObservableHashSet<ResultItem> _results = new();
    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(40)
    };

    private int _cursor = 4;
    private List<TimeTableItem> _deck = new();
    private DateTime _nextTimePoint;
    private DateTime? _firstTimePoint;
    private readonly string? _user = Director.ReadCache().User;
    private readonly string _project = Director.ReadCache().Region;
    private bool _picFlag = true;
    private OpOrderStatus _orderStat = new None();
    private DateTime _orderPreparePoint = DateTime.Now;
    private (Order, int)? _opOrderInfo;

    public bool IsCtrlKeyPressed { get; set; }

    private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) IsCtrlKeyPressed = true;
        else switch (IsCtrlKeyPressed)
        {
            case true when e.Key == VirtualKey.Q:
                ManualTrigger();
                break;
        }
    }

    private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) IsCtrlKeyPressed = false;
    }

    public ControlDashboardPage()
    {
        InitializeComponent();

        // 全体のウィンドウキャプチャ
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[0])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate
            {
                await _capture!.SnapShot();
            });

        // 相手のオーダー情報を読取る
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[1])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate
            {
                // オーダー情報を読取る
                var cap = await _capture!.CaptureOrderInfo();
                var info = Order.List
                        .Select(order => (order, Algo.LevenshteinRate(order.Name, cap)))
                        .Where(item => item.Item2 < 0.6)
                        .ToArray();

                if (info.Length <= 0) return;
                // 読取れたため、オーダー情報をストアする
                var res = info.MinBy(item => item.Item2);
                if (_opOrderInfo == null || res.Item2 < _opOrderInfo?.Item2)
                {
                    _opOrderInfo = ((Order, int)?)res;
                }
            });

        // 味方のオーダー情報を読取る
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[2])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate
            {
                switch (await Analyze(await _capture!.TryCaptureOrderInfo()))
                {
                    case SuccessResult(var user, var order):
                    {
                        if (_reminds.Count == 0) break;
                        var ordered = Order.List.MinBy(o => Algo.LevenshteinRate(o.Name, order));
                        if (_deck.Select(e => e.Order.Index).ToArray().Contains(ordered.Index)
                            && !_results.Select(r => r.Order.Index).ToArray().Contains(ordered.Index))
                        {
                            Update(user, ordered);
                        }
                        break;
                    }
                    case FailureResult(var raw) when raw != string.Empty: break;
                }
            });

        // 相手のオーダーの 準備/発動/終了 をスキャンする
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[3])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate { await OrderScan(); });

        // 味方オーダーの発動前通知を出す
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[4])
            .Subscribe(delegate
            {
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
            });

        _timer.Tick += delegate
        {
            if (_capture != null)
            {
                foreach (var scheduler in _schedulers)
                {
                    scheduler.AdvanceBy(TimeSpan.FromMilliseconds(40));
                }
            }
            else
            {
                try
                {
                    if (InitBar.AccessKey != "ERROR") return;
                    Init(Search.WindowHandleFromCaption("Assaultlily"));
                }
                catch
                {
                    if (InitBar.AccessKey != "ERROR")
                    {
                        InitBar.IsOpen = true;
                        InitBar.AccessKey = "ERROR";
                        InitBar.Severity = InfoBarSeverity.Error;
                        InitBar.Title = "ラスバレのウィンドウが見つかりませんでした、起動して最前面の状態にしてください";

                        var menu = new MenuFlyout { Placement = FlyoutPlacementMode.Bottom };
                        foreach (var (caption, handle) in Search.GetWindowList())
                        {
                            if (caption == string.Empty) continue;
                            var item = new MenuFlyoutItem
                            {
                                Text = caption,
                                Command = new Defer(delegate
                                {
                                    Init(handle);
                                    return Task.CompletedTask;
                                }),
                            };
                            menu.Items.Add(item);
                        }

                        InitBar.Content = new DropDownButton
                        {
                            Content = "または画面を選択する",
                            Flyout = menu,
                        };
                    }
                }
            }
        };
        _timer.Start();
    }

    private void Init(IntPtr handle)
    {
        _capture = new WindowCapture(handle);
        InitBar.AccessKey = "SUCCESS";
        InitBar.IsOpen = false;
        InitBar.Content = null;
        _schedulers[1].AdvanceBy(TimeSpan.FromMilliseconds(40));
        _schedulers[2].AdvanceBy(TimeSpan.FromMilliseconds(80));
        _schedulers[3].AdvanceBy(TimeSpan.FromMilliseconds(120));
        _schedulers[4].AdvanceBy(TimeSpan.FromMilliseconds(160));
        //Load sample data
        var imageBytes = File.ReadAllBytes(@"C:\Users\lolig\source\repos\MitamatchOperations\MitamatchOperations\MitamatchOperations\Assets\dataset\wait_or_active\active\active01.png");
        var sampleData = new MLOrderModel.ModelInput()
        {
            ImageSource = imageBytes,
        };

        //Load model and predict output
        _ = MLOrderModel.Predict(sampleData);

    }

    private Task OrderScan()
    {
        switch (_orderStat)
        {
            // 相手オーダー発動中
            case Active(var name, var point, var span):
                {
                    OpponentInfoBar.IsOpen = true;
                    var spend = (DateTime.Now - point).Seconds + (DateTime.Now - point).Minutes * 60;
                    OpponentInfoBar.Title = $"{name ?? "Opponent's Order"}: {span - spend} seconds remaining.";
                    // オーダーが終わる3秒前には表示をやめる
                    if (span - spend < 3)
                    {
                        // 次のオーダー検知にむけて初期化
                        _orderStat = new None();
                        _opOrderInfo = null;
                        OpponentInfoBar.IsOpen = false;
                    }
                    break;
                }
            // 相手オーダー準備中でも発動中でもない
            case None:
                {
                    switch (_capture!.CaptureOpponentsOrder())
                    {
                        // オーダー準備中を検知
                        case WaitStat(var image):
                            {
                                image.Save("C:\\Users\\lolig\\source\\repos\\MitamatchOperations\\MitamatchOperations\\MitamatchOperations\\Assets\\dataset\\wait_or_active\\wait\\debug.png");
                                _orderStat = new Waiting();
                                _orderPreparePoint = DateTime.Now;
                                break;
                            }
                    }
                    break;
                }
            // 相手オーダー準備中
            case Waiting:
                {
                    OpponentInfoBar.IsOpen = true;
                    OpponentInfoBar.Title = "Waiting...";

                    switch (_capture!.CaptureOpponentsOrder())
                    {
                        // オーダー発動検知
                        case ActiveStat(var image):
                            {
                                image.Save("C:\\Users\\lolig\\source\\repos\\MitamatchOperations\\MitamatchOperations\\MitamatchOperations\\Assets\\dataset\\wait_or_active\\active\\debug.png");
                                if (_opOrderInfo != null)
                                {
                                    _orderStat = new Active(_opOrderInfo?.Item1.Name, DateTime.Now, _opOrderInfo?.Item1.ActiveTime - 1 ?? 0);
                                }
                                else
                                {
                                    var prepareTime = (DateTime.Now - _orderPreparePoint).Seconds;
                                    _orderStat = new[] { 5, 15, 20, 30 }.MinBy(t => Math.Abs(prepareTime - t)) switch
                                    {
                                        5 => new Active(null, DateTime.Now, 120 - 1),
                                        15 => new Active(null, DateTime.Now, 60 - 1),
                                        20 => new Active(null, DateTime.Now, 80 - 1),
                                        30 => new Active(null, DateTime.Now, 120 - 1),
                                        _ => throw new UnreachableException(),
                                    };
                                    _opOrderInfo = null;
                                }

                                goto End;
                            }
                    }

                    if (_orderStat is not Active)
                    {
                        switch (_capture!.IsActivating())
                        {
                            case ActiveStat(var image):
                                image.Save("C:\\Users\\lolig\\source\\repos\\MitamatchOperations\\MitamatchOperations\\MitamatchOperations\\Assets\\dataset\\is_activaing\\True\\debug.png");
                                break;
                            default:
                                goto End;
                        }

                        if (_opOrderInfo is ({ ActiveTime: > 0 }, _))
                        {
                            _orderStat = new Active(
                                _opOrderInfo?.Item1.Name,
                                DateTime.Now,
                                _opOrderInfo?.Item1.ActiveTime ?? 0
                            );
                            _opOrderInfo = null;
                        }
                        _orderStat = new None();
                        _opOrderInfo = null;
                        OpponentInfoBar.IsOpen = false;
                    }

                    End:
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(_orderStat));
        }
        return Task.CompletedTask;
    }

    private void ReCalcTimeTable()
    {
        if (_deck.Count == 0) return;
        var table = _deck.ToArray();
        _deck.Clear();
        var first = table.First();
        var previous = first with
        {
            Start = 15 * 60 - first.Delay,
            End = 15 * 60 - first.Order.PrepareTime - first.Order.ActiveTime
        };
        _deck.Add(previous);

        foreach (var item in table.Skip(1))
        {
            var prepareTime = previous.Order.Index switch
            {
                52 => 5, // レギオンマッチスキル準備時間短縮Lv.3
                _ => item.Order.PrepareTime
            };
            previous = item with
            {
                Start = previous.End - item.Delay,
                End = (previous.End - item.Delay) - prepareTime - item.Order.ActiveTime
            };
            _deck.Add(previous);
        }
    }

    private void Update(string user, Order ordered)
    {
        var now = DateTime.Now;

        // タイムテーブル再計算
        if (_reminds.First().Order != ordered)
        {
            var idx = _deck.IndexOf(ordered);
            _deck.Insert(_cursor - _reminds.Count, _deck[idx]);
            _deck.RemoveAt(idx + 1);

            ReCalcTimeTable();
            var remaining = _reminds.Count;
            _reminds.Clear();
            foreach (var item in _deck.GetRange(_cursor - remaining, remaining))
            {
                _reminds.Add(item);
            }
        }

        if (ordered.Kind is Elemental)
        {
            var originalRemindsCount = _reminds.Count;
            foreach (var item in _deck
                         .GetRange(_cursor - _reminds.Count, _deck.Count - _cursor + _reminds.Count)
                         .Where(item => item.Order != ordered)
                         .Where(item => item.Order.Kind is Elemental)
                         .Where(item => item.Order.Kind.As<Elemental>().Element == ordered.Kind.As<Elemental>().Element)
                         .ToArray())
            {
                _deck.Remove(item);
                _reminds.Remove(item);
            }

            if (_cursor <= _deck.Count)
            {
                var remaining = _reminds.Count;
                _reminds.Clear();
                foreach (var item in _deck.GetRange(_cursor - originalRemindsCount, remaining))
                {
                    _reminds.Add(item);
                }

                ReCalcTimeTable();
            }
        }

        // 差分計算
        _firstTimePoint ??= now;
        var totalTime = ordered.PrepareTime + ordered.ActiveTime;
        _nextTimePoint = now + new TimeSpan(0, 0, totalTime / 60, totalTime % 60);
        var span = now - _firstTimePoint;
        var deviation = span.Value.Minutes * 60 + span.Value.Seconds - (15 * 60 - _reminds.First().Start);

        // 発動済みオーダーを取り除き、
        _reminds.Remove(ordered);
        // 次のオーダーがあれば追加する
        if (_deck.Count > _cursor && _reminds.Count < 4) _reminds.Add(_deck[_cursor++]);

        // 条件付きオーダーがあればチェックする
        if (_reminds.Count > 0 && _reminds.First().Conditional && deviation >= 30)
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

        // 発動結果に追加
        _results.Add(new ResultItem(user, ordered, deviation, now));
        // 画面更新
        RemainderBoard.ItemsSource = _reminds;
        ResultBoard.ItemsSource = _results.Distinct().OrderByDescending(r => r.ActivatedAt).ToList();
        RemainderBoard.SelectedIndex = 0;
    }

    private static Task<AnalyzeResult> Analyze(string raw)
    {
        var orderedRegex = OrderPrepareRegex();
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

    private void LoadButton_OnClick(object sender, RoutedEventArgs e)
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

        using var onClose = new Defer(async delegate
        {
            TeachingInfoBar.IsOpen = true;
            await Task.Delay(2000);
            TeachingInfoBar.IsOpen = false;
        });
    }

    private void ManualTriggerButton_OnClick(object sender, RoutedEventArgs e)
    {
        ManualTrigger();
    }

    private void ManualTrigger()
    {
        if (_reminds.Count == 0) return;
        Update(_reminds.First().Pic, _reminds.First().Order);
    }

    private static void PlayAlert(ElementSoundKind soundKind)
    {
        using var onClose = new Defer(async delegate
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
            ElementSoundPlayer.Play(soundKind);
            await Task.Delay(400);
            ElementSoundPlayer.Play(soundKind);
            await Task.Delay(400);
            ElementSoundPlayer.Play(soundKind);
            await Task.Delay(800);
            ElementSoundPlayer.State = ElementSoundPlayerState.Off;
        });
    }

    private static void PlayAlert(params ElementSoundKind[] soundKinds)
    {
        using var onClose = new Defer(async delegate
        {
            ElementSoundPlayer.State = ElementSoundPlayerState.On;
            foreach (var soundKind in soundKinds)
            {
                ElementSoundPlayer.Play(soundKind);
                await Task.Delay(400);
            }
            await Task.Delay(400);
            ElementSoundPlayer.State = ElementSoundPlayerState.Off;
        });
    }

    private void DeckLoadBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadButton.IsEnabled = true;
    }

    [GeneratedRegex("(.+)がオーダー(.+)を準備")]
    private static partial Regex OrderPrepareRegex();
}

internal record ResultItem(string Pic, Order Order, int Deviation, DateTime ActivatedAt)
{
    public string DeviationFmt => $"({Deviation})";
    bool IEquatable<ResultItem?>.Equals(ResultItem? other) => Order.Index == other?.Order.Index;

}

internal abstract record OpOrderStatus;
internal record Waiting : OpOrderStatus;
internal record Active(string? Name, DateTime Point, int Span): OpOrderStatus;
internal record None : OpOrderStatus;
