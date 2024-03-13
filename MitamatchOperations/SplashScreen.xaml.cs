using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Mitama.Lib;
using Mitama.Pages.Common;
using static Mitama.Repository.Repository;

namespace Mitama;

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
        if (jwt is not null && App.DecodeJwt(jwt).IsOk())
        {
            Login.IsEnabled = false;
        }
        else
        {
            while (!await PerformLogin())
            {
                await Task.Delay(1000);
            }
        }
        var cache = Director.ReadCache();
        if (cache.FetchDate is null || cache.FetchDate < Storage.ListObjects("data").First().UpdatedDateTimeOffset)
        {
            await Task.Run(() =>
            {
                using var stream = File.OpenWrite($@"{Director.DatabaseDir()}\data");
                Storage.DownloadObject("data", stream);
                Director.CacheWrite((cache with { FetchDate = DateTimeOffset.Now }).ToJsonBytes());
            });
        }
        else
        {
            await Task.Delay(500);
        }
        await Repository.Repository.LiteDB.ExtractImages();
    }
}
