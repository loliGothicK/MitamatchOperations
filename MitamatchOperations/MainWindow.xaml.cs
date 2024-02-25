using Microsoft.UI.Xaml;
using Mitama.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MitamatchOperations
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mainPage = new MainPage();
            Content = mainPage;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(mainPage.GetAppTitleBar);
        }
    }
}
