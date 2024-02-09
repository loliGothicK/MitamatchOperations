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
using Tensorflow;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.DeckBuilder;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MemoriaManagePage : Page
{
    private string imgPath;
    private int verticalCount = 3;
    private int horizontalCount = 8;
    private readonly ObservableCollection<MemoriaWithConcentration> Memorias = [];
    private readonly ObservableCollection<Memoria> Pool = [.. Memoria.List.DistinctBy(x => x.Name)];
    private List<Memoria> selectedMemorias = [];

    public MemoriaManagePage()
    {
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
        var (result, detected) = await MemoriaSearch.Recognise(img, v:verticalCount, h:horizontalCount);
        foreach (var item in detected.Select(x => new MemoriaWithConcentration(x, 4)))
        {
            Memorias.Add(item);
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

    private void Concentration_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender.As<ComboBox>();
        if (comboBox.AccessKey == string.Empty) return;
        var id = int.Parse(comboBox.AccessKey);
        foreach (var item in Memorias.ToList())
        {
            if (item.Memoria.Id == id)
            {
                var newItem = item with { Concentration = comboBox.SelectedIndex };
                var idx = Memorias.IndexOf(item);
                Memorias[idx] = newItem;
            }
        }
    }

    private void Memeria_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items.First() is Memoria)
        {
            selectedMemorias = e.Items.Select(v => (Memoria)v).ToList();
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
        else
        {
            selectedMemorias = e.Items.Select(v => ((MemoriaWithConcentration)v).Memoria).ToList();
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
    }
    
    private void MemeriaSources_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }
    
    private void MemeriaSources_Drop(object sender, DragEventArgs e)
    {
        foreach (var toAdd in Memoria
            .List
            .Where(m => selectedMemorias.Select(s => s.Name).Contains(m.Name)))
        {
            Pool.Add(toAdd);
        }
        foreach (var toRemove in Memorias.Where(m => selectedMemorias.Contains(m.Memoria)))
        {
            Memorias.Remove(toRemove);
        }
    }

    private void Memeria_Drop(object sender, DragEventArgs e)
    {
        foreach (var toRemove in Memoria
            .List
            .Where(m => selectedMemorias.Select(s => s.Name).Contains(m.Name)))
        {
            Pool.Remove(toRemove);
        }
        foreach (var toAdd in selectedMemorias)
        {
            Memorias.Add(new MemoriaWithConcentration(toAdd, 4));
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
                List<MemoriaIdAndConcentration> memorias
                    = info.Memorias == null
                    ? [.. Memorias.Select(m => new MemoriaIdAndConcentration(m.Memoria.Id, m.Concentration))]
                    : [.. info.Memorias.Concat(Memorias.Select(m => new MemoriaIdAndConcentration(m.Memoria.Id, m.Concentration)))];
                var writeJson = (info with {
                    UpdatedAt = DateTime.Now,
                    Memorias = [.. memorias.DistinctBy(x => idToName[x.Id])],
                }).ToJson();
                var save = new UTF8Encoding(true).GetBytes(writeJson);
                sr.Close();
                using var fs = Director.CreateFile(path);
                fs.Write(save, 0, save.Length);
                return Task.CompletedTask;
            }))
            .Build();

        await dialog.ShowAsync();
    }
}
