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
using System.Threading;
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
using mitama.Domain.OrderKinds;
using mitama.Pages.ControlDashboard;
using SimdLinq;
using MitamatchOperations;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace mitama.Pages;

internal enum Windows
{
    Main,
    SubPc,
    SubMobile,
}

/// <summary>
/// Control Dashboard Page navigated to within a Main Page.
/// </summary>
public sealed partial class ControlDashboardPage
{
    // �E�B���h�E�L���v�`���̂��߂̃��\�[�X���i��
    private WindowCapture? _capture;
    private WindowCapture? _subCapture;

    // �E�B���h�E�L���v�`���̂��߂̃��b�`
    private readonly CountdownEvent _captureEvent = new(1);
    private readonly CountdownEvent _subCaptureEvent = new(1);
    // �T�u�X�N���C�o�[�̃X�P�W���[��
    private readonly HistoricalScheduler[] _schedulers = { new(), new(), new(), new(), new() };
    
    // �I�[�_�[�̕\�����s�����߂̃R���N�V����
    private readonly ObservableCollection<TimeTableItem> _reminds = new();
    private readonly ObservableHashSet<ResultItem> _results = new();
    
    // �X�P�W���[����i�߂邽�߂̃^�C�}�[
    private static readonly Lazy<DispatcherTimer> Timer = new(() => 
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(40)
        };
        timer.Start();
        return timer;
    });

    private readonly Dictionary<Windows, ((int, int), (int, int))> _opOrderIconPos = new()
    {
        // ���C��PC
        [Windows.Main] = ((1800, 620), (120, 120)),
        // ��������[�X�}�z
        [Windows.SubMobile] = ((1610, 840), (80, 80)),
        // ��������[PC
        [Windows.SubPc] = ((1485, 545), (70, 70)),
    };
    private readonly Dictionary<Windows, ((int, int), (int, int))> _opOrderActivatePos = new()
    {
        // ���C��PC
        [Windows.Main] = ((260, 120), (500, 120)),
        // ��������[�X�}�z
        [Windows.SubMobile] = ((1110, 160), (600, 520)),
        // ��������[PC
        [Windows.SubPc] = ((1100, 250), (540, 520)),
    };
    private readonly Dictionary<Windows, ((int, int), (int, int))> _allyOrderInfoPos = new()
    {
        // ���C��PC
        [Windows.Main] = ((260, 120), (500, 120)),
        // ��������[�X�}�z
        [Windows.SubMobile] = ((450, 170), (350, 70)),
        // ��������[PC
        [Windows.SubPc] = ((450, 170), (350, 70)),
    };

    // �Ȃ�₩���Ŏg����ԕϐ�
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
    private bool _useSub;

    public bool IsCtrlKeyPressed { get; set; }

    private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Control) IsCtrlKeyPressed = true;
        else switch (IsCtrlKeyPressed)
        {
            case true when e.Key == VirtualKey.Q:
                ManualTrigger();
                break;
            case true when e.Key == VirtualKey.C:
                _orderStat = new None();
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

        // �S�̂̃E�B���h�E�L���v�`��
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[0])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(delegate
            {
                Task.Run(async () =>
                {
                    _captureEvent.Reset(1);
                    await _capture!.SnapShot();
                    _captureEvent.Signal();
                });
                if (_useSub)
                {
                    Task.Run(async () =>
                    {
                        _subCaptureEvent.Reset(1);
                        await _subCapture!.SnapShot();
                        _subCaptureEvent.Signal();
                    });
                }
            });

        // ����̃I�[�_�[����ǎ��
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[1])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate
            {
                _captureEvent.Wait();
                // �I�[�_�[����ǎ��
                var cap = await _capture!.CaptureOrderInfo();
                var info = Order.List
                        .Select(order => (order, Algo.LevenshteinRate(order.Name, cap)))
                        .Where(item => item.Item2 < 0.6)
                        .ToArray();

                if (info.Length <= 0) return;
                // �ǎ�ꂽ���߁A�I�[�_�[�����X�g�A����
                var res = info.MinBy(item => item.Item2);
                if (_opOrderInfo == null || res.Item2 < _opOrderInfo?.Item2)
                {
                    _opOrderInfo = ((Order, int)?)res;
                }
            });

        // �����̃I�[�_�[����ǎ��
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[2])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate
            {
                _captureEvent.Wait();
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
                    case FailureResult when _useSub:
                    {
                        _subCaptureEvent.Wait();
                        switch (await Analyze(await _subCapture!.TryCaptureOrderInfo(_allyOrderInfoPos[Windows.SubMobile].Item1, _allyOrderInfoPos[Windows.SubMobile].Item2)))
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
                        }
                        break;
                    }
                }
            });

        // ����̃I�[�_�[�� ����/����/�I�� ���X�L��������
        Observable.Interval(TimeSpan.FromMilliseconds(200), _schedulers[3])
            // ReSharper disable once AsyncVoidLambda
            .Subscribe(async delegate { await OrderScan(); });

        // �����I�[�_�[�̔����O�ʒm���o��
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
                        $"{_reminds.First().Pic} ����� {_reminds.First().Order.Name} �����܂ł��� {(_nextTimePoint - DateTime.Now).Seconds} �b";
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

        Timer.Value.Tick += async delegate
        {
            if (_capture != null && (!_useSub || _subCapture != null))
            {
                foreach (var scheduler in _schedulers)
                {
                    scheduler.AdvanceBy(TimeSpan.FromMilliseconds(40));
                }
            }
            else if (InitBar.AccessKey != "SUCCESS")
            {
                try
                {
                    await Init(Search.WindowHandleFromCaption("Assaultlily"), out _capture);
                }
                catch
                {
                    if (InitBar.AccessKey != "ERROR")
                    {
                        InitBar.IsOpen = true;
                        InitBar.AccessKey = "ERROR";
                        InitBar.Severity = InfoBarSeverity.Error;
                        InitBar.Title = "���X�o���̃E�B���h�E��������܂���ł����A�N�����čőO�ʂ̏�Ԃɂ��Ă�������";

                        var menu = new MenuFlyout { Placement = FlyoutPlacementMode.Bottom };
                        foreach (var (caption, handle) in Search.GetWindowList())
                        {
                            if (caption == string.Empty) continue;
                            var item = new MenuFlyoutItem
                            {
                                Text = caption,
                                Command = new Defer(async delegate { await Init(handle, out _capture); }),
                            };
                            menu.Items.Add(item);
                        }

                        InitBar.Content = new DropDownButton
                        {
                            Content = "�܂��͉�ʂ�I������",
                            Flyout = menu,
                        };
                    }
                }
            }
            else if (SubCaptureSelectBar.AccessKey != "SELECTED" && _useSub)
            {
                try
                {
                    _subCapture = new WindowCapture(Search.WindowHandleFromCaption("�������Ⴂ��FC - Discord"));
                    SubCaptureSelectBar.AccessKey = "SELECTED";
                }
                catch
                {
                    if (SubCaptureSelectBar.AccessKey != "SELECTED")
                    {
                        if (SubCaptureSelectBar.AccessKey == "NOT_SELECTED") return;
                        SubCaptureSelectBar.IsOpen = true;
                        SubCaptureSelectBar.AccessKey = "NOT_SELECTED";
                        SubCaptureSelectBar.Severity = InfoBarSeverity.Warning;
                        SubCaptureSelectBar.Title = "��������[�̉�ʂ�I�����Ă�������";

                        var menu = new MenuFlyout { Placement = FlyoutPlacementMode.Bottom };
                        foreach (var (caption, handle) in Search.GetWindowList())
                        {
                            if (caption == string.Empty) continue;
                            var item = new MenuFlyoutItem
                            {
                                Text = caption,
                                Command = new Defer(async delegate
                                {
                                    _subCapture = new WindowCapture(handle);
                                    SubCaptureSelectBar.AccessKey = "SELECTED";
                                    await _subCapture!.SnapShot();
                                    SubCaptureSelectBar.IsOpen = false;
                                }),
                            };
                            menu.Items.Add(item);
                        }

                        SubCaptureSelectBar.Content = new DropDownButton
                        {
                            Content = "��ʂ�I������",
                            Flyout = menu,
                        };
                    }
                }
            }
        };
    }

    private Task Init(IntPtr handle, out WindowCapture cap)
    {
        cap = new WindowCapture(handle);
        InitBar.AccessKey = "SUCCESS";
        InitBar.IsOpen = false;
        InitBar.Content = null;

        // IO�̃^�C�~���O�����炷���߂� 40 ms �����炷
        _schedulers[3].AdvanceBy(TimeSpan.FromMilliseconds(40));
        _schedulers[2].AdvanceBy(TimeSpan.FromMilliseconds(80));
        _schedulers[1].AdvanceBy(TimeSpan.FromMilliseconds(120));
        _schedulers[0].AdvanceBy(TimeSpan.FromMilliseconds(160));

        // �J�^���O�f�[�^�����炩���߃��������[�W�����ɂ̂��Ă������߂Ɉ�x Predict ���Ă�
        var sampleData1 = new MLOrderModel.ModelInput
        {
            ImageSource = File.ReadAllBytes(@"C:\Users\lolig\source\repos\MitamatchOperations\MitamatchOperations\MitamatchOperations\Assets\dataset\wait_or_active\active\active01.png"),
        };
        var sampleData2 = new MLActivatingModel.ModelInput
        {
            ImageSource = File.ReadAllBytes(@"C:\Users\lolig\source\repos\MitamatchOperations\MitamatchOperations\MitamatchOperations\Assets\dataset\is_activating\False\False01.png"),
        };

        _ = MLOrderModel.Predict(sampleData1);
        _ = MLActivatingModel.Predict(sampleData2);

        return Task.CompletedTask;
    }

    private Task OrderScan()
    {
        switch (_orderStat)
        {
            // ����I�[�_�[������
            case Active(var name, var point, var span):
                {
                    name = _opOrderInfo?.Item1.Name ?? name ?? "�s��";
                    span = _opOrderInfo?.Item1.ActiveTime - 1 ?? span;
                    OpponentInfoBar.IsOpen = true;
                    var spend = (DateTime.Now - point).Seconds + (DateTime.Now - point).Minutes * 60;
                    OpponentInfoBar.Title = $"����I�[�_�[: {name} => �c�� {span - spend} �b";
                    // �I�[�_�[���I���3�b�O�ɂ͕\������߂�
                    if (span - spend < 3)
                    {
                        // ���̃I�[�_�[���m�ɂނ��ď�����
                        _orderStat = new None();
                        _opOrderInfo = null;
                        OpponentInfoBar.IsOpen = false;
                    }
                    break;
                }
            // ����I�[�_�[�������ł��������ł��Ȃ�
            case None:
                {
                    _captureEvent.Wait();
                    switch (_capture!.CaptureOpponentsOrder())
                    {
                        // �I�[�_�[�����������m
                        case WaitStat(var image):
                            {
                                image.Save("C:\\Users\\lolig\\source\\repos\\MitamatchOperations\\MitamatchOperations\\MitamatchOperations\\Assets\\dataset\\wait_or_active\\wait\\debug.png");
                                _orderStat = new Waiting();
                                _orderPreparePoint = DateTime.Now;
                                break;
                            }
                        default:
                            {
                                if (!_useSub) break;
                                _subCaptureEvent.Wait();
                                switch (_subCapture!.CaptureOpponentsOrder(_opOrderIconPos[Windows.SubMobile].Item1, _opOrderIconPos[Windows.SubMobile].Item2))
                                {
                                    // �I�[�_�[�����������m
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
                    }
                    break;
                }
            // ����I�[�_�[������
            case Waiting:
                {
                    OpponentInfoBar.IsOpen = true;
                    var waitingFor = _opOrderInfo != null ? $"for {_opOrderInfo?.Item1.Name}..." : "...";
                    OpponentInfoBar.Title = $@"Waiting {waitingFor}";

                    if (_opOrderInfo != null)
                    {
                        _captureEvent.Wait();

                        switch (_capture!.IsActivating())
                        {
                            case ActiveStat:
                                {
                                    if (_opOrderInfo?.Item1.ActiveTime == 0)
                                    {
                                        _opOrderInfo = null;
                                        _orderStat = new None();
                                    }
                                    else
                                    {
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
                                    }
                                    goto End;
                                }
                            default:
                                {
                                    if (!_useSub) break;
                                    _subCaptureEvent.Wait();
                                    switch (_subCapture!.IsActivating(_opOrderActivatePos[Windows.SubMobile].Item1, _opOrderActivatePos[Windows.SubMobile].Item2))
                                    {
                                        case ActiveStat:
                                        {
                                            if (_opOrderInfo?.Item1.ActiveTime == 0)
                                            {
                                                _opOrderInfo = null;
                                                _orderStat = new None();
                                            }
                                            else
                                            {
                                                _orderStat = new Active(_opOrderInfo?.Item1.Name, DateTime.Now, _opOrderInfo?.Item1.ActiveTime - 1 ?? 0);
                                            }
                                            goto End;
                                        }
                                    }
                                    break;
                                }
                        }
                    }

                    _captureEvent.Wait();

                    if (_orderStat is not Active)
                    {
                        switch (_capture!.IsActivating())
                        {
                            case ActiveStat(var image):
                                _opOrderInfo = null;
                                image.Save("C:\\Users\\lolig\\source\\repos\\MitamatchOperations\\MitamatchOperations\\MitamatchOperations\\Assets\\dataset\\is_activating\\True\\debug.png");
                                break;
                            default:
                                {
                                    if (!_useSub) break;
                                    switch (_subCapture!.IsActivating(_opOrderActivatePos[Windows.SubMobile].Item1, _opOrderActivatePos[Windows.SubMobile].Item2))
                                    {
                                        case ActiveStat(var image):
                                            _opOrderInfo = null;
                                            image.Save("C:\\Users\\lolig\\source\\repos\\MitamatchOperations\\MitamatchOperations\\MitamatchOperations\\Assets\\dataset\\is_activating\\True\\debug.png");
                                            break;
                                        default:
                                            goto End;
                                    }
                                    goto End;
                                }
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
                52 => 5, // ���M�I���}�b�`�X�L���������ԒZ�kLv.3
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

    /// <summary>
    /// Updates the time table.
    /// </summary>
    /// <param name="user">The user who prepared the order.</param>
    /// <param name="ordered">The order in which it is prepared.</param>
    private void Update(string user, Order ordered)
    {
        var now = DateTime.Now;

        // �^�C���e�[�u���Čv�Z
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

        // �����v�Z
        _firstTimePoint ??= now;
        var totalTime = ordered.PrepareTime + ordered.ActiveTime;
        _nextTimePoint = now + new TimeSpan(0, 0, totalTime / 60, totalTime % 60);
        var span = now - _firstTimePoint;
        var deviation = span.Value.Minutes * 60 + span.Value.Seconds - (15 * 60 - _reminds.First().Start);

        // �����ς݃I�[�_�[����菜���A
        _reminds.Remove(ordered);
        // ���̃I�[�_�[������Βǉ�����
        if (_deck.Count > _cursor && _reminds.Count < 4) _reminds.Add(_deck[_cursor++]);

        // �����t���I�[�_�[������΃`�F�b�N����
        if (_reminds.Count > 0 && _reminds.First().Conditional && deviation >= 30)
        {
            var skip = _reminds.First();
            _reminds.RemoveAt(0);
            if (_deck.Count > _cursor) _reminds.Add(_deck[_cursor++]);
            ConditionalOrderInfo.IsOpen = true;
            ConditionalOrderInfo.Severity = InfoBarSeverity.Warning;
            ConditionalOrderInfo.Title = $"{skip.Order.Name} �̓X�L�b�v���Ă�������";
        }
        else
        {
            ConditionalOrderInfo.IsOpen = false;
        }

        // �������ʂɒǉ�
        _results.Add(new ResultItem(user, ordered, deviation, now));
        // ��ʍX�V
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

    // ReSharper disable once NotAccessedPositionalProperty.Local
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

    [GeneratedRegex("(.+)���I�[�_�[(.+)������")]
    private static partial Regex OrderPrepareRegex();

    private void AddScreen_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton toggle) return;
        _useSub = toggle.IsChecked == true;
    }

    private void CounterButton_OnClick(object sender, RoutedEventArgs e)
    {
        var newWindow = new Window();

        var counterView = new CounterView();
        newWindow.Content = counterView;

        // �E�B���h�E��\��
        newWindow.Activate();
    }
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