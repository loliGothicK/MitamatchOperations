using System.Linq;
using static System.IO.Directory;
using static System.Environment;

namespace mitama.Pages.Common;

internal class Util
{
    internal static string[] LoadRegionNames()
        => GetDirectories(@$"{GetFolderPath(SpecialFolder.Desktop)}\MitamatchOperations\Regions")
            .Select(path => path.Split('\\').Last())
            .ToArray();
}