using Microsoft.UI.Xaml.Controls;
using mitama.Pages.Common;
using mitama.Pages.LegionSheet;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LegionSheetPage : Page
    {
        public LegionSheetPage()
        {
            InitializeComponent();
            foreach (var info in Util.LoadMembersInfo(Director.ReadCache().Legion))
            {
                Tabs.TabItems.Add(new TabViewItem()
                {
                    Header = info.Name,
                    Content = new DataGrid(info)
                });
            }
        }
    }
}
