using System;
using System.Threading.Tasks;

namespace mitama;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SplashScreen : WinUIEx.SplashScreen
{
    private readonly Func<Task<bool>> PerformLogin;
    public SplashScreen(Type window, Func<Task<bool>> IO) : base(window)
    {
        InitializeComponent();
        PerformLogin = IO;
        Login.Click += async (_, _) =>
        {
            // open the mitamatch login page by default in the default browser
            await Windows.System.Launcher.LaunchUriAsync(new Uri("http://localhost:3000/api/auth"));
        };
    }

    protected override async Task OnLoading()
    {
        while (!await PerformLogin())
        {
            await Task.Delay(1000);
        }
        for (int i = 0; i < 100; i += 5)
        {
            Status.Text = $"Loading {i}%...";
            ProgressBar.Value = i;
            await Task.Delay(50);
        }
    }
}
