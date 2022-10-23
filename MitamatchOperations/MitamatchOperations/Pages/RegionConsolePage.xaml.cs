using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RegionConsolePage
{
    private string _regionName = Director.ReadCache().LoggedIn;

    public RegionConsolePage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Project.Text = _regionName = Director.ReadCache().LoggedIn;
        var query = from item in Directory.GetFiles($@"{Director.RegionDir()}\{_regionName}", "*.json")
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
}

// GroupInfoList class definition:
internal class GroupInfoList : List<Member>
{
    public GroupInfoList(IEnumerable<Member> items) : base(items)
    {
    }
    public object Key { get; set; } = null!;
}

