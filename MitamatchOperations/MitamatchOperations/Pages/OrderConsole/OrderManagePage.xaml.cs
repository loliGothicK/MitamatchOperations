using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using mitama.Domain;
using mitama.Domain.OrderKinds;
using WinRT;
using mitama.Pages.Common;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// Order Manager Page navigated to within a Order Console Tab.
/// </summary>
public sealed partial class OrderManagerPage
{
    private ObservableCollection<Order> Sources { get; } = new();
    private ObservableCollection<Order> OrdersInPossession { get; } = new();
    public string SelectedRegion = Director.ReadCache().LoggedIn;
    public string? SelectedMember;

    public OrderManagerPage()
    {
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        ElementalCheckBox.IsChecked
            = BuffCheckBox.IsChecked
            = DeBuffCheckBox.IsChecked
            = MpCheckBox.IsChecked
            = TriggerRateFluctuationCheckBox.IsChecked
            = FormationCheckBox.IsChecked
            = ShieldCheckBox.IsChecked
            = OthersCheckBox.IsChecked = true;
    }

    private void Update()
    {
        OrderSources.ItemsSource = Sources;
        OrdersInPossessionView.ItemsSource = OrdersInPossession;
    }

    private void Move_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (OrdersInPossessionView.SelectedItems.Count > 0 || OrderSources.SelectedItems.Count > 0)
        {
            button.IsEnabled = true;
        }
        else
        {
            button.IsEnabled = false;
        }
    }

    private void Move_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;

        if (button.Parent is StackPanel { Parent: FlyoutPresenter { Parent: Popup popup } })
        {
            popup.IsOpen = false;
        }

        var left = OrdersInPossessionView.SelectedItems
            .Select(item => (Order)item).ToArray();
        var right = OrderSources.SelectedItems
            .Select(item => (Order)item).ToArray();
        foreach (var ordered in right)
        {
            Sources.Remove(ordered);
            OrdersInPossession.Add(ordered);
        }
        foreach (var ordered in left)
        {
            OrdersInPossession.Remove(ordered);
            Sources.Add(ordered);
        }

        Update();
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

        e.Data.SetText(((Order)e.Items[0]).Index.ToString());
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

    private async void Load_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = Dialog.Builder(XamlRoot)
            .WithTitle("レギオンとメンバー名を選択してください")
            .WithBody(new LoadDialogContent((item) =>
            {
                switch (item)
                {
                    case Region region:
                    {
                        SelectedRegion = region.Name;
                        break;
                    }
                    case Member member:
                    {
                        SelectedMember = member.Name;
                        break;
                    }
                }
            }))
            .WithPrimary("Load")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = $@"{desktop}\MitamatchOperations\Regions\{SelectedRegion}\{SelectedMember}.json";

            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            var json = sr.ReadToEnd();
            OrdersInPossession.Clear();
            foreach (var index in Domain.Member.FromJson(json).OrderIndices)
            {
                Sources.Remove(Order.List[index]);
                OrdersInPossession.Add(Order.List[index]);
            }

            Update();
        });

        await dialog.ShowAsync();
    }

    private async void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = Dialog.Builder(XamlRoot)
            .WithTitle("レギオンとメンバー名を選択してください")
            .WithBody(new SaveDialogContent((item) =>
            {
                switch (item)
                {
                    case Region region:
                    {
                        SelectedRegion = region.Name;
                        break;
                    }
                    case Member member:
                    {
                        SelectedMember = member.Name;
                        break;
                    }
                }
            }))
            .WithPrimary("Save")
            .WithCancel("Cancel")
            .Build();

        dialog.PrimaryButtonCommand = new Defer(delegate
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = $@"{desktop}\MitamatchOperations\Regions\{SelectedRegion}\{SelectedMember}.json";

            Director.CreateDirectory(@$"{desktop}\MitamatchOperations\decks");
            using var fs = Director.CreateFile(path);
            if (SelectedMember != null)
            {
                var json = new Domain.Member(DateTime.Now, DateTime.Now, SelectedMember,
                    new Front(FrontCategory.Normal), OrdersInPossession.Select(order => order.Index).ToArray()).ToJson();
                var save = new UTF8Encoding(true).GetBytes(json);
                fs.Write(save, 0, save.Length);
            }

            dialog.Content.As<SaveDialogContent>().UpdateComboBox();
        });

        await dialog.ShowAsync();
    }
}
