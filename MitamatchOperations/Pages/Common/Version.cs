
namespace Mitama.Pages.Common;

using System;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;

internal readonly partial record struct Version(ushort Major, ushort Minor, ushort Bugfix) : IComparable<Version>
{
    internal static Version Current => new(
        Package.Current.Id.Version.Major,
        Package.Current.Id.Version.Minor,
        Package.Current.Id.Version.Build
    );

    public override string ToString() => $"v{Major}.{Minor}.{Bugfix}";

    public static Version Parse(string version)
    {
        var regex = MyRegex();
        var match = regex.Match(version);
        return new Version(
            ushort.Parse(match.Groups["major"].Value),
            ushort.Parse(match.Groups["minor"].Value),
            ushort.Parse(match.Groups["bugfix"].Value)
        );
    }

    [GeneratedRegex(@"^v(?<major>\d+)\.(?<minor>\d+)\.(?<bugfix>\d+)$")]
    private static partial Regex MyRegex();

    public int CompareTo(Version other)
    {
        var major = Minor.CompareTo(other.Minor);
        if (major != 0) return major;
        var minor = Minor.CompareTo(other.Minor);
        if (minor != 0) return minor;
        return Bugfix.CompareTo(other.Bugfix);
    }

    // less than operator
    public static bool operator <(Version left, Version right)
    {
        return left.CompareTo(right) < 0;
    }
    public static bool operator >(Version left, Version right)
    {
        return left.CompareTo(right) > 0;
    }

}
