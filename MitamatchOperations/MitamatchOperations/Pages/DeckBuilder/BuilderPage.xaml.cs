using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Data;
using DynamicData;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using NumSharp.Utilities;
using Windows.ApplicationModel.DataTransfer;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.DeckBuilder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuilderPage : Page
    {
        // Temporarily store the selected Memoria
        private Memoria[] selectedMemorias = [];

        private ObservableCollection<Memoria> _deck { get; set; } = new();
        private ListCollectionView _deckView { get; set; }
        private ListCollectionView _sourceView { get; set; }
        private ObservableCollection<Memoria> Sources { get; set; } = new(Memoria.List.Where(m => Costume.List[1].CanBeEquipped(m)));
        private ObservableCollection<MyTreeNode> TreeNodes { get; set; } = new();
        private Hashset<FilterType> _currentFilters = [];

        // Filter
        private readonly Dictionary<FilterType, Func<Memoria, bool>> Filters = new();

        public BuilderPage()
        {
            InitFilters();
            InitializeComponent();
            _deckView = new ListCollectionView(_deck);
            _sourceView = new ListCollectionView(Sources);
            InitAdvancedOptions();
        }

        private void MemeriaSources_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            selectedMemorias = e.Items.Select(v => (Memoria)v).ToArray();
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void MemeriaSources_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }
        private void Deck_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        private void Deck_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {

        }

        private void Deck_Drop(object sender, DragEventArgs e)
        {
            _deck.Add(selectedMemorias);
            Sources.Remove(selectedMemorias);
            _deckView = new ListCollectionView(Sources);
            MemoriaSources.ItemsSource = _deckView;
        }


        private void MemeriaSources_Drop(object sender, DragEventArgs e)
        {
            Sources.Add(selectedMemorias);
            _deck.Remove(selectedMemorias);
            _deckView = new ListCollectionView(Sources);
            _deckView.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));

            MemoriaSources.ItemsSource = _deckView;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    _deck.Clear();
                    _deckView = new ListCollectionView(_deck);
                    Sources = new(Memoria.List.Where(memoria => Costume.List[0].CanBeEquipped(memoria)));

                    Deck.ItemsSource = _deckView;
                    MemoriaSources.ItemsSource = Sources;
                    FilterContent.Children.Clear();
                    FilterContent.Children.Add(new CheckBox { Name = "NSingle", Content = "’Êí’P‘Ì" });
                    FilterContent.Children.Add(new CheckBox { Name = "NRange", Content = "’Êí”ÍˆÍ" });
                    FilterContent.Children.Add(new CheckBox { Name = "SpRange", Content = "“ÁŽê’P‘Ì" });
                    FilterContent.Children.Add(new CheckBox { Name = "SpRange", Content = "“ÁŽê”ÍˆÍ" });
                    _currentFilters = [
                        FilterType.NormalSingle,
                        FilterType.NormalRange,
                        FilterType.SpecialSingle,
                        FilterType.SpecialRange,
                    ];
                }
                else
                {
                    _deck.Clear();
                    _deckView = new ListCollectionView(_deck);
                    Sources = new(Memoria.List.Where(memoria => Costume.List[1].CanBeEquipped(memoria)));
                    FilterContent.Children.Clear();
                    FilterContent.Children.Add(new CheckBox { Name = "Buff", Content = "Žx‰‡" });
                    FilterContent.Children.Add(new CheckBox { Name = "Debuff", Content = "–WŠQ" });
                    FilterContent.Children.Add(new CheckBox { Name = "Heal", Content = "‰ñ•œ" });
                    _currentFilters = [
                        FilterType.Support,
                        FilterType.Interference,
                        FilterType.Recovery
                    ];
                    Deck.ItemsSource = _deckView;
                    MemoriaSources.ItemsSource = Sources;
                }
            }
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;

            switch (box.Content)
            {
                case "’Êí’P‘Ì":
                    {
                        _currentFilters.Add(FilterType.NormalSingle);
                        break;
                    }
                case "’Êí”ÍˆÍ":
                    {
                        _currentFilters.Add(FilterType.NormalRange);
                        break;
                    }
                case "“ÁŽê’P‘Ì":
                    {
                        _currentFilters.Add(FilterType.SpecialSingle);
                        break;
                    }
                case "“ÁŽê”ÍˆÍ":
                    {
                        _currentFilters.Add(FilterType.SpecialRange);
                        break;
                    }
                case "Žx‰‡":
                    {
                        _currentFilters.Add(FilterType.Support);
                        break;
                    }
                case "–WŠQ":
                    {
                        _currentFilters.Add(FilterType.Interference);
                        break;
                    }
                case "‰ñ•œ":
                    {
                        _currentFilters.Add(FilterType.Recovery);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            Sources.Add(Memoria.List.Where(memoria => !Sources.Contains(memoria) && _currentFilters.All(filter => Filters[filter](memoria))));
        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;

            switch (box.Content)
            {
                case "’Êí’P‘Ì":
                    {
                        _currentFilters.Remove(FilterType.NormalSingle);
                        break;
                    }
                case "’Êí”ÍˆÍ":
                    {
                        _currentFilters.Remove(FilterType.NormalRange);
                        break;
                    }
                case "“ÁŽê’P‘Ì":
                    {
                        _currentFilters.Remove(FilterType.SpecialSingle);
                        break;
                    }
                case "“ÁŽê”ÍˆÍ":
                    {
                        _currentFilters.Remove(FilterType.SpecialRange);
                        break;
                    }
                case "Žx‰‡":
                    {
                        _currentFilters.Remove(FilterType.Support);
                        break;
                    }
                case "–WŠQ":
                    {
                        _currentFilters.Remove(FilterType.Interference);
                        break;
                    }
                case "‰ñ•œ":
                    {
                        _currentFilters.Remove(FilterType.Recovery);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            Sources.Remove(Memoria.List.Where(memoria => _currentFilters.All(filter => Filters[filter](memoria))));
        }

        private void InitAdvancedOptions()
        {
            TreeNodes =
            [
                new()
                {
                    Text = "‘®«",
                    Children =
                    [
                        new() { Text = "‰Î" },
                        new() { Text = "…" },
                        new() { Text = "•—" },
                        new() { Text = "Œõ" },
                        new() { Text = "ˆÅ" }
                    ]
                },
                new()
                {
                    Text = "”ÍˆÍ",
                    Children =
                    [
                        new() { Text = "A" },
                        new() { Text = "B" },
                        new() { Text = "C" },
                        new() { Text = "D" },
                        new() { Text = "E" }
                    ]
                },
                new()
                {
                    Text = "Œø‰Ê—Ê",
                    Children =
                    [
                        new() { Text = "‡T" },
                        new() { Text = "‡U" },
                        new() { Text = "‡V" },
                        new() { Text = "‡V+" },
                        new() { Text = "‡W" },
                        new() { Text = "‡W+" },
                        new() { Text = "‡X" },
                        new() { Text = "‡X+" },
                        new() { Text = "LG" },
                        new() { Text = "LG+" }
                    ]
                },
                new()
                {
                    Text = "ƒXƒLƒ‹Œø‰Ê",
                    Children =
                    [
                        // UŒ‚Œn
                        new() { Text = "A up" },
                        new() { Text = "A down" },
                        new() { Text = "SA up" },
                        new() { Text = "SA down" },
                        // –hŒäŒn
                        new() { Text = "D up" },
                        new() { Text = "D down" },
                        new() { Text = "SD up" },
                        new() { Text = "SD down" },
                        // ‘®«Œn
                        new() { Text = "Fire Power up" },
                        new() { Text = "Fire Power down" },
                        new() { Text = "Water Power up" },
                        new() { Text = "Water Power down" },
                        new() { Text = "Wind Power up" },
                        new() { Text = "Wind Power down" },
                        new() { Text = "Light Power up" },
                        new() { Text = "Light Power down" },
                        new() { Text = "Dark Power up" },
                        new() { Text = "Dark Power down" },
                        new() { Text = "Fire Guard up" },
                        new() { Text = "Fire Guard down" },
                        new() { Text = "Water Guard up" },
                        new() { Text = "Water Guard down" },
                        new() { Text = "Wind Guard up" },
                        new() { Text = "Wind Guard down" },
                        new() { Text = "Light Guard up" },
                        new() { Text = "Light Guard down" },
                        new() { Text = "Dark Guard up" },
                        new() { Text = "Dark Guard down" },
                        // ‚»‚Ì‘¼
                        new() { Text = "HP up" },
                    ]
                }
            ];

            AdvancedOptions.ItemsSource = TreeNodes;
            _currentFilters = [
                FilterType.Support,
                FilterType.Interference,
                FilterType.Recovery
            ];
        }

        private void AdvancedOption_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;

            switch (box.Content)
            {
                case "‘®«":
                    {
                        foreach (var node in TreeNodes[0].Children)
                        {
                            node.IsChecked = true;
                        }
                        FilterType[] remove = [
                            FilterType.Fire,
                            FilterType.Water,
                            FilterType.Wind,
                            FilterType.Light,
                            FilterType.Dark,
                        ];
                        _currentFilters.RemoveWhere(filter => remove.Contains(filter));
                        break;
                    }
                case "‰Î":
                    {
                        _currentFilters.Add(FilterType.Fire);
                        break;
                    }
                case "…":
                    {
                        _currentFilters.Add(FilterType.Water);
                        break;
                    }
                case "•—":
                    {
                        _currentFilters.Add(FilterType.Wind);
                        break;
                    }
                case "Œõ":
                    {
                        _currentFilters.Add(FilterType.Light);
                        break;
                    }
                case "ˆÅ":
                    {
                        _currentFilters.Add(FilterType.Dark);
                        break;
                    }
                case "”ÍˆÍ":
                    {
                        foreach (var node in TreeNodes[1].Children)
                        {
                            node.IsChecked = true;
                        }
                        FilterType[] remove = [
                            FilterType.A,
                            FilterType.B,
                            FilterType.C,
                            FilterType.D,
                            FilterType.E,
                        ];
                        _currentFilters.RemoveWhere(filter => remove.Contains(filter));
                        break;
                    }
                case "A":
                    {
                        _currentFilters.Add(FilterType.A);
                        break;
                    }
                case "B":
                    {
                        _currentFilters.Add(FilterType.B);
                        break;
                    }
                case "C":
                    {
                        _currentFilters.Add(FilterType.C);
                        break;
                    }
                case "D":
                    {
                        _currentFilters.Add(FilterType.D);
                        break;
                    }
                case "E":
                    {
                        _currentFilters.Add(FilterType.E);
                        break;
                    }
                case "Œø‰Ê—Ê":
                    {
                        foreach (var node in TreeNodes[2].Children)
                        {
                            node.IsChecked = true;
                        }
                        FilterType[] remove =[
                            FilterType.One,
                            FilterType.Two,
                            FilterType.Three,
                            FilterType.ThreePlus,
                            FilterType.Four,
                            FilterType.FourPlus,
                            FilterType.Five,
                            FilterType.FivePlus,
                            FilterType.Lg,
                            FilterType.LgPlus,
                        ];
                        _currentFilters.RemoveWhere(filter => remove.Contains(filter));
                        break;
                    }
                case "‡T":
                    {
                        _currentFilters.Add(FilterType.One);
                        break;
                    }
                case "‡U":
                    {
                        _currentFilters.Add(FilterType.Two);
                        break;
                    }
                case "‡V":
                    {
                        _currentFilters.Add(FilterType.Three);
                        break;
                    }
                case "‡V+":
                    {
                        _currentFilters.Add(FilterType.ThreePlus);
                        break;
                    }
                case "‡W":
                    {
                        _currentFilters.Add(FilterType.Four);
                        break;
                    }
                case "‡W+":
                    {
                        _currentFilters.Add(FilterType.FourPlus);
                        break;
                    }
                case "‡X":
                    {
                        _currentFilters.Add(FilterType.Five);
                        break;
                    }
                case "‡X+":
                    {
                        _currentFilters.Add(FilterType.FivePlus);
                        break;
                    }
                case "LG":
                    {
                        _currentFilters.Add(FilterType.Lg);
                        break;
                    }
                case "LG+":
                    {
                        _currentFilters.Add(FilterType.LgPlus);
                        break;
                    }
                case "ƒXƒLƒ‹Œø‰Ê":
                    {
                        foreach (var node in TreeNodes[3].Children)
                        {
                            node.IsChecked = true;
                        }
                        FilterType[] remove = [
                            FilterType.Au,
                            FilterType.Ad,
                            FilterType.SAu,
                            FilterType.SAd,
                            FilterType.Du,
                            FilterType.Dd,
                            FilterType.SDu,
                            FilterType.SDd,
                            FilterType.HPu,
                            FilterType.FPu,
                            FilterType.FPd,
                            FilterType.WaPu,
                            FilterType.WaPd,
                            FilterType.WiPu,
                            FilterType.WiPd,
                            FilterType.LPu,
                            FilterType.LPd,
                            FilterType.DPu,
                            FilterType.DPd,
                            FilterType.FGu,
                            FilterType.FGd,
                            FilterType.WaGu,
                            FilterType.WaGd,
                            FilterType.WiGu,
                            FilterType.WiGd,
                            FilterType.LGu,
                            FilterType.LGd,
                            FilterType.DGu,
                            FilterType.DGd,
                        ];
                        _currentFilters.RemoveWhere(filter => remove.Contains(filter));
                        break;
                    }
                case "A up":
                    {
                        _currentFilters.Add(FilterType.Au);
                        break;
                    }
                case "A down":
                    {
                        _currentFilters.Add(FilterType.Ad);
                        break;
                    }
                case "SA up":
                    {
                        _currentFilters.Add(FilterType.SAu);
                        break;
                    }
                case "SA down":
                    {
                        _currentFilters.Add(FilterType.SAd);
                        break;
                    }
                case "D up":
                    {
                        _currentFilters.Add(FilterType.Du);
                        break;
                    }
                case "D down":
                    {
                        _currentFilters.Add(FilterType.Dd);
                        break;
                    }
                case "SD up":
                    {
                        _currentFilters.Add(FilterType.SDu);
                        break;
                    }
                case "SD down":
                    {
                        _currentFilters.Add(FilterType.SDd);
                        break;
                    }
                case "Fire Power up":
                    {
                        _currentFilters.Add(FilterType.FPu);
                        break;
                    }
                case "Fire Power down":
                    {
                        _currentFilters.Add(FilterType.FPd);
                        break;
                    }
                case "Water Power up":
                    {
                        _currentFilters.Add(FilterType.WaPu);
                        break;
                    }
                case "Water Power down":
                    {
                        _currentFilters.Add(FilterType.WaPd);
                        break;
                    }
                case "Wind Power up":
                    {
                        _currentFilters.Add(FilterType.WiPu);
                        break;
                    }
                case "Wind Power down":
                    {
                        _currentFilters.Add(FilterType.WiPd);
                        break;
                    }
                case "Light Power up":
                    {
                        _currentFilters.Add(FilterType.LPu);
                        break;
                    }
                case "Light Power down":
                    {
                        _currentFilters.Add(FilterType.LPd);
                        break;
                    }
                case "Dark Power up":
                    {
                        _currentFilters.Add(FilterType.DPu);
                        break;
                    }
                case "Dark Power down":
                    {
                        _currentFilters.Add(FilterType.DPd);
                        break;
                    }
                case "Fire Guard up":
                    {
                        _currentFilters.Add(FilterType.FGu);
                        break;
                    }
                case "Fire Guard down":
                    {
                        _currentFilters.Add(FilterType.FGd);
                        break;
                    }
                case "Water Guard up":
                    {
                        _currentFilters.Add(FilterType.WaGu);
                        break;
                    }
                case "Water Guard down":
                    {
                        _currentFilters.Add(FilterType.WaGd);
                        break;
                    }
                case "Wind Guard up":
                    {
                        _currentFilters.Add(FilterType.WiGu);
                        break;
                    }
                case "Wind Guard down":
                    {
                        _currentFilters.Add(FilterType.WiGd);
                        break;
                    }
                case "Light Guard up":
                    {
                        _currentFilters.Add(FilterType.LGu);
                        break;
                    }
                case "Light Guard down":
                    {
                        _currentFilters.Add(FilterType.LGd);
                        break;
                    }
                case "Dark Guard up":
                    {
                        _currentFilters.Add(FilterType.DGu);
                        break;
                    }
                case "Dark Guard down":
                    {
                        _currentFilters.Add(FilterType.DGd);
                        break;
                    }
                case "HP up":
                    {
                        _currentFilters.Add(FilterType.HPu);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            Sources.Add(Memoria.List.Where(memoria => !Sources.Contains(memoria) && _currentFilters.All(filter => Filters[filter](memoria))));
        }

        private void AdvancedOption_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;

            switch (box.Content)
            {
                case "‘®«":
                    {
                        foreach (var node in TreeNodes[0].Children)
                        {
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "‰Î":
                    {
                        _currentFilters.Remove(FilterType.Fire);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Fire]));
                        break;
                    }
                case "…":
                    {
                        _currentFilters.Remove(FilterType.Water);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Water]));
                        break;
                    }
                case "•—":
                    {
                        _currentFilters.Remove(FilterType.Wind);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Wind]));
                        break;
                    }
                case "Œõ":
                    {
                        _currentFilters.Remove(FilterType.Light);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Light]));
                        break;
                    }
                case "ˆÅ":
                    {
                        _currentFilters.Remove(FilterType.Dark);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Dark]));
                        break;
                    }
                case "”ÍˆÍ":
                    {
                        foreach (var node in TreeNodes[1].Children)
                        {
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "A":
                    {
                        _currentFilters.Remove(FilterType.A);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.A]));
                        break;
                    }
                case "B":
                    {
                        _currentFilters.Remove(FilterType.B);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.B]));
                        break;
                    }
                case "C":
                    {
                        _currentFilters.Remove(FilterType.C);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.C]));
                        break;
                    }
                case "D":
                    {
                        _currentFilters.Remove(FilterType.D);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.D]));
                        break;
                    }
                case "E":
                    {
                        _currentFilters.Remove(FilterType.E);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.E]));
                        break;
                    }
                case "Œø‰Ê—Ê":
                    {
                        foreach (var node in TreeNodes[2].Children)
                        {
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "‡T":
                    {
                        _currentFilters.Remove(FilterType.One);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.One]));
                        break;
                    }
                case "‡U":
                    {
                        _currentFilters.Remove(FilterType.Two);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Two]));
                        break;
                    }
                case "‡V":
                    {
                        _currentFilters.Remove(FilterType.Three);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Three]));
                        break;
                    }
                case "‡V+":
                    {
                        _currentFilters.Remove(FilterType.ThreePlus);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.ThreePlus]));
                        break;
                    }
                case "‡W":
                    {
                        _currentFilters.Remove(FilterType.Four);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Four]));
                        break;
                    }
                case "‡W+":
                    {
                        _currentFilters.Remove(FilterType.FourPlus);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.FourPlus]));
                        break;
                    }
                case "‡X":
                    {
                        _currentFilters.Remove(FilterType.Five);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Five]));
                        break;
                    }
                case "‡X+":
                    {
                        _currentFilters.Remove(FilterType.FivePlus);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.FivePlus]));
                        break;
                    }
                case "LG":
                    {
                        _currentFilters.Remove(FilterType.Lg);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Lg]));
                        break;
                    }
                case "LG+":
                    {
                        _currentFilters.Remove(FilterType.LgPlus);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.LgPlus]));
                        break;
                    }
                case "ƒXƒLƒ‹Œø‰Ê":
                    {
                        foreach (var node in TreeNodes[3].Children)
                        {
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "A up":
                    {
                        _currentFilters.Remove(FilterType.Au);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Au]));
                        break;
                    }
                case "A down":
                    {
                        _currentFilters.Remove(FilterType.Ad);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Ad]));
                        break;
                    }
                case "SA up":
                    {
                        _currentFilters.Remove(FilterType.SAu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.SAu]));
                        break;
                    }
                case "SA down":
                    {
                        _currentFilters.Remove(FilterType.SAd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.SAd]));
                        break;
                    }
                case "D up":
                    {
                        _currentFilters.Remove(FilterType.Du);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Du]));
                        break;
                    }
                case "D down":
                    {
                        _currentFilters.Remove(FilterType.Dd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.Dd]));
                        break;
                    }
                case "SD up":
                    {
                        _currentFilters.Remove(FilterType.SDu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.SDu]));
                        break;
                    }
                case "SD down":
                    {
                        _currentFilters.Remove(FilterType.SDd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.SDd]));
                        break;
                    }
                case "Fire Power up":
                    {
                        _currentFilters.Remove(FilterType.FPu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.FPu]));
                        break;
                    }
                case "Fire Power down":
                    {
                        _currentFilters.Remove(FilterType.FPd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.FPd]));
                        break;
                    }
                case "Water Power up":
                    {
                        _currentFilters.Remove(FilterType.WaPu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WaPu]));
                        break;
                    }
                case "Water Power down":
                    {
                        _currentFilters.Remove(FilterType.WaPd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WaPd]));
                        break;
                    }
                case "Wind Power up":
                    {
                        _currentFilters.Remove(FilterType.WiPu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WiPu]));
                        break;
                    }
                case "Wind Power down":
                    {
                        _currentFilters.Remove(FilterType.WiPd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WiPd]));
                        break;
                    }
                case "Light Power up":
                    {
                        _currentFilters.Remove(FilterType.LPu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.LPu]));
                        break;
                    }
                case "Light Power down":
                    {
                        _currentFilters.Remove(FilterType.LPd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.LPd]));
                        break;
                    }
                case "Dark Power up":
                    {
                        _currentFilters.Remove(FilterType.DPu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.DPu]));
                        break;
                    }
                case "Dark Power down":
                    {
                        _currentFilters.Remove(FilterType.DPd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.DPd]));
                        break;
                    }
                case "Fire Guard up":
                    {
                        _currentFilters.Remove(FilterType.FGu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.FGu]));
                        break;
                    }
                case "Fire Guard down":
                    {
                        _currentFilters.Remove(FilterType.FGd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.FGd]));
                        break;
                    }
                case "Water Guard up":
                    {
                        _currentFilters.Remove(FilterType.WaGu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WaGu]));
                        break;
                    }
                case "Water Guard down":
                    {
                        _currentFilters.Remove(FilterType.WaGd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WaGd]));
                        break;
                    }
                case "Wind Guard up":
                    {
                        _currentFilters.Remove(FilterType.WiGu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WiGu]));
                        break;
                    }
                case "Wind Guard down":
                    {
                        _currentFilters.Remove(FilterType.WiGd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.WiGd]));
                        break;
                    }
                case "Light Guard up":
                    {
                        _currentFilters.Remove(FilterType.LGu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.LGu]));
                        break;
                    }
                case "Light Guard down":
                    {
                        _currentFilters.Remove(FilterType.LGd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.LGd]));
                        break;
                    }
                case "Dark Guard up":
                    {
                        _currentFilters.Remove(FilterType.DGu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.DGu]));
                        break;
                    }
                case "Dark Guard down":
                    {
                        _currentFilters.Remove(FilterType.DGd);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.DGd]));
                        break;
                    }
                case "HP up":
                    {
                        _currentFilters.Remove(FilterType.HPu);
                        Sources.Remove(Memoria.List.Where(Filters[FilterType.HPu]));
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
        }
        private void InitFilters()
        {
            Filters.Add(FilterType.Sentinel, _ => false);

            Filters.Add(FilterType.NormalSingle, memoria => memoria.Kind is Vanguard(VanguardKind.NormalSingle));
            Filters.Add(FilterType.NormalRange, memoria => memoria.Kind is Vanguard(VanguardKind.NormalRange));
            Filters.Add(FilterType.SpecialSingle, memoria => memoria.Kind is Vanguard(VanguardKind.SpecialSingle));
            Filters.Add(FilterType.SpecialRange, memoria => memoria.Kind is Vanguard(VanguardKind.SpecialRange));
            Filters.Add(FilterType.Support, memoria => memoria.Kind is Rearguard(RearguardKind.Support));
            Filters.Add(FilterType.Interference, memoria => memoria.Kind is Rearguard(RearguardKind.Interference));
            Filters.Add(FilterType.Recovery, memoria => memoria.Kind is Rearguard(RearguardKind.Recovery));

            Filters.Add(FilterType.Fire, memoria => memoria.Element is Element.Fire);
            Filters.Add(FilterType.Water, memoria => memoria.Element is Element.Water);
            Filters.Add(FilterType.Wind, memoria => memoria.Element is Element.Wind);
            Filters.Add(FilterType.Light, memoria => memoria.Element is Element.Light);
            Filters.Add(FilterType.Dark, memoria => memoria.Element is Element.Dark);

            Filters.Add(FilterType.A, memoria => memoria.Skill.Range is Domain.Range.A);
            Filters.Add(FilterType.B, memoria => memoria.Skill.Range is Domain.Range.B);
            Filters.Add(FilterType.C, memoria => memoria.Skill.Range is Domain.Range.C);
            Filters.Add(FilterType.D, memoria => memoria.Skill.Range is Domain.Range.D);
            Filters.Add(FilterType.E, memoria => memoria.Skill.Range is Domain.Range.E);

            Filters.Add(FilterType.One, memoria => memoria.Skill.Level is Level.One);
            Filters.Add(FilterType.Two, memoria => memoria.Skill.Level is Level.Two);
            Filters.Add(FilterType.Three, memoria => memoria.Skill.Level is Level.Three);
            Filters.Add(FilterType.ThreePlus, memoria => memoria.Skill.Level is Level.ThreePlus);
            Filters.Add(FilterType.Four, memoria => memoria.Skill.Level is Level.Four);
            Filters.Add(FilterType.FourPlus, memoria => memoria.Skill.Level is Level.FourPlus);
            Filters.Add(FilterType.Five, memoria => memoria.Skill.Level is Level.Five);
            Filters.Add(FilterType.FivePlus, memoria => memoria.Skill.Level is Level.FivePlus);
            Filters.Add(FilterType.Lg, memoria => memoria.Skill.Level is Level.Lg);
            Filters.Add(FilterType.LgPlus, memoria => memoria.Skill.Level is Level.LgPlus);

            Filters.Add(
                FilterType.Au,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusUp && stat.As<StatusUp>().Stat is Atk)
            );
            Filters.Add(
               FilterType.Ad,
               memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusDown && stat.As<StatusDown>().Stat is Atk)
            );
            Filters.Add(
               FilterType.SAu,
               memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusUp && stat.As<StatusUp>().Stat is SpAtk)
            );
            Filters.Add(
                FilterType.SAd,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusDown && stat.As<StatusDown>().Stat is SpAtk)
            );
            Filters.Add(
                FilterType.Du,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusUp && stat.As<StatusUp>().Stat is Def)
            );
            Filters.Add(
                FilterType.Dd,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusDown && stat.As<StatusDown>().Stat is Def)
            );
            Filters.Add(
                FilterType.SDu,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusUp && stat.As<StatusUp>().Stat is SpDef)
            );
            Filters.Add(
                FilterType.SDd,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusDown && stat.As<StatusDown>().Stat is SpDef)
            );
            Filters.Add(
                FilterType.HPu,
                memoria => memoria.Skill.StatusChanges.Any(stat => stat is StatusUp && stat.As<StatusUp>().Stat is Life)
            );
            Filters.Add(
                FilterType.FPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementAttack
                        && stat.As<StatusUp>().Stat.As<ElementAttack>().Element is Element.Fire
                    )
            );
            Filters.Add(
                FilterType.FPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementAttack
                        && stat.As<StatusDown>().Stat.As<ElementAttack>().Element is Element.Fire
                    )
            );
            Filters.Add(
                FilterType.WaPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementAttack
                        && stat.As<StatusUp>().Stat.As<ElementAttack>().Element is Element.Water
                    )
            );
            Filters.Add(
                FilterType.WaPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementAttack
                        && stat.As<StatusDown>().Stat.As<ElementAttack>().Element is Element.Water
                    )
            );
            Filters.Add(
                FilterType.WiPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementAttack
                        && stat.As<StatusUp>().Stat.As<ElementAttack>().Element is Element.Wind
                    )
            );
            Filters.Add(
                FilterType.WiPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementAttack
                        && stat.As<StatusDown>().Stat.As<ElementAttack>().Element is Element.Wind
                    )
            );
            Filters.Add(
                FilterType.LPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementAttack
                        && stat.As<StatusUp>().Stat.As<ElementAttack>().Element is Element.Light
                    )
            );
            Filters.Add(
                FilterType.LPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementAttack
                        && stat.As<StatusDown>().Stat.As<ElementAttack>().Element is Element.Light
                    )
            );
            Filters.Add(
                FilterType.DPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementAttack
                        && stat.As<StatusUp>().Stat.As<ElementAttack>().Element is Element.Dark
                    )
            );
            Filters.Add(
                FilterType.DPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementAttack
                        && stat.As<StatusDown>().Stat.As<ElementAttack>().Element is Element.Dark
                    )
            );
            Filters.Add(
                FilterType.FGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementGuard
                        && stat.As<StatusUp>().Stat.As<ElementGuard>().Element is Element.Fire
                    )
            );
            Filters.Add(
                FilterType.FGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementGuard
                        && stat.As<StatusDown>().Stat.As<ElementGuard>().Element is Element.Fire
                    )
            );
            Filters.Add(
                FilterType.WaGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementGuard(Element.Water)
                    )
            );
            Filters.Add(
                FilterType.WaGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementGuard(Element.Water)
                    )
            );
            Filters.Add(
                FilterType.WiGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementGuard
                        && stat.As<StatusUp>().Stat.As<ElementGuard>().Element is Element.Wind
                    )
            );
            Filters.Add(
                FilterType.WiGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementGuard
                        && stat.As<StatusDown>().Stat.As<ElementGuard>().Element is Element.Wind
                    )
            );
            Filters.Add(
                FilterType.LGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementGuard
                        && stat.As<StatusUp>().Stat.As<ElementGuard>().Element is Element.Light
                    )
            );
            Filters.Add(
                FilterType.LGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementGuard
                        && stat.As<StatusDown>().Stat.As<ElementGuard>().Element is Element.Light
                    )
            );
            Filters.Add(
                FilterType.DGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp
                        && stat.As<StatusUp>().Stat is ElementGuard
                        && stat.As<StatusUp>().Stat.As<ElementGuard>().Element is Element.Dark
                    )
            );
            Filters.Add(
                FilterType.DGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown
                        && stat.As<StatusDown>().Stat is ElementGuard
                        && stat.As<StatusDown>().Stat.As<ElementGuard>().Element is Element.Dark
                    )
            );
        }
    }

    public enum FilterType
    {
        Sentinel,
        // Kinds
        NormalSingle,
        NormalRange,
        SpecialSingle,
        SpecialRange,
        Support,
        Interference,
        Recovery,
        // Elements
        Fire,
        Water,
        Wind,
        Light,
        Dark,
        // Ranges
        A,
        B,
        C,
        D,
        E,
        // Skill Levels
        One,
        Two,
        Three,
        ThreePlus,
        Four,
        FourPlus,
        Five,
        FivePlus,
        Lg,
        LgPlus,
        // Skill Effects
        Au,
        Ad,
        SAu,
        SAd,
        Du,
        Dd,
        SDu,
        SDd,
        HPu,
        FPu,
        FPd,
        WaPu,
        WaPd,
        WiPu,
        WiPd,
        LPu,
        LPd,
        DPu,
        DPd,
        FGu,
        FGd,
        WaGu,
        WaGd,
        WiGu,
        WiGd,
        LGu,
        LGd,
        DGu,
        DGd,
    }
    public class MyTreeNode
    {
        public string Text { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = true;

        public ObservableCollection<MyTreeNode> Children { get; set; } = [];
    }
}
