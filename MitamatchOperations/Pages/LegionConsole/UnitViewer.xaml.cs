using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.IO;
using Microsoft.UI.Xaml.Navigation;
using mitama.Domain;
using mitama.Pages.Common;
using WinRT;
using System.Linq;
using System.Text;
using System;

namespace mitama.Pages.LegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class UnitViewer
{
    private readonly string _LegionName;
    private string _picked = null;

    public UnitViewer()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
        _LegionName = Director.ReadCache().Legion;
    }

    private void UnitTreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        if (Util.LoadMemberNames(_LegionName).Contains(args.InvokedItem.As<ExplorerItem>().Name!))
        {
            return;
        }
        var path = _picked switch
        {
            _ when _picked is not null => $@"{_picked}\{args.InvokedItem.As<ExplorerItem>().Parent!}\{args.InvokedItem.As<ExplorerItem>().Name!}.json",
            _ => @$"{Director.UnitDir(_LegionName, args.InvokedItem.As<ExplorerItem>().Parent!)}\{args.InvokedItem.As<ExplorerItem>().Name!}.json",
        };
        using var sr = new StreamReader(path);
        var json = sr.ReadToEnd();
        var (isLegacy, unit) = Unit.FromJson(json);
        if (isLegacy)
        {
            File.WriteAllBytes(path, new UTF8Encoding(true).GetBytes(unit.ToJson()));
        }
        UnitView.ItemsSource = unit.Memorias;
    }

    private void UnitTreeView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TreeView view) return;
        Init(ref view);
    }

    private void Init(ref TreeView view)
    {
        view.ItemsSource = new ObservableCollection<ExplorerItem>(Util.LoadMemberNames(_LegionName).Select(name =>
        {
            return new ExplorerItem
            {
                Name = name,
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children = new ObservableCollection<ExplorerItem>(
                    Directory.GetFiles($"{Director.UnitDir(_LegionName, name)}").Select(path =>
                    {
                        var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                        var json = sr.ReadToEnd();
                        var (isLegacy, unit) = Unit.FromJson(json);
                        sr.Close();
                        if (isLegacy)
                        {
                            File.WriteAllBytes(path, new UTF8Encoding(true).GetBytes(unit.ToJson()));
                        }
                        return new ExplorerItem
                        {
                            Parent = name,
                            Name = unit.UnitName,
                            Path = path,
                            Type = ExplorerItem.ExplorerItemType.File
                        };
                    }))
            };
        }));
    }

    private void DeleteConfirmation_Click(object sender, RoutedEventArgs _)
    {
        if (sender is not Button  button) return;
        File.Delete(button.AccessKey);
        Init(ref UnitTreeView);
    }

    private void Reset_Click(object _, RoutedEventArgs _e)
    {
        Init(ref UnitTreeView);
        _picked = null;
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        Load(ref UnitTreeView);
    }

    private void Load(ref TreeView unitTreeView)
    {
        var date = $"{Calendar.Date:yyyy-MM-dd}";

        var OpponentDir = _picked = @$"{Director.LogDir(_LegionName)}/{date}/Opponents";
        var opponentNames = Directory.GetDirectories(OpponentDir).Select(path => path.Split('\\').Last()).ToArray();
        unitTreeView.ItemsSource = new ObservableCollection<ExplorerItem>(opponentNames.Select(name =>
        {
            return new ExplorerItem
            {
                Name = name,
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children = new ObservableCollection<ExplorerItem>(
                    Directory.GetFiles(@$"{OpponentDir}\{name}\Units").Select(path =>
                    {
                        var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                        var json = sr.ReadToEnd();
                        var (isLegacy, unit) = Unit.FromJson(json);
                        sr.Close();
                        if (isLegacy)
                        {
                            File.WriteAllBytes(path, new UTF8Encoding(true).GetBytes(unit.ToJson()));
                        }
                        return new ExplorerItem
                        {
                            Parent = name,
                            Name = unit.UnitName,
                            Path = path,
                            Type = ExplorerItem.ExplorerItemType.File
                        };
                    }))
            };
        }));
    }
}

public class ExplorerItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public enum ExplorerItemType { Folder, File };
    public string Parent { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }

    public ExplorerItemType Type { get; set; }
    private ObservableCollection<ExplorerItem> _children;
    public ObservableCollection<ExplorerItem> Children
    {
        get => _children ??= [];
        set => _children = value;
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            NotifyPropertyChanged(nameof(IsExpanded));
        }
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

internal class ExplorerItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate FolderTemplate { get; set; }
    public DataTemplate FileTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item)
    {
        var explorerItem = (ExplorerItem)item;
        return explorerItem.Type == ExplorerItem.ExplorerItemType.Folder ? FolderTemplate : FileTemplate;
    }
}
