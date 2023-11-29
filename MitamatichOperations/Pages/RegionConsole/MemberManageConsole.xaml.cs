using System.Collections.Generic;
using System.Collections.ObjectModel;
using mitama.Domain;
using mitama.Pages.Common;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MemberManageConsole
{
    private string _regionName = Director.ReadCache().Region;

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
}

// GroupInfoList class definition:
internal class GroupInfoList(IEnumerable<MemberInfo> items) : List<MemberInfo>(items)
{
    public object Key { get; set; } = null!;
}
