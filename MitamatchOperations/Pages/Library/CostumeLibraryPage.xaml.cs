using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using System;
using Microsoft.UI.Xaml;
using System.Linq;
using mitama.Pages.Common;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CostumeLibraryPage : Page
    {
        private readonly ObservableCollection<Costume> _costumes = new(Costume.List);
        private readonly MemberInfo[] Info = Util.LoadMembersInfo(Director.ReadCache().Region);

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
                    "’Êí’P‘Ì" => costume.Type is not NormalSingleCostume,
                    "’Êí”ÍˆÍ" => costume.Type is not NormalRangeCostume,
                    "“ÁŽê’P‘Ì" => costume.Type is not SpecialSingleCostume,
                    "“ÁŽê”ÍˆÍ" => costume.Type is not SpecialRangeCostume,
                    "Žx‰‡" => costume.Type is not AssistCostume,
                    "–WŠQ" => costume.Type is not InterferenceCostume,
                    "‰ñ•œ" => costume.Type is not RecoveryCostume,
                    _ => false,
                }))
                {
                    _costumes.Remove(costume);
                }
            }
            else
            {
                var other = (Other)args.SelectedItem;
                if (other.Value == "‰Î")
                {
                    args.DisplayText = "‰Î";
                    foreach (var costume in _costumes.ToList().Where(costume => !costume.ExSkill.HasValue || !costume.ExSkill.Value.Description.Contains("‰Î")))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "…")
                {
                    args.DisplayText = "…";
                    foreach (var costume in _costumes.ToList().Where(costume => !costume.ExSkill.HasValue || !costume.ExSkill.Value.Description.Contains("…")))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "•—")
                {
                    args.DisplayText = "•—";
                    foreach (var costume in _costumes.ToList().Where(costume => !costume.ExSkill.HasValue || !costume.ExSkill.Value.Description.Contains("•—")))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "’Êí")
                {
                    args.DisplayText = "’Êí";
                    foreach (var costume in _costumes.ToList().Where(costume => 
                    {
                        var status = costume.Status;
                        return status.Atk < status.SpAtk;
                    }))
                    {
                        _costumes.Remove(costume);
                    }
                }
                else if (other.Value == "“ÁŽê")
                {
                    args.DisplayText = "“ÁŽê";
                    foreach (var costume in _costumes.ToList().Where(costume => {
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
            sender.ItemsSource = args.Prefix switch {
                "@" => _costumes.Where(costume => costume.Lily.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase)).Select(costume => new Lily(costume.Lily, costume.Path)).DistinctBy(lily => lily.Name),
                "#" => _costumes.Where(costume => costume.RareSkill.Name.Contains(args.QueryText, StringComparison.OrdinalIgnoreCase)).Select(costume => costume.RareSkill).DistinctBy(RareSkill => RareSkill.Name),
                "\\" => new Position[]{ new("’Êí’P‘Ì"), new("’Êí”ÍˆÍ"), new("“ÁŽê’P‘Ì"), new("“ÁŽê”ÍˆÍ"), new("Žx‰‡"), new("–WŠQ"), new("‰ñ•œ") },
                "!" => new Other[]{ new("‰Î"), new("…"), new("•—"), new("15%"), new("Lv.16"), new("’Êí"), new("“ÁŽê")  },
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
                .Select(info => {
                    var Ex = info.Costumes.First(item => item.Index == index).Ex;
                    return (info.Name, Ex switch
                    {
                        ExInfo.None => string.Empty,
                        ExInfo.Active => @"(EX‚ ‚è)",
                        ExInfo.Inactive => @"(EX‚È‚µ)",
                        _ => throw new NotImplementedException(),
                    });
                });
            stackPanel.Children.Add(new TextBlock
            {
                Text = string.Join(", ", hasCostume.Select(item => $"{item.Name} {item.Item2}")),
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 0),
                TextWrapping = TextWrapping.WrapWholeWords,
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
