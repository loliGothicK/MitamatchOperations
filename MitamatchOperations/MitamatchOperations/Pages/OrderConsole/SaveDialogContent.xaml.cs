using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;

namespace mitama.Pages.OrderConsole;

public abstract record Selected;
public record Region(string Name) : Selected;
public record Member(string Name) : Selected;

/// <summary>
/// Save Dialog Content used in the Save button flyout in Deck Editor Page.
/// </summary>
public sealed partial class SaveDialogContent
{
    public delegate void OnChanged(Selected selected);

    private readonly Dictionary<string, List<string>> _regionToMembersMap = new();
    private readonly OnChanged _onChangedAction;
    private string? _region = null;
    private string? _member = null;

    public SaveDialogContent(OnChanged onChanged)
    {
        _onChangedAction = onChanged;
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        InitComboBox();
    }

    internal void UpdateComboBox()
    {
        if (_region is null) { return; }
        if (_member is null) { return; }

        if (!_regionToMembersMap.ContainsKey(_region))
        {
            _regionToMembersMap[_region] = new List<string>{ _member };
        }
        else
        {
            if (!_regionToMembersMap[_region].Contains(_member))
            {
                _regionToMembersMap[_region].Add(_member);
            }
        }
    }

    private void InitComboBox()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var regions = Directory.GetDirectories(@$"{desktop}\MitamatchOperations\Regions").ToArray();

        foreach (var regionPath in regions)
        {
            var regionName = regionPath.Split(@"\").Last();
            _regionToMembersMap.Add(regionName, Directory.GetFiles(regionPath, "*.json").Select(path =>
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
        foreach (var member in _regionToMembersMap[region])
        {
            MemberComboBox.Items.Add(member);
            MemberComboBox.PlaceholderText = "メンバーを選択または入力してください";
        }

        _onChangedAction.Invoke(new Region(region));
        _region = region;
    }

    private void MemberComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var member = e.AddedItems[0].ToString();
        if (member != null) _onChangedAction.Invoke(new Member(member));
        _member = member;
    }
}