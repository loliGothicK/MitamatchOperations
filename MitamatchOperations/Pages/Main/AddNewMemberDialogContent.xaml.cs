using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using mitama.Domain;
using WinRT;

namespace mitama.Pages.Main;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AddNewMemberDialogContent
{
    private readonly Action<NewMemberFragment> _onCommit;

    public AddNewMemberDialogContent(Action<NewMemberFragment> onCommit)
    {
        _onCommit = onCommit;
        InitializeComponent();
    }

    private void NameBox_OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        _onCommit(new NewMemberName(sender.As<TextBox>().Text));
    }

    private void PositionBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _onCommit(new NewMemberPosition(Position.FromStr(sender.As<ComboBox>().SelectedValue.As<string>())));
    }
}

public abstract record NewMemberFragment;
public record NewMemberName(string Name) : NewMemberFragment;
public record NewMemberPosition(Position Position) : NewMemberFragment;
