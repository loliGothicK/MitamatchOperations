using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mitama.Domain;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mitama.Pages.Library;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CharmLibraryPage : Page
{
    private readonly ObservableCollection<Charm> _charms = new(Charm.List);
    public CharmLibraryPage()
    {
        InitializeComponent();
    }

    private void OnSuggestionChosen(RichSuggestBox sender, SuggestionChosenEventArgs args)
    {
        if (args.Prefix == "#")
        {
            // User selected a hashtag item
            var element = ((Element)args.SelectedItem).Text;
            args.DisplayText = element;
            foreach (var charm in _charms.ToList().Where(charm => !charm.Ability.Contains(element)))
            {
                _charms.Remove(charm);
            }
        }
        else if (args.Prefix == "!")
        {
            // User selected a mention item
            var name = ((Other)args.SelectedItem).Value;
            args.DisplayText = name;
        }

    }

    private void OnSuggestionRequested(RichSuggestBox sender, SuggestionRequestedEventArgs args)
    {
        sender.ItemsSource = args.Prefix switch
        {
            "#" => new Element[] { new("火"), new("水"), new("風"), new("光"), new("闇") },
            _ => null,
        };
    }

    private void OnClear(object sender, RoutedEventArgs e)
    {
        SuggestingBox.Clear();
        _charms.Clear();
        foreach (var charm in Charm.List)
        {
            _charms.Add(charm);
        }
    }
}

public class CharmSuggestionTemplateSelector : DataTemplateSelector
{
    public DataTemplate Element { get; set; }

    public DataTemplate Other { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        return item is Element ? Element! : Other;
    }
}
