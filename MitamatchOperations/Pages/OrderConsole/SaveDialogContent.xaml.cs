using Microsoft.UI.Xaml.Controls;
using Mitama.Pages.Common;

namespace Mitama.Pages.OrderConsole;

/// <summary>
/// Save Dialog Content used in the Save button flyout in Deck Editor Page.
/// </summary>
public sealed partial class SaveDialogContent
{
    public delegate void OnChanged(string member);

    private readonly OnChanged _onChangedAction;

    public SaveDialogContent(OnChanged onChanged)
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
