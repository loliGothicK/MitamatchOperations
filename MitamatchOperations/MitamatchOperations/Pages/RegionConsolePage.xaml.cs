using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using mitama.Algorithm.IR;
using mitama.Pages.RegionConsole;
using WinRT;
using System.ComponentModel;

namespace mitama.Pages;

/// <summary>
/// Region Console Page navigated to within a Main Page.
/// </summary>
public sealed partial class RegionConsolePage
{
    private string _regionName = Director.ReadCache().Region;
    private MemberInfo? _selectedMember;

    public RegionConsolePage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
        SwitchExpander.Header = "ユニットマネージコンソール";
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Init();
    }

    private void Init()
    {
        _regionName = Director.ReadCache().Region;
        var query = from item in Util.LoadMembersInfo(_regionName)
                    group item by item.Position into g
                    orderby g.Key
                    select new GroupInfoList(g) { Key = g.Key };

        MemberCvs.Source = new ObservableCollection<GroupInfoList>(query);
    }

    private void Grid_DragOver(object _, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void Grid_Drop(object _, DragEventArgs e)
    {
        if (!e.DataView.Contains(StandardDataFormats.StorageItems)) return;

        var items = await e.DataView.GetStorageItemsAsync();

        if (items.Count <= 0) return;

        using var img = new Bitmap(items[0].As<StorageFile>()!.Path);
        var (result, detected) = await Match.Recognise(img, _selectedMember!.Position);

        result.Save($@"{Director.MitamatchDir()}\.temp\result.jpg");

        var dialog = new DialogBuilder(XamlRoot)
            .WithTitle("読込結果")
            .WithPrimary("名前をつけて保存")
            .WithCancel("やっぱりやめる")
            .WithBody(new RecogniseDialogContent(detected))
            .Build();
        dialog.PrimaryButtonCommand = new Defer(() =>
        {
            dialog.Closed += async (_, _) =>
            {
                var body = new TextBox();
                var naming = new DialogBuilder(XamlRoot)
                    .WithTitle("ユニット名")
                    .WithPrimary("保存")
                    .WithCancel("やっぱりやめる")
                    .WithBody(body)
                    .Build();
                naming.PrimaryButtonCommand = new Defer(async delegate
                {
                    new DirectoryInfo($@"{Director.ProjectDir()}\{_regionName}\Members\{_selectedMember?.Name}\Units").Create();
                    var path = $@"{Director.ProjectDir()}\{_regionName}\Members\{_selectedMember?.Name}\Units\{body.Text}.json";
                    await using var unit = File.Create(path);
                    await unit.WriteAsync(new UTF8Encoding(true).GetBytes(new Unit(body.Text, _selectedMember!.Position is Front, detected.ToList()).ToJson()));
                });

                await naming.ShowAsync();
            };
            return Task.CompletedTask;
        });

        await dialog.ShowAsync();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedMember = e.AddedItems[0].As<MemberInfo>();
        TargetMember.Text = _selectedMember.Name;
    }

    private void UnitTreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        using var sr =
            new StreamReader(@$"{Director.UnitDir(_regionName, args.InvokedItem.As<ExplorerItem>().Parent!)}\{args.InvokedItem.As<ExplorerItem>().Name!}.json");
        var json = sr.ReadToEnd();
        var unit = Unit.FromJson(json);
        UnitView.ItemsSource = unit.Memorias;
    }

    private void UnitTreeView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TreeView view) return;
        view.ItemsSource = new ObservableCollection<ExplorerItem>(Util.LoadMemberNames(_regionName).Select(name =>
        {
            return new ExplorerItem
            {
                Name = name,
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children = new ObservableCollection<ExplorerItem>(Directory.GetFiles($"{Director.UnitDir(_regionName, name)}").Select(path =>
                {
                    using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                    var json = sr.ReadToEnd();
                    return new ExplorerItem
                    {
                        Parent = name,
                        Name = Unit.FromJson(json).UnitName,
                        Type = ExplorerItem.ExplorerItemType.File
                    };
                }))
            };
        }));
    }
}

// GroupInfoList class definition:
internal class GroupInfoList : List<MemberInfo>
{
    public GroupInfoList(IEnumerable<MemberInfo> items) : base(items)
    {
    }
    public object Key { get; set; } = null!;
}

public class ExplorerItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public enum ExplorerItemType { Folder, File };
    public string? Parent { get; set; }
    public string? Name { get; set; }
    public ExplorerItemType Type { get; set; }
    private ObservableCollection<ExplorerItem>? _children;
    public ObservableCollection<ExplorerItem> Children
    {
        get => _children ??= new ObservableCollection<ExplorerItem>();
        set => _children = value;
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            NotifyPropertyChanged("IsExpanded");
        }
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
