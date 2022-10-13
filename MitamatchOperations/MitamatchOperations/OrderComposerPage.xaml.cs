using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using mitama.OrderKinds;

namespace mitama;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderComposerPage
{
    public static readonly int[] TimeSource = Enumerable.Range(0, 12).Select(t => t * 5).ToArray();
    private ObservableCollection<TimeTableItem> Deck { get; } = new();
    private ObservableCollection<Order> Sources { get; } = new();
    private new uint Margin { get; set; } = 5;

    public OrderComposerPage()
    {
        InitializeComponent();
        ElementalCheckBox.IsChecked
            = BuffCheckBox.IsChecked
            = DeBuffCheckBox.IsChecked
            = MpCheckBox.IsChecked
            = TriggerRateFluctuationCheckBox.IsChecked
            = FormationCheckBox.IsChecked
            = ShieldCheckBox.IsChecked
            = OthersCheckBox.IsChecked = true;
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

        OrderSources.ItemsSource = Sources;
        OrderDeck.ItemsSource = Deck;
    }

    private void PushOrder(Order ordered)
    {
        if (Deck.Count == 0)
        {
            Deck.Add(new TimeTableItem(ordered, 0, 15 * 60, 15 * 60 - ordered.PrepareTIme - ordered.ActiveTime));
        }
        else
        {
            var prev = Deck.Last();
            var prepareTime = prev.Order.Index switch
            {
                52 => 5u, // レギオンマッチスキル準備時間短縮Lv.3
                _ => ordered.PrepareTIme,
            };
            Deck.Add(new TimeTableItem(ordered, Margin, prev.End - Margin,
                prev.End - Margin - prepareTime - ordered.ActiveTime));
        }
    }

    private void ReCalcTimeTable()
    {
        if (Deck.Count == 0) return;
        var table = Deck.ToArray();
        Deck.Clear();
        var first = table.First();
        var prev = first with
        {
            Start = 15 * 60 - first.Delay,
            End = 15 * 60 - first.Order.PrepareTIme - first.Order.ActiveTime
        };
        Deck.Add(prev);

        foreach (var item in table.Skip(1))
        {
            var curr = item with
            {
                Start = prev.End - item.Delay,
                End = prev.End - Margin - item.Order.PrepareTIme - item.Order.ActiveTime
            };
            Deck.Add(curr);
            prev = curr;
        }

        OrderDeck.ItemsSource = Deck;
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
            _ => throw new UnreachableException("Unreachable"),
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
                        Deck.Remove(TimeTableItem.Proxy(order));
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

        if (button.Parent is StackPanel panel)
        {
            foreach (var elem in panel.Children)
            {
                switch (elem)
                {
                    case ComboBox box:
                        {
                            if (!uint.TryParse(box.Text, out var newValue)) return;
                            var index = Deck.IndexOf(Order.List[int.Parse(box.AccessKey)]);
                            var item = Deck[index];
                            Deck[index] = item with { Delay = newValue };
                            break;
                        }
                    case TextBox box:
                        {
                            var index = Deck.IndexOf(Order.List[int.Parse(box.AccessKey)]);
                            var item = Deck[index];
                            Deck[index] = item with { Pic = box.Text };
                            break;
                        }
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
        Elemental elemental => !Deck
            .Where(o => o.Order.Kind is Elemental)
            .Select(o => ((Elemental)o.Order.Kind).Element)
            .Contains(elemental.Element),
        _ => true
    };

    private void ConfirmButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;

        var selected = OrderSources.SelectedItems
            .Select(item => (Order)item).ToArray();

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

    private void ApplyMargin_OnClick(object sender, RoutedEventArgs e)
    {
        if (!uint.TryParse(GlobalMargin.Text, out var seconds)) return;
        
        foreach (var item in Deck)
        {
            if (item.Delay == Margin) item.Delay = seconds;
        }

        Margin = seconds;
        ReCalcTimeTable();
    }
}

internal record TimeTableItem(Order Order, uint Delay, uint Start, uint End, string Pic = "")
{
    internal string StartTime => $"{Start / 60:00}:{Start % 60:00}";
    internal string EndTime => $"{End / 60:00}:{End % 60:00}";
    internal string PicFmt => Pic != "" ? $"[{Pic}]" : "";
    internal string PicPlaceholder => Pic != "" ? Pic : @"担当プレイヤーを入力してください";

    internal uint Delay { get; set; } = Delay;
    internal string Pic { get; set; } = Pic;

    bool IEquatable<TimeTableItem?>.Equals(TimeTableItem? other) => Order.Index == other?.Order.Index;

    public static implicit operator TimeTableItem(Order order) => new(order, 0, 0, 0);

    internal static TimeTableItem Proxy(Order order) => new(order, 0, 0, 0);
};
