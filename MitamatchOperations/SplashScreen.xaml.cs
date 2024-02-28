using System;
using System.Threading.Tasks;
using Mitama.Lib;
using Mitama.Pages.Common;

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
            _ = await Windows.System.Launcher.LaunchUriAsync(new Uri("https://mitama-oauth.vercel.app"));
        };
    }

    protected override async Task OnLoading()
    {
        var jwt = Director.ReadCache().JWT;
        if (jwt is not null && App.DecodeJwt(jwt) is Ok<string, string>)
        {
            Login.IsEnabled = false;
            await Task.Delay(1000);
            return;
        }

        while (!await PerformLogin())
        {
            await Task.Delay(1000);
        }
    }
}
