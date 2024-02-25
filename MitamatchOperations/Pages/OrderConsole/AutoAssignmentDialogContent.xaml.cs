using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mitama.Pages.OrderConsole;

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
        if (CandidateListView is null) return;

        CandidateListView.ItemsSource = Timeline.Zip(Candidates[(int)sender.Value - 1])
            .Select(zipped => $@"{zipped.First.Order.Name} => {zipped.Second}");

        Hook((int)sender.Value - 1);
    }
}
