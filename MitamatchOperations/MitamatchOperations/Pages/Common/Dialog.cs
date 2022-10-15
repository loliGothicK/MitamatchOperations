using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace mitama.Pages.Common
{
    internal class Dialog
    {
        public static DialogBuilder Builder(XamlRoot root) => new(root);
    }

    internal record struct DialogBuilder(
        XamlRoot Root,
        object? Title = null,
        object? Body = null,
        string? PrimaryButtonText = null,
        string? SecondaryButtonText = null,
        string? CloseButtonText = null
    )
    {
        internal ContentDialog Build()
            => new()
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = Root,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = Title,
                PrimaryButtonText = PrimaryButtonText,
                SecondaryButtonText = SecondaryButtonText,
                CloseButtonText = CloseButtonText,
                DefaultButton = ContentDialogButton.Primary,
                Content = Body
            };

        internal DialogBuilder WithTitle(string title) => this with { Title = title };
        internal DialogBuilder WithBody(object body) => this with { Body = body };
        internal DialogBuilder WithPrimary(string primary) => this with { PrimaryButtonText = primary };
        internal DialogBuilder WithSecondary(string secondary) => this with { SecondaryButtonText = secondary };
        internal DialogBuilder WithCancel(string cancel) => this with { CloseButtonText = cancel };
    }
}
