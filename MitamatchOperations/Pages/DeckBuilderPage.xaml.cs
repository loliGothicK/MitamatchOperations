using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using mitama.Pages.DeckBuilder;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace mitama.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeckBuilderPage : Page
    {
        public DeckBuilderPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            EditFrame.Navigate(typeof(BuilderPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string parameter && !string.IsNullOrWhiteSpace(parameter))
            {
                EditFrame.Navigate(typeof(BuilderPage), parameter);
            }
            ManageFrame.Navigate(typeof(MemoriaManagePage), e);
        }
    }
}
