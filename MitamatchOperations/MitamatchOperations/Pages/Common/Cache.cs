using System.Text;
using System.Text.Json;

namespace mitama.Pages.Common;

internal record struct Cache(string Region, string? User = null) {
    internal static Cache FromJson(string json) => JsonSerializer.Deserialize<Cache>(json);

    internal byte[] ToJsonBytes() => new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(this));
}