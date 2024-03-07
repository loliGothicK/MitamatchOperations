using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LiteDB;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Mitama.Lib;
using Mitama.Pages.Common;
using Mitama.Repository;
using Syncfusion.UI.Xaml.Data;

namespace Mitama.Domain;

public record struct IdAndConcentration(int Id, int Concentration);

public record struct Unit(string UnitName, bool IsFront, List<MemoriaWithConcentration> Memorias)
{
    public readonly string ToJson() =>
        System.Text.Json.JsonSerializer.Serialize(new UnitDto(UnitName, IsFront, Memorias.Select(m => new IdAndConcentration(m.Memoria.Id, m.Concentration)).ToArray()));

    public static (bool, Unit) FromJson(string json)
    {
        try
        {
            var dto = System.Text.Json.JsonSerializer.Deserialize<UnitDto>(json);
            var selector = Memoria.List.Value.ToDictionary(m => m.Id);
            return (false, new Unit(
                dto.UnitName,
                dto.IsFront,
                dto.Items.Select(item => new MemoriaWithConcentration(selector[item.Id], item.Concentration)).ToList()
            ));
        }
        catch
        {
            var dto = System.Text.Json.JsonSerializer.Deserialize<RegacyUnitDto>(json);
            var selector = Memoria.List.Value.ToDictionary(m => m.Id);
            return (true, new Unit(
                dto.UnitName,
                dto.IsFront,
                dto.Items.Select(idx => new MemoriaWithConcentration(selector[idx], 4)).ToList()
            ));
        }
    }
}

public record struct UnitDto(string UnitName, bool IsFront, IdAndConcentration[] Items);
public record struct RegacyUnitDto(string UnitName, bool IsFront, int[] Items);

public enum Type
{
    Normal,
    Special,
}

public enum Level
{
    NoLevel,
    One,
    Two,
    Three,
    ThreePlus,
    Four,
    FourPlus,
    Five,
    FivePlus,
    Lg,
    LgPlus,
}

public abstract record StatusChange;

public abstract record Stat;

public record Atk : Stat;

public record Def : Stat;

public record SpAtk : Stat;

public record SpDef : Stat;

public record ElementAttack(Element Element) : Stat;

public record ElementGuard(Element Element) : Stat;

public record Life : Stat;

public enum Amount
{
    // 小
    Small,

    // 中
    Medium,

    // 大
    Large,

    // 特大
    ExtraLarge,

    // 超特大
    SuperExtraLarge,
}

public record StatusUp(Stat Stat, Amount Amount) : StatusChange;

public record StatusDown(Stat Stat, Amount Amount) : StatusChange;

public enum Range
{
    A,
    B,
    C,
    D,
    E,
}

public abstract record SkillEffect;

public record ElementStimulation(Element Element) : SkillEffect;

public record Heal : SkillEffect;

public record Charge : SkillEffect;

public record Recover : SkillEffect;

public record ElementSpread(Element Element) : SkillEffect;

public record ElementStrengthen(Element Element) : SkillEffect;

public record ElementWeaken(Element Element) : SkillEffect;

public record Counter : SkillEffect;

public record Skill(
    string Name,
    string Description,
    SkillEffect[] Effects,
    // ステータス変動
    StatusChange[] StatusChanges,
    Level Level,
    Range Range
);

public enum Trigger
{
    // 攻
    Attack,

    // 援
    Support,

    // 回
    Recovery,

    // コ
    Command,
}

public abstract record SupportEffect;

public record NormalMatchPtUp : SupportEffect;

public record SpecialMatchPtUp : SupportEffect;

public record DamageUp : SupportEffect;

public record PowerUp(Type Type) : SupportEffect;

public record PowerDown(Type Type) : SupportEffect;

public record GuardUp(Type Type) : SupportEffect;

public record GuardDown(Type Type) : SupportEffect;

public record ElementPowerUp(Element Element) : SupportEffect;

public record ElementPowerDown(Element Element) : SupportEffect;

public record ElementGuardUp(Element Element) : SupportEffect;

public record ElementGuardDown(Element Element) : SupportEffect;

public record SupportUp : SupportEffect;

public record RecoveryUp : SupportEffect;

public record MpCostDown : SupportEffect;

public record RangeUp(int Plus) : SupportEffect;

public record SupportSkill(
    string Name,
    string Description,
    Trigger Trigger,
    SupportEffect[] Effects,
    Level Level
);

public abstract record MemoriaKind
{
    public virtual string Icon { get; }
}

public enum VanguardKind
{
    NormalSingle,
    NormalRange,
    SpecialSingle,
    SpecialRange,
}

public enum RearguardKind
{
    Support,
    Interference,
    Recovery,
}

public record Vanguard(VanguardKind Kind) : MemoriaKind
{
    public override string Icon => Kind switch
    {
        VanguardKind.NormalSingle => "/Assets/Images/NormalSingle.png",
        VanguardKind.NormalRange => "/Assets/Images/NormalRange.png",
        VanguardKind.SpecialSingle => "/Assets/Images/SpecialSingle.png",
        VanguardKind.SpecialRange => "/Assets/Images/SpecialRange.png",
        _ => throw new NotImplementedException(),
    };
}

public record Rearguard(RearguardKind Kind) : MemoriaKind
{
    public override string Icon => Kind switch
    {
        RearguardKind.Support => "/Assets/Images/Assist.png",
        RearguardKind.Interference => "/Assets/Images/Interference.png",
        RearguardKind.Recovery => "/Assets/Images/Recovery.png",
        _ => throw new NotImplementedException(),
    };
}

public record MemoriaWithConcentration(Memoria Memoria, int Concentration)
{
    public Memoria Memoria { get; set; } = Memoria;
    public int Concentration { get; set; } = Concentration;

    public int FontSize => Concentration switch
    {
        4 => 12,
        _ => 18,
    };

    public Thickness Margin => Concentration switch
    {
        4 => new(0, 30, 2, 0),
        _ => new(0, 26, -4, 0)
    };

    public string LimitBreak => Concentration switch
    {
        0 => "0",
        1 => "1",
        2 => "2",
        3 => "3",
        4 => "MAX",
        _ => throw new UnreachableException("Unreachable"),
    };

    public static implicit operator Memoria(MemoriaWithConcentration m) => m.Memoria;

    public BasicStatus Status => Memoria.Status[Concentration];

    public override int GetHashCode() => HashCode.Combine(Memoria.Id, Concentration);

    bool IEquatable<MemoriaWithConcentration>.Equals(MemoriaWithConcentration other)
        => other is not null
        && Memoria.Id == other.Memoria.Id
        && Concentration == other.Concentration;
}

public record Memoria(
    int Id,
    string Link,
    string FullName,
    string Name,
    MemoriaKind Kind,
    Element Element,
    BasicStatus[] Status,
    int Cost,
    Skill Skill,
    SupportSkill SupportSkill,
    bool IsLegendary = false
)
{
    public string Path = $@"{Director.MitamatchDir()}\Images\Memoria\{Name}.png";
    public string ToJson()
    {
        return new Indoc($$"""
            {
                "link": "{{HttpUtility.UrlEncode(Link)}}",
                "name": "{{HttpUtility.UrlEncode(Name)}}"
            }
        """).Text;
    }

    public string ToPrettyJson()
    {
        var json = new
        {
            Id,
            Link,
            FullName,
            Name,
            Kind = Kind switch
            {
                Vanguard(VanguardKind.NormalSingle) => "NormalSingle",
                Vanguard(VanguardKind.NormalRange) => "NormalRange",
                Vanguard(VanguardKind.SpecialSingle) => "SpecialSingle",
                Vanguard(VanguardKind.SpecialRange) => "SpecialRange",
                Rearguard(RearguardKind.Support) => "Support",
                Rearguard(RearguardKind.Interference) => "Interference",
                Rearguard(RearguardKind.Recovery) => "Recovery",
                _ => throw new NotImplementedException(),
            },
            Element = Element switch
            {
                Element.Water => "Water",
                Element.Wind => "Wind",
                Element.Fire => "Fire",
                Element.Light => "Light",
                Element.Dark => "Dark",
                _ => throw new NotImplementedException(),
            },
            Status = Status.Select(stat => new[] { stat.Atk, stat.SpAtk, stat.Def, stat.SpDef }).ToArray(),
            Cost,
            Skill = new
            {
                Skill.Name,
                Skill.Description,
            },
            Support = new
            {
                SupportSkill.Name,
                SupportSkill.Description,
            },
            IsLegendary,
        };
        return System.Text.Json.JsonSerializer.Serialize(json, options: new() { WriteIndented = true });
    }

    public virtual bool Equals(Memoria other) => Id == other?.Id;
    public override int GetHashCode() => Name.GetHashCode();

    public static Memoria Of(int id) => List.Value[^(id + 1)];

    public static readonly Lazy<Memoria[]> List = new(() =>
    {
        List<Memoria> list = [];
        using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
        var data = db.GetCollection<MemoriaPOCO>("memoria").FindAll().ToList();
        foreach (var poco in data)
        {
            var skill = SkillDto.FromJson(poco.skill);
            var support = SupportDto.FromJson(poco.support);
            list.Add(new Memoria(
                poco.id - 1,
                poco.link,
                poco.full_name,
                poco.name,
                poco.kind switch
                {
                    "通常単体" => new Vanguard(VanguardKind.NormalSingle),
                    "通常範囲" => new Vanguard(VanguardKind.NormalRange),
                    "特殊単体" => new Vanguard(VanguardKind.SpecialSingle),
                    "特殊範囲" => new Vanguard(VanguardKind.SpecialRange),
                    "支援" => new Rearguard(RearguardKind.Support),
                    "妨害" => new Rearguard(RearguardKind.Interference),
                    "回復" => new Rearguard(RearguardKind.Recovery),
                    _ => throw new NotImplementedException(),
                },
                poco.element switch
                {
                    "水" => Element.Water,
                    "風" => Element.Wind,
                    "火" => Element.Fire,
                    "光" => Element.Light,
                    "闇" => Element.Dark,
                    _ => throw new NotImplementedException(),
                },
                poco.status.Select(s => BasicStatus.FromRaw(s)).ToArray(),
                poco.cost,
                skill.IntoSkill,
                support.IntoSupportSkill,
                poco.is_legendary
            ));
        }
        list.Sort((a, b) => b.Id.CompareTo(a.Id));
        return [.. list];
    });
}
