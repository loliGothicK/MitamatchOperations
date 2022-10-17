using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using mitama.Domain;
using mitama.Domain.OrderKinds;
using WinRT;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DeckEditorPage
{
    public static readonly int[] TimeSource = Enumerable.Range(0, 12).Select(t => t * 5).ToArray();
    private ObservableCollection<TimeTableItem> Deck { get; } = new();
    private ObservableCollection<Order> Sources { get; } = new();
    private new uint Margin { get; set; } = 5;
    private Dictionary<string, List<string>> RegionToMembersDict { get; set; } = new();

    public DeckEditorPage()
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

        InitRegionToMembersDict();
    }

    private void InitRegionToMembersDict()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var regions = Directory.GetDirectories(@$"{desktop}\MitamatchOperations\Regions").ToArray();

        foreach (var regionPath in regions)
        {
            var regionName = regionPath.Split(@"\").Last();
            var names = Directory.GetFiles(regionPath, "*.json").Select(path =>
            {
                using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                var json = sr.ReadToEnd();
                return JsonSerializer.Deserialize<OrderPossession>(json).Name;
            });
            if (!RegionToMembersDict.ContainsKey(regionName))
            {
                RegionToMembersDict[regionName] = new List<string>();
            }
            foreach (var name in names)
            {
                RegionToMembersDict[regionName].Add(name);
            }
        }
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
                _ => ordered.PrepareTIme
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
        var previous = first with
        {
            Start = 15 * 60 - first.Delay,
            End = 15 * 60 - first.Order.PrepareTIme - first.Order.ActiveTime
        };
        Deck.Add(previous);

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
            Deck.Add(previous);
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

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dt = DateTime.Now;
        var proxy = new DeckJson(DeckName.Text, dt, Deck.Select(item => (DeckJsonProxy)item).ToArray());
        var jsonStr = JsonSerializer.Serialize(proxy);
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        Director.CreateDirectory(@$"{desktop}\MitamatchOperations\decks");
        var file = @$"{desktop}\MitamatchOperations\decks\{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}-{dt.Hour:00}{dt.Minute:00}{dt.Second:00}.json";
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

        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var decks = Directory.GetFiles(@$"{desktop}\MitamatchOperations\decks", "*.json").Select(path =>
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
        Deck.Clear();
        foreach (var item in deck.Items.Select(item => (TimeTableItem)item))
        {
            Deck.Add(item);
            Sources.Remove(item.Order);
        }

        Update();
    }

    private void MarginSecondsComboBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox box) return;
        var index = Deck.IndexOf(Order.List[int.Parse(box.AccessKey)]);
        var item = Deck[index];
        box.SelectedItem = item.Delay.ToString();
    }

    private void PicEditor_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox box) return;
        var index = Deck.IndexOf(Order.List[int.Parse(box.AccessKey)]);
        var item = Deck[index];
        box.Text = item.Pic;
    }

    private void SelectPlayerButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not DropDownButton button) return;
        
        var flyout = new MenuFlyout();
        foreach (var menuFlyoutSubItem in RegionToMembersDict.Select(kv =>
                 {
                     var (region, members) = kv;
                     var subItem = new MenuFlyoutSubItem { Text = region };
                     foreach (var member in members) subItem.Items.Add(new MenuFlyoutItem { Text = member });
                     return subItem;
                 }))
        {
            flyout.Items.Add(menuFlyoutSubItem);
        }
        button.Flyout = flyout;
    }
}

internal class Director
{
    public static void CreateDirectory(string path)
    {
        using var isoStore = IsolatedStorageFile.GetStore(
            IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        try
        {
            isoStore.CreateDirectory(path);
        }
        catch (Exception e)
        {
            throw new Exception(@$"The process failed: {e}");
        }
    }

    public static IsolatedStorageFileStream CreateFile(string path)
    {
        using var isoStore = IsolatedStorageFile.GetStore(
            IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        try
        {
            return isoStore.CreateFile(path);
        }
        catch (Exception e)
        {
            throw new Exception(@$"The process failed: {e}");
        }
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
    internal string PicPlaceholder => Pic != "" ? Pic : @"担当プレイヤーを入力してください";

    internal uint Delay { get; set; } = Delay;
    internal string Pic { get; set; } = Pic;

    bool IEquatable<TimeTableItem?>.Equals(TimeTableItem? other) => Order.Index == other?.Order.Index;

    public static implicit operator TimeTableItem(Order order) => new(order, 0, 0, 0);

    internal static TimeTableItem Proxy(Order order) => new(order, 0, 0, 0);
};
