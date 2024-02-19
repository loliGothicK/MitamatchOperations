using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using MitamatchOperations;
using Size = OpenCvSharp.Size;

namespace mitama.Pages.Capture;

internal abstract record OrderStat;

internal record WaitStat(Bitmap Image) : OrderStat;
internal record  ActiveStat(Bitmap Image) : OrderStat;
internal record Nothing(Bitmap Image) : OrderStat;


public partial class WindowCapture
{
    private readonly MemoryStream _bufferStream;
    private readonly Bitmap _capture;
    private readonly Rect _rect;

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct Rect
    {
        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void GetWindowRect(IntPtr hWnd, out Rect rect);

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowCapture"/> class from window handle and memory capacity.
    /// </summary>
    /// <param name="handle">Handle pointer to the window for capturing.</param>
    /// <param name="capacity">memory stream capacity</param>
    public WindowCapture(IntPtr handle, int capacity = 1000)
    {
        GetWindowRect(handle, out _rect);
        _capture = new Bitmap(_rect.Right - _rect.Left, _rect.Bottom - _rect.Top);
        _bufferStream = new MemoryStream(capacity)
        {
            Position = 0
        };
    }

    /// <summary>
    /// Updates the capture full image.
    /// </summary>
    public Task SnapShot()
    {
        // Graphicsの作成
        using var g = Graphics.FromImage(_capture);
        try
        {
            g.CopyFromScreen(new System.Drawing.Point(_rect.Left, _rect.Top), new System.Drawing.Point(0, 0), _capture.Size);
        }
        catch
        {
            Console.WriteLine(@"キャプチャに失敗しました");
        }
        return Task.CompletedTask;
    }

    private Bitmap GetRect((int, int) topLeft, (int, int) size) =>
        _capture.Clone(new Rectangle(topLeft.Item1, topLeft.Item2, size.Item1, size.Item2), _capture.PixelFormat);

    private async Task<SoftwareBitmap> GetSoftwareSnapShot(Image snap)
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

    private static async Task<string> RecognizeText(SoftwareBitmap snap)
    {
        var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
        var ocrResult = await ocrEngine?.RecognizeAsync(snap);
        return ocrResult.Text.Replace(" ", string.Empty);
    }

    /// <summary>
    /// Returns a text recognition string of order information.
    /// </summary>
    /// <param name="topLeft">top left position [default is (260, 120)].</param>
    /// <param name="size">capture size [default is (500, 120)].</param>
    /// <returns>
    /// The result string of text recognition.
    /// </returns>
    internal async Task<string> TryCaptureOrderInfo((int, int)? topLeft = null, (int, int)? size = null)
    {
        var image = GetRect(topLeft ?? (260, 120), size ?? (500, 120));
        return await RecognizeText(await GetSoftwareSnapShot(image));
    }

    /// <summary>
    /// Returns a image classification result of opponent's order icon.
    /// </summary>
    /// <param name="topLeft">top left position [default is (1800, 620)].</param>
    /// <param name="size">capture size [default is (120, 120)].</param>
    internal OrderStat CaptureOpponentsOrder((int, int)? topLeft = null, (int, int)? size = null)
    {
        var bitmap = GetRect(topLeft ?? (1800, 620), size ?? (120, 120));

        var srcImage = bitmap.ToMat();
        Mat grayImage = new();

        Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGR2GRAY);

        // ノイズを軽減するために画像を平滑化
        Cv2.GaussianBlur(grayImage, grayImage, new Size(9, 9), 2, 2);

        // 円の検出
        var circles = Cv2.HoughCircles(
            grayImage,
            HoughModes.GradientAlt,
            1,
            20,
            100,
            0.9,
            10,
            50
        );

        if (circles.Length == 0)
        {
            return new Nothing(bitmap);
        }

        // Load model and predict output
        var result = MLOrderModel.Predict(new()
        {
            ImageSource = bitmap.ToMat().ToBytes(),
        });

        return result.PredictedLabel == "wait"
                ? new WaitStat(bitmap)
                : new ActiveStat(bitmap);
    }

    /// <summary>
    /// Returns a image classification result of activating order icon.
    /// </summary>
    /// <param name="topLeft">top left position [default is (1300, 230)].</param>
    /// <param name="size">capture size [default is (500, 500)].</param>
    internal OrderStat IsActivating((int, int)? topLeft = null, (int, int)? size = null)
    {
        var bitmap = GetRect(topLeft ?? (1300, 230), size ?? (500, 500));

        var result = MLActivatingModel.Predict(new()
        {
            ImageSource = bitmap.ToMat().ToBytes(),
        });

        return result.PredictedLabel == "True" ? new ActiveStat(bitmap) : new Nothing(bitmap);
    }

    public bool IsStack()
    {
        var bitmap = GetRect((1800, 750), (55, 55));

        var srcImage = bitmap.ToMat();
        Mat grayImage = new();

        Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGR2GRAY);

        // ノイズを軽減するために画像を平滑化
        Cv2.GaussianBlur(grayImage, grayImage, new Size(9, 9), 2, 2);

        // 円の検出
        var circles = Cv2.HoughCircles(
            grayImage,
            HoughModes.GradientAlt,
            1,
            20,
            100,
            0.9,
            10,
            50
        );

        return circles.Length > 0;
    }

    /// <summary>
    /// Returns a text recognition string of opponent's order information.
    /// </summary>
    /// <param name="topLeft"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public async Task<string> CaptureOrderInfo((int, int)? topLeft = null, (int, int)? size = null)
    {
        var snapShot = GetRect(topLeft ?? (1040, 330), size ?? (250, 100));
        return await RecognizeText(await GetSoftwareSnapShot(snapShot));
    }

    public async Task Dump((int, int)? topLeft = null, (int, int)? size = null)
    {
        topLeft ??= (0, 0);
        size ??= (_capture.Width, _capture.Height);
        var image = GetRect(topLeft.Value, size.Value);
        await Task.Run(() => image.Save(@"C:\Users\lolig\OneDrive\デスクトップ\MitamatchOperations\cap.png", ImageFormat.Png));
    }
}
