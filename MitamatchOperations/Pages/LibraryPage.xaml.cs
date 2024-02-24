using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.Library;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {

        public LibraryPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            CostumeLibraryFrame.Navigate(typeof(CostumeLibraryPage));
            CharmLibraryFrame.Navigate(typeof(CharmLibraryPage));
            CostumeManagerFrame.Navigate(typeof(CostumeManagePage));
        }
    }
}
