
namespace Mitama.Pages.Common;
using Windows.ApplicationModel;

internal readonly record struct Version(ushort Major, ushort Minor, ushort Bugfix)
{
    internal static Version Current => new(
        Package.Current.Id.Version.Major,
        Package.Current.Id.Version.Minor,
        Package.Current.Id.Version.Build
    );

    public override string ToString() => $"v{Major}.{Minor}.{Bugfix}";
}
