using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using mitama.Pages.Common;
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
        private List<Memoria> selectedMemorias = [];

        private ObservableCollection<Memoria> Deck { get; set; } = [];
        private ObservableCollection<Memoria> LegendaryDeck { get; set; } = [];
        private ObservableCollection<Memoria> Sources { get; set; } = new(Memoria.List.Where(m => Costume.List[1].CanBeEquipped(m)));
        private ObservableCollection<MyTreeNode> TreeNodes { get; set; } = [];
        private HashSet<FilterType> _currentFilters = [];
        private readonly Domain.Status StatSum = new();
        private string region = "";
        private MemberInfo[] members = [];

        // Filter
        private readonly Dictionary<FilterType, Func<Memoria, bool>> Filters = new();

        public BuilderPage()
        {
            InitFilters();
            InitMembers();
            InitializeComponent();
            InitAdvancedOptions();
        }

        private void InitMembers()
        {
            var cache = Director.ReadCache();
            region = cache.Region;
            members = Util.LoadMembersInfo(region);
        }

        private void Memeria_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            selectedMemorias = e.Items.Select(v => (Memoria)v).ToList();
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

        private void MyDropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
        }

        private void Cleanup()
        {
            selectedMemorias.Clear();

            var (atk, spatk, def, spdef) = Deck.Concat(LegendaryDeck).Select(m => m.Status).Aggregate((a, b) => a + b);
            Atk.Content = $"Atk: {atk}";
            SpAtk.Content = $"SpAtk: {spatk}";
            Def.Content = $"Def: {def}";
            SpDef.Content = $"SpDef: {spdef}";

            Fire.Content = $"��: {Deck.Concat(LegendaryDeck).Count(m => m.Element is Element.Fire)}";
            Water.Content = $"��: {Deck.Concat(LegendaryDeck).Count(m => m.Element is Element.Water)}";
            Wind.Content = $"��: {Deck.Concat(LegendaryDeck).Count(m => m.Element is Element.Wind)}";
            Light.Content = $"��: {Deck.Concat(LegendaryDeck).Count(m => m.Element is Element.Light)}";
            Dark.Content = $"��: {Deck.Concat(LegendaryDeck).Count(m => m.Element is Element.Dark)}";

            if (Deck.DistinctBy(m => m.Name).Count() != Deck.Count)
            {
                GeneralInfoBar.Title = "���o���̃������A���d�����Ă��܂�";
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                GeneralInfoBar.IsOpen = true;
            } else
            {
                GeneralInfoBar.IsOpen = false;
            }
        }

        private void Deck_Drop(object sender, DragEventArgs e)
        {
            foreach (var memoria in selectedMemorias.Where(m => !m.IsLegendary))
            {
                Deck.Add(memoria);
            }
            foreach (var memoria in selectedMemorias.Where(m => m.IsLegendary))
            {
                LegendaryDeck.Add(memoria);
            }
            foreach (var toRemove in Sources.Where(m => selectedMemorias.Select(s => s.Name).Contains(m.Name)).ToList())
            {
                Sources.Remove(toRemove);
            }
            Cleanup();
        }

        private void MemeriaSources_Drop(object sender, DragEventArgs e)
        {
            var dummyCostume = Switch.IsOn ? Costume.List[0] : Costume.List[1];
            foreach (var toAdd in Memoria
                .List
                .Where(dummyCostume.CanBeEquipped)
                .Where(m => selectedMemorias.Select(s => s.Name).Contains(m.Name)))
            {
                Sources.Add(toAdd);
            }
            foreach (var toRemove in selectedMemorias)
            {
                Deck.Remove(toRemove);
               LegendaryDeck.Remove(toRemove);
            }
            Sort(SortOption.SelectedIndex);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    Deck.Clear();
                    Cleanup();
                    Sources = new(Memoria.List.Where(memoria => Costume.List[0].CanBeEquipped(memoria)));

                    MemoriaSources.ItemsSource = Sources;
                    FilterContent.Children.Clear();
                    
                    var c1 = new CheckBox { Content = "�ʏ�P��", IsChecked = true };
                    c1.Checked += Option_Checked;
                    c1.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c1);
                    var c2 = new CheckBox { Content = "�ʏ�͈�", IsChecked = true };
                    c2.Checked += Option_Checked;
                    c2.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c2);
                    var c3 = new CheckBox { Content = "����P��", IsChecked = true };
                    c3.Checked += Option_Checked;
                    c3.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c3);
                    var c4 = new CheckBox { Content = "����͈�", IsChecked = true };
                    c4.Checked += Option_Checked;
                    c4.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c4);

                    _currentFilters = [
                        FilterType.NormalSingle,
                        FilterType.NormalRange,
                        FilterType.SpecialSingle,
                        FilterType.SpecialRange,
                    ];
                }
                else
                {
                    Deck.Clear();
                    Sources = new(Memoria.List.Where(memoria => Costume.List[1].CanBeEquipped(memoria)));
                    FilterContent.Children.Clear();

                    var c1 = new CheckBox { Content = "�x��", IsChecked = true };
                    c1.Checked += Option_Checked;
                    c1.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c1);
                    var c2 = new CheckBox { Content = "�W�Q", IsChecked = true };
                    c2.Checked += Option_Checked;
                    c2.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c2);
                    var c3 = new CheckBox { Content = "��", IsChecked = true };
                    c3.Checked += Option_Checked;
                    c3.Unchecked += Option_Unchecked;
                    FilterContent.Children.Add(c3);

                    _currentFilters = [
                        FilterType.Support,
                        FilterType.Interference,
                        FilterType.Recovery
                    ];
                    MemoriaSources.ItemsSource = Sources;
                }
            }
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;

            switch (box.Content)
            {
                case "�ʏ�P��":
                    {
                        _currentFilters.Add(FilterType.NormalSingle);
                        break;
                    }
                case "�ʏ�͈�":
                    {
                        _currentFilters.Add(FilterType.NormalRange);
                        break;
                    }
                case "����P��":
                    {
                        _currentFilters.Add(FilterType.SpecialSingle);
                        break;
                    }
                case "����͈�":
                    {
                        _currentFilters.Add(FilterType.SpecialRange);
                        break;
                    }
                case "�x��":
                    {
                        _currentFilters.Add(FilterType.Support);
                        break;
                    }
                case "�W�Q":
                    {
                        _currentFilters.Add(FilterType.Interference);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Add(FilterType.Recovery);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            foreach (var memoria in Memoria
                .List
                .Where(memoria => _currentFilters.Where(IsKindFilter).Any(key => Filters[key](memoria)))
                .Where(memoria => !Sources.Contains(memoria))
                .Where(memoria => ApplyFilter(memoria, Filters, _currentFilters)))
            {
                Sources.Add(memoria);
            }
        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;

            switch (box.Content)
            {
                case "�ʏ�P��":
                    {
                        _currentFilters.Remove(FilterType.NormalSingle);
                        break;
                    }
                case "�ʏ�͈�":
                    {
                        _currentFilters.Remove(FilterType.NormalRange);
                        break;
                    }
                case "����P��":
                    {
                        _currentFilters.Remove(FilterType.SpecialSingle);
                        break;
                    }
                case "����͈�":
                    {
                        _currentFilters.Remove(FilterType.SpecialRange);
                        break;
                    }
                case "�x��":
                    {
                        _currentFilters.Remove(FilterType.Support);
                        break;
                    }
                case "�W�Q":
                    {
                        _currentFilters.Remove(FilterType.Interference);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Remove(FilterType.Recovery);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            foreach (var memoria in Sources.ToList().Where(memoria => !_currentFilters.Where(IsKindFilter).Any(key => Filters[key](memoria))))
            {
                Sources.Remove(memoria);
            }
        }

        private void InitAdvancedOptions()
        {
            TreeNodes =
            [
                new()
                {
                    Text = "����",
                    Children =
                    [
                        new() { Text = "��" },
                        new() { Text = "��" },
                        new() { Text = "��" },
                        new() { Text = "��" },
                        new() { Text = "��" }
                    ]
                },
                new()
                {
                    Text = "�͈�",
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
                    Text = "���ʗ�",
                    Children =
                    [
                        new() { Text = "�T" },
                        new() { Text = "�U" },
                        new() { Text = "�V" },
                        new() { Text = "�V+" },
                        new() { Text = "�W" },
                        new() { Text = "�W+" },
                        new() { Text = "�X" },
                        new() { Text = "�X+" },
                        new() { Text = "LG" },
                        new() { Text = "LG+" }
                    ]
                },
                new()
                {
                    Text = "�X�L������",
                    Children =
                    [
                        // �U���n
                        new() { Text = "A up" },
                        new() { Text = "A down" },
                        new() { Text = "SA up" },
                        new() { Text = "SA down" },
                        // �h��n
                        new() { Text = "D up" },
                        new() { Text = "D down" },
                        new() { Text = "SD up" },
                        new() { Text = "SD down" },
                        // �����n
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
                        // ���̑�
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
            var prevCoount = _currentFilters.Count;
            switch (box.Content)
            {
                case "����":
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
                case "��":
                    {
                        _currentFilters.Add(FilterType.Fire);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Add(FilterType.Water);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Add(FilterType.Wind);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Add(FilterType.Light);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Add(FilterType.Dark);
                        break;
                    }
                case "�͈�":
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
                case "���ʗ�":
                    {
                        foreach (var node in TreeNodes[2].Children)
                        {
                            node.IsChecked = true;
                        }
                        FilterType[] remove = [
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
                case "�T":
                    {
                        _currentFilters.Add(FilterType.One);
                        break;
                    }
                case "�U":
                    {
                        _currentFilters.Add(FilterType.Two);
                        break;
                    }
                case "�V":
                    {
                        _currentFilters.Add(FilterType.Three);
                        break;
                    }
                case "�V+":
                    {
                        _currentFilters.Add(FilterType.ThreePlus);
                        break;
                    }
                case "�W":
                    {
                        _currentFilters.Add(FilterType.Four);
                        break;
                    }
                case "�W+":
                    {
                        _currentFilters.Add(FilterType.FourPlus);
                        break;
                    }
                case "�X":
                    {
                        _currentFilters.Add(FilterType.Five);
                        break;
                    }
                case "�X+":
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
                case "�X�L������":
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
                case "�Ό��ʃA�b�v":
                    {
                        _currentFilters.Add(FilterType.FireStimulation);
                        break;
                    }
                case "�����ʃA�b�v":
                    {
                        _currentFilters.Add(FilterType.WaterStimulation);
                        break;
                    }
                case "�����ʃA�b�v":
                    {
                        _currentFilters.Add(FilterType.WindStimulation);
                        break;
                    }
                case "�����ʃA�b�v":
                    {
                        _currentFilters.Add(FilterType.LightStimulation);
                        break;
                    }
                case "�Ō��ʃA�b�v":
                    {
                        _currentFilters.Add(FilterType.DarkStimulation);
                        break;
                    }
                case "�΋�":
                    {
                        _currentFilters.Add(FilterType.FireStrong);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Add(FilterType.WaterStrong);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Add(FilterType.WindStrong);
                        break;
                    }
                case "�Ύ�":
                    {
                        _currentFilters.Add(FilterType.FireWeak);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Add(FilterType.WaterWeak);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Add(FilterType.WindWeak);
                        break;
                    }
                case "�Ίg":
                    {
                        _currentFilters.Add(FilterType.FireSpread);
                        break;
                    }
                case "���g":
                    {
                        _currentFilters.Add(FilterType.WaterSpread);
                        break;
                    }
                case "���g":
                    {
                        _currentFilters.Add(FilterType.WindSpread);
                        break;
                    }
                case "�q�[��":
                    {
                        _currentFilters.Add(FilterType.Heal);
                        break;
                    }
                case "�`���[�W":
                    {
                        _currentFilters.Add(FilterType.Charge);
                        break;
                    }
                case "���J�o�[":
                    {
                        _currentFilters.Add(FilterType.Recover);
                        break;
                    }
                case "�J�E���^�[":
                    {
                        _currentFilters.Add(FilterType.Counter);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            if (prevCoount == _currentFilters.Count) return;
            foreach (var memoria in Memoria
                    .List
                    .Where(memoria => _currentFilters.Where(IsKindFilter).Any(key => Filters[key](memoria)))
                    .Where(memoria => !Sources.Contains(memoria))
                    .Where(memoria => ApplyFilter(memoria, Filters, _currentFilters)))
            {
                Sources.Add(memoria);
            }
        }

        private void AdvancedOption_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox box) return;
            var prevCoount = _currentFilters.Count;

            switch (box.Content)
            {
                case "����":
                    {
                        foreach (var node in TreeNodes[0].Children)
                        {
                            if (!node.IsChecked) return;
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Remove(FilterType.Fire);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Remove(FilterType.Water);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Remove(FilterType.Wind);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Remove(FilterType.Light);
                        break;
                    }
                case "��":
                    {
                        _currentFilters.Remove(FilterType.Dark);
                        break;
                    }
                case "�͈�":
                    {
                        foreach (var node in TreeNodes[1].Children)
                        {
                            if (!node.IsChecked) return;
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "A":
                    {
                        _currentFilters.Remove(FilterType.A);
                        break;
                    }
                case "B":
                    {
                        _currentFilters.Remove(FilterType.B);
                        break;
                    }
                case "C":
                    {
                        _currentFilters.Remove(FilterType.C);
                        break;
                    }
                case "D":
                    {
                        _currentFilters.Remove(FilterType.D);
                        break;
                    }
                case "E":
                    {
                        _currentFilters.Remove(FilterType.E);
                        break;
                    }
                case "���ʗ�":
                    {
                        foreach (var node in TreeNodes[2].Children)
                        {
                            if (!node.IsChecked) return;
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "�T":
                    {
                        _currentFilters.Remove(FilterType.One);
                        break;
                    }
                case "�U":
                    {
                        _currentFilters.Remove(FilterType.Two);
                        break;
                    }
                case "�V":
                    {
                        _currentFilters.Remove(FilterType.Three);
                        break;
                    }
                case "�V+":
                    {
                        _currentFilters.Remove(FilterType.ThreePlus);
                        break;
                    }
                case "�W":
                    {
                        _currentFilters.Remove(FilterType.Four);
                        break;
                    }
                case "�W+":
                    {
                        _currentFilters.Remove(FilterType.FourPlus);
                        break;
                    }
                case "�X":
                    {
                        _currentFilters.Remove(FilterType.Five);
                        break;
                    }
                case "�X+":
                    {
                        _currentFilters.Remove(FilterType.FivePlus);
                        break;
                    }
                case "LG":
                    {
                        _currentFilters.Remove(FilterType.Lg);
                        break;
                    }
                case "LG+":
                    {
                        _currentFilters.Remove(FilterType.LgPlus);
                        break;
                    }
                case "�X�L������":
                    {
                        foreach (var node in TreeNodes[3].Children)
                        {
                            if (!node.IsChecked) return;
                            node.IsChecked = false;
                        }
                        Sources.Clear();
                        break;
                    }
                case "A up":
                    {
                        _currentFilters.Remove(FilterType.Au);
                        break;
                    }
                case "A down":
                    {
                        _currentFilters.Remove(FilterType.Ad);
                        break;
                    }
                case "SA up":
                    {
                        _currentFilters.Remove(FilterType.SAu);
                        break;
                    }
                case "SA down":
                    {
                        _currentFilters.Remove(FilterType.SAd);
                        break;
                    }
                case "D up":
                    {
                        _currentFilters.Remove(FilterType.Du);
                        break;
                    }
                case "D down":
                    {
                        _currentFilters.Remove(FilterType.Dd);
                        break;
                    }
                case "SD up":
                    {
                        _currentFilters.Remove(FilterType.SDu);
                        break;
                    }
                case "SD down":
                    {
                        _currentFilters.Remove(FilterType.SDd);
                        break;
                    }
                case "Fire Power up":
                    {
                        _currentFilters.Remove(FilterType.FPu);
                        break;
                    }
                case "Fire Power down":
                    {
                        _currentFilters.Remove(FilterType.FPd);
                        break;
                    }
                case "Water Power up":
                    {
                        _currentFilters.Remove(FilterType.WaPu);
                        break;
                    }
                case "Water Power down":
                    {
                        _currentFilters.Remove(FilterType.WaPd);
                        break;
                    }
                case "Wind Power up":
                    {
                        _currentFilters.Remove(FilterType.WiPu);
                        break;
                    }
                case "Wind Power down":
                    {
                        _currentFilters.Remove(FilterType.WiPd);
                        break;
                    }
                case "Light Power up":
                    {
                        _currentFilters.Remove(FilterType.LPu);
                        break;
                    }
                case "Light Power down":
                    {
                        _currentFilters.Remove(FilterType.LPd);
                        break;
                    }
                case "Dark Power up":
                    {
                        _currentFilters.Remove(FilterType.DPu);
                        break;
                    }
                case "Dark Power down":
                    {
                        _currentFilters.Remove(FilterType.DPd);
                        break;
                    }
                case "Fire Guard up":
                    {
                        _currentFilters.Remove(FilterType.FGu);
                        break;
                    }
                case "Fire Guard down":
                    {
                        _currentFilters.Remove(FilterType.FGd);
                        break;
                    }
                case "Water Guard up":
                    {
                        _currentFilters.Remove(FilterType.WaGu);
                        break;
                    }
                case "Water Guard down":
                    {
                        _currentFilters.Remove(FilterType.WaGd);
                        break;
                    }
                case "Wind Guard up":
                    {
                        _currentFilters.Remove(FilterType.WiGu);
                        break;
                    }
                case "Wind Guard down":
                    {
                        _currentFilters.Remove(FilterType.WiGd);
                        break;
                    }
                case "Light Guard up":
                    {
                        _currentFilters.Remove(FilterType.LGu);
                        break;
                    }
                case "Light Guard down":
                    {
                        _currentFilters.Remove(FilterType.LGd);
                        break;
                    }
                case "Dark Guard up":
                    {
                        _currentFilters.Remove(FilterType.DGu);
                        break;
                    }
                case "Dark Guard down":
                    {
                        _currentFilters.Remove(FilterType.DGd);
                        break;
                    }
                case "HP up":
                    {
                        _currentFilters.Remove(FilterType.HPu);
                        break;
                    }
                case "�Ό��ʃA�b�v":
                    {
                        _currentFilters.Remove(FilterType.FireStimulation);
                        break;
                    }
                case "�����ʃA�b�v":
                    {
                        _currentFilters.Remove(FilterType.WaterStimulation);
                        break;
                    }
                case "�����ʃA�b�v":
                    {
                        _currentFilters.Remove(FilterType.WindStimulation);
                        break;
                    }
                case "�����ʃA�b�v":
                    {
                        _currentFilters.Remove(FilterType.LightStimulation);
                        break;
                    }
                case "�Ō��ʃA�b�v":
                    {
                        _currentFilters.Remove(FilterType.DarkStimulation);
                        break;
                    }
                case "�΋�":
                    {
                        _currentFilters.Remove(FilterType.FireStrong);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Remove(FilterType.WaterStrong);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Remove(FilterType.WindStrong);
                        break;
                    }
                case "�Ύ�":
                    {
                        _currentFilters.Remove(FilterType.FireWeak);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Remove(FilterType.WaterWeak);
                        break;
                    }
                case "����":
                    {
                        _currentFilters.Remove(FilterType.WindWeak);
                        break;
                    }
                case "�Ίg":
                    {
                        _currentFilters.Remove(FilterType.FireSpread);
                        break;
                    }
                case "���g":
                    {
                        _currentFilters.Remove(FilterType.WaterSpread);
                        break;
                    }
                case "���g":
                    {
                        _currentFilters.Remove(FilterType.WindSpread);
                        break;
                    }
                case "�q�[��":
                    {
                        _currentFilters.Remove(FilterType.Heal);
                        break;
                    }
                case "�`���[�W":
                    {
                        _currentFilters.Remove(FilterType.Charge);
                        break;
                    }
                case "���J�o�[":
                    {
                        _currentFilters.Remove(FilterType.Recover);
                        break;
                    }
                case "�J�E���^�[":
                    {
                        _currentFilters.Remove(FilterType.Counter);
                        break;
                    }
                default:
                    {
                        throw new UnreachableException("Unreachable");
                    }
            }
            if (prevCoount == _currentFilters.Count) return;
            foreach (var memoria in Sources.ToList().Where(memoria => !ApplyFilter(memoria, Filters, _currentFilters, false)))
            {
                Sources.Remove(memoria);
            }
        }
        private void InitFilters()
        {
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
                        stat is StatusUp(ElementAttack(Element.Fire), _)
                    )
            );
            Filters.Add(
                FilterType.FPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementAttack(Element.Fire), _)
                    )
            );
            Filters.Add(
                FilterType.WaPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementAttack(Element.Water), _)
                    )
            );
            Filters.Add(
                FilterType.WaPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementAttack(Element.Water), _)
                    )
            );
            Filters.Add(
                FilterType.WiPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementAttack(Element.Wind), _)
                    )
            );
            Filters.Add(
                FilterType.WiPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementAttack(Element.Wind), _)
                    )
            );
            Filters.Add(
                FilterType.LPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementAttack(Element.Light), _)
                    )
            );
            Filters.Add(
                FilterType.LPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementAttack(Element.Light), _)
                    )
            );
            Filters.Add(
                FilterType.DPu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementAttack(Element.Dark), _)
                    )
            );
            Filters.Add(
                FilterType.DPd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementAttack(Element.Dark), _)
                    )
            );
            Filters.Add(
                FilterType.FGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementGuard(Element.Fire), _)
                    )
            );
            Filters.Add(
                FilterType.FGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementGuard(Element.Fire), _)
                    )
            );
            Filters.Add(
                FilterType.WaGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat => stat is StatusUp(ElementGuard(Element.Water), _))
            );
            Filters.Add(
                FilterType.WaGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementGuard(Element.Water), _)
                    )
            );
            Filters.Add(
                FilterType.WiGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementGuard(Element.Wind), _)
                    )
            );
            Filters.Add(
                FilterType.WiGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementGuard(Element.Wind), _)
                    )
            );
            Filters.Add(
                FilterType.LGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementGuard(Element.Light), _)
                    )
            );
            Filters.Add(
                FilterType.LGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementGuard(Element.Light), _)
                    )
            );
            Filters.Add(
                FilterType.DGu,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusUp(ElementGuard(Element.Dark), _)
                    )
            );
            Filters.Add(
                FilterType.DGd,
                memoria => memoria
                    .Skill
                    .StatusChanges
                    .Any(stat =>
                        stat is StatusDown(ElementGuard(Element.Dark), _)
                    )
            );
            // ����
            Filters.Add(
                FilterType.FireStimulation,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStimulation(Element.Fire)
                    )
            );
            Filters.Add(
                FilterType.WaterStimulation,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStimulation(Element.Water)
                    )
            );
            Filters.Add(
                FilterType.WindStimulation,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStimulation(Element.Wind)
                    )
            );
            Filters.Add(
                FilterType.LightStimulation,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStimulation(Element.Light)
                    )
            );
            Filters.Add(
                FilterType.DarkStimulation,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStimulation(Element.Dark)
                    )
            );
            // ����
            Filters.Add(
                FilterType.FireStrong,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStrengthen(Element.Fire)
                    )
            );
            Filters.Add(
                FilterType.WaterStrong,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStrengthen(Element.Water)
                    )
            );
            Filters.Add(
                FilterType.WindStrong,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementStrengthen(Element.Wind)
                    )
            );
            // ����
            Filters.Add(
               FilterType.FireWeak,
               memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementWeaken(Element.Fire)
                    )
            );
            Filters.Add(
               FilterType.WaterWeak,
               memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementWeaken(Element.Water)
                    )
            );
            Filters.Add(
               FilterType.WindWeak,
               memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementWeaken(Element.Wind)
                    )
            );
            // ���g
            Filters.Add(
                FilterType.FireSpread,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementSpread(Element.Fire)
                    )
            );
            Filters.Add(
                FilterType.WaterSpread,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementSpread(Element.Water)
                    )
            );
            Filters.Add(
                FilterType.WindSpread,
                memoria => memoria
                    .Skill
                    .Effects
                    .Any(eff =>
                        eff is ElementSpread(Element.Wind)
                    )
            );
            // �q�[��
            Filters.Add(FilterType.Heal,�@memoria => memoria.Skill.Effects.Any(eff =>�@eff is Heal));
            // �`���[�W
            Filters.Add(FilterType.Charge, memoria => memoria.Skill.Effects.Any(eff => eff is Charge));
            // ���J�o�[
            Filters.Add(FilterType.Recover, memoria => memoria.Skill.Effects.Any(eff => eff is Recover));
            // �J�E���^�[
            Filters.Add(FilterType.Counter, memoria => memoria.Skill.Effects.Any(eff => eff is Counter));
        }

        bool ApplyFilter(Memoria memoria, Dictionary<FilterType, Func<Memoria, bool>> filters, HashSet<FilterType> predicates, bool flip=true)
        {
            var p1 = predicates.Where(IsElementFilter).ToList();
            var c1 = p1.Any(key => filters[key](memoria));
            var p2 = predicates.Where(IsRangeFilter).ToList();
            var c2 = p2.Any(key => filters[key](memoria));
            var p3 = predicates.Where(IsLevelFilter).ToList();
            var c3 = p3.Any(key => filters[key](memoria));
            var p4 = predicates.Where(IsEffectFilter).ToList();
            var c4 = p4.Any(key => filters[key](memoria));
            if (flip) { 
                return (p1.Count == 0 || c1) && (p2.Count == 0 || c2) && (p3.Count == 0 || c3) && (p4.Count == 0 || c4);
            } else
            {
                return (p1.Count != 0 && !c1) || (p2.Count != 0 && !c2) || (p3.Count != 0 && !c3) || (p4.Count != 0 && !c4);
            }
        }

        bool IsKindFilter(FilterType filter)
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
        bool IsElementFilter(FilterType filter)
        {
            FilterType[] elementFilters = [
                FilterType.Fire,
                FilterType.Water,
                FilterType.Wind,
                FilterType.Light,
                FilterType.Dark,
            ];

            return elementFilters.Contains(filter);
        }
        bool IsRangeFilter(FilterType filter)
        {
            FilterType[] rangeFilters = [
                FilterType.A,
                FilterType.B,
                FilterType.C,
                FilterType.D,
                FilterType.E,
            ];

            return rangeFilters.Contains(filter);
        }
        bool IsLevelFilter(FilterType filter)
        {
            FilterType[] LevelFilters = [
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

            return LevelFilters.Contains(filter);
        }
        bool IsEffectFilter(FilterType filter)
        {
            FilterType[] effectFilters = [
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
                FilterType.FireStimulation,
                FilterType.WaterStimulation,
                FilterType.WindStimulation,
                FilterType.LightStimulation,
                FilterType.DarkStimulation,
                FilterType.FireStrong,
                FilterType.WaterStrong,
                FilterType.WindStrong,
                FilterType.FireWeak,
                FilterType.WaterWeak,
                FilterType.WindWeak,
                FilterType.FireSpread,
                FilterType.WaterSpread,
                FilterType.WindSpread,
                FilterType.Heal,
                FilterType.Charge,
                FilterType.Recover,
                FilterType.Counter,

            ];

            return effectFilters.Contains(filter);
        }

        private void Sort(int option)
        {
            switch (option)
            {
                case 0:
                    Sources = new(Sources.OrderBy(memoria => memoria.Id));
                    break;
                case 1:
                    Sources = new(Sources.OrderBy(memoria => memoria.Status.Atk));
                    break;
                case 2:
                    Sources = new(Sources.OrderBy(memoria => memoria.Status.SpAtk));
                    break;
                case 3:
                    Sources = new(Sources.OrderBy(memoria => memoria.Status.Def));
                    break;
                case 4:
                    Sources = new(Sources.OrderBy(memoria => memoria.Status.SpDef));
                    break;
                case 5:
                    Sources = new(Sources.OrderBy(memoria => memoria.Status.Atk + memoria.Status.SpAtk));
                    break;
                case 6:
                    Sources = new(Sources.OrderBy(memoria => memoria.Status.Def + memoria.Status.SpDef));
                    break;
            }
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is RadioButtons rb)
            {
                Sort(rb.SelectedIndex);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var member = MemberSelect.SelectedItem.As<MemberInfo>();
            if (LegendaryDeck.Count > 4 || Deck.Count > 20)
            {
                GeneralInfoBar.Title = "�f�b�L��20���܂łł��i���W�F���_���[��4���܂łł��j�B";
                GeneralInfoBar.IsOpen = true;
                GeneralInfoBar.Severity = InfoBarSeverity.Error;
                await Task.Delay(3000);
                GeneralInfoBar.IsOpen = false;
                return;
            } else
            {
                var name = DeckName.Text;
                new DirectoryInfo($@"{Director.ProjectDir()}\{region}\Members\{member.Name}\Units").Create();
                var path = $@"{Director.ProjectDir()}\{region}\Members\{member.Name}\Units\{name}.json";
                await using var unit = File.Create(path);
                await unit.WriteAsync(new UTF8Encoding(true).GetBytes(
                    new Unit(name, member.Position is Front, [.. Deck, .. LegendaryDeck]).ToJson()));
                GeneralInfoBar.Title = "�ۑ����܂���";
                GeneralInfoBar.IsOpen = true;
                GeneralInfoBar.Severity = InfoBarSeverity.Informational;
                await Task.Delay(3000);
                GeneralInfoBar.IsOpen = false;
            }
        }
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
        // Skill Effects (Status Changes)
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
        // Other Skill Effects
        FireStimulation,
        WaterStimulation,
        WindStimulation,
        LightStimulation,
        DarkStimulation,
        FireStrong,
        WaterStrong,
        WindStrong,
        FireWeak,
        WaterWeak,
        WindWeak,
        FireSpread,
        WaterSpread,
        WindSpread,
        Heal,
        Charge,
        Recover,
        Counter,
    }
    public class MyTreeNode
    {
        public string Text { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = true;

        public ObservableCollection<MyTreeNode> Children { get; set; } = [];
    }
}