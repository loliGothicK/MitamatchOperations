using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages.OrderConsole;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AutoAssignmentDialogContent : Page
{
    private readonly List<TimeTableItem> Timeline;
    private readonly List<List<string>> Candidates;
    private readonly List<string> Default;
    private readonly Action<int> Hook;

    public AutoAssignmentDialogContent(List<TimeTableItem> timeline, List<List<string>> candidates, Action<int> hook)
    {
        Timeline = timeline;
        Candidates = candidates;
        Default = Timeline.Zip(Candidates[0])
            .Select(zipped => $@"{zipped.First.Order.Name} => {zipped.Second}")
            .ToList();
        Hook = hook;
        InitializeComponent();
    }

    private void NumberBoxSpinButtonPlacementExample_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if ((int)sender.Value == 1) return;

        CandidateListView.ItemsSource = Timeline.Zip(Candidates[(int)sender.Value - 1])
            .Select(zipped => $@"{zipped.First.Order.Name} => {zipped.Second}");

        Hook((int)sender.Value - 1);
    }
}
