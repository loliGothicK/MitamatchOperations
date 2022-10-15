using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using mitama.OrderKinds;
using WinRT;
using mitama.Pages.Common;
using Microsoft.UI.Xaml.Shapes;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderManagerPage
{
    private ObservableCollection<Order> Sources { get; } = new();
    private ObservableCollection<Order> OrdersInPossession { get; } = new();
    public readonly List<string> Selected = new();

    public OrderManagerPage()
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

        InitRegionsMenuFlyout();
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
        OrdersInPossessionView.ItemsSource = OrdersInPossession;
    }

    private void PushOrder(Order ordered)
    {
        if (OrdersInPossession.Count == 0)
        {
            OrdersInPossession.Add(ordered);
        }
    }

    //===================================================================================================================
    // Drag/Drop Implementation
    //===================================================================================================================

    private void Source_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        var orders = e.Items.Select(v => (Order)v).ToArray();

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
        var def = e.GetDeferral();

        var text = await e.DataView.GetTextAsync();
        var items = text.Split(',').Select(index => Order.List[int.Parse(index)]);

        var (from, to) = sender.As<GridView>().Name switch
        {
            "OrderSources" => (OrdersInPossession, Sources),
            "OrdersInPossessionView" => (Sources, OrdersInPossession),
            _ => throw new UnreachableException()
        };

        foreach (var item in items)
        {
            from.Remove(item);
            to.Add(item);
        }

        e.AcceptedOperation = DataPackageOperation.Move;
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

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO

        if (((Button)sender).Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }

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
        // TODO
        OrdersInPossessionView.ItemsSource = OrdersInPossession;
    }

    private void InitRegionsMenuFlyout()
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
            var region = new MenuFlyoutSubItem { Text = regionName };
            foreach (var name in names)
            {
                region.Items.Add(new MenuFlyoutItem { Text = name });
            }
            RegionsMenuFlyout.Items.Add(region);
        }
    }

    private async void Load_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = Dialog.Builder(XamlRoot)
            .WithTitle("レギオンとメンバー名を選択してください")
            .WithBody(new LoadDialogContent((item) =>
            {
                if (string.IsNullOrEmpty(item))
                    throw new ArgumentException(@"Value cannot be null or empty.", nameof(item));
                Selected.Add(item);
            }))
            .WithPrimary("Load")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = $@"{desktop}\MitamatchOperations\Regions\{Selected[0]}\{Selected[1]}.json";

            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            var json = sr.ReadToEnd();
            var de = JsonSerializer.Deserialize<OrderPossession>(json);
            OrdersInPossession.Clear();
            foreach (var index in de.OrderIndices)
            {
                OrdersInPossession.Add(Order.List[index]);
            }
        });

        await dialog.ShowAsync();
    }

    private async void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = Dialog.Builder(XamlRoot)
            .WithTitle("レギオンとメンバー名を選択してください")
            .WithBody(new SaveDialogContent((item) =>
            {
                if (string.IsNullOrEmpty(item))
                    throw new ArgumentException(@"Value cannot be null or empty.", nameof(item));
                Selected.Add(item);
            }))
            .WithPrimary("Save")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = $@"{desktop}\MitamatchOperations\Regions\{Selected[0]}\{Selected[1]}.json";

            Director.CreateDirectory(@$"{desktop}\MitamatchOperations\decks");
            using var fs = Director.CreateFile(path);
            var proxy = new OrderPossession(Selected[1], DateTime.Now,
                OrdersInPossession.Select(order => order.Index).ToArray());
            var save = new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(proxy));
            fs.Write(save, 0, save.Length);
        });

        await dialog.ShowAsync();
    }
}

internal record struct OrderPossession(string Name, DateTime? UpdatedAt, ushort[] OrderIndices);
