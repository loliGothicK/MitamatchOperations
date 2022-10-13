namespace mitama;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
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
