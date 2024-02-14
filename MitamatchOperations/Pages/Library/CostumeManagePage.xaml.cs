using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Algorithm.IR;
using mitama.Domain;
using mitama.Pages.Common;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.Library;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CostumeManagePage : Page
{
    private string imgPath;
    private int verticalCount = 3;
    private int horizontalCount = 8;
    private readonly ObservableCollection<Costume> Costumes = [];
    private readonly ObservableCollection<Costume> Pool = [.. Costume.List];
    private List<Costume> selectedCostumes = [];
    private readonly HashSet<FilterType> currentFilters = [.. Enum.GetValues(typeof(FilterType)).Cast<FilterType>()];
    private readonly Dictionary<FilterType, Func<Costume, bool>> Filters = [];

    public CostumeManagePage()
    {
        InitFilters();
        InitializeComponent();
    }

    private async void PickOpenButton_Click(object sender, RoutedEventArgs e)
    {
        // Create a file picker
        var openPicker = new FileOpenPicker();

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var window = WindowHelper.GetWindowForElement(this);
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add(".png");
        openPicker.FileTypeFilter.Add(".jpg");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            GeneralInfoBar.Title = $"{file.Name} ‚ð“Ç‚Ýž‚Ý‚Ü‚µ‚½";
            GeneralInfoBar.IsOpen = true;
            imgPath = file.Path;
        }
    }

    private async void RecognizeButton_Click(object sender, RoutedEventArgs e)
    {
        using var img = new System.Drawing.Bitmap(imgPath);
        var (result, detected) = await CostumeSearch.Recognise(img, v:verticalCount, h:horizontalCount);
        result.Save($@"{Director.MitamatchDir()}/result.png");
        foreach (var item in detected)
        {
            Costumes.Add(item);
            Pool.Remove(item);
        }
    }

    private void VerticalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        verticalCount = int.Parse(e.AddedItems[0].ToString());
    }

    private void HorizontalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        horizontalCount = int.Parse(e.AddedItems[0].ToString());
    }

    private void Memeria_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        selectedCostumes = e.Items.Select(v => (Costume)v).ToList();
        e.Data.RequestedOperation = DataPackageOperation.Move;
    }

    private void MemeriaSources_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }
    
    private void MemeriaSources_Drop(object sender, DragEventArgs e)
    {
        foreach (var toAdd in Costume
            .List
            .Where(m => selectedCostumes.Select(s => s.Name).Contains(m.Name)))
        {
            Pool.Add(toAdd);
        }
        foreach (var toRemove in Costumes.ToList().Where(selectedCostumes.Contains))
        {
            Costumes.Remove(toRemove);
        }
        BuilderPageHelpers.Sort(Pool, (a, b) => b.Index.CompareTo(a.Index));
    }

    private void Memeria_Drop(object sender, DragEventArgs e)
    {
        foreach (var toRemove in Costume
            .List
            .Where(m => selectedCostumes.Select(s => s.Name).Contains(m.Name)))
        {
            Pool.Remove(toRemove);
        }
        foreach (var toAdd in selectedCostumes)
        {
            Costumes.Add(toAdd);
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var comboBox = new ComboBox();
        foreach (var member in Util.LoadMemberNames(Director.ReadCache().Region))
        {
            comboBox.Items.Add(member);
        }
        comboBox.SelectedIndex = 0;
        string selected = string.Empty;
        comboBox.SelectionChanged += (_, args) =>
        {
            selected = comboBox.Items[comboBox.SelectedIndex].ToString();
        };

        var dialog = new DialogBuilder(XamlRoot)
            .WithBody(comboBox)
            .WithPrimary("’Ç‰Á", new Defer(delegate
            {
                var path = $@"{Director.ProjectDir()}\{Director.ReadCache().Region}\Members\{selected}\info.json";
                if (selected == string.Empty) return Task.CompletedTask;
                using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                var readJson = sr.ReadToEnd();
                var info = MemberInfo.FromJson(readJson);
                var idToName = Memoria
                    .List
                    .ToDictionary(m => m.Id, m => m.Name);
                List<int> costumes
                    = info.CostumeIndices == null
                    ? [.. Costumes.Select(m => m.Index)]
                    : [.. Costumes.Select(m => m.Index).Concat(info.CostumeIndices)];
                var writeJson = (info with {
                    UpdatedAt = DateTime.Now,
                    CostumeIndices = [.. costumes],
                }).ToJson();
                var save = new UTF8Encoding(true).GetBytes(writeJson);
                sr.Close();
                using var fs = Director.CreateFile(path);
                fs.Write(save, 0, save.Length);
                foreach (var costume in Costumes)
                {
                    Pool.Add(costume);
                }
                Costumes.Clear();
                BuilderPageHelpers.Sort(Pool, (a, b) => b.Index.CompareTo(a.Index));
                return Task.CompletedTask;
            }))
            .Build();

        await dialog.ShowAsync();
    }

    private void Filter_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        var prevCount = currentFilters.Count;
        var filter = checkBox.Content.ToString();
        switch (filter)
        {
            case "’Êí’P‘Ì":
                {
                    currentFilters.Add(FilterType.NormalSingle);
                    break;
                }
            case "’Êí”ÍˆÍ":
                {
                    currentFilters.Add(FilterType.NormalRange);
                    break;
                }
            case "“ÁŽê’P‘Ì":
                {
                    currentFilters.Add(FilterType.SpecialSingle);
                    break;
                }
            case "“ÁŽê”ÍˆÍ":
                {
                    currentFilters.Add(FilterType.SpecialRange);
                    break;
                }
            case "Žx‰‡":
                {
                    currentFilters.Add(FilterType.Support);
                    break;
                }
            case "–WŠQ":
                {
                    currentFilters.Add(FilterType.Interference);
                    break;
                }
            case "‰ñ•œ":
                {
                    currentFilters.Add(FilterType.Recovery);
                    break;
                }
            default:
                {
                    throw new NotImplementedException();
                }
        }

        if (prevCount == currentFilters.Count) return;

        foreach (var costume in Costume
                .List
                .DistinctBy(x => x.Name)
                .Where(costume => !Pool.Contains(costume))
                .Where(costume => !Costumes.Select(m => m.Index).Contains(costume.Index))
                .Where(ApplyFilter))
        {
            Pool.Add(costume);
        }
        BuilderPageHelpers.Sort(Pool, (a, b) => b.Index.CompareTo(a.Index));
    }

    private void Filter_Unchecked(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox checkBox) return;
        var prevCount = currentFilters.Count;
        var filter = checkBox.Content.ToString();

        switch (filter)
        {
            case "’Êí’P‘Ì":
                {
                    currentFilters.Remove(FilterType.NormalSingle);
                    break;
                }
            case "’Êí”ÍˆÍ":
                {
                    currentFilters.Remove(FilterType.NormalRange);
                    break;
                }
            case "“ÁŽê’P‘Ì":
                {
                    currentFilters.Remove(FilterType.SpecialSingle);
                    break;
                }
            case "“ÁŽê”ÍˆÍ":
                {
                    currentFilters.Remove(FilterType.SpecialRange);
                    break;
                }
            case "Žx‰‡":
                {
                    currentFilters.Remove(FilterType.Support);
                    break;
                }
            case "–WŠQ":
                {
                    currentFilters.Remove(FilterType.Interference);
                    break;
                }
            case "‰ñ•œ":
                {
                    currentFilters.Remove(FilterType.Recovery);
                    break;
                }
            default:
                {
                    throw new NotImplementedException();
                }
        }
        if (prevCount == currentFilters.Count) return;
        foreach (var memoria in Pool
            .ToList()
            .Where(memoria => !ApplyFilter(memoria)))
        {
            Pool.Remove(memoria);
        }
        BuilderPageHelpers.Sort(Pool, (a, b) => b.Index.CompareTo(a.Index));
    }


    private void InitFilters()
    {
        Filters.Add(FilterType.NormalSingle, costume => costume.Type is NormalSingleCostume);
        Filters.Add(FilterType.NormalRange, costume => costume.Type is NormalRangeCostume);
        Filters.Add(FilterType.SpecialSingle, costume => costume.Type is SpecialSingleCostume);
        Filters.Add(FilterType.SpecialRange, costume => costume.Type is SpecialRangeCostume);
        Filters.Add(FilterType.Support, costume => costume.Type is AssistCostume);
        Filters.Add(FilterType.Interference, costume => costume.Type is InterferenceCostume);
        Filters.Add(FilterType.Recovery, costume => costume.Type is RecoveryCostume);
    }

    bool ApplyFilter(Costume costume)
    {
        var p0 = currentFilters.Where(IsKindFilter).Any(key => Filters[key](costume));
        return p0;
    }

    private static bool IsKindFilter(FilterType filter)
    {
        FilterType[] kindFilters = [
            FilterType.NormalSingle,
            FilterType.NormalRange,
            FilterType.SpecialSingle,
            FilterType.SpecialRange,
            FilterType.Support,
            FilterType.Interference,
            FilterType.Recovery,
        ];

        return kindFilters.Contains(filter);
    }

    public enum FilterType
    {
        // Kinds
        NormalSingle,
        NormalRange,
        SpecialSingle,
        SpecialRange,
        Support,
        Interference,
        Recovery,
    }
}
