using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.Json;
using mitama.Domain;
using static System.IO.Directory;
using static System.Environment;

namespace mitama.Pages.Common;

internal class Util
{
    internal static string[] LoadRegionNames()
    {
        if (Exists(Director.ProjectDir())) {
            var dirs = GetDirectories(Director.ProjectDir());
            return dirs
                .Select(path => path.Split('\\').Last())
                .ToArray();
        }

        Director.CreateDirectory(Director.ProjectDir());
        return [];
    }

    internal static string[] LoadMemberNames(string project)
    {
        var membersDir = @$"{Director.ProjectDir()}\{project}\Members";
        if (Exists(membersDir)) {
            return GetDirectories(membersDir)
                .Select(path => path.Split('\\').Last())
                .ToArray();
        }

        Director.CreateDirectory(membersDir);
        return [];
    }

    internal static string[] LoadUnitNames(string project, string name)
    {
        var unitDir = @$"{Director.ProjectDir()}\{project}\Members\{name}\Units";
        if (Exists(unitDir))
        {
            return GetFiles(unitDir)
                .Select(path => path.Split('\\').Last().Replace(".json", string.Empty))
                .ToArray();
        }

        Director.CreateDirectory(unitDir);
        return [];
    }

    internal static MemberInfo[] LoadMembersInfo(string project)
    {
        var membersDir = @$"{Director.ProjectDir()}\{project}\Members";

        if (Exists(membersDir)) {
            return GetDirectories(membersDir)
                .Select(dir => {
                    using var sr = new StreamReader($@"{dir}\info.json", Encoding.GetEncoding("UTF-8"));
                    var json = sr.ReadToEnd();
                    return MemberInfo.FromJson(json);
                })
                .ToArray();
        }

        Director.CreateDirectory(membersDir);
        return [];
    }

    private static bool IsFileInUse(string filePath)
    {
        try
        {
            using FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            // ファイルを開ける場合は利用中でないとみなす
            return false;
        }
        catch (IOException)
        {
            // IOExceptionが発生した場合はファイルが他のプロセスによって利用中
            return true;
        }
    }
}

internal class Director {
    public static bool IsFileInUse(string filePath)
    {
        try
        {
            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            // ファイルを開けた場合は使用中ではない
            return false;
        }
        catch (IOException)
        {
            // IOExceptionが発生した場合はファイルが使用中
            return true;
        }
    }
    public static void CreateDirectory(string path) {
        using var isoStore = IsolatedStorageFile.GetStore(
            IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        try {
            isoStore.CreateDirectory(path);
        }
        catch (Exception e) {
            throw new Exception(@$"The process failed: {e}");
        }
    }

    public static IsolatedStorageFileStream CreateFile(string path) {
        using var isoStore = IsolatedStorageFile.GetStore(
            IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        try {
            return isoStore.CreateFile(path);
        }
        catch (Exception e) {
            throw new Exception(@$"The process failed: {e}");
        }
    }

    internal static string MitamatchDir() {
        var dir = $@"{GetFolderPath(SpecialFolder.Desktop)}\MitamatchOperations";
        if (!Exists(dir)) CreateDirectory(dir);
        return dir;
    }
    internal static string ProjectDir() {
        var dir = $@"{MitamatchDir()}\Projects";
        if (!Exists(dir)) CreateDirectory(dir);
        return dir;
    }
    internal static string DeckDir(string project) {
        var dir = $@"{ProjectDir()}\{project}\Decks";
        if (!Exists(dir)) CreateDirectory(dir);
        return dir;
    }
    internal static string IndividualDir(string project, string member) {
        var dir = $@"{ProjectDir()}\{project}\Members\{member}";
        if (!Exists(dir)) CreateDirectory(dir);
        return dir;
    }

    internal static string UnitDir(string project, string member) {
        var dir = $@"{ProjectDir()}\{project}\Members\{member}\Units";
        if (!Exists(dir)) CreateDirectory(dir);
        return dir;
    }

    internal static string LogDir(string project)
    {
        var dir = $@"{ProjectDir()}\{project}\BattleLog\";
        if (!Exists(dir)) CreateDirectory(dir);
        return dir;
    }

    internal static void CacheWrite(byte[] json) {
        using var fs = CreateFile($@"{MitamatchDir()}\Cache\cache.json");
        fs.Write(json, 0, json.Length);
    }

    internal static Cache ReadCache() {
        using var sr = new StreamReader($@"{MitamatchDir()}\Cache\cache.json", Encoding.GetEncoding("UTF-8"));
        var json = sr.ReadToEnd();
        return JsonSerializer.Deserialize<Cache>(json);
    }

}
