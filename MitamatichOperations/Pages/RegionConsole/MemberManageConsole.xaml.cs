using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using mitama.Domain;
using mitama.Pages.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Linq;
using System;
using mitama.Algorithm.IR;
using WinRT;
using System.IO;
using System.Text;
using Microsoft.UI.Xaml.Navigation;

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MemberManageConsole
{
    private string _regionName = Director.ReadCache().Region;
    private MemberInfo _selectedMember;

    public MemberManageConsole()
    {
        InitializeComponent();
        Init();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
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

        try
        {
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
                        new DirectoryInfo(
                            $@"{Director.ProjectDir()}\{_regionName}\Members\{_selectedMember?.Name}\Units").Create();
                        var path =
                            $@"{Director.ProjectDir()}\{_regionName}\Members\{_selectedMember?.Name}\Units\{body.Text}.json";
                        await using var unit = File.Create(path);
                        await unit.WriteAsync(new UTF8Encoding(true).GetBytes(
                            new Unit(body.Text, _selectedMember!.Position is Front, detected.ToList()).ToJson()));
                    });

                    await naming.ShowAsync();
                };
                return Task.CompletedTask;
            });

            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            var dialog = new DialogBuilder(XamlRoot)
                .WithTitle("読込失敗")
                .WithPrimary("OK")
                .WithBody(new TextBlock
                {
                    Text = ex.ToString()
                })
                .Build();

            await dialog.ShowAsync();
        }
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedMember = e.AddedItems[0].As<MemberInfo>();
        TargetMember.Text = _selectedMember.Name;
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
