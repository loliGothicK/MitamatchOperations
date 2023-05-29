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

namespace mitama.Pages.Capture;

internal abstract record OrderStat;

internal record WaitStat(Bitmap Image) : OrderStat;
internal record ActiveStat(Bitmap Image) : OrderStat;
internal record Nothing : OrderStat;


internal partial class WindowCapture {
    private readonly MemoryStream _bufferStream;
    private readonly IntPtr _handle;

    public WindowCapture(IntPtr handle) {
        _handle = handle;
        _bufferStream = new MemoryStream(1000) {
            Position = 0
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void GetWindowRect(IntPtr hwnd, out Rect lpRect);

    public Bitmap SnapShot((int, int) topLeft, (int, int) size) {
        // ウィンドウサイズ取得
        GetWindowRect(_handle, out var rect);
        var (x, y) = topLeft;
        var (width, height) = size;

        var bmp = new Bitmap(width, height);

        //Graphicsの作成
        using var g = Graphics.FromImage(bmp);
        try {
            g.CopyFromScreen(new System.Drawing.Point(rect.left + x, rect.top + y), new System.Drawing.Point(0, 0), bmp.Size);
        }
        catch {
            Console.WriteLine(@"キャプチャに失敗しました");
        }

        return bmp;
    }

    public async Task<SoftwareBitmap> GetSoftwareSnapShot(Bitmap snap) {
        // 取得したキャプチャ画像をストリームに保存
        _bufferStream.Seek(0, SeekOrigin.Begin);
        snap.Save(_bufferStream, ImageFormat.Png);

        // 保存した画像をSoftwareBitmap形式で読み込み
        var decoder = await BitmapDecoder.CreateAsync(_bufferStream.AsRandomAccessStream());
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        // SoftwareBitmap形式の画像を返す
        return softwareBitmap;
    }

    public async Task<string> RecognizeText(SoftwareBitmap snap) {
        var ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
        var ocrResult = await ocrEngine?.RecognizeAsync(snap);
        return ocrResult.Text.Replace(" ", string.Empty);
    }

    public async Task<string> TryCaptureOrderInfo() {
        return await RecognizeText(await GetSoftwareSnapShot(SnapShot((260, 120), (500, 120))));
    }

    public OrderStat CaptureOpponentsOrder()
    {
        var bitmap = SnapShot((1800, 620), (120, 120));
        bitmap.Save(@"C:\Users\lolig\OneDrive\デスクトップ\MitamatchOperations\debug.png");

        var srcImage = bitmap.ToMat();
        Mat grayImage = new();

        Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGR2GRAY);

        // ノイズを軽減するために画像を平滑化
        Cv2.GaussianBlur(grayImage, grayImage, new OpenCvSharp.Size(9, 9), 2, 2);

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

        var sampleData = new MLOrderModel.ModelInput()
        {
            ImageSource = bitmap.ToMat().ToBytes(),
        };

        // Load model and predict output
        var result = MLOrderModel.Predict(sampleData);

        return circles.Length == 0
            ? new Nothing()
            : result.PredictedLabel == "wait"
                ? new WaitStat(bitmap)
                : new ActiveStat(bitmap);
    }

    public bool IsActivating()
    {
        var bitmap = SnapShot((1300, 230), (500, 500));
        bitmap.Save(@"C:\Users\lolig\OneDrive\デスクトップ\MitamatchOperations\debug_active.png");

        //Load sample data
        var sampleData = new MLActivatingModel.ModelInput()
        {
            ImageSource = bitmap.ToMat().ToBytes(),
        };

        //Load model and predict output
        var result = MLActivatingModel.Predict(sampleData);

        return result.PredictedLabel == "True";
    }
        
    public bool IsStack()
    {
        var bitmap = SnapShot((1800, 750), (55, 55));

        var srcImage = bitmap.ToMat();
        Mat grayImage = new();

        Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGR2GRAY);

        // ノイズを軽減するために画像を平滑化
        Cv2.GaussianBlur(grayImage, grayImage, new OpenCvSharp.Size(9, 9), 2, 2);

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
}
