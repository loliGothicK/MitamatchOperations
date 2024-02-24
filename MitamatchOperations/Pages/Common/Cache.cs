using System.Text;
using System.Text.Json;

namespace mitama.Pages.Common;

internal record struct Cache(string Legion, string User = null)
{
    internal static Cache FromJson(string json) => JsonSerializer.Deserialize<Cache>(json);

    internal readonly byte[] ToJsonBytes() => new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(this));
}