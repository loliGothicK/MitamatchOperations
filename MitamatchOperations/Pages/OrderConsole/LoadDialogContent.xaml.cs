using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using mitama.Pages.Common;

namespace mitama.Pages.OrderConsole;

/// <summary>
/// Load Dialog Content used in the Load button flyout in Deck Editor Page.
/// </summary>
public sealed partial class LoadDialogContent
{
    public delegate void OnChanged(string member);

    private readonly Dictionary<string, List<string>> _LegionToMembersMap = new();
    private readonly OnChanged _onChangedAction;

    public LoadDialogContent(OnChanged onChanged)
    {
        _onChangedAction = onChanged;
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        InitComboBox();
    }

    private void InitComboBox()
    {
        foreach (var name in Util.LoadMemberNames(Director.ReadCache().Legion))
        {
            MemberComboBox.Items.Add(name);
        }
    }

    private void MemberComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var member = e.AddedItems[0].ToString();
        if (member != null) _onChangedAction.Invoke(member);
    }
}
