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

namespace mitama.Pages.RegionConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class UnitViewer
{
    string _regionName;

    public UnitViewer()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
        _regionName = Director.ReadCache().Region;
    }

    private void UnitTreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        using var sr =
            new StreamReader(@$"{Director.UnitDir(_regionName, args.InvokedItem.As<ExplorerItem>().Parent!)}\{args.InvokedItem.As<ExplorerItem>().Name!}.json");
        var json = sr.ReadToEnd();
        var unit = Unit.FromJson(json);
        UnitView.ItemsSource = unit.Memorias;
    }

    private void UnitTreeView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TreeView view) return;
        view.ItemsSource = new ObservableCollection<ExplorerItem>(Util.LoadMemberNames(_regionName).Select(name =>
        {
            return new ExplorerItem
            {
                Name = name,
                Type = ExplorerItem.ExplorerItemType.Folder,
                Children = new ObservableCollection<ExplorerItem>(Directory.GetFiles($"{Director.UnitDir(_regionName, name)}").Select(path =>
                {
                    using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
                    var json = sr.ReadToEnd();
                    return new ExplorerItem
                    {
                        Parent = name,
                        Name = Unit.FromJson(json).UnitName,
                        Type = ExplorerItem.ExplorerItemType.File
                    };
                }))
            };
        }));
    }
}

public class ExplorerItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public enum ExplorerItemType { Folder, File };
    public string? Parent { get; set; }
    public string? Name { get; set; }
    public ExplorerItemType Type { get; set; }
    private ObservableCollection<ExplorerItem>? _children;
    public ObservableCollection<ExplorerItem> Children
    {
        get => _children ??= new ObservableCollection<ExplorerItem>();
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
            NotifyPropertyChanged("IsExpanded");
        }
    }

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
