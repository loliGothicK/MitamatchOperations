using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;

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
        if (e.Parameter is NavigationRootPageArgs { Parameter: string parameter } && !string.IsNullOrWhiteSpace(parameter))
        {
            _regionName = parameter;
            MemberCvs.Source = Member.LoadMembersGrouped(parameter);
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

