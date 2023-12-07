using System;
using System.Drawing;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using mitama.Domain;
using Windows.Storage;

namespace mitama.Algorithm.IR;

internal class Match {
    public static async Task<(Bitmap, Memoria[])> Recognise(Bitmap img, bool IsVanguard) 
    {
        var target = img.ToMat();
        var grayMat = target.CvtColor(ColorConversionCodes.BGR2GRAY);
        var thresholdMat = grayMat.Threshold(230, 255, ThresholdTypes.BinaryInv);
        thresholdMat.FindContours(out var contours, out _, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

        var rects = new List<Rect>();
        foreach (var contour in contours) {
            var area = Cv2.ContourArea(contour);
            switch (area) {
                case <= 10000 or > 100000:
                    continue;
                default: {
                        var rect = Cv2.BoundingRect(contour);
                        if (IsSquare(rect)) {
                            rects.Add(rect);
                        }
                        break;
                    }
            }
        }

        var akaze = AKAZE.Create();

        rects = Clean(rects);
        rects = Interpolation(rects, img.Width);

        var dummyCostume = IsVanguard ? Costume.DummyVanguard : Costume.DummyRearguard;

        foreach (var rect in rects) Cv2.Rectangle(target, rect, Scalar.Aquamarine, 5);

        {
            var templates = await Task.WhenAll(Memoria.List.Where(dummyCostume.CanBeEquipped).Select(async memoria => {
                try
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(memoria.Uri);
                    var image = new Bitmap((await FileIO.ReadBufferAsync(file)).AsStream());
                    var descriptors = new Mat();
                    akaze.DetectAndCompute(image.ToMat(), null, out _, descriptors);
                    return (memoria, descriptors);
                }
                catch (Exception ex)
                {
                    throw new Exception($"「{memoria.Name}」が見つかりません:\n{ex}");
                }
            }));

            var detected = rects.AsParallel()
                .Select(target.Clone)
                .Select(mat => {
                    var descriptors = new Mat();
                    akaze.DetectAndCompute(mat, null, out _, descriptors);
                    return descriptors;
                }).Select(source => {
                    return templates.MinBy(template => {
                        var (_, train) = template;
                        var matcher = new BFMatcher(NormTypes.Hamming);
                        var matches = matcher.Match(source, train);
                        var sum = matches.Sum(x => x.Distance);
                        return sum / matches.Length;
                    }).memoria;
                }).ToArray();
            return (target.ToBitmap(), detected);
        }
    }

    private static bool IsSquare(Rect rect)
        => Math.Min(Math.Abs(rect.Height), Math.Abs(rect.Width)) > 0.95 * Math.Max(Math.Abs(rect.Height), Math.Abs(rect.Width));

    private static List<Rect> Clean(List<Rect> memorias) 
    {
        var size = (int)memorias.Select(memoria => (memoria.Width + memoria.Height) / 2.0).Mean();

        List<List<Rect>> lines = new();
        while (memorias.Count != 0) {
            var top = memorias.MinBy(memoria => memoria.Top)!;
            var line = new List<Rect> { top };
            memorias.Remove(top);
            foreach (var memoria in memorias.Where(memoria => Math.Abs(memoria.Top - top.Top) < size / 10).ToArray()) {
                line.Add(memoria);
                memorias.Remove(memoria);
            }
            line.Sort((x, y) => x.Left.CompareTo(y.Left));
            lines.Add(line);
        }

        var margin = double.PositiveInfinity;

        for (var i = 1; i < lines[0].Count; i++) {
            var s = lines[0][i].Left - lines[0][i - 1].Right;
            if (s > 1 && s < margin) margin = s;
        }
        for (var i = 1; i < lines[1].Count; i++) {
            var s = lines[1][i].Left - lines[1][i - 1].Right;
            if (s > 1 && s < margin) margin = s;
        }

        foreach (var line in lines) {
            for (var i = 1; i < line.Count; i++) {
                var space = line[i].TopLeft.X - line[i - 1].BottomRight.X;
                if (space <= 0 || (margin + 10 < space && space < margin + size)) {
                    line.Remove(line[i]);
                }
            }
        }
        return lines.SelectMany(xs => xs).ToList();
    }

    private static List<Rect> Interpolation(List<Rect> memorias, int width)
    {
        var size = (int)memorias.Select(memoria => (memoria.Width + memoria.Height) / 2.0).Mean();

        List<List<Rect>> lines = new();
        while (memorias.Count != 0) {
            var top = memorias.MinBy(memoria => memoria.Top)!;
            var line = new List<Rect> { top };
            memorias.Remove(top);
            foreach (var memoria in memorias.Where(memoria => Math.Abs(memoria.Top - top.Top) < size / 10).ToArray()) {
                line.Add(memoria);
                memorias.Remove(memoria);
            }
            line.Sort((x, y) => x.Left.CompareTo(y.Left));
            lines.Add(line);
        }

        var marginD = double.PositiveInfinity;

        for (var i = 1; i < lines[0].Count; i++) {
            var s = lines[0][i].Left - lines[0][i - 1].Right;
            if (s > 1 && s < marginD) marginD = s;
        }
        for (var i = 1; i < lines[1].Count; i++) {
            var s = lines[1][i].Left - lines[1][i - 1].Right;
            if (s > 1 && s < marginD) marginD = s;
        }

        var margin = (int)marginD;

        foreach (var line in lines) {
            foreach (var (a, b) in line.Zip(line.Skip(1)).ToArray()) {
                var space = b.Left - a.Right;
                if (space < size) continue;

                var num = space / (margin + size);
                var basis = a.BottomRight;

                for (var i = 0; i < num; i++) {
                    var bottomLeft = basis with {
                        X = basis.X + margin * (i + 1) + size * i,
                    };
                    var topRight = bottomLeft with {
                        X = bottomLeft.X + size,
                        Y = bottomLeft.Y - size
                    };
                    line.Add(Cv2.BoundingRect(new[] { topRight, bottomLeft }));
                }
            }
            line.Sort((x, y) => x.Left.CompareTo(y.Left));
        }

        for (var i = 0; i < 2; i++) {
            while (lines[i].Count < 10) {
                var line = lines[i];
                var left = line.First();
                var right = line.Last();

                if (left.Left > width - right.Right) {
                    line.Insert(0, Cv2.BoundingRect(new[]
                    {
                        left.TopLeft with { X = left.TopLeft.X - margin },
                        left.TopLeft with { X = left.TopLeft.X - margin - size, Y = left.TopLeft.Y + size },
                    }));
                }
                else {
                    line.Add(Cv2.BoundingRect(new[]
                    {
                        right.BottomRight with { X = right.BottomRight.X + margin },
                        right.BottomRight with { X = right.BottomRight.X + margin + size, Y = right.BottomRight.Y - size },
                    }));
                }
            }
        }

        var verticalMargins = lines[0].Zip(lines[1]).Select(t => t.Second.Top - t.First.Bottom).ToList();
        verticalMargins.Sort();
        var verticalMargin = verticalMargins[verticalMargins.Count / 2];
        lines.Insert(2,
        [
            Cv2.BoundingRect(new[]
            {
                lines[1][0].TopLeft with { Y = lines[1][0].TopLeft.Y + verticalMargin + size },
                lines[1][0].BottomRight with { Y = lines[1][0].BottomRight.Y + verticalMargin + size }
            }),
            Cv2.BoundingRect(new[]
            {
                lines[1][1].TopLeft with { Y = lines[1][1].TopLeft.Y + verticalMargin + size },
                lines[1][1].BottomRight with { Y = lines[1][1].BottomRight.Y + verticalMargin + size }
            }),
            Cv2.BoundingRect(new[]
            {
                lines[1][2].TopLeft with { Y = lines[1][2].TopLeft.Y + verticalMargin + size },
                lines[1][2].BottomRight with { Y = lines[1][2].BottomRight.Y + verticalMargin + size }
            }),
            Cv2.BoundingRect(new[]
            {
                lines[1][3].TopLeft with { Y = lines[1][3].TopLeft.Y + verticalMargin + size },
                lines[1][3].BottomRight with { Y = lines[1][3].BottomRight.Y + verticalMargin + size }
            })
        ]);

        return lines.Take(3).SelectMany(xs => xs).ToList();
    }
}
