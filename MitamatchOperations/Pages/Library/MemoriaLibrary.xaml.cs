using System.Linq;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Models.DataGrid;
using System.Collections.ObjectModel;
using MitamatchOperations.Lib;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.Library;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MemoriaLibrary : Page
{
    public ObservableCollection<MemoriaData> MemoriaList { get; } = new([.. Memoria.List.Select(m => new MemoriaData(m))]);
    public MemoriaLibrary()
    {
        InitializeComponent();
        sfDataGrid.ItemsSource = MemoriaList;
    }

    private void CopyToClipboardButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var json = string.Join(",", MemoriaList
            .Select(data => new Indoc($@"
                {{
                    ""id"": {data.Id},
                    ""link"": ""{data.Link}"",
                    ""name"": ""{data.Name}"",
                    ""kind"": ""{data.Kind}"",
                    ""element"": ""{data.Element}"",
                    ""cost"": {data.Cost},
                    ""skill"": {{
                        ""name"": ""{data.Skill}"",
                        ""description"": ""{data.SkillDescription}""
                    }},
                    ""supportSkill"": {{
                        ""name"": ""{data.SupportSkill}"",
                        ""description"": ""{data.SupportSkillDescription}""
                    }}
                }}
            ").Text));

        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dataPackage.SetText(json);
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
    }
}
