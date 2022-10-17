using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Contacts;
using Microsoft.UI.Xaml;
using mitama.Domain;
using System.IO;
using System;
using System.Linq;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RegionConsolePage
{

    public RegionConsolePage()
    {
        InitializeComponent();
    }


    private void RegionSelectButton_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not DropDownButton button) return;
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var regions = Directory.GetDirectories(@$"{desktop}\MitamatchOperations\Regions")
            .Select(path => path.Split('\\').Last());

        foreach (var region in regions)
        {
            RegionSelectOptions.Items.Add(new MenuFlyoutItem
            {
                Text = region,
                Command = new Defer(() =>
                {
                    MemberCvs.Source = Member.LoadMembersGrouped(region);
                })
            });
        }
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

