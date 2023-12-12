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
            foreach (var (x, y) in History.Select(item => ((item.Key.Minute * 60 + item.Key.Second) * 500 / 600 + 50, -ToTarget(item.Value) + 200)))
            {
                Graph.Points.Add(new(x, y));
            }
        }

        private float ToTarget(AllStatus status)
        {
            return Target switch
            {
                "ATK" => status.Attack / 2000.0f,
                "Sp.ATK" => status.SpecialAttack / 2000.0f,
                "DEF" => status.Defense / 2000.0f,
                "Sp.DEF" => status.SpecialDefense / 2000.0f,
                "Wind ATK" => status.WindAttack / 700.0f,
                "Wind DEF" => status.WindDefense / 700.0f,
                "Fire ATK" => status.FireAttack / 700.0f,
                "Fire DEF" => status.FireDefense / 700.0f,
                "Water ATK" => status.WaterAttack / 700.0f,
                "Water DEF" => status.WaterDefense / 700.0f,
                _ => throw new ArgumentException("Invalid target"),
            };
        }
    }
}
