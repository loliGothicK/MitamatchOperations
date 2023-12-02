using System.Collections.Generic;
using System.Collections.ObjectModel;
using mitama.Domain;
using mitama.Pages.Common;
using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using WinRT;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using mitama.Pages.OrderConsole;
using System.IO;
using System.Text;
using System.Text.Json;

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MemberManageConsole
{
    private string _regionName = Director.ReadCache().Region;
    private string chara1;
    private string chara2;
    private string skill1;
    private string skill2;
    private string personnel1;
    private string personnel2;

    public MemberManageConsole()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
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

    private void Opponent_TextChanged(object sender, TextChangedEventArgs e)
    {
        OpponentSettings.Description = sender.As<TextBox>().Text;
    }

    private void RareSkill_Loaded(object sender, RoutedEventArgs e)
    {
        sender.As<ComboBox>().ItemsSource = Costume.List.Select(x => x.Lily).Distinct();
    }

    private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        chara1 = sender.As<ComboBox>().SelectedItem.As<string>();
        Skill1.ItemsSource = Costume.List.Where(x => x.Lily == chara1).Select(x => x.RareSkill.Name).Distinct();
    }

    private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        chara2 = sender.As<ComboBox>().SelectedItem.As<string>();
        Skill2.ItemsSource = Costume.List.Where(x => x.Lily == chara2).Select(x => x.RareSkill.Name).Distinct();
    }

    private void Skill1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        skill1 = sender.As<ComboBox>().SelectedItem.As<string>();
    }

    private void Skill2_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        skill2 = sender.As<ComboBox>().SelectedItem.As<string>();
    }

    private void Personnel1_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        personnel1 = sender.As<ComboBox>().SelectedItem.As<string>();
    }

    private void Personnel2_SelectionChanged(object sender, SelectionChangedEventArgs _)
    {
        personnel2 = sender.As<ComboBox>().SelectedItem.As<string>();
    }

    private void Personnel_Loaded(object sender, RoutedEventArgs e)
    {
        sender.As<ComboBox>().ItemsSource = Util.LoadMembersInfo(_regionName).Select(x => x.Name);
    }

    private void Timeline_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox box) return;

        if (!Directory.Exists(Director.DeckDir(_regionName)))
        {
            Director.CreateDirectory(Director.DeckDir(_regionName));
        }
        var decks = Directory.GetFiles(Director.DeckDir(_regionName), "*.json").Select(path =>
        {
            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            var json = sr.ReadToEnd();
            return JsonSerializer.Deserialize<DeckJson>(json);
        }).ToList();

        box.ItemsSource = decks;
    }

    private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
    {

    }
}

// GroupInfoList class definition:
internal class GroupInfoList(IEnumerable<MemberInfo> items) : List<MemberInfo>(items)
{
    public object Key { get; set; } = null!;
}
