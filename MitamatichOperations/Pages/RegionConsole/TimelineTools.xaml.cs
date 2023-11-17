using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using mitama.Pages.Common;
using mitama.Pages.OrderConsole;

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TimelineTools
{
    private readonly string _project;

    public TimelineTools()
    {
        _project = Director.ReadCache().Region;
        InitializeComponent();
        LoadDeck();
    }

    public void LoadDeck()
    {
        foreach (var deck in Directory.GetFiles(Director.DeckDir(_project)).Select(path =>
                 {
                     using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                     var json = sr.ReadToEnd();
                     var deck = JsonSerializer.Deserialize<DeckJson>(json);
                     return deck;
                 }))
        {
            DeckItem.Items.Add(new MenuFlyoutItem
            {
                Text = deck.Name,
                Command = new Defer(() =>
                {
                    foreach (var image in deck.Items
                                 .Select(item => new Image
                                 {
                                     Source = new BitmapImage(((TimeTableItem)item).Order.Uri),
                                     Width = 50,
                                     Height = 50,
                                 }))
                    {
                        TimelinePanel.Children.Add(image);
                    }
                    return Task.CompletedTask;
                }),
            });
        }
    }
}
