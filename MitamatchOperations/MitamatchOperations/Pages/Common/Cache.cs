using System.Text.Json;

namespace mitama.Pages.Common;

internal record struct Cache(string LoggedIn)
{
    internal static Cache FromJson(string json) => JsonSerializer.Deserialize<Cache>(json);
    internal string ToJson() => JsonSerializer.Serialize(this);
}