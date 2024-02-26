using System.Threading.Tasks;

namespace mitama;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SplashScreen : WinUIEx.SplashScreen
{
    public SplashScreen(System.Type window) : base(window)
    {
        InitializeComponent();
    }

    protected override async Task OnLoading()
    {
        //TODO: Do some actual work
        for (int i = 0; i < 100; i += 5)
        {
            Status.Text = $"Loading {i}%...";
            ProgressBar.Value = i;
            await Task.Delay(50);
        }
    }
}
