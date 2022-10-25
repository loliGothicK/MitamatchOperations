using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace mitama.Pages.Capture;

internal class WindowCapture
{
    private readonly MemoryStream _bufferStream;
    private readonly IntPtr _handle;

    public WindowCapture(IntPtr handle)
    {
        _handle = handle;
        _bufferStream = new MemoryStream(1000)
        {
            Position = 0
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

    public Bitmap SnapShot()
    {
        // ウィンドウサイズ取得
        GetWindowRect(_handle, out var rect);

        var bmp = new Bitmap(500, 120);

        //Graphicsの作成
        using var g = Graphics.FromImage(bmp);
        try
        {
            g.CopyFromScreen(new Point(rect.left + 260, rect.top + 120), new Point(0, 0), bmp.Size);
        }
        catch
        {
            Console.WriteLine(@"キャプチャに失敗しました");
        }

        return bmp;
    }

    public async Task<SoftwareBitmap> GetSoftwareSnapShot(Bitmap snap)
    {
        // 取得したキャプチャ画像をストリームに保存
        _bufferStream.Seek(0, SeekOrigin.Begin);
        snap.Save(_bufferStream, ImageFormat.Png);

        // 保存した画像をSoftwareBitmap形式で読み込み
        var decoder = await BitmapDecoder.CreateAsync(_bufferStream.AsRandomAccessStream());
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        // SoftwareBitmap形式の画像を返す
        return softwareBitmap;
    }

    public async Task<string> RecognizeText(SoftwareBitmap snap)
    {
        var ocrEngine = OcrEngine.TryCreateFromLanguage(new Language("ja-JP"));
        var ocrResult = await ocrEngine?.RecognizeAsync(snap);
        return ocrResult.Text.Replace(" ", string.Empty);
    }

    public async Task<string> TryCaptureOrderInfo()
    {
        return await RecognizeText(await GetSoftwareSnapShot(SnapShot()));
    }
}