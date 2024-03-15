using System;
using System.Collections.Generic;
using Mitama.Pages.Common;

namespace Mitama.Domain;

public record struct Charm(
    int Index,
    string Name,
    string Ability,
    BasicStatus Status,
    DateOnly Date
)
{
    public readonly string Path => $@"{Director.CharmImageDir()}\{Name}.png";

    public readonly string ToPrettyJSON()
    {
        var json = new
        {
            name = Name,
            ability = Ability,
            status = Status.ToArray(),
            date = Date.ToString("yyyy-MM-dd"),
        };
        return System.Text.Json.JsonSerializer.Serialize(json, options: new() { WriteIndented = true });
    }

    public static readonly Lazy<Charm[]> List = new(() =>
    {
        List<Charm> list = [];
        foreach (var poco in Repository.Repository.LiteDB.List<Repository.Charm.POCO>())
        {
            list.Add(new Charm(
                poco.id - 1,
                poco.name,
                poco.ability,
                BasicStatus.FromRaw(poco.status),
                DateOnly.FromDateTime(poco.date)
            ));
        }
        list.Sort((a, b) => b.Date.CompareTo(a.Date));
        return [.. list];
    });
}
