using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
using mitama.AutomateAssign;
using MitamatchOperations.Lib;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// Deck Editor Page navigated to within a Order Console Tab.
/// </summary>
public sealed partial class DeckEditorPage
{
    public static readonly int[] TimeSource = Enumerable.Range(0, 12).Select(t => t * 5).ToArray();
    private ObservableCollection<TimeTableItem> _deck = [];
    private readonly ObservableCollection<TimeTableItem> _referDeck = [];
    private ObservableCollection<Order> Sources = [.. Order.List.Where(o => o.Payed)];
    private new int Margin { get; set; } = 5;
    private MemberInfo[] _members = [];
    private readonly List<HoldOn> _holdOns = [];
    private readonly string _project = Director.ReadCache().Legion;
    private Order[] _selectedOrder = [];


    private abstract record HoldOn;
    private record ChangeMargin(int Margin) : HoldOn;
    private record ChangePic(string Name) : HoldOn;
    private record RemovePic : HoldOn;
    private record IntoConditional : HoldOn;

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

        DeckGrid1.Background = DeckGrid2.Background = ((FrameworkElement)Content).ActualTheme == ElementTheme.Dark
            ? new AcrylicBrush { TintColor = Color.FromArgb(90, 200, 200, 200) }
            : new AcrylicBrush { TintColor = Color.FromArgb(90, 20, 20, 20) };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _members = Util.LoadMembersInfo(_project);
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
            var ordered = Order.Of(int.Parse(button.AccessKey));
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
            _deck.Add(new TimeTableItem(ordered, 0, 15 * 60, 15 * 60 - ordered.PrepareTime - ordered.ActiveTime));
        }
        else
        {
            var prev = _deck.Last();
            var prepareTime = prev.Order.Index switch
            {
                117 => 5, // レギオンマッチスキル準備時間短縮Lv.3
                _ => ordered.PrepareTime
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
            End = 15 * 60 - first.Order.PrepareTime - first.Order.ActiveTime
        };
        _deck.Add(previous);

        foreach (var item in table.Skip(1))
        {
            var prepareTime = previous.Order.Index switch
            {
                117 => 5, // レギオンマッチスキル準備時間短縮Lv.3
                _ => item.Order.PrepareTime
            };
            previous = item with
            {
                Start = previous.End - item.Delay,
                End = (previous.End - item.Delay) - prepareTime - item.Order.ActiveTime
            };
            _deck.Add(previous);
        }
        OrderDeck.ItemsSource = _deck;

        if (_referDeck.Count == 0) return;
        table = [.. _referDeck];
        _referDeck.Clear();
        first = table.First();
        previous = first with
        {
            Start = 15 * 60 - first.Delay,
            End = 15 * 60 - first.Order.PrepareTime - first.Order.ActiveTime
        };
        _referDeck.Add(previous);

        foreach (var item in table.Skip(1))
        {
            var prepareTime = previous.Order.Index switch
            {
                117 => 5, // レギオンマッチスキル準備時間短縮Lv.3
                _ => item.Order.PrepareTime
            };
            previous = item with
            {
                Start = previous.End - item.Delay,
                End = (previous.End - item.Delay) - prepareTime - item.Order.ActiveTime
            };
            _referDeck.Add(previous);
        }
        ReferOrderDeck.ItemsSource = _referDeck;
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

        _selectedOrder = e.Items.Select(item => (Order)item).ToArray();

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

    private void View_Drop(object sender, DragEventArgs e)
    {
        var name = sender switch
        {
            ListView list => list.Name,
            GridView grid => grid.Name,
            _ => throw new UnreachableException("Unreachable")
        };
        var def = e.GetDeferral();

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
                    if (e.Data.Properties.TryGetValue("From", out object from))
                    {
                        var key = from.As<ListView>().AccessKey;
                        var deck = key == "Ally"
                            ? ref _deck
                            : ref _referDeck;
                        foreach (var order in _selectedOrder)
                        {
                            deck.Remove(TimeTableItem.Proxy(order));
                            if (key == "Ally")
                            {
                                Push(Sources, order, (x, y) => x.Index > y.Index);
                            }
                        }
                    }

                    break;
                }
            case "OrderDeck":
                {
                    var pos = e.GetPosition(OrderDeck);
                    var index = 0;

                    if (OrderDeck.Items.Count != 0)
                    {

                        var topItem = OrderDeck.ContainerFromIndex(0).As<ListViewItem>();
                        var itemHeight = topItem.ActualHeight + topItem.Margin.Top + topItem.Margin.Bottom;
                        index = Math.Min(OrderDeck.Items.Count - 1, (int)(pos.Y / itemHeight));
                        var targetItem = OrderDeck.ContainerFromIndex(index).As<ListViewItem>();
                        var positionInItem = e.GetPosition(targetItem);

                        if (positionInItem.Y > itemHeight / 2)
                        {
                            index++;
                        }

                        index = Math.Min(OrderDeck.Items.Count, index);
                    }
                    _selectedOrder = _selectedOrder.Where(item => !_deck.Contains(item)).ToArray();
                    foreach (var item in _selectedOrder.Reverse())
                    {
                        Sources.Remove(item);
                        var margin = index == 0 ? 0 : Margin;
                        _deck.Insert(index, (TimeTableItem)item with { Delay = margin });
                    }

                    break;
                }
            case "ReferOrderDeck":
                {
                    var pos = e.GetPosition(OrderDeck);
                    var index = 0;

                    if (ReferOrderDeck.Items.Count != 0)
                    {

                        var topItem = ReferOrderDeck.ContainerFromIndex(0).As<ListViewItem>();
                        var itemHeight = topItem.ActualHeight + topItem.Margin.Top + topItem.Margin.Bottom;
                        index = Math.Min(ReferOrderDeck.Items.Count - 1, (int)(pos.Y / itemHeight));
                        var targetItem = ReferOrderDeck.ContainerFromIndex(index).As<ListViewItem>();
                        var positionInItem = e.GetPosition(targetItem);

                        if (positionInItem.Y > itemHeight / 2)
                        {
                            index++;
                        }

                        index = Math.Min(ReferOrderDeck.Items.Count, index);
                    }

                    _selectedOrder = _selectedOrder.Where(item => !_referDeck.Contains(item)).ToArray();
                    foreach (var item in _selectedOrder.Reverse())
                    {
                        var margin = index == 0 ? 0 : Margin;
                        _referDeck.Insert(index, (TimeTableItem)item with { Delay = margin });
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

        _selectedOrder = e.Items.Select(item => ((TimeTableItem)item).Order).ToArray();
        e.Data.RequestedOperation = DataPackageOperation.Move;
        e.Data.Properties.Add("From", sender);
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
            case "無課金":
                {
                    foreach (var item in Sources.Where(item => !item.Payed).ToArray())
                    {
                        Sources.Remove(item);
                    }
                    break;
                }
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
            case "無課金":
                {
                    foreach (var item in Order.List.Where(item => !item.Payed))
                        Sources.Add(item);
                    break;
                }
            case "属性":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.ElementalOrders : Order.ElementalOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "バフ":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.BuffOrders : Order.BuffOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "デバフ":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.DeBuffOrders : Order.DeBuffOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "MP":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.MpOrders : Order.MpOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "発動率":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.TriggerRateFluctuationOrders : Order.TriggerRateFluctuationOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "再編":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.FormationOrders : Order.FormationOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "盾":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.ShieldOrders : Order.ShieldOrders.Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            case "その他":
                {
                    foreach (var item in (bool)NotPayedCheckBox.IsChecked ? Order.StackOrders.Concat(Order.OtherOrders) : Order.StackOrders.Concat(Order.OtherOrders).Where(o => o.Payed))
                        Sources.Add(item);
                    break;
                }
            default:
                {
                    throw new UnreachableException("Unreachable");
                }
        }
        Sources = [.. Sources.DistinctBy(o => o.Index)];
        Sources.Sort((a, b) => b.Index.CompareTo(a.Index));
        OrderSources.ItemsSource = Sources;
    }

    private void TimelineFlyoutConfirmationButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var _ = new Defer(delegate
        {
            _holdOns.Clear();
            return Task.CompletedTask;
        });

        var order = Order.Of(int.Parse(button.AccessKey));
        var target = _deck[_deck.IndexOf(order)];

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
                case IntoConditional:
                    {
                        target.Conditional = true;
                        break;
                    }
            }
        }

        ReCalcTimeTable();

        if (button.Parent is Grid { Parent: FlyoutPresenter { Parent: Popup popup } })
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

        var elemental = selected.Where(order => order.Kind is Elemental).Select(order => (Elemental)order.Kind).ToArray();
        var distinct = elemental.Distinct().ToArray();

        button.IsEnabled = elemental.Length == distinct.Length;
    }
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DeckName.Text.Length == 0) return;
        var dt = DateTime.Now;
        var proxy = new DeckJson(DeckName.Text, dt, _deck.Select(item => (DeckJsonProxy)item).ToArray());
        var jsonStr = JsonSerializer.Serialize(proxy);
        if (!Directory.Exists(Director.DeckDir(_project)))
        {
            Director.CreateDirectory(Director.DeckDir(_project));
        }
        var file = @$"{Director.DeckDir(_project)}\{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}-{dt.Hour:00}{dt.Minute:00}{dt.Second:00}.json";
        using var fs = Director.CreateFile(file);
        var save = new UTF8Encoding(true).GetBytes(jsonStr);
        fs.Write(save, 0, save.Length);

        if (sender.As<Button>().Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }

        OnDeckSavedTips.Subtitle = file;
        OnDeckSavedTips.IsOpen = true;
    }

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
        _deck.Clear();
        Sources.Clear();
        Sources = [.. ((bool)NotPayedCheckBox.IsChecked ? Order.List : Order.List.Where(o => o.Payed))];
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
        var index = int.Parse(button.AccessKey);

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

                                if (Math.Abs(existIndex - addedIndex) == 0)
                                {
                                    OnValidateChangesOnHold(button.Parent.As<StackPanel>().Parent.As<StackPanel>().Parent.As<Grid>().Children, new Disable("クロノを挟まずに2回の担当が割り振られいます"));
                                    break;
                                }

                                var span = (existIndex > addedIndex) switch
                                {
                                    true => _deck.ToList().GetRange(addedIndex, existIndex - addedIndex),
                                    false => _deck.ToList().GetRange(existIndex, addedIndex - existIndex),
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
                    return Task.CompletedTask;
                })
            });
        }
        button.Flyout = flyout;
    }

    private void RemovePicButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button) return;
        _holdOns.RemoveAll(item => item is ChangePic);
        _holdOns.Add(new RemovePic());
    }

    private void MarginComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox) return;
        _holdOns.RemoveAll(item => item is ChangeMargin);
        _holdOns.Add(new ChangeMargin(int.Parse(e.AddedItems[0].ToString()!)));
    }

    private void ConditionalCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox) return;
        _holdOns.RemoveAll(item => item is ChangeMargin);
        _holdOns.Add(new IntoConditional());
    }

    private abstract record Validated
    {
        internal abstract (bool, string) IntoTuple();
    }

    private void ResetButton_Click(object _sender, RoutedEventArgs _e) => _deck.Clear();

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

    private void DeckItemFlyout_OnClosed(object sender, object e)
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

        dialog.PrimaryButtonCommand = new Defer(async delegate
        {
            try
            {
                switch (AutomateAssign.AutomateAssign.ExecAutoAssign(_project, ref _deck))
                {
                    case Failure(var msg):
                        {
                            GeneralInfoBar.IsOpen = true;
                            GeneralInfoBar.Title = msg;
                            GeneralInfoBar.Severity = InfoBarSeverity.Error;
                            await Task.Delay(2000);
                            GeneralInfoBar.IsOpen = false;
                            break;
                        }
                    case Success(var candidates):
                        {
                            int selected = 0;
                            void hook(int index)
                            {
                                selected = index;
                            }
                            var newDialog = new DialogBuilder(XamlRoot)
                                .WithTitle(@$"{candidates.Count} 通りの候補が見つかりました")
                                .WithBody(new AutoAssignmentDialogContent([.. _deck], candidates, hook))
                                .WithPrimary("実行", new Defer(async delegate {
                                    foreach (var (pic, index) in candidates[selected].Select((x, i) => (x, i)))
                                    {
                                        _deck[index] = _deck[index] with { Pic = pic };
                                    }
                                    GeneralInfoBar.IsOpen = true;
                                    GeneralInfoBar.Title = "Successfully assigned!";
                                    GeneralInfoBar.Severity = InfoBarSeverity.Success;
                                    await Task.Delay(2000);
                                    GeneralInfoBar.IsOpen = false;
                                }))
                                .WithSecondary("ランダム",new Defer(async delegate {
                                    Random engine = new();
                                    selected = engine.Next(candidates.Count);
                                    foreach (var (pic, index) in candidates[selected].Select((x, i) => (x, i)))
                                    {
                                        _deck[index] = _deck[index] with { Pic = pic };
                                    }
                                    GeneralInfoBar.IsOpen = true;
                                    GeneralInfoBar.Title = "Successfully assigned!";
                                    GeneralInfoBar.Severity = InfoBarSeverity.Success;
                                    await Task.Delay(2000);
                                    GeneralInfoBar.IsOpen = false;
                                }))
                                .WithCancel("やっぱりやめる")
                                .Build();
                            dialog.Closed += async (_s, _a) => await newDialog.ShowAsync();
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                GeneralInfoBar.IsOpen = true;
                GeneralInfoBar.Title = e.ToString();
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                await Task.Delay(2000);
                GeneralInfoBar.IsOpen = false;
            }
            OrderDeck.ItemsSource = _deck;
        });

        await dialog.ShowAsync();
    }

    private void Margin_OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(GlobalMargin.Text, out var seconds)) return;
        foreach (var item in _deck)
        {
            if (item.Delay == Margin) item.Delay = seconds;
        }
        Margin = seconds;
        ReCalcTimeTable();
    }

    private void DeckLoadBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        DeckLoadButton.IsEnabled = true;
    }

    private async void Delete_OnClick(object sender, RoutedEventArgs e)
    {
        var content = new StackPanel();

        foreach (var (path, name) in Directory.GetFiles(Director.DeckDir(_project)).Select(path =>
                 {
                     using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                     var json = sr.ReadToEnd();
                     var deck = JsonSerializer.Deserialize<DeckJson>(json);
                     return (path, deck.Name);
                 }))
        {
            content.Children.Add(new CheckBox
            {
                Content = name,
                AccessKey = path
            });
        }

        var dialog = new DialogBuilder(XamlRoot)
            .WithTitle("削除するデッキを選ぶ")
            .WithBody(content)
            .WithPrimary("消しさる")
            .WithCancel("やっぱりやめる")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            foreach (var box in content.Children.Where(item => item is CheckBox box && (box.IsChecked ?? false)))
            {
                File.Delete(box.AccessKey);
            }
            return Task.CompletedTask;
        });

        await dialog.ShowAsync();
    }
}

internal record struct DeckJson(string Name, DateTime DateTime, DeckJsonProxy[] Items)
{
    internal readonly string Display =>
        @$"{Name} ({DateTime.Year}-{DateTime.Month}-{DateTime.Day}-{DateTime.Hour}:{DateTime.Minute})";
};

internal record struct DeckJsonProxy(int Index, int Delay, int Start, int End, string Pic, bool Conditional, int? Version = 2)
{
    private readonly static Dictionary<int, int> LegacyToV2 = Order
        .List
        .Where(o => o.Payed)
        .Reverse()
        .Select((order, index) => (order, index))
        .ToDictionary(pair => pair.index, pair => pair.order.Index);
    public static implicit operator DeckJsonProxy(TimeTableItem item)
        => new(item.Order.Index, item.Delay, item.Start, item.End, item.Pic, item.Conditional);
    public static implicit operator TimeTableItem(DeckJsonProxy item)
        => item.Version == 2
        ? new(Order.Of(item.Index), item.Delay, item.Start, item.End, item.Pic, item.Conditional)
        : new(Order.Of(LegacyToV2[item.Index]), item.Delay, item.Start, item.End, item.Pic, item.Conditional);
}

public record TimeTableItem(Order Order, int Delay, int Start, int End, string Pic = "", bool Conditional = false)
{
    private static string TimeFormat(int time)
    {
        var min = time / 60;
        var sec = time % 60;

        if (min < 0 || sec < 0)
        {
            return $"-{Math.Abs(min):00}:{Math.Abs(sec):00}";
        }

        return $" {min:00}:{sec:00}";
    }
    internal string StartTime => TimeFormat(Start);
    internal string EndTime => TimeFormat(End);
    internal string PicFmt => Pic != "" ? $"[{Pic}]" : "";
    internal string PicPlaceholder => Pic != "" ? $"{Pic}" : "Select";

    internal int Delay { get; set; } = Delay;
    internal int DelayIndex = Delay / 5;
    internal string Pic { get; set; } = Pic;
    internal bool Conditional { get; set; } = Conditional;

    bool IEquatable<TimeTableItem>.Equals(TimeTableItem other) => Order.Index == other?.Order.Index;

    public static implicit operator TimeTableItem(Order order) => new(order, 0, 0, 0);

    internal static TimeTableItem Proxy(Order order) => new(order, 0, 0, 0);
};
