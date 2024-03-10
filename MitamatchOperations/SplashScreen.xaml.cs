using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using LiteDB;
using Mitama.Domain;
using Mitama.Lib;
using Mitama.Pages.Common;
using Newtonsoft.Json;

namespace Mitama;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SplashScreen : WinUIEx.SplashScreen
{
    private readonly Func<Task<bool>> PerformLogin;
    public SplashScreen(System.Type window, Func<Task<bool>> IO) : base(window)
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
        // Download data from the GCS bucket
        var json = new
        {
            type = "service_account",
            project_id = "assaultlily",
            private_key_id = "13b3e809d5e493489d67018ac1d69d5c2e2eaa04",
            private_key = "GOOGLE_CLOUD_PRIVATE_KEY",
            client_email = "mitamatch@assaultlily.iam.gserviceaccount.com",
            client_id = "116107053801726389433",
            auth_uri = "https://accounts.google.com/o/oauth2/auth",
            token_uri = "https://oauth2.googleapis.com/token",
            auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
            client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/mitamatch%40assaultlily.iam.gserviceaccount.com",
            universe_domain = "googleapis.com"
        };
        var credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromJson(JsonConvert.SerializeObject(json));
        var client = StorageClient.Create(credential);
        var cache = Director.ReadCache();
        if (cache.FetchDate is null || cache.FetchDate < client.ListObjects("mitamatch", "data").First().TimeStorageClassUpdatedDateTimeOffset)
        {
            await Task.Run(() =>
            {
                using var stream = File.OpenWrite($@"{Director.DatabaseDir()}\data");
                client.DownloadObject("mitamatch", "data", stream);
            });
            Director.CacheWrite((cache with
            {
                FetchDate = DateTimeOffset.UtcNow,
            }).ToJsonBytes());
        }
        else
        {
            await Task.Delay(500);
        }
        {   // Extract Memoria images
            var index = cache.MemoriaIndex ?? 0;
            foreach (var chunk in Memoria.List.Value.Where(m => m.Id > index).Chunk(40))
            {
                using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
                foreach (var memoria in chunk)
                {
                    db.FileStorage.FindById($"$/memoria/{memoria.Name}.png").SaveAs($@"{Director.MemoriaImageDir()}\{memoria.Name}.png");
                }
                await Task.Delay(50);
            }
        }
        {   // Extract Costume images
            var index = cache.CostumeIndex ?? 0;
            foreach (var chunk in Costume.List.Value.Where(c => c.Index > index).Chunk(40))
            {
                using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
                foreach (var costume in chunk)
                {
                    db.FileStorage.FindById($"$/costume/{costume.Lily}/{costume.Name}.png").SaveAs($@"{Director.CostumeImageDir(costume.Lily)}\{costume.Name}.png");
                }
                await Task.Delay(50);
            }
        }
        {   // Extract Order images
            var index = cache.OrderIndex ?? 0;
            foreach (var chunk in Order.List.Value.Where(o => o.Index > index).Chunk(40))
            {
                using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
                foreach (var order in chunk)
                {
                    db.FileStorage.FindById($"$/order/{order.Name}.png").SaveAs($@"{Director.OrderImageDir()}\{order.Name}.png");
                }
                await Task.Delay(50);
            }
        }
        Director.CacheWrite((cache with
        {
            MemoriaIndex = MaxId<Repository.Memoria.POCO>("memoria"),
            CostumeIndex = MaxId<Repository.Costume.POCO>("costume"),
            OrderIndex = MaxId<Repository.Order.POCO>("order"),
        }).ToJsonBytes());
    }
    private static int MaxId<T>(string table)
    {
        try
        {
            using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
            var col = db.GetCollection<T>(table);
            var max = col.Max("_id");
            return max.RawValue is null ? 0 : (int)max.RawValue;
        }
        catch
        {
            return 0;
        }
    }

}
