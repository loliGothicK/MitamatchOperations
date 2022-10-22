using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using mitama.Domain;
using mitama.Pages.Capture;
using mitama.Pages.Common;
using Windows.Media.Ocr;

namespace mitama.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ControlRoomPage
{
    private readonly WindowCapture _capture = new(Search.WindowHandleFromCaption("Assaultlily"));
    private readonly DispatcherTimer _timer;

    public ControlRoomPage()
    {
        InitializeComponent();
        _timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        _timer.Tick += async (o, e) =>
        {
            OrderCapture.Text = await Analyze(await _capture.TryCaptureOrderInfo()) switch
            {
                SuccessResult(var user, var order) => user + ": " + order,
                FailureResult(_) => "empty",
                _ => throw new ArgumentOutOfRangeException()
            };
        };
        _timer.Start();
    }

    private Task<AnalyzeResult> Analyze(string raw)
    {
        var orderedRegex = new Regex("(.+)がオーダー(.+)を準備");
        var match = orderedRegex.Match(raw);

        return Task.FromResult<AnalyzeResult>(match.Success
            ? new SuccessResult(match.Groups[1].Value, match.Groups[2].Value)
            : new FailureResult(raw));
    }

    private abstract record AnalyzeResult;

    private record SuccessResult(string User, string Order) : AnalyzeResult;

    private record FailureResult(string Raw) : AnalyzeResult;
}