using System;
using Microsoft.UI.Xaml.Controls;
using static Microsoft.UI.Xaml.Controls.InfoBarSeverity;
namespace mitama.Pages.Common;

public record struct InfoProps(string Title, InfoBarSeverity Severity = Informational);

public record Props(string Project, Action<InfoProps>? InvokeInfo)
{
    public Props(string project) : this(project, null)
    {
    }
}