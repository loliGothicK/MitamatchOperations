using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DynamicData.Kernel;
using LiteDB;
using Microsoft.UI.Xaml;
using Mitama.Pages.Common;

namespace Mitama.Domain;

public abstract record CostumeType
{
    public abstract double Value { get; }
}

// 通単
public record NormalSingleCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

// 通範
public record NormalRangeCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

// 特単
public record SpecialSingleCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

// 特範
public record SpecialRangeCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

// 支援
public record AssistCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

// 妨害
public record InterferenceCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

// 回復
public record RecoveryCostume(double Calibration) : CostumeType
{
    public override double Value => Calibration;
}

public record RareSkill(string Name, string Description);
public record ExSkill(string Name, string Description);

public abstract record LilySkill;
public record Common(LilySkill[] Skills) : LilySkill;
public record Unique(LilySkill[] Skills) : LilySkill;
public record LilyHp(int Value) : LilySkill;
public record LilyAtk(int Value) : LilySkill;
public record LilyDef(int Value) : LilySkill;
public record LilySpAtk(int Value) : LilySkill;
public record LilySpDef(int Value) : LilySkill;
public record Calibration(double Percentage) : LilySkill;

public record struct LilyStatus(int Hp, int Atk, int Def, int SpAtk, int SpDef)
{
    public static LilyStatus operator +(LilyStatus a, LilyStatus b) => new(a.Hp + b.Hp, a.Atk + b.Atk, a.Def + b.Def, a.SpAtk + b.SpAtk, a.SpDef + b.SpDef);
    public static LilyStatus operator -(LilyStatus a, LilyStatus b) => new(a.Hp - b.Hp, a.Atk - b.Atk, a.Def - b.Def, a.SpAtk - b.SpAtk, a.SpDef - b.SpDef);
    public static LilyStatus operator *(LilyStatus a, LilyStatus b) => new(a.Hp * b.Hp, a.Atk * b.Atk, a.Def * b.Def, a.SpAtk * b.SpAtk, a.SpDef * b.SpDef);
    public static LilyStatus operator /(LilyStatus a, LilyStatus b) => new(a.Hp / b.Hp, a.Atk / b.Atk, a.Def / b.Def, a.SpAtk / b.SpAtk, a.SpDef / b.SpDef);
    public static LilyStatus operator %(LilyStatus a, LilyStatus b) => new(a.Hp % b.Hp, a.Atk % b.Atk, a.Def % b.Def, a.SpAtk % b.SpAtk, a.SpDef % b.SpDef);
    public static LilyStatus operator +(LilyStatus a, int b) => new(a.Hp + b, a.Atk + b, a.Def + b, a.SpAtk + b, a.SpDef + b);
    public static LilyStatus operator -(LilyStatus a, int b) => new(a.Hp - b, a.Atk - b, a.Def - b, a.SpAtk - b, a.SpDef - b);
    public static LilyStatus operator *(LilyStatus a, int b) => new(a.Hp * b, a.Atk * b, a.Def * b, a.SpAtk * b, a.SpDef * b);
    public static LilyStatus operator /(LilyStatus a, int b) => new(a.Hp / b, a.Atk / b, a.Def / b, a.SpAtk / b, a.SpDef / b);
    public static LilyStatus operator %(LilyStatus a, int b) => new(a.Hp % b, a.Atk % b, a.Def % b, a.SpAtk % b, a.SpDef % b);
}

public record CostumeWithEx(Costume Costume, ExInfo Ex, string Icon = $"/Assets/Images/lily_true.png")
{
    public static implicit operator CostumeWithEx(Costume v)
    {
        return new CostumeWithEx(v, v.ExSkill.HasValue ? ExInfo.Active : ExInfo.None);
    }

    public static implicit operator Costume(CostumeWithEx v)
    {
        return v.Costume;
    }

    public Visibility Visibility = Costume.ExSkill.HasValue ? Visibility.Visible : Visibility.Collapsed;
}

public partial record struct Costume(
    int Index,
    string Lily,
    string Name,
    CostumeType Type,
    RareSkill RareSkill,
    LilySkill[] LilySkills,
    Optional<ExSkill> ExSkill
)
{
    public string Path = @$"{Director.CostumeImageDir(Lily)}\{Name}.png";

    public string DisplayName = $"{Lily}/{Name}";
    public string DisplayExSkill = ExSkill.HasValue ? $"{ExSkill.Value.Description}" : @"なし";

    public static Costume Of(int id) => List.Value[^(id + 1)];

    public readonly LilyStatus Status => LilySkills.Aggregate(new LilyStatus(), (stat, skill) =>
    {
        return skill switch
        {
            Common common => common.Skills.Where(x => x is not Calibration).Aggregate(stat, (stat, skill) =>
            {
                return skill switch
                {
                    LilyHp hp => stat with { Hp = stat.Hp + hp.Value },
                    LilyAtk atk => stat with { Atk = stat.Atk + atk.Value },
                    LilyDef def => stat with { Def = stat.Def + def.Value },
                    LilySpAtk spAtk => stat with { SpAtk = stat.SpAtk + spAtk.Value },
                    LilySpDef spDef => stat with { SpDef = stat.SpDef + spDef.Value },
                    _ => throw new UnreachableException(""),
                };
            }),
            Unique unique => unique.Skills.Where(x => x is not Calibration).Aggregate(stat, (stat, skill) =>
            {
                return skill switch
                {
                    LilyHp hp => stat with { Hp = stat.Hp + hp.Value },
                    LilyAtk atk => stat with { Atk = stat.Atk + atk.Value },
                    LilyDef def => stat with { Def = stat.Def + def.Value },
                    LilySpAtk spAtk => stat with { SpAtk = stat.SpAtk + spAtk.Value },
                    LilySpDef spDef => stat with { SpDef = stat.SpDef + spDef.Value },
                    _ => throw new UnreachableException(""),
                };
            }),
            _ => throw new UnreachableException(""),
        };
    });

    public readonly bool CanBeEquipped(Memoria memoria)
    {
        return memoria.Kind switch
        {
            Vanguard => Type switch
            {
                NormalSingleCostume => true,
                NormalRangeCostume => true,
                SpecialSingleCostume => true,
                SpecialRangeCostume => true,
                _ => false,
            },
            Rearguard => Type switch
            {
                AssistCostume => true,
                InterferenceCostume => true,
                RecoveryCostume => true,
                _ => false,
            },
            _ => throw new UnreachableException("")
        };
    }

    public static Costume DummyVanguard => new(-1, "dummy", "dummy", new NormalSingleCostume(0), new RareSkill("", ""), [], Optional<ExSkill>.None);
    public static Costume DummyRearguard => new(-1, "dummy", "dummy", new RecoveryCostume(0), new RareSkill("", ""), [], Optional<ExSkill>.None);

    readonly bool IEquatable<Costume>.Equals(Costume other) => Name == other.Name && Lily == other.Lily;

    public override readonly int GetHashCode() => HashCode.Combine(Name, Lily);

    public static Lazy<Costume[]> List => new(() =>
    {
        static CostumeType IntoType(string type)
        {
            Regex regex = MyRegex();
            var match = regex.Match(type);
            return type switch
            {
                _ when type.StartsWith("通単") => new NormalSingleCostume(double.Parse(match.Groups[1].Value)),
                _ when type.StartsWith("通範") => new NormalRangeCostume(double.Parse(match.Groups[1].Value)),
                _ when type.StartsWith("特単") => new SpecialSingleCostume(double.Parse(match.Groups[1].Value)),
                _ when type.StartsWith("特範") => new SpecialRangeCostume(double.Parse(match.Groups[1].Value)),
                _ when type.StartsWith("支援") => new AssistCostume(double.Parse(match.Groups[1].Value)),
                _ when type.StartsWith("妨害") => new InterferenceCostume(double.Parse(match.Groups[1].Value)),
                _ when type.StartsWith("回復") => new RecoveryCostume(double.Parse(match.Groups[1].Value)),
                _ => throw new NotImplementedException($"{type}"),
            };
        }

        List<Costume> list = [];
        using var db = new LiteDatabase(@$"{Director.DatabaseDir()}\data");
        var data = db.GetCollection<Repository.Costume.POCO>("costume").FindAll().ToList();
        foreach (var poco in data)
        {
            var skills = poco.skills.Select(skill => Repository.Costume.SkillDto.FromJson(skill).IntoLilySkill);
            list.Add(new Costume(
                poco.id - 1,
                poco.lily,
                poco.name,
                IntoType(poco.type),
                new RareSkill(poco.rare.Split(":::").First(), poco.rare.Split(":::").Last()),
                [.. skills],
                poco.ex is null
                    ? Optional<ExSkill>.None
                    : new ExSkill(poco.rare.Split(":::").First(), poco.rare.Split(":::").Last())
            ));
        }
        list.Sort((a, b) => b.Index.CompareTo(a.Index));
        return [.. list];
    });

    [GeneratedRegex(@"\+(\d+\.?\d+)%$")]
    private static partial Regex MyRegex();
}
