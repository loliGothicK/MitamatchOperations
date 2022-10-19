using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RegionConsolePage
{
    private string? _regionName;

    public RegionConsolePage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is NavigationRootPageProps { Parameter: Props { Project: { } regionName } })
        {
            _regionName = regionName;
            MemberCvs.Source = Member.LoadMembersGrouped(regionName);
        }
        base.OnNavigatedTo(e);
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

