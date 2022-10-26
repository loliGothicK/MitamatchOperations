using Microsoft.UI.Xaml;

namespace mitama.Pages;

/// <summary>
/// Home Page navigated to within a Main Page.
/// </summary>
public sealed partial class HomePage
{
    public string LogoPath => ((FrameworkElement)Content).ActualTheme == ElementTheme.Light
        ? "/Assets/Images/MO_LIGHT.png"
        : "/Assets/Images/MO_DARK.png";

    public HomePage()
    {
        InitializeComponent();
        NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
    }
}