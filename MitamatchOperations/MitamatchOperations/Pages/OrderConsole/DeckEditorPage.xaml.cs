using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using mitama.Domain;
using mitama.Domain.OrderKinds;
using mitama.Pages.Common;
using WinRT;
using Microsoft.UI.Xaml.Navigation;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DeckEditorPage
{
    public static readonly int[] TimeSource = Enumerable.Range(0, 12).Select(t => t * 5).ToArray();
    private ObservableCollection<TimeTableItem> _deck = new();
    private ObservableCollection<Order> Sources { get; set; } = new();
    private new uint Margin { get; set; } = 5;
    private Domain.Member[] _members = { };
    private readonly List<HoldOn> _holdOns = new();
    private string? _loginRegion;


    private abstract record HoldOn;
    private record ChangeMargin(uint Margin) : HoldOn;
    private record ChangePic(string Name) : HoldOn;
    private record RemovePic : HoldOn;

    public DeckEditorPage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;

        ElementalCheckBox.IsChecked
            = BuffCheckBox.IsChecked
            = DeBuffCheckBox.IsChecked
            = MpCheckBox.IsChecked
            = TriggerRateFluctuationCheckBox.IsChecked
            = FormationCheckBox.IsChecked
            = ShieldCheckBox.IsChecked
            = OthersCheckBox.IsChecked = true;

        DeckGrid.Background = ((FrameworkElement)Content).ActualTheme == ElementTheme.Dark
            ? new AcrylicBrush { TintColor = Color.FromArgb(90, 200, 200, 200) }
            : new AcrylicBrush { TintColor = Color.FromArgb(90, 20, 20, 20) };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _loginRegion = Director.ReadCache().LoggedIn;
        _members = Directory.GetFiles($@"{Director.RegionDir()}\{_loginRegion}", "*.json")
            .Select(path =>
            {
                using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                var json = sr.ReadToEnd();
                return Domain.Member.FromJson(json);
            }).ToArray();
    }

    private void AddConfirmation_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;

        if (button.Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }

        if (OrderSources.SelectedItems.Count > 0)
        {
            foreach (var ordered in OrderSources.SelectedItems
                         .Select(item => (Order)item).ToArray())
            {
                Sources.Remove(ordered);
                PushOrder(ordered);
            }
        }
        else
        {
            var ordered = Order.List[uint.Parse(button.AccessKey)];
            Sources.Remove(ordered);
            PushOrder(ordered);
        }

        Update();
    }

    private void Update()
    {
        OrderSources.ItemsSource = Sources;
        OrderDeck.ItemsSource = _deck;
    }

    private void PushOrder(Order ordered)
    {
        if (_deck.Count == 0)
        {
            _deck.Add(new TimeTableItem(ordered, 0, 15 * 60, 15 * 60 - ordered.PrepareTIme - ordered.ActiveTime));
        }
        else
        {
            var prev = _deck.Last();
            var prepareTime = prev.Order.Index switch
            {
                52 => 5u, // レギオンマッチスキル準備時間短縮Lv.3
                _ => ordered.PrepareTIme
            };
            _deck.Add(new TimeTableItem(ordered, Margin, prev.End - Margin,
                prev.End - Margin - prepareTime - ordered.ActiveTime));
        }
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
            End = 15 * 60 - first.Order.PrepareTIme - first.Order.ActiveTime
        };
        _deck.Add(previous);

        foreach (var item in table.Skip(1))
        {
            var prepareTime = previous.Order.Index switch
            {
                52 => 5u, // レギオンマッチスキル準備時間短縮Lv.3
                _ => item.Order.PrepareTIme
            };
            previous = item with
            {
                Start = previous.End - item.Delay,
                End = (previous.End - item.Delay) - prepareTime - item.Order.ActiveTime
            };
            _deck.Add(previous);
        }
        OrderDeck.ItemsSource = _deck;
    }

    //===================================================================================================================
    // Fire when Deck is changed
    //===================================================================================================================

    private void OrderDeck_OnChanged(ListViewBase sender, DragItemsCompletedEventArgs args)
    {
        ReCalcTimeTable();
    }

    //===================================================================================================================
    // Drag/Drop Implementation
    //===================================================================================================================

    private void Source_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        var orders = e.Items.Select(v => (Order)v).ToArray();

        if (!orders.Where(o => o.Kind is Elemental).Select(o => o.Kind).All(CheckAvailable))
        {
            ElementalOrderWaringTips.IsOpen = true;
            e.Cancel = true;
            return;
        }

        var elemental = orders.Where(order => order.Kind is Elemental).Select(order => (Elemental)order.Kind).ToArray();
        var distinct = elemental.Distinct().ToArray();

        if (distinct.Length != elemental.Length)
        {
            ElementalOrderWaringTips.IsOpen = true;
            e.Cancel = true;
            return;
        }

        // Set the content of the DataPackage
        e.Data.SetText(string.Join(',', orders.Select(order => order.Index)));

        e.Data.RequestedOperation = DataPackageOperation.Move;
    }

    private void Target_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    private void Source_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    private async void View_Drop(object sender, DragEventArgs e)
    {
        var name = sender switch
        {
            ListView list => list.Name,
            GridView grid => grid.Name,
            StackPanel panel => panel.Name,
            _ => throw new UnreachableException("Unreachable")
        };
        var def = e.GetDeferral();

        var text = await e.DataView.GetTextAsync();
        var items = text.Split(',').Select(index => Order.List[int.Parse(index)]);

        static void Push<T>(IList<T> col, T item, Func<T, T, bool> cmp)
        {
            var i = 0;
            for (; i < col.Count; i++)
            {
                if (cmp(col[i], item))
                    break;
            }

            col.Insert(i, item);
        }

        switch (name)
        {
            // Find correct source list
            case "OrderSources":
                {
                    foreach (var order in items)
                    {
                        _deck.Remove(TimeTableItem.Proxy(order));
                        Push(Sources, order, (x, y) => x.Index > y.Index);
                    }

                    break;
                }
            case "OrderDeck" or "DeckPanel":
                {
                    foreach (var item in items)
                    {
                        Sources.Remove(item);
                        PushOrder(item);
                    }

                    break;
                }
        }

        e.AcceptedOperation = DataPackageOperation.Move;
        ReCalcTimeTable();
        OrderSources.ItemsSource = Sources;

        def.Complete();
    }

    private void Target_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items.Count != 1) return;

        e.Data.SetText(((TimeTableItem)e.Items[0]).Order.Index.ToString());
        e.Data.RequestedOperation = DataPackageOperation.Move;
    }

    private void Target_DragEnter(object sender, DragEventArgs e)
    {
        // We don't want to show the Move icon
        e.DragUIOverride.IsGlyphVisible = false;
    }

    private void Option_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox box) return;

        switch (box.Content)
        {
            case "属性":
                {
                    foreach (var item in Sources.Where(item => item.Kind is Elemental).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "バフ":
                {
                    foreach (var item in Sources.Where(item => item.Kind is Buff).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "デバフ":
                {
                    foreach (var item in Sources.Where(item => item.Kind is DeBuff).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "MP":
                {
                    foreach (var item in Sources.Where(item => item.Kind is Mp).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "発動率":
                {
                    foreach (var item in Sources.Where(item => item.Kind is TriggerRateFluctuation).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "再編":
                {
                    foreach (var item in Sources.Where(item => item.Kind is Formation).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "盾":
                {
                    foreach (var item in Sources.Where(item => item.Kind is Shield).ToArray())
                        Sources.Remove(item);
                    break;
                }
            case "その他":
                {
                    foreach (var item in Sources.Where(item => item.Kind is Stack or Other).ToArray())
                        Sources.Remove(item);
                    break;
                }
            default:
                {
                    throw new UnreachableException("Unreachable");
                }
        }

        OrderSources.ItemsSource = Sources;
    }

    private void Option_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox box) return;

        switch (box.Content)
        {
            case "属性":
                {
                    foreach (var item in Order.ElementalOrders)
                        Sources.Add(item);
                    break;
                }
            case "バフ":
                {
                    foreach (var item in Order.BuffOrders)
                        Sources.Add(item);
                    break;
                }
            case "デバフ":
                {
                    foreach (var item in Order.DeBuffOrders)
                        Sources.Add(item);
                    break;
                }
            case "MP":
                {
                    foreach (var item in Order.MpOrders)
                        Sources.Add(item);
                    break;
                }
            case "発動率":
                {
                    foreach (var item in Order.TriggerRateFluctuationOrders)
                        Sources.Add(item);
                    break;
                }
            case "再編":
                {
                    foreach (var item in Order.FormationOrders)
                        Sources.Add(item);
                    break;
                }
            case "盾":
                {
                    foreach (var item in Order.ShieldOrders)
                        Sources.Add(item);
                    break;
                }
            case "その他":
                {
                    foreach (var item in Order.StackOrders.Concat(Order.OtherOrders))
                        Sources.Add(item);
                    break;
                }
            default:
                {
                    throw new UnreachableException("Unreachable");
                }
        }

        OrderSources.ItemsSource = Sources;
    }

    private void TimelineFlyoutConfirmationButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var _ = new Defer(() => _holdOns.Clear());

        var target = _deck[_deck.IndexOf(Order.List[int.Parse(button.AccessKey)])];

        foreach (var onHold in _holdOns)
        {
            switch (onHold)
            {
                case ChangeMargin(Margin: var margin):
                    {
                        target.Delay = margin;
                        break;
                    }
                case ChangePic(Name: var pic):
                    {
                        target.Pic = pic;
                        break;
                    }
                case RemovePic:
                    {
                        target.Pic = string.Empty;
                        break;
                    }
            }
        }

        ReCalcTimeTable();

        if (button.Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }
    }

    private bool CheckAvailable(Kind kind) => kind switch
    {
        Elemental elemental => !_deck
            .Where(o => o.Order.Kind is Elemental)
            .Where(o => o.Order.Kind.As<Elemental>().Element is not Element.Special)
            .Select(o => ((Elemental)o.Order.Kind).Element)
            .Contains(elemental.Element),
        _ => true
    };

    private void ConfirmButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;

        var selected = OrderSources.SelectedItems
            .Select(item => (Order)item)
            .ToArray();

        button.IsEnabled = selected.Length == 0
            ? CheckAvailable(Order.List[int.Parse(button.AccessKey)].Kind)
            : selected.Select(o => o.Kind).All(CheckAvailable);

        if (!button.IsEnabled)
        {
            button.Content = "同系統の属性オーダーを編成済み";
            return;
        }

        var elemental = selected.Where(order => order.Kind is Elemental).Select(order => (Elemental)order.Kind).ToArray();
        var distinct = elemental.Distinct().ToArray();

        button.IsEnabled = elemental.Length == distinct.Length;

        if (button.IsEnabled) return;
        button.Content = "同系統の属性オーダーを編成済み";
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DeckName.Text.Length == 0) return;
        var dt = DateTime.Now;
        var proxy = new DeckJson(DeckName.Text, dt, _deck.Select(item => (DeckJsonProxy)item).ToArray());
        var jsonStr = JsonSerializer.Serialize(proxy);
        if (!Directory.Exists(Director.DeckDir()))
        {
            Director.CreateDirectory(Director.DeckDir());
        }
        var file = @$"{Director.DeckDir()}\{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}-{dt.Hour:00}{dt.Minute:00}{dt.Second:00}.json";
        using var fs = Director.CreateFile(file);
        var save = new UTF8Encoding(true).GetBytes(jsonStr);
        fs.Write(save, 0, save.Length);

        if (((Button)sender).Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }

        OnDeckSavedTips.Subtitle = file;
        OnDeckSavedTips.IsOpen = true;
    }

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
        _deck.Clear();
        Sources.Clear();
        Sources = new ObservableCollection<Order>(Order.List);
        foreach (var item in deck.Items.Select(item => (TimeTableItem)item))
        {
            _deck.Add(item);
            Sources.Remove(item.Order);
        }

        Update();

        if (((Button)sender).Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }
    }

    private void SelectPlayerButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not DropDownButton button) return;

        var flyout = new MenuFlyout();
        foreach (var member in _members.Select(mem => mem.Name))
        {
            flyout.Items.Add(new MenuFlyoutItem
            {
                Text = member,
                Command = new Defer(delegate
                {
                    _holdOns.RemoveAll(item => item is ChangePic or RemovePic);
                    _holdOns.Add(new ChangePic(member));
                    var index = uint.Parse(button.AccessKey);
                    UpdateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children);

                    var exists = _deck.Where(item => item.Order.Index != index)
                        .Where(item => item.Pic == member).ToArray();

                    switch (exists.Length)
                    {
                        case 2:
                            OnValidateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children, new Disable("一人に3回以上の担当が割り振られいます"));
                            break;
                        case 1:
                            {
                                var exist = exists.First();
                                var added = _deck.First(item => item.Order.Index == index);
                                var existIndex = _deck.IndexOf(exist);
                                var addedIndex = _deck.IndexOf(added);

                                if (Math.Abs(existIndex - addedIndex) == 1)
                                {
                                    OnValidateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children, new Disable("クロノを挟まずに2回の担当が割り振られいます"));
                                    break;
                                }

                                var span = (existIndex > addedIndex) switch
                                {
                                    true => _deck.ToList().GetRange(addedIndex, existIndex - addedIndex - 1),
                                    false => _deck.ToList().GetRange(existIndex, addedIndex - existIndex - 1),
                                };
                                if (span.Any(item => item.Order.Name == "刻戻りのクロノグラフ"))
                                {
                                    OnValidateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children, new Enable("適用する"));
                                }
                                else
                                {
                                    OnValidateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children, new Disable("クロノを挟まずに2回の担当が割り振られいます"));
                                }

                                break;
                            }
                        default:
                            OnValidateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children, new Enable("適用する"));
                            break;
                    }
                })
            });
        }
        button.Flyout = flyout;
    }

    private void RemovePicButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        _holdOns.RemoveAll(item => item is ChangePic);
        _holdOns.Add(new RemovePic());
        UpdateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children);
    }

    private void MarginComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox box) return;
        _holdOns.RemoveAll(item => item is ChangeMargin);
        _holdOns.Add(new ChangeMargin(uint.Parse(e.AddedItems[0].ToString()!)));
        UpdateChangesOnHold(box.Parent.As<StackPanel>().Parent.As<Grid>().Children);
    }

    private void UpdateChangesOnHold(IEnumerable<object> controls)
    {
        var msgBlock = controls
            .Where(ctrl => ctrl is TextBlock)
            .Select(ctrl => ctrl.As<TextBlock>())
            .First(block => block.Name == "ChangesOnHold");

        if (msgBlock is null) return;

        msgBlock.Text = _holdOns.Select(hold => hold switch
        {
            ChangeMargin changeMargin => $@"ディレイ => {changeMargin.Margin} 秒",
            ChangePic changePic => $@"オーダー担当 => {changePic.Name}",
            RemovePic => @"オーダー担当をリセット",
            _ => throw new ArgumentOutOfRangeException(nameof(hold), hold, null)
        }).Aggregate((a, b) => $@"{a}\n{b}");
    }

    private abstract record Validated
    {
        internal abstract (bool, string) IntoTuple();
    }

    private record Enable(string Msg) : Validated
    {
        internal override (bool, string) IntoTuple() => (true, Msg);
    }
    private record Disable(string Msg) : Validated
    {
        internal override (bool, string) IntoTuple() => (false, Msg);
    }


    private static void OnValidateChangesOnHold(IEnumerable<object> controls, Validated validated)
    {
        var (isEnabled, msg) = validated.IntoTuple();
        var confirmButton = controls
            .Where(ctrl => ctrl is Button)
            .Select(ctrl => ctrl.As<Button>())
            .First(block => block.Name == "TimelineFlyoutConfirmationButton");

        if (confirmButton is null) return;
        confirmButton.Content = msg;
        confirmButton.IsEnabled = isEnabled;
    }

    private void DeckItemFlyout_OnClosed(object? sender, object e)
    {
        _holdOns.Clear();
    }

    private async void Assign_OnClick(object sender, RoutedEventArgs args)
    {
        var builder = new DialogBuilder(XamlRoot);

        var dialog = builder
            .WithTitle("自動オーダー担当者割当てを実行しますか？")
            .WithPrimary("実行")
            .WithCancel("やっぱりやめる")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            _ = AutomateAssign.AutomateAssign.ExecAutoAssign(_loginRegion!, ref _deck);
            OrderDeck.ItemsSource = _deck;
        });

        await dialog.ShowAsync();
    }

    private void Margin_OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (!uint.TryParse(GlobalMargin.Text, out var seconds)) return;
        foreach (var item in _deck)
        {
            if (item.Delay == Margin) item.Delay = seconds;
        }
        Margin = seconds;
        ReCalcTimeTable();
    }
}

internal record struct DeckJson(string Name, DateTime DateTime, DeckJsonProxy[] Items)
{
    internal readonly string Display =>
        @$"{Name} ({DateTime.Year}-{DateTime.Month}-{DateTime.Day}-{DateTime.Hour}:{DateTime.Minute})";
};

internal record struct DeckJsonProxy(uint Index, uint Delay, uint Start, uint End, string Pic)
{
    public static implicit operator DeckJsonProxy(TimeTableItem item)
        => new(item.Order.Index, item.Delay, item.Start, item.End, item.Pic);
    public static implicit operator TimeTableItem(DeckJsonProxy item)
        => new(Order.List[item.Index], item.Delay, item.Start, item.End, item.Pic);
}

internal record TimeTableItem(Order Order, uint Delay, uint Start, uint End, string Pic = "")
{
    internal string StartTime => $"{Start / 60:00}:{Start % 60:00}";
    internal string EndTime => $"{End / 60:00}:{End % 60:00}";
    internal string PicFmt => Pic != "" ? $"[{Pic}]" : "";

    internal int Deviation
    {
        get
        {
            var target = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 00, 00) + new TimeSpan(0, 0, (int)(15 * 60 - Start));
            var actual = DateTime.Now;
            return (target < actual) switch
            {
                true => -((actual - target).Minutes * 60 + (actual - target).Seconds),
                false => ((actual - target).Minutes * 60 + (actual - target).Seconds),
            };
        }
    }

    internal uint Delay { get; set; } = Delay;
    internal string Pic { get; set; } = Pic;

    bool IEquatable<TimeTableItem?>.Equals(TimeTableItem? other) => Order.Index == other?.Order.Index;

    public static implicit operator TimeTableItem(Order order) => new(order, 0, 0, 0);

    internal static TimeTableItem Proxy(Order order) => new(order, 0, 0, 0);
};
