using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Mitama.Pages.Common;

namespace Mitama.Domain;

public record Charm(
    int Index,
    string Name,
    string Ability,
    BasicStatus Status,
    DateOnly Date
)
{
    public string Path => $@"{Director.CharmImageDir()}\{Name}.png";

    public string ToPrettyJSON()
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
        using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
        var data = db.GetCollection<Repository.Charm.POCO>("charm").FindAll().ToList();
        foreach (var poco in data)
        {
            list.Add(new Charm(
                poco.id - 1,
                poco.name,
                poco.ability,
                BasicStatus.FromRaw(poco.status),
                DateOnly.FromDateTime(poco.date)
            ));
        }
        list.Sort((a, b) => b.Index.CompareTo(a.Index));
        return [.. list];
    });
}
