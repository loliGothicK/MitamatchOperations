using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.Json;
using static System.IO.Directory;
using static System.Environment;

namespace mitama.Pages.Common;

internal class Util
{
    internal static string[] LoadRegionNames()
    {
        if (Exists(Director.RegionDir()))
        {
            var dirs = GetDirectories(Director.RegionDir());
            return dirs
                .Select(path => path.Split('\\').Last())
                .ToArray();
        }

        Director.CreateDirectory(Director.RegionDir());
        return new string[] { };
    }
}

internal class Director
{
    public static void CreateDirectory(string path)
    {
        using var isoStore = IsolatedStorageFile.GetStore(
            IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        try
        {
            isoStore.CreateDirectory(path);
        }
        catch (Exception e)
        {
            throw new Exception(@$"The process failed: {e}");
        }
    }

    public static IsolatedStorageFileStream CreateFile(string path)
    {
        using var isoStore = IsolatedStorageFile.GetStore(
            IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        try
        {
            return isoStore.CreateFile(path);
        }
        catch (Exception e)
        {
            throw new Exception(@$"The process failed: {e}");
        }
    }

    internal static string MitamatchDir()
        => $@"{GetFolderPath(SpecialFolder.Desktop)}\MitamatchOperations";
    internal static string DeckDir()
        => $@"{GetFolderPath(SpecialFolder.Desktop)}\MitamatchOperations\Decks";
    internal static string RegionDir()
        => $@"{GetFolderPath(SpecialFolder.Desktop)}\MitamatchOperations\Regions";

    internal static void CacheWrite(byte[] json)
    {
        using var fs = CreateFile($@"{MitamatchDir()}\Cache\cache.json");
        fs.Write(json, 0, json.Length);
    }

    internal static Cache ReadCache()
    {
        using var sr = new StreamReader($@"{MitamatchDir()}\Cache\cache.json", Encoding.GetEncoding("UTF-8"));
        var json = sr.ReadToEnd();
        return JsonSerializer.Deserialize<Cache>(json);
    }

}
