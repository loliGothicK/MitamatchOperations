using System.Windows.Input;
using System.Windows.Media.Media3D;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace mitama.Pages.Common;

internal class Dialog {
    public static DialogBuilder Builder(XamlRoot root) => new(root);
}

internal record struct DialogBuilder(
    XamlRoot Root,
    double Width = 0,
    double Height = 0,
    object Title = null,
    object Body = null,
    string PrimaryButtonText = null,
    ICommand PrimaryButtonCommand = null,
    string SecondaryButtonText = null,
    string CloseButtonText = null
) {
    internal readonly ContentDialog Build()
        => new() {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = Root,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            MinWidth = Width,
            MinHeight = Height,
            Title = Title,
            PrimaryButtonText = PrimaryButtonText,
            PrimaryButtonCommand = PrimaryButtonCommand,
            SecondaryButtonText = SecondaryButtonText,
            CloseButtonText = CloseButtonText,
            DefaultButton = ContentDialogButton.Primary,
            Content = Body
        };

    internal readonly DialogBuilder WithTitle(string title) => this with { Title = title };
    internal readonly DialogBuilder WithSize(double width, double height) => this with { Width = width, Height = height };
    internal readonly DialogBuilder WithBody(object body) => this with { Body = body };
    internal readonly DialogBuilder WithPrimary(string primary) => this with { PrimaryButtonText = primary };
    internal readonly DialogBuilder WithPrimary(string primary, ICommand command) => this with { PrimaryButtonText = primary, PrimaryButtonCommand = command };
    internal readonly DialogBuilder WithSecondary(string secondary) => this with { SecondaryButtonText = secondary };
    internal readonly DialogBuilder WithCancel(string cancel) => this with { CloseButtonText = cancel };
}
