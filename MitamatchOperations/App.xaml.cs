using System;
using System.Diagnostics;
using System.Threading.Channels;
using Google.Cloud.Datastore.V1;
using JWT.Builder;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Mitama.Domain;
using Mitama.Lib;
using Mitama.Pages.Common;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using static Mitama.Repository.Repository;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mitama;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private readonly Channel<Result<DiscordUser, string>> channel
        = Channel.CreateUnbounded<Result<DiscordUser, string>>();
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("SYNCFUSION_LICENSE_KEY");
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Get the activation args
        var appArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

        // Get or register the main instance
        var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

        // If the main instance isn't this current instance
        if (!mainInstance.IsCurrent)
        {
            // Redirect activation to that instance
            await mainInstance.RedirectActivationToAsync(appArgs);

            // And exit our instance and stop
            Process.GetCurrentProcess().Kill();
            return;
        }
        else
        {
            On_Activated(null, appArgs);
        }

        // Otherwise, register for activation redirection
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().Activated += On_Activated;

        var splash = new SplashScreen(typeof(MainWindow), async () =>
            await channel.Reader.ReadAsync() switch
            {
                Ok<DiscordUser, string> => true,
                _ => false,
            });
        splash.Completed += (_, e) => m_window = e;
    }

    private async void On_Activated(object _, AppActivationArguments args)
    {
        if (args.Kind == ExtendedActivationKind.Protocol)
        {
            var eventArgs = args.Data as Windows.ApplicationModel.Activation.ProtocolActivatedEventArgs;
            var query = eventArgs.Uri.Query;
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(query);
            var jwtToken = queryDictionary["token"];
            // verify JWT token
            switch (DecodeJwt(jwtToken))
            {
                case Ok<string, string>(var json):
                    {
                        var user = JsonConvert.DeserializeObject<DiscordUser>(json);
                        DataStore.Upsert(user);
                        // Cache に JWT token を保存
                        var cache = Director.ReadCache() with { JWT = jwtToken, User = user.global_name };
                        Director.CacheWrite(cache.ToJsonBytes());
                        await channel.Writer.WriteAsync(new Ok<DiscordUser, string>(user));
                        break;
                    }
                case Err<string, string>(var msg):
                    {
                        await channel.Writer.WriteAsync(new Err<DiscordUser, string>(msg));
                        break;
                    }
            }
        }
    }

    public static Result<string, string> DecodeJwt(string token)
    {
        try
        {
            var json = JwtBuilder
                .Create()
                .WithAlgorithm(new JWT.Algorithms.HMACSHA256Algorithm())
                .WithSecret("MITAMA_AUTH_JWT_SECRET")
                .MustVerifySignature()
                .Decode(token);
            return new Ok<string, string>(json);
        }
        catch (Exception ex)
        {
            return new Err<string, string>(ex.Message);
        }
    }
#pragma warning disable IDE0052 // Remove unread private members
    private Window m_window;
#pragma warning restore IDE0052 // Remove unread private members
}
