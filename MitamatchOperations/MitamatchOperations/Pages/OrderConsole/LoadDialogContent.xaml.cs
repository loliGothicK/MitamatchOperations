using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoadDialogContent
{
    public delegate void OnChanged(string selected);

    private Dictionary<string, List<string>> RegionToMembersMap = new();
    private OnChanged OnChangedAction;

    public LoadDialogContent(OnChanged onChanged)
    {
        OnChangedAction = onChanged;
        InitializeComponent();

        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var regions = Directory.GetDirectories(@$"{desktop}\MitamatchOperations\Regions").ToArray();

        foreach (var regionPath in regions)
        {
            var regionName = regionPath.Split(@"\").Last();
            RegionToMembersMap.Add(regionName, Directory.GetFiles(regionPath, "*.json").Select(path =>
            {
                using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                var json = sr.ReadToEnd();
                return JsonSerializer.Deserialize<OrderPossession>(json).Name;
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

        OnChangedAction.Invoke(region);
    }

    private void MemberComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var member = e.AddedItems[0].ToString();
        if (member != null) OnChangedAction.Invoke(member);
    }
}