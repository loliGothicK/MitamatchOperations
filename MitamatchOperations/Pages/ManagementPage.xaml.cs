using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Mitama.Pages.DeckBuilder;
using Mitama.Pages.Library;
using Mitama.Pages.OrderConsole;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ManagementPage : Page
{
    public ManagementPage()
    {
        InitializeComponent();

        NavigationCacheMode = NavigationCacheMode.Enabled;

        CostumeManagerFrame.Navigate(typeof(CostumeManagePage));
        MemoriaManagerFrame.Navigate(typeof(MemoriaManagePage));
        OrderManagerFrame.Navigate(typeof(OrderManagerPage));
    }
}
