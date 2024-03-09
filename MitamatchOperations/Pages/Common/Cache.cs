﻿using System;
using System.Text;
using System.Text.Json;

namespace Mitama.Pages.Common;

internal record struct Cache(
    string Legion,
    string User = null,
    string JWT = null,
    int? MemoriaIndex = null,
    int? CostumeIndex = null,
    DateTimeOffset? FetchDate = null,
    Version? version = null
)
{
    internal static Cache FromJson(string json) => JsonSerializer.Deserialize<Cache>(json);

    internal readonly byte[] ToJsonBytes() => new UTF8Encoding(true).GetBytes(JsonSerializer.Serialize(this));
}
