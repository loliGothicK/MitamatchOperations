using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

namespace mitama.Pages;

/// <summary>
/// Region Console Page navigated to within a Main Page.
/// </summary>
public sealed partial class RegionConsolePage
{
    private string _regionName = Director.ReadCache().Region;
    private Member? _selectedMember;

    public RegionConsolePage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Init();
    }

    private void Init()
    {
        _regionName = Director.ReadCache().Region;
        var query = from item in Directory.GetFiles($@"{Director.MemberDir(_regionName)}", "*.json")
                .Select(path =>
                {
                    using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                    var json = sr.ReadToEnd();
                    return Member.FromJson(json);
                })
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
        var (_, detected) = await Match.Recognise(img);
        await new DialogBuilder(XamlRoot)
            .WithTitle("ì«çûåãâ ")
            .WithPrimary("ñºëOÇÇ¬ÇØÇƒï€ë∂")
            .WithCancel("Ç‚Ç¡ÇœÇËÇ‚ÇﬂÇÈ")
            .WithBody(new RecogniseDialogContent(detected))
            .Build()
            .ShowAsync();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedMember = e.AddedItems[0].As<Member>();
        TargetMember.Text = _selectedMember.Name;
    }
}

// GroupInfoList class definition:
internal class GroupInfoList : List<Member>
{
    public GroupInfoList(IEnumerable<Member> items) : base(items)
    {
    }
    public object Key { get; set; } = null!;
}

