using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MitamatchOperations.Pages.RegionConsole
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GraphDialog : Page
    {
        private readonly SortedDictionary<TimeOnly, AllStatus> History;
        private readonly string Target;

        public GraphDialog(string target, SortedDictionary<TimeOnly, AllStatus> history)
        {
            History = history;
            Target = target;
            InitializeComponent();
            foreach (var (x, y) in History.Select(item => (item.Key.Minute * 60 + item.Key.Second + 50, -ToTarget(item.Value) / 1000.0 + 200)))
            {
                Graph.Points.Add(new(x, y));
            }
        }

        private int ToTarget(AllStatus status)
        {
            return Target switch
            {
                "ATK" => status.Attack,
                "Sp.ATK" => status.SpecialAttack,
                "DEF" => status.Defense,
                "Sp.DEF" => status.SpecialDefense,
                "Wind ATK" => status.WindAttack,
                "Wind DEF" => status.WindDefense,
                "Fire ATK" => status.FireAttack,
                "Fire DEF" => status.FireDefense,
                "Water ATK" => status.WaterAttack,
                "Water DEF" => status.WaterDefense,
                _ => throw new ArgumentException("Invalid target"),
            };
        }
    }
}
