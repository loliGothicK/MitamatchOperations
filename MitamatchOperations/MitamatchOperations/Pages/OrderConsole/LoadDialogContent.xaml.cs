using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.UI.Xaml.Controls;
using mitama.Pages.Common;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// Load Dialog Content used in the Load button flyout in Deck Editor Page.
/// </summary>
public sealed partial class LoadDialogContent
{
    public delegate void OnChanged(Selected selected);

    private Dictionary<string, List<string>> RegionToMembersMap = new();
    private OnChanged OnChangedAction;

    public LoadDialogContent(OnChanged onChanged)
    {
        OnChangedAction = onChanged;
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        var regions = Directory.GetDirectories(Director.ProjectDir()).ToArray();

        foreach (var regionPath in regions)
        {
            var regionName = regionPath.Split(@"\").Last();
            RegionToMembersMap.Add(regionName, Directory.GetFiles(Director.MemberDir(regionName), "*.json").Select(path =>
            {
                using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                var json = sr.ReadToEnd();
                return Domain.Member.FromJson(json).Name;
            }).ToList());
            RegionComboBox.Items.Add(regionName);
        }
    }

    private void RegionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var region = e.AddedItems[0].ToString();
        if (region == null) return;
        foreach (var member in RegionToMembersMap[region])
        {
            MemberComboBox.Items.Add(member);
            MemberComboBox.PlaceholderText = "メンバーを選択してください";
        }

        OnChangedAction.Invoke(new Region(region));
    }

    private void MemberComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var member = e.AddedItems[0].ToString();
        if (member != null) OnChangedAction.Invoke(new Member(member));
    }
}