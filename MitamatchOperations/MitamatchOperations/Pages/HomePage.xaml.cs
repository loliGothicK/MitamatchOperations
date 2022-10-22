using System;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
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