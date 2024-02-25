using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mitama.Domain;
using Mitama.Pages.Common;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mitama.Pages.Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CostumeLibraryPage : Page
    {
        private readonly ObservableCollection<Costume> _costumes = new(Costume.List);
        private readonly MemberInfo[] Info = Util.LoadMembersInfo(Director.ReadCache().Legion);

        public CostumeLibraryPage()
        {
            InitializeComponent();
        }

        private void OnSuggestionChosen(RichSuggestBox _, SuggestionChosenEventArgs args)
        {
            if (args.Prefix == "#")
            {
                // User selected a hashtag item
                var sikillName = ((RareSkill)args.SelectedItem).Name;
                args.DisplayText = sikillName;
                foreach (var costume in _costumes.ToList().Where(costume => costume.RareSkill.Name != sikillName))
                {
                    _costumes.Remove(costume);
                }
            }
            else if (args.Prefix == "@")
            {
                // User selected a mention item
                var name = ((Lily)args.SelectedItem).Name;
                args.DisplayText = name;
                foreach (var costume in _costumes.ToList().Where(costume => costume.Lily != name))
                {
                    _costumes.Remove(costume);
                }
            }
            else if (args.Prefix == "\\")
            {
                var name = ((Position)args.SelectedItem).Text;
                args.DisplayText = name;
                foreach (var costume in _costumes.ToList().Where(costume => name switch
                {
                    "通常単体" => costume.Type is not NormalSingleCostume,
                    "通常範囲" => costume.Type is not NormalRangeCostume,
                    "特殊単体" => costume.Type is not SpecialSingleCostume,
                    "特殊範囲" => costume.Type is not SpecialRangeCostume,
                    "支援" => costume.Type is not AssistCostume,
                    "妨害" => costume.Type is not InterferenceCostume,
                    "回復" => costume.Type is not RecoveryCostume,
                    _ => false,
                }))
                {
                    _costumes.Remove(costume);
                }
            }
            else
            {
                var other = (Other)args.SelectedItem;
                if (other.Value == "火")
                {
                    args.DisplayText = "火";
                    foreach (var costume in _costumes.ToList().Where(costume => !costume.ExSkill.HasValue || !costume.ExSkill.Value.Description.Contains("火")))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "水")
                {
                    args.DisplayText = "水";
                    foreach (var costume in _costumes.ToList().Where(costume => !costume.ExSkill.HasValue || !costume.ExSkill.Value.Description.Contains("水")))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "風")
                {
                    args.DisplayText = "風";
                    foreach (var costume in _costumes.ToList().Where(costume => !costume.ExSkill.HasValue || !costume.ExSkill.Value.Description.Contains("風")))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "通常")
                {
                    args.DisplayText = "通常";
                    foreach (var costume in _costumes.ToList().Where(costume =>
                    {
                        var status = costume.Status;
                        return status.Atk < status.SpAtk;
                    }))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "特殊")
                {
                    args.DisplayText = "特殊";
                    foreach (var costume in _costumes.ToList().Where(costume =>
                    {
                        var status = costume.Status;
                        return status.Atk > status.SpAtk;
                    }))

                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "Lv.16")
                {
                    args.DisplayText = "Lv.16";
                    foreach (var costume in _costumes.ToList().Where(costume => costume.LilySkills.Length != 16))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "15%")
                {
                    args.DisplayText = "15%";
                    foreach (var costume in _costumes.ToList().Where(costume => costume.Type.Value != 15))
                    {
                        _costumes.Remove(costume);
                    }
                }
            }
        }

        private void OnSuggestionRequested(RichSuggestBox sender, SuggestionRequestedEventArgs args)
        {
            sender.ItemsSource = args.Prefix switch
            {
                "@" => _costumes.Where(costume => costume.Lily.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase)).Select(costume => new Lily(costume.Lily, costume.Path)).DistinctBy(lily => lily.Name),
                "#" => _costumes.Where(costume => costume.RareSkill.Name.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase)).Select(costume => costume.RareSkill).DistinctBy(RareSkill => RareSkill.Name),
                "\\" => new Position[] { new("通常単体"), new("通常範囲"), new("特殊単体"), new("特殊範囲"), new("支援"), new("妨害"), new("回復") },
                "!" => new Other[] { new("火"), new("水"), new("風"), new("15%"), new("Lv.16"), new("通常"), new("特殊") },
                _ => null,
            };
        }

        private void OnClear(object _, RoutedEventArgs _e)
        {
            SuggestingBox.Clear();
            _costumes.Clear();
            foreach (var costume in Costume.List)
            {
                _costumes.Add(costume);
            }
        }

        private void HasCostume_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not StackPanel stackPanel)
            {
                return;
            }
            var index = int.Parse(stackPanel.AccessKey);
            var hasCostume = Info
                .Where(info => info.Costumes != null && info.Costumes.Any(costume => costume.Index == index))
                .Select(info =>
                {
                    var Ex = info.Costumes.First(item => item.Index == index).Ex;
                    return (info.Name, Ex switch
                    {
                        ExInfo.None => string.Empty,
                        ExInfo.Active => @"(EXあり)",
                        ExInfo.Inactive => @"(EXなし)",
                        _ => throw new NotImplementedException(),
                    });
                });
            stackPanel.Children.Add(new TextBlock
            {
                Text = string.Join("\n", hasCostume.Select(item => $"{item.Name} {item.Item2}")),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 0),
            });
        }
    }

    public class SuggestionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Lily { get; set; }

        public DataTemplate RareSkill { get; set; }

        public DataTemplate Position { get; set; }

        public DataTemplate Other { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is RareSkill ? RareSkill!
                 : item is Lily ? Lily!
                 : item is Position ? Position!
                 : Other;
        }
    }

    public record Lily(string Name, string Path);
    public record Element(string Text);
    public record Position(string Text);
    public record Other(string Value);
}
