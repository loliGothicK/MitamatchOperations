using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using System.Linq;
using SimdLinq;
using System;
using Microsoft.UI.Xaml;
using System.Xml.Linq;
using DynamicData.Kernel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MitamatchOperations.Pages.Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CostumeLibraryPage : Page
    {
        private readonly ObservableCollection<Costume> _costumes = new(Costume.List);
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
                else
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
              "!" => new Other[]{ new("‰Î"), new("…"), new("•—"), new("15%"),  },
              _ => null,
            };
        }
    }

    public class SuggestionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Lily { get; set; }

        public DataTemplate RareSkill { get; set; }

        public DataTemplate Other { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is RareSkill ? RareSkill!
                : item is Lily ? Lily!
                : Other;
        }
    }

    public record Lily(string Name, string Path);
    public record Element(string Text);
    public record Other(string Value);
}
