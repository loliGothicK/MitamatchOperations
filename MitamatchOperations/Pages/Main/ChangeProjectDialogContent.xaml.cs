using System;
using Microsoft.UI.Xaml.Controls;
using mitama.Pages.Common;

namespace mitama.Pages.Main;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ChangeProjectDialogContent
{
    private readonly Action<string> _onSelectionChanged;

    public ChangeProjectDialogContent(Action<string> onSelectionChanged)
    {
        _onSelectionChanged = onSelectionChanged;
        InitializeComponent();
        foreach (var Legion in Util.LoadLegionNames())
        {
            LegionComboBox.Items.Add(Legion);
        }
    }

    private void LegionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _onSelectionChanged(e.AddedItems[0].ToString()!);
    }
}