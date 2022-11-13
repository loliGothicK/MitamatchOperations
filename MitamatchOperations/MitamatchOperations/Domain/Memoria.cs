﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace mitama.Domain;

public record struct Unit(string UnitName, bool IsFront, List<Memoria> Memorias)
{
    public string ToJson() => JsonSerializer.Serialize(new UnitDto(UnitName, IsFront, Memorias.Select(m => m.Name).ToArray()));

    public static Unit FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<UnitDto>(json);
        var dummyCostume = dto.IsFront ? Costume.List[0] : Costume.List[1];
        var selector = Memoria.List.Where(dummyCostume.CanBeEquipped).ToDictionary(m => m.Name);
        return new Unit(dto.UnitName, dto.IsFront, dto.Names.Select(name => selector[name]).ToList());
    }
}
public record struct UnitDto(string UnitName, bool IsFront, string[] Names);

internal class MemoriaUtil
{
    internal static Stat[] StatsFromRaw(string trimed)
    {
        if (trimed.EndsWith("ライトパワー")) return new[] { Stat.Atk, Stat.LightPower };
        else if (trimed.EndsWith("ライトガード")) return new[] { Stat.Def, Stat.LightGuard };
        else if (trimed.EndsWith("Sp.ライトパワー")) return new[] { Stat.SpAtk, Stat.LightPower };
        else if (trimed.EndsWith("Sp.ライトガード")) return new[] { Stat.SpDef, Stat.LightGuard };

        else if (trimed.EndsWith("ダークパワー")) return new[] { Stat.Atk, Stat.DarkPower };
        else if (trimed.EndsWith("ダークガード")) return new[] { Stat.Def, Stat.DarkGuard };
        else if (trimed.EndsWith("Sp.ダークパワー")) return new[] { Stat.SpAtk, Stat.DarkPower };
        else if (trimed.EndsWith("Sp.ダークガード")) return new[] { Stat.SpDef, Stat.DarkGuard };

        else if (trimed.EndsWith("ウォーターパワー")) return new[] { Stat.Atk, Stat.WaterPower };
        else if (trimed.EndsWith("ウォーターガード")) return new[] { Stat.Def, Stat.WaterGuard };
        else if (trimed.EndsWith("Sp.ウォーターパワー")) return new[] { Stat.SpAtk, Stat.WaterPower };
        else if (trimed.EndsWith("Sp.ウォーターガード")) return new[] { Stat.SpDef, Stat.WaterGuard };

        else if (trimed.EndsWith("ウィンドパワー")) return new[] { Stat.Atk, Stat.WindPower };
        else if (trimed.EndsWith("ウィンドガード")) return new[] { Stat.Def, Stat.WindGuard };
        else if (trimed.EndsWith("Sp.ウィンドパワー")) return new[] { Stat.SpAtk, Stat.WindPower };
        else if (trimed.EndsWith("Sp.ウィンドガード")) return new[] { Stat.SpDef, Stat.WindGuard };

        else if (trimed.EndsWith("マイト")) return new[] { Stat.Atk, Stat.Def };
        else if (trimed.EndsWith("Sp.マイト")) return new[] { Stat.SpAtk, Stat.SpDef };

        else if (trimed.EndsWith("ディファー")) return new[] { Stat.SpAtk, Stat.Def };
        else if (trimed.EndsWith("Sp.ディファー")) return new[] { Stat.Atk, Stat.SpDef };

        else if (trimed.EndsWith("Wパワー")) return new[] { Stat.Atk, Stat.SpAtk };
        else if (trimed.EndsWith("Wガード")) return new[] { Stat.Def, Stat.SpDef };

        else if (trimed.EndsWith("Sp.パワー")) return new[] { Stat.SpAtk };
        else if (trimed.EndsWith("パワー")) return new[] { Stat.Atk };

        else if (trimed.EndsWith("Sp.ガード")) return new[] { Stat.SpDef };
        else if (trimed.EndsWith("ガード")) return new[] { Stat.Def };

        else return new Stat[] { };
    }

    internal static string StatsToString(Stat[] Stats)
    {
        return Stats switch
        {
            [Stat.Atk, Stat.LightPower] => "ライトパワー",
            [Stat.Def, Stat.LightGuard] => "ライトガード",
            [Stat.SpAtk, Stat.LightPower] => "Sp.ライトパワー",
            [Stat.SpDef, Stat.LightGuard] => "Sp.ライトガード",

            [Stat.Atk, Stat.DarkPower] => "ダークパワー",
            [Stat.Def, Stat.DarkGuard] => "ダークガード",
            [Stat.SpAtk, Stat.DarkPower] => "Sp.ダークパワー",
            [Stat.SpDef, Stat.DarkGuard] => "Sp.ダークガード",

            [Stat.Atk, Stat.WaterPower] => "ウォーターパワー",
            [Stat.Def, Stat.WaterGuard] => "ウォーターガード",
            [Stat.SpAtk, Stat.WaterPower] => "Sp.ウォーターパワー",
            [Stat.SpDef, Stat.WaterGuard] => "Sp.ウォーターガード",

            [Stat.Atk, Stat.WindPower] => "ウィンドパワー",
            [Stat.Def, Stat.WindGuard] => "ウィンドガード",
            [Stat.SpAtk, Stat.WindPower] => "Sp.ウィンドパワー",
            [Stat.SpDef, Stat.WindGuard] => "Sp.ウィンドガード",

            [Stat.Atk, Stat.Def] => "マイト",
            [Stat.SpAtk, Stat.SpDef] => "Sp.マイト",

            [Stat.SpAtk, Stat.Def] => "ディファー",
            [Stat.Atk, Stat.SpDef] => "Sp.ディファー",

            [Stat.Atk, Stat.SpAtk] => "Wパワー",
            [Stat.Def, Stat.SpDef] => "Wガード",

            [Stat.SpAtk] => "Sp.パワー",
            [Stat.Atk] => "パワー",

            [Stat.SpDef] => "Sp.ガード",
            [Stat.Def] => "ガード",

            [] => string.Empty,
            _ => throw new NotImplementedException()
        };
    }
}

public enum Stat
{
    Atk, Def, SpAtk, SpDef,
    DarkPower, LightPower, WaterPower, WindPower, FirePower,
    DarkGuard, LightGuard, WaterGuard, WindGuard, FireGuard,
    Life
}

public enum Range
{
    A, B, C, D, E
}

public enum Level
{
    One,
    Two,
    Three,
    ThreePlus,
    Four,
    FourPlus,
    Five,
    LG,
}

public enum PrefixEffect
{
    Non,
    Charge,
    Heal,
    Water,
    Wind,
    Fire,
    WaterSpread,
    WindSpread,
    FireSpread
}

public abstract record MemoriaSkill
{
    private static Regex regex = new Regex(@"(?<body>.+?)(?<range>[A|B|C|D|E]) (?<level>.+)", RegexOptions.Compiled);

    public static implicit operator MemoriaSkill(string skill)
    {
        var matches = regex.Matches(skill);

        var range = matches[0].Groups["range"].Value switch
        {
            "A" => Range.A,
            "B" => Range.B,
            "C" => Range.C,
            "D" => Range.D,
            "E" => Range.E,
            _ => throw new ArgumentOutOfRangeException()
        };

        var level = matches[0].Groups["level"].Value switch
        {
            "Ⅰ" => Level.One,
            "Ⅱ" => Level.Two,
            "Ⅲ" => Level.Three,
            "Ⅲ+" => Level.ThreePlus,
            "Ⅳ" => Level.Four,
            "Ⅳ+" => Level.FourPlus,
            "Ⅴ" => Level.Five,
            "LG" => Level.LG,
            _ => throw new ArgumentOutOfRangeException()
        };

        PrefixEffect intoElementalEffect(string body)
        {
            if (body.StartsWith("チャージ"))
            {
                return PrefixEffect.Charge;
            }
            else if (body.StartsWith("ヒール"))
            {
                return PrefixEffect.Heal;
            }
            else if (body.StartsWith("水："))
            {
                return PrefixEffect.Water;
            }
            else if (body.StartsWith("風："))
            {
                return PrefixEffect.Wind;
            }
            else if (body.StartsWith("火："))
            {
                return PrefixEffect.Fire;
            }
            else if (body.StartsWith("水拡："))
            {
                return PrefixEffect.WaterSpread;
            }
            else if (body.StartsWith("風拡："))
            {
                return PrefixEffect.WindSpread;
            }
            else if (body.StartsWith("火拡："))
            {
                return PrefixEffect.FireSpread;
            }
            else
            {
                return PrefixEffect.Non;
            }
        }

        var body = matches[0].Groups["body"].Value;

        if (body.EndsWith("ストライク"))
        {
            return new Strike(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("ストライク", string.Empty)), intoElementalEffect(body)));
        }
        else if (body.EndsWith("ブレイク"))
        {
            return new Break(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("ブレイク", string.Empty)), intoElementalEffect(body)));
        }
        else if (body.EndsWith("スマッシュ"))
        {
            return new Smash(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("スマッシュ", string.Empty)), intoElementalEffect(body)));
        }
        else if (body.EndsWith("バースト"))
        {
            return new Burst(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("バースト", string.Empty)), intoElementalEffect(body)));
        }
        else if (body.EndsWith("アシスト"))
        {
            return new Assist(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("アシスト", string.Empty)), intoElementalEffect(body)));
        }
        else if (body.EndsWith("フォール"))
        {
            return new Fall(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("フォール", string.Empty)), intoElementalEffect(body)));
        }
        else if (body.EndsWith("ヒール"))
        {
            return new Heal(new MemoriaData(range, level, MemoriaUtil.StatsFromRaw(body.Replace("ヒール", string.Empty)), intoElementalEffect(body)));
        }

        throw new ArgumentOutOfRangeException($"{body}");
    }

    public virtual string? Name { get; }
}

public static class Meta
{
    public static string GetName(this PrefixEffect eff) => eff switch
    {
        PrefixEffect.Non => string.Empty,
        PrefixEffect.Charge => "チャージ",
        PrefixEffect.Heal => "ヒール",
        PrefixEffect.Water => "水: ",
        PrefixEffect.Wind => "風: ",
        PrefixEffect.Fire => "火: ",
        PrefixEffect.WaterSpread => "水拡: ",
        PrefixEffect.WindSpread => "風拡: ",
        PrefixEffect.FireSpread => "火拡: ",
        _ => throw new NotImplementedException(),
    };
    public static string GetName(this Level level) => level switch
    {
        Level.One => "Ⅰ",
        Level.Two => "Ⅱ",
        Level.Three => "Ⅲ",
        Level.ThreePlus => "Ⅲ+",
        Level.Four => "Ⅳ",
        Level.FourPlus => "Ⅳ+",
        Level.Five => "Ⅴ",
        Level.LG => "LG",
    };
    public static string GetName(this Passive pa) => pa switch
    {
        Passive.Attack => "攻",
        Passive.Assist => "援",
        Passive.Heal => "回",
        Passive.SubAssist => "副援",
        Passive.Command => "コ",
    };
    public static string GetName(this PassiveKind pk) => pk switch
    {
        PassiveKind.DamageUp => "ダメージUP",
        PassiveKind.AssistUp => "支援UP",
        PassiveKind.HealUp => "回復UP",
        PassiveKind.MpConsumptionDown => "MP消費DOWN",
    };
}

public record struct MemoriaData(Range Range, Level Level, Stat[] Stats, PrefixEffect Prefix = PrefixEffect.Non)
{
    public MemoriaData(Range Range, Level Level, PrefixEffect prefix = PrefixEffect.Non) : this(Range, Level, new Stat[] { }, prefix)
    {
    }

    public string Name(string category) => $"{Prefix.GetName()}{MemoriaUtil.StatsToString(Stats)}{category}{Range} {Level.GetName()}";
}

public record Strike(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("ストライク");
}

public record Break(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("ブレイク");
}
public record Smash(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("スマッシュ");
}
public record Burst(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("バースト");
}
public record Assist(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("アシスト");
}
public record Fall(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("フォール");
}
public record Heal(MemoriaData Data) : MemoriaSkill
{
    public override string Name => Data.Name("ヒール");
}

public record struct Skill(MemoriaSkill MemoriaSkill, string Description)
{
    public string? Name => MemoriaSkill.Name;
}

public record SupportSkills(SupportSkill[] Skills, Level Level)
{
    private static Regex regex = new Regex(@"(?<kind>.+?):(?<body>.+)", RegexOptions.Compiled);
    private static Regex compound = new Regex(@"(?<first>.+?)/(?<second>.+) (?<level>.+)", RegexOptions.Compiled);
    private static Regex single = new Regex(@"(?<effect>.+?) (?<level>.+)", RegexOptions.Compiled);

    public string Name
    {
        get
        {
            if (Skills.All(skill => skill.PassivePrefix == Skills.First().PassivePrefix))
            {
                var name = string.Join("/", Skills.Select(skill => skill.Name));
                return $"{Skills.First().PassivePrefix.GetName()}: {name} {Level.GetName()}";
            }
            else
            {
                var name = string.Join("/", Skills.Select(skill => skill.FullName));
                return $"{name} {Level.GetName()}";
            }
        }
    }

    public static implicit operator SupportSkills(string text)
    {
        static Level LevelFromRaw(string raw) => raw switch
        {
            "Ⅰ" => Level.One,
            "Ⅱ" => Level.Two,
            "Ⅲ" => Level.Three,
            "Ⅲ+" => Level.ThreePlus,
            "Ⅳ" => Level.Four,
            "Ⅳ+" => Level.FourPlus,
            "Ⅴ" => Level.Five,
            "LG" => Level.LG,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (text == "回:回復UP/副援:支援UP Ⅱ") return new(new SupportSkill[] { new PassiveSupportSkill(Passive.Heal, PassiveKind.HealUp), new PassiveSupportSkill(Passive.SubAssist, PassiveKind.AssistUp) }, Level.Two);
        else
        {
            var matches = regex.Matches(text);

            switch (matches[0].Groups["kind"].Value)
            {
                case "攻":
                    {
                        var effects = matches[0].Groups["body"].Value;
                        if (effects.Contains('/'))
                        {
                            matches = compound.Matches(effects);
                            var first = SupportSkill.FromStr(Passive.Attack, matches[0].Groups["first"].Value);
                            var second = SupportSkill.FromStr(Passive.Attack, matches[0].Groups["second"].Value);
                            var level = LevelFromRaw(matches[0].Groups["level"].Value);
                            return new(new SupportSkill[] { first, second }, level);
                        }
                        else
                        {
                            matches = single.Matches(effects);
                            var effect = SupportSkill.FromStr(Passive.Attack, matches[0].Groups["effect"].Value);
                            var level = LevelFromRaw(matches[0].Groups["level"].Value);
                            return new(new SupportSkill[] { effect }, level);
                        }
                    }
                case "援":
                    {
                        var effects = matches[0].Groups["body"].Value;
                        if (effects.Contains('/'))
                        {
                            matches = compound.Matches(effects);
                            var first = SupportSkill.FromStr(Passive.Assist, matches[0].Groups["first"].Value);
                            var second = SupportSkill.FromStr(Passive.Assist, matches[0].Groups["second"].Value);
                            var level = LevelFromRaw(matches[0].Groups["level"].Value);
                            return new(new SupportSkill[] { first, second }, level);
                        }
                        else
                        {
                            matches = single.Matches(effects);
                            var effect = SupportSkill.FromStr(Passive.Assist, matches[0].Groups["effect"].Value);
                            var level = LevelFromRaw(matches[0].Groups["level"].Value);
                            return new(new SupportSkill[] { effect }, level);
                        }
                    }
                case "回":
                    {
                        var effects = matches[0].Groups["body"].Value;
                        if (effects.Contains('/'))
                        {
                            matches = compound.Matches(effects);
                            var first = SupportSkill.FromStr(Passive.Heal, matches[0].Groups["first"].Value);
                            var second = SupportSkill.FromStr(Passive.Heal, matches[0].Groups["second"].Value);
                            var level = LevelFromRaw(matches[0].Groups["level"].Value);
                            return new(new SupportSkill[] { first, second }, level);
                        }
                        else
                        {
                            matches = single.Matches(effects);
                            var effect = SupportSkill.FromStr(Passive.Heal, matches[0].Groups["effect"].Value);
                            var level = LevelFromRaw(matches[0].Groups["level"].Value);
                            return new(new SupportSkill[] { effect }, level);
                        }
                    }
                case "コ":
                    {// 現状、コマンド実行時サポート効果はこれしかない
                        return new(new SupportSkill[] { new PassiveSupportSkill(Passive.Command, PassiveKind.MpConsumptionDown) }, Level.Two);
                    }
                default: throw new ArgumentException();
            }

        }
    }
}

public enum PassiveKind
{
    DamageUp,
    AssistUp,
    HealUp,
    MpConsumptionDown,
}

public enum Passive
{
    Attack,
    Assist,
    Heal,
    SubAssist,
    Command,
}

public enum UpDown
{
    UP,
    DOWN,
}

public abstract record SupportSkill
{
    public static SupportSkill FromStr(Passive passive, string text)
    {
        if (text.StartsWith("ダメージUP")) return new PassiveSupportSkill(passive, PassiveKind.DamageUp);
        else if (text.StartsWith("支援UP")) return new PassiveSupportSkill(passive, PassiveKind.AssistUp);
        else if (text.StartsWith("回復UP")) return new PassiveSupportSkill(passive, PassiveKind.HealUp);
        else if (text.StartsWith("MP消費DOWN")) return new PassiveSupportSkill(passive, PassiveKind.MpConsumptionDown);
        else return new StatSupportSkill(passive, MemoriaUtil.StatsFromRaw(text.Replace(text.EndsWith("UP") ? "UP" : "DOWN", string.Empty)), text.EndsWith("UP") ? UpDown.UP : UpDown.DOWN);
    }

    public abstract string? Name { get; }
    public abstract string? FullName { get; }
    public abstract Passive PassivePrefix { get; }
}

public record StatSupportSkill(Passive Passive, Stat[] Stats, UpDown Type) : SupportSkill
{
    public override string? Name => $"{MemoriaUtil.StatsToString(Stats)}{Type}";
    public override string? FullName => $"{Passive.GetName()}:{MemoriaUtil.StatsToString(Stats)}{Type}";
    public override Passive PassivePrefix => Passive;

    public static implicit operator StatSupportSkill((Passive, Stat[], UpDown) from) => new(from.Item1, from.Item2, from.Item3);
}

public record PassiveSupportSkill(Passive Passive, PassiveKind PassiveKind) : SupportSkill
{
    public override string? Name => $"{PassiveKind.GetName()}";
    public override string? FullName => $"{Passive.GetName()}:{PassiveKind.GetName()}";
    public override Passive PassivePrefix => Passive;

    public static implicit operator PassiveSupportSkill((Passive, PassiveKind) from) => new(from.Item1, from.Item2);
}

public record struct Support(SupportSkills SupportSkill, string Description)
{
    public string? Name => SupportSkill.Name;
}

public record struct Memoria(
    string Name,
    string Kind,
    string Element,
    int[] Status,
    int Cost,
    Skill Skill,
    Support Support)
{
    public Uri Uri => new($"ms-appx:///Assets/memoria/{Name}.jpg");
    public string Path = $"/Assets/memoria/{Name}.jpg";

    public static Memoria[] List => new Memoria[]
    {
        new(
            "覚醒の兆し",
            "支援",
            "水",
            new[]
            {
                1669,
                4488,
                1670,
                3494
            },
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "覚醒の兆し",
            "特殊単体",
            "水",
            new[]
            {
                1669,
                4488,
                1670,
                3494
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "騒がし乙女の凱旋",
            "通常範囲",
            "水",
            new[]
            {
                4500,
                1652,
                3470,
                1656
            },
            19,
            new Skill(
                "ウォーターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "騒がし乙女の凱旋",
            "妨害",
            "水",
            new[]
            {
                4500,
                1652,
                3470,
                1656
            },
            19,
            new Skill(
                "ウォーターガードフォールB Ⅱ",
                "敵1～2体のDEFと水属性防御力をダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "あなたとおそろい",
            "特殊範囲",
            "水",
            new[]
            {
                1661,
                4504,
                1657,
                3481
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "あなたとおそろい",
            "回復",
            "水",
            new[]
            {
                1661,
                4504,
                1657,
                3481
            },
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "ワンマンアーミー (星8)",
            "支援",
            "風",
            new[]
            {
                4127,
                3182,
                3177,
                2717
            },
            18,
            new Skill(
                "WパワーアシストD LG",
                "味方2体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:支援UP/パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。さらに、支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "縄跳びトレーニング",
            "支援",
            "水",
            new[]
            {
                4520,
                1705,
                3505,
                1700
            },
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "縄跳びトレーニング",
            "通常範囲",
            "水",
            new[]
            {
                4520,
                1705,
                3505,
                1700
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "暮れなずむ空",
            "通常範囲",
            "風",
            new[]
            {
                4534,
                1687,
                3534,
                1682
            },
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "暮れなずむ空",
            "妨害",
            "風",
            new[]
            {
                4534,
                1687,
                3534,
                1682
            },
            19,
            new Skill(
                "チャージガードフォールB Ⅱ",
                "敵1～2体のDEFをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "かめ、のち、えがお",
            "回復",
            "風",
            new[]
            {
                1649,
                1645,
                4503,
                3491
            },
            19,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "かめ、のち、えがお",
            "特殊範囲",
            "風",
            new[]
            {
                1649,
                1645,
                4503,
                3491
            },
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "ひめひめコールお願いっ！",
            "妨害",
            "風",
            new[]
            {
                1668,
                4466,
                1662,
                3477
            },
            19,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "ひめひめコールお願いっ！",
            "特殊範囲",
            "風",
            new[]
            {
                1668,
                4466,
                1662,
                3477
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "カワウソづくし",
            "支援",
            "水",
            new[]
            {
                4496,
                1668,
                3473,
                1683
            },
            19,
            new Skill(
                "水：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "カワウソづくし",
            "通常範囲",
            "水",
            new[]
            {
                4496,
                1668,
                3473,
                1683
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-不動劔と至宝-",
            "特殊範囲",
            "水",
            new[]
            {
                1650,
                4494,
                1673,
                3473
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-不動劔と至宝-",
            "妨害",
            "水",
            new[]
            {
                1650,
                4494,
                1673,
                3473
            },
            19,
            new Skill(
                "Sp.ウォーターガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと水属性防御力をダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-ねんねこぐろっぴ-",
            "通常範囲",
            "風",
            new[]
            {
                4474,
                1663,
                3499,
                1657
            },
            19,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-ねんねこぐろっぴ-",
            "支援",
            "風",
            new[]
            {
                4474,
                1663,
                3499,
                1657
            },
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "CHARMという兵器",
            "支援",
            "風",
            new[]
            {
                4506,
                1679,
                3492,
                1669
            },
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "CHARMという兵器",
            "通常範囲",
            "風",
            new[]
            {
                4506,
                1679,
                3492,
                1669
            },
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ワーオ！　エキサイティン！！",
            "妨害",
            "風",
            new[]
            {
                4472,
                1657,
                3502,
                1667
            },
            19,
            new Skill(
                "ウィンドガードフォールC Ⅲ",
                "敵1～3体のDEFと風属性防御力をダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ワーオ！　エキサイティン！！",
            "通常範囲",
            "風",
            new[]
            {
                4472,
                1657,
                3502,
                1667
            },
            19,
            new Skill(
                "ウィンドパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "束の間の休息",
            "回復",
            "風",
            new[]
            {
                1665,
                1683,
                3490,
                4493
            },
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "束の間の休息",
            "特殊範囲",
            "風",
            new[]
            {
                1665,
                1683,
                3490,
                4493
            },
            19,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "作戦会議です！",
            "支援",
            "風",
            new[]
            {
                1462,
                2918,
                1493,
                2657
            },
            17,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "作戦会議です！",
            "特殊単体",
            "風",
            new[]
            {
                1462,
                2918,
                1493,
                2657
            },
            17,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "予想外の事態",
            "通常範囲",
            "風",
            new[]
            {
                4497,
                1671,
                3481,
                1661
            },
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "予想外の事態",
            "回復",
            "風",
            new[]
            {
                4497,
                1671,
                3481,
                1661
            },
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "優雅なティータイム",
            "特殊範囲",
            "風",
            new[]
            {
                1681,
                4476,
                1661,
                3475
            },
            19,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "優雅なティータイム",
            "支援",
            "風",
            new[]
            {
                1681,
                4476,
                1661,
                3475
            },
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。"
            ),
            new Support(
                "援:WガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "西住流の誇り",
            "特殊範囲",
            "風",
            new[]
            {
                1681,
                4503,
                1666,
                3473
            },
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "西住流の誇り",
            "妨害",
            "風",
            new[]
            {
                1681,
                4503,
                1666,
                3473
            },
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "形勢逆転！！",
            "通常範囲",
            "風",
            new[]
            {
                4505,
                1667,
                3504,
                1647
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "形勢逆転！！",
            "支援",
            "風",
            new[]
            {
                4505,
                1667,
                3504,
                1647
            },
            19,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "ワイン色の思い出",
            "特殊範囲",
            "風",
            new[]
            {
                1669,
                4500,
                1665,
                3466
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ワイン色の思い出",
            "妨害",
            "風",
            new[]
            {
                1669,
                4500,
                1665,
                3466
            },
            19,
            new Skill(
                "風：WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:WガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFとSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "月夜に吠える天狼",
            "妨害",
            "風",
            new[]
            {
                4160,
                3644,
                2736,
                2701
            },
            21,
            new Skill(
                "WパワーフォールE LG",
                "敵2～3体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "雲間から差し込む光",
            "特殊単体",
            "水",
            new[]
            {
                1672,
                4500,
                1655,
                3479
            },
            19,
            new Skill(
                "Sp.ウォーターガードバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "雲間から差し込む光",
            "回復",
            "水",
            new[]
            {
                1672,
                4500,
                1655,
                3479
            },
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "好機を待つのじゃ",
            "支援",
            "水",
            new[]
            {
                4474,
                1672,
                3492,
                1644
            },
            19,
            new Skill(
                "水：パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "好機を待つのじゃ",
            "通常範囲",
            "水",
            new[]
            {
                4474,
                1672,
                3492,
                1644
            },
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ひめひめ華麗に参上！",
            "特殊範囲",
            "水",
            new[]
            {
                1668,
                4480,
                1654,
                3477
            },
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ひめひめ華麗に参上！",
            "妨害",
            "水",
            new[]
            {
                1668,
                4480,
                1654,
                3477
            },
            19,
            new Skill(
                "水拡：Sp.パワーフォールB Ⅳ",
                "敵1～2体のSp.ATKを特大ダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体のSp.ATKを特大ダウンさせる。※..."
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-紅葉の帳-",
            "支援",
            "水",
            new[]
            {
                1668,
                4481,
                1678,
                3497
            },
            19,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-紅葉の帳-",
            "特殊範囲",
            "水",
            new[]
            {
                1668,
                4481,
                1678,
                3497
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-紅葉も頬も色づいて-",
            "回復",
            "水",
            new[]
            {
                1650,
                1679,
                4498,
                3482
            },
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-紅葉も頬も色づいて-",
            "通常範囲",
            "水",
            new[]
            {
                1650,
                1679,
                4498,
                3482
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-秋月夜の彩り-",
            "特殊範囲",
            "水",
            new[]
            {
                1658,
                4467,
                1670,
                3492
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-秋月夜の彩り-",
            "支援",
            "水",
            new[]
            {
                1658,
                4467,
                1670,
                3492
            },
            19,
            new Skill(
                "Sp.ウォーターパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-秋の木漏れ日-",
            "通常範囲",
            "水",
            new[]
            {
                4501,
                1681,
                3475,
                1667
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-秋の木漏れ日-",
            "回復",
            "水",
            new[]
            {
                4501,
                1681,
                3475,
                1667
            },
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "はじらいマミー",
            "通常範囲",
            "水",
            new[]
            {
                4357,
                2093,
                3536,
                2079
            },
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "どきどきデビル",
            "支援",
            "水",
            new[]
            {
                4471,
                1679,
                3480,
                1677
            },
            19,
            new Skill(
                "Sp.ディファーアシストC Ⅳ",
                "味方1～3体のATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "どきどきデビル",
            "通常範囲",
            "水",
            new[]
            {
                4471,
                1679,
                3480,
                1677
            },
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "いたずらゴースト",
            "妨害",
            "水",
            new[]
            {
                1670,
                4493,
                1650,
                3469
            },
            19,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "いたずらゴースト",
            "特殊範囲",
            "水",
            new[]
            {
                1670,
                4493,
                1650,
                3469
            },
            19,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "夜空に響く勝利の歌",
            "通常範囲",
            "水",
            new[]
            {
                4478,
                1655,
                3482,
                1671
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "夜空に響く勝利の歌",
            "支援",
            "水",
            new[]
            {
                4478,
                1655,
                3482,
                1671
            },
            19,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "陽だまりシュッツエンゲル",
            "回復",
            "水",
            new[]
            {
                1672,
                4466,
                1657,
                3469
            },
            19,
            new Skill(
                "Sp.ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKと水属性攻撃力を小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "陽だまりシュッツエンゲル",
            "特殊範囲",
            "水",
            new[]
            {
                1672,
                4466,
                1657,
                3469
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-くるくおーらんたん-",
            "妨害",
            "水",
            new[]
            {
                4473,
                1661,
                3479,
                1650
            },
            19,
            new Skill(
                "ウォーターパワーフォールC Ⅲ",
                "敵1～3体のATKと水属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-くるくおーらんたん-",
            "通常範囲",
            "水",
            new[]
            {
                4473,
                1661,
                3479,
                1650
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-こころにいたずら-",
            "支援",
            "水",
            new[]
            {
                1645,
                4495,
                1680,
                3472
            },
            19,
            new Skill(
                "水：Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-こころにいたずら-",
            "特殊範囲",
            "水",
            new[]
            {
                1645,
                4495,
                1680,
                3472
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-Early Trick-",
            "特殊範囲",
            "水",
            new[]
            {
                1660,
                4502,
                1658,
                3491
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-Early Trick-",
            "支援",
            "水",
            new[]
            {
                1660,
                4502,
                1658,
                3491
            },
            19,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-ジャックコーデ-",
            "通常範囲",
            "水",
            new[]
            {
                4498,
                1681,
                3494,
                1655
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-ジャックコーデ-",
            "回復",
            "水",
            new[]
            {
                4498,
                1681,
                3494,
                1655
            },
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "秋空ピクニック",
            "特殊範囲",
            "水",
            new[]
            {
                1679,
                4489,
                1672,
                3471
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "秋空ピクニック",
            "回復",
            "水",
            new[]
            {
                1679,
                4489,
                1672,
                3471
            },
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "ソーイングマスター姫歌",
            "通常単体",
            "水",
            new[]
            {
                4500,
                1659,
                3480,
                1681
            },
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "ソーイングマスター姫歌",
            "支援",
            "水",
            new[]
            {
                4500,
                1659,
                3480,
                1681
            },
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "庭園の護り人",
            "特殊範囲",
            "水",
            new[]
            {
                1660,
                4487,
                1680,
                3499
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "庭園の護り人",
            "妨害",
            "水",
            new[]
            {
                1660,
                4487,
                1680,
                3499
            },
            19,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "ふたりの距離",
            "通常範囲",
            "水",
            new[]
            {
                4467,
                1676,
                3498,
                1646
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "ふたりの距離",
            "回復",
            "水",
            new[]
            {
                4467,
                1676,
                3498,
                1646
            },
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "ダイスキをキャンバスに",
            "支援",
            "水",
            new[]
            {
                4491,
                1651,
                3498,
                1669
            },
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "ダイスキをキャンバスに",
            "通常範囲",
            "水",
            new[]
            {
                4491,
                1651,
                3498,
                1669
            },
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "らんとたづさのかくれんぼ",
            "通常単体",
            "水",
            new[]
            {
                4482,
                1684,
                3471,
                1674
            },
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "らんとたづさのかくれんぼ",
            "回復",
            "水",
            new[]
            {
                4482,
                1684,
                3471,
                1674
            },
            19,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "藍は舞い降りた",
            "特殊範囲",
            "水",
            new[]
            {
                1679,
                4487,
                1657,
                3492
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "藍は舞い降りた",
            "支援",
            "水",
            new[]
            {
                1679,
                4487,
                1657,
                3492
            },
            19,
            new Skill(
                "ディファーアシストC Ⅳ",
                "味方1～3体のSp.ATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "静寂に佇む狩人",
            "妨害",
            "水",
            new[]
            {
                4491,
                1644,
                3477,
                1684
            },
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "静寂に佇む狩人",
            "通常範囲",
            "水",
            new[]
            {
                4491,
                1644,
                3477,
                1684
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-好きなものを一緒に-",
            "回復",
            "水",
            new[]
            {
                2070,
                2071,
                3534,
                4363
            },
            21,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-星空のどうぶつ探し-",
            "支援",
            "水",
            new[]
            {
                2099,
                2064,
                4362,
                3536
            },
            21,
            new Skill(
                "水：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-二人の奏でる夜の歌-",
            "特殊範囲",
            "水",
            new[]
            {
                2063,
                4356,
                2092,
                3543
            },
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-すすきの道しるべ-",
            "通常範囲",
            "水",
            new[]
            {
                4324,
                2087,
                3541,
                2098
            },
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "息を潜めて",
            "支援",
            "水",
            new[]
            {
                1657,
                4486,
                1661,
                3487
            },
            19,
            new Skill(
                "Sp.ウォーターパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "息を潜めて",
            "特殊範囲",
            "水",
            new[]
            {
                1657,
                4486,
                1661,
                3487
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ミッドナイトスティール",
            "通常範囲",
            "水",
            new[]
            {
                4505,
                1666,
                3496,
                1664
            },
            19,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "ミッドナイトスティール",
            "妨害",
            "水",
            new[]
            {
                4505,
                1666,
                3496,
                1664
            },
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "真夜中の極秘作戦",
            "回復",
            "水",
            new[]
            {
                2079,
                2093,
                3559,
                4359
            },
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "誠実なる守護者",
            "通常単体",
            "水",
            new[]
            {
                4155,
                2730,
                3634,
                2716
            },
            18,
            new Skill(
                "パワーストライクA LG",
                "敵1体に通常超特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "一葉ののんびりタイム",
            "妨害",
            "水",
            new[]
            {
                4503,
                1681,
                3466,
                1674
            },
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "一葉ののんびりタイム",
            "通常範囲",
            "水",
            new[]
            {
                4503,
                1681,
                3466,
                1674
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "クリエイターズコラボ-ペアトレ-",
            "回復",
            "水",
            new[]
            {
                2088,
                2088,
                4334,
                3549
            },
            21,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-先輩ふぁいと☆-",
            "支援",
            "水",
            new[]
            {
                3549,
                4355,
                2086,
                2070
            },
            21,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-もっと優しく-",
            "特殊範囲",
            "水",
            new[]
            {
                2079,
                4343,
                2091,
                3542
            },
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-りざるとちぇっく-",
            "通常範囲",
            "水",
            new[]
            {
                4341,
                2095,
                3527,
                2081
            },
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "親愛なるルームメイト",
            "特殊範囲",
            "水",
            new[]
            {
                2089,
                4328,
                2074,
                3557
            },
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "戦場のお色直し",
            "妨害",
            "水",
            new[]
            {
                1650,
                4500,
                1650,
                3485
            },
            19,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと水属性防御力をダウンさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "戦場のお色直し",
            "特殊単体",
            "水",
            new[]
            {
                1650,
                4500,
                1650,
                3485
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "白鳥の姫騎士",
            "通常範囲",
            "水",
            new[]
            {
                4476,
                1670,
                3467,
                1647
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "白鳥の姫騎士",
            "支援",
            "水",
            new[]
            {
                4476,
                1670,
                3467,
                1647
            },
            19,
            new Skill(
                "水拡：パワーアシストB Ⅳ",
                "味方1～2体のATKを特大アップさせる。オーダースキル「水属性効果増加」を発動中は味方2体のATKを特大アップさせる。※..."
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "氷帝",
            "回復",
            "水",
            new[]
            {
                1673,
                1670,
                4490,
                3480
            },
            19,
            new Skill(
                "ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKと水属性攻撃力を小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "氷帝",
            "通常単体",
            "水",
            new[]
            {
                1673,
                1670,
                4490,
                3480
            },
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "その瞳に映るモノ",
            "特殊単体",
            "水",
            new[]
            {
                1668,
                4470,
                1668,
                3498
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "その瞳に映るモノ",
            "妨害",
            "水",
            new[]
            {
                1668,
                4470,
                1668,
                3498
            },
            19,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと水属性防御力をダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "アクロバット・シューター",
            "特殊範囲",
            "水",
            new[]
            {
                1655,
                4468,
                1649,
                3500
            },
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "アクロバット・シューター",
            "回復",
            "水",
            new[]
            {
                1655,
                4468,
                1649,
                3500
            },
            19,
            new Skill(
                "Sp.ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKと水属性攻撃力を小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "天からの強襲",
            "支援",
            "水",
            new[]
            {
                4497,
                1681,
                3470,
                1645
            },
            19,
            new Skill(
                "ウォーターパワーアシストB Ⅱ",
                "味方1～2体のATKと水属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "天からの強襲",
            "通常範囲",
            "水",
            new[]
            {
                4497,
                1681,
                3470,
                1645
            },
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "ナイトガンスリンガー",
            "特殊範囲",
            "光",
            new[]
            {
                2703,
                4156,
                2712,
                3654
            },
            21,
            new Skill(
                "Sp.ガードバーストD LG",
                "敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-夏色日和-",
            "回復",
            "闇",
            new[]
            {
                2075,
                2070,
                3553,
                4327
            },
            21,
            new Skill(
                "Sp.ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと光属性防御力を小アップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "クリエイターズコラボ-打ち上げ花火-",
            "妨害",
            "光",
            new[]
            {
                4329,
                2068,
                3561,
                2084
            },
            21,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-涼やかな響き-",
            "通常範囲",
            "光",
            new[]
            {
                4354,
                2079,
                3526,
                2071
            },
            21,
            new Skill(
                "ライトガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと光属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-納涼かき氷-",
            "特殊範囲",
            "闇",
            new[]
            {
                2085,
                4351,
                2078,
                3562
            },
            21,
            new Skill(
                "Sp.ダークガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと闇属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "飛び出せミリアム！",
            "回復",
            "光",
            new[]
            {
                1676,
                1659,
                4480,
                3503
            },
            19,
            new Skill(
                "ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "飛び出せミリアム！",
            "通常単体",
            "光",
            new[]
            {
                1676,
                1659,
                4480,
                3503
            },
            19,
            new Skill(
                "ライトパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと光属性攻撃力をアップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ポイ越しの笑顔",
            "特殊範囲",
            "闇",
            new[]
            {
                1679,
                4470,
                1647,
                3493
            },
            19,
            new Skill(
                "Sp.ライトパワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと光属性攻撃力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ポイ越しの笑顔",
            "支援",
            "闇",
            new[]
            {
                1679,
                4470,
                1647,
                3493
            },
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "祭囃子と恋の音",
            "通常範囲",
            "闇",
            new[]
            {
                4348,
                2066,
                3551,
                2078
            },
            21,
            new Skill(
                "ライトパワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKと光属性攻撃力を小ダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-想いを込めた歌声-",
            "回復",
            "闇",
            new[]
            {
                2092,
                2082,
                4351,
                3559
            },
            21,
            new Skill(
                "ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと光属性防御力を小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-花咲くハーモニー-",
            "支援",
            "光",
            new[]
            {
                4341,
                2075,
                3524,
                2094
            },
            21,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-躍動の旋律-",
            "通常範囲",
            "闇",
            new[]
            {
                4352,
                2061,
                3561,
                2096
            },
            21,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-きらめきステージ-",
            "特殊範囲",
            "光",
            new[]
            {
                2088,
                4349,
                2083,
                3528
            },
            21,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "夏の海とかき氷",
            "通常範囲",
            "闇",
            new[]
            {
                4478,
                1653,
                3490,
                1670
            },
            19,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "夏の海とかき氷",
            "妨害",
            "闇",
            new[]
            {
                4478,
                1653,
                3490,
                1670
            },
            19,
            new Skill(
                "ダークガードフォールB Ⅱ",
                "敵1～2体のDEFと闇属性防御力をダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "見返り美人",
            "特殊範囲",
            "光",
            new[]
            {
                1657,
                4503,
                1663,
                3494
            },
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "見返り美人",
            "妨害",
            "光",
            new[]
            {
                1657,
                4503,
                1663,
                3494
            },
            19,
            new Skill(
                "Sp.ダークパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと闇属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "新刊1部ください！？",
            "特殊範囲",
            "光",
            new[]
            {
                1256,
                1225,
                2197,
                2054
            },
            16,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "新刊1部ください！？",
            "回復",
            "光",
            new[]
            {
                1256,
                1225,
                2197,
                2054
            },
            16,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "Diverse",
            "特殊範囲",
            "闇",
            new[]
            {
                1649,
                4493,
                1664,
                3489
            },
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "Diverse",
            "回復",
            "闇",
            new[]
            {
                1649,
                4493,
                1664,
                3489
            },
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "Cherish",
            "通常範囲",
            "光",
            new[]
            {
                4494,
                1659,
                3478,
                1669
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "Cherish",
            "妨害",
            "光",
            new[]
            {
                4494,
                1659,
                3478,
                1669
            },
            19,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "激戦の終わりに",
            "特殊範囲",
            "光",
            new[]
            {
                2083,
                4354,
                2071,
                3539
            },
            21,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "極限コンビネーション",
            "回復",
            "闇",
            new[]
            {
                1654,
                1672,
                3466,
                4499
            },
            19,
            new Skill(
                "Sp.ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと光属性防御力を小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "極限コンビネーション",
            "特殊単体",
            "闇",
            new[]
            {
                1654,
                1672,
                3466,
                4499
            },
            19,
            new Skill(
                "Sp.ダークパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "不屈の一太刀",
            "通常範囲",
            "光",
            new[]
            {
                4474,
                1658,
                3501,
                1665
            },
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "不屈の一太刀",
            "支援",
            "光",
            new[]
            {
                4474,
                1658,
                3501,
                1665
            },
            19,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-らぶらぶぴーす-",
            "支援",
            "光",
            new[]
            {
                4334,
                2095,
                3539,
                2080
            },
            21,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-ひまわりとんだよ-",
            "妨害",
            "闇",
            new[]
            {
                4335,
                2076,
                3528,
                2067
            },
            21,
            new Skill(
                "ライトパワーフォールB Ⅱ",
                "敵1～2体のATKと光属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-レディーティータイム-",
            "特殊範囲",
            "光",
            new[]
            {
                2074,
                4351,
                2089,
                3560
            },
            21,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クリエイターズコラボ-月に顔をそむけて-",
            "通常範囲",
            "闇",
            new[]
            {
                4348,
                2078,
                3528,
                2089
            },
            21,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "バトル・デプロイメント",
            "特殊単体",
            "闇",
            new[]
            {
                1673,
                4487,
                1646,
                3494
            },
            19,
            new Skill(
                "Sp.ダークパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "バトル・デプロイメント",
            "支援",
            "闇",
            new[]
            {
                1673,
                4487,
                1646,
                3494
            },
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "淀みを蹴って",
            "特殊範囲",
            "闇",
            new[]
            {
                1649,
                4467,
                1667,
                3506
            },
            19,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "淀みを蹴って",
            "妨害",
            "闇",
            new[]
            {
                1649,
                4467,
                1667,
                3506
            },
            19,
            new Skill(
                "Sp.ライトパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと光属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "アンブッシュ",
            "通常範囲",
            "闇",
            new[]
            {
                4490,
                1647,
                3480,
                1682
            },
            19,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "アンブッシュ",
            "回復",
            "闇",
            new[]
            {
                4490,
                1647,
                3480,
                1682
            },
            19,
            new Skill(
                "ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと光属性防御力を小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "晴れときどきサンオイル",
            "通常範囲",
            "光",
            new[]
            {
                4486,
                1682,
                3506,
                1670
            },
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "晴れときどきサンオイル",
            "回復",
            "光",
            new[]
            {
                4486,
                1682,
                3506,
                1670
            },
            19,
            new Skill(
                "ダークガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "楽しいを探しに行こう！",
            "通常範囲",
            "光",
            new[]
            {
                4475,
                1647,
                3467,
                1653
            },
            19,
            new Skill(
                "ダークパワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKと闇属性攻撃力を小ダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。"
            )
        ),
        new(
            "楽しいを探しに行こう！",
            "妨害",
            "光",
            new[]
            {
                4475,
                1647,
                3467,
                1653
            },
            19,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "祝1.5周年りりふぇす!!",
            "支援",
            "闇",
            new[]
            {
                1838,
                1892,
                1251,
                1273
            },
            16,
            new Skill(
                "WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:WパワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ダイビング・アタッカー",
            "支援",
            "光",
            new[]
            {
                1650,
                4486,
                1667,
                3500
            },
            19,
            new Skill(
                "Sp.ダークガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと闇属性防御力をアップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "ダイビング・アタッカー",
            "特殊範囲",
            "光",
            new[]
            {
                1650,
                4486,
                1667,
                3500
            },
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ウッドクラフトに挑戦",
            "通常範囲",
            "闇",
            new[]
            {
                4338,
                2073,
                3523,
                2097
            },
            21,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "炊事は任せた！",
            "特殊範囲",
            "闇",
            new[]
            {
                2072,
                4357,
                2091,
                3537
            },
            21,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "面目躍如のサバイバル",
            "回復",
            "光",
            new[]
            {
                1678,
                1656,
                4469,
                3488
            },
            19,
            new Skill(
                "ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "面目躍如のサバイバル",
            "通常単体",
            "光",
            new[]
            {
                1678,
                1656,
                4469,
                3488
            },
            19,
            new Skill(
                "ライトパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "殲滅のシルバーバレット",
            "妨害",
            "光",
            new[]
            {
                4473,
                1680,
                3471,
                1647
            },
            19,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "殲滅のシルバーバレット",
            "通常範囲",
            "光",
            new[]
            {
                4473,
                1680,
                3471,
                1647
            },
            19,
            new Skill(
                "ライトガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと光属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "やめられない刺激",
            "妨害",
            "闇",
            new[]
            {
                4504,
                1676,
                3487,
                1647
            },
            19,
            new Skill(
                "ライトパワーフォールB Ⅱ",
                "敵1～2体のATKと光属性攻撃力をダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "やめられない刺激",
            "通常範囲",
            "闇",
            new[]
            {
                4504,
                1676,
                3487,
                1647
            },
            19,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "光の注ぐ夜",
            "回復",
            "光",
            new[]
            {
                1659,
                1660,
                3485,
                4471
            },
            19,
            new Skill(
                "Sp.ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと闇属性防御力を小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "光の注ぐ夜",
            "特殊範囲",
            "光",
            new[]
            {
                1659,
                1660,
                3485,
                4471
            },
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ゼロ距離のしあわせ",
            "特殊範囲",
            "闇",
            new[]
            {
                1650,
                4483,
                1650,
                3501
            },
            19,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "ゼロ距離のしあわせ",
            "支援",
            "闇",
            new[]
            {
                1650,
                4483,
                1650,
                3501
            },
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "水も滴るいい乙女",
            "通常範囲",
            "光",
            new[]
            {
                4491,
                1666,
                3492,
                1672
            },
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "水も滴るいい乙女",
            "支援",
            "光",
            new[]
            {
                4491,
                1666,
                3492,
                1672
            },
            19,
            new Skill(
                "ライトパワーアシストB Ⅱ",
                "味方1～2体のATKと光属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "リトル・アークメイジ",
            "妨害",
            "水",
            new[]
            {
                2705,
                2734,
                4126,
                3624
            },
            18,
            new Skill(
                "WガードフォールD LG",
                "敵2体のDEFとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP/ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。さらに、支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "信じる想いを力に変えて",
            "特殊範囲",
            "光",
            new[]
            {
                1678,
                4474,
                1648,
                3478
            },
            19,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "信じる想いを力に変えて",
            "回復",
            "光",
            new[]
            {
                1678,
                4474,
                1648,
                3478
            },
            19,
            new Skill(
                "Sp.ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと闇属性防御力を小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "絆のアルケミートレース",
            "通常範囲",
            "闇",
            new[]
            {
                4483,
                1682,
                3479,
                1659
            },
            19,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "絆のアルケミートレース",
            "妨害",
            "闇",
            new[]
            {
                4483,
                1682,
                3479,
                1659
            },
            19,
            new Skill(
                "ダークガードフォールB Ⅱ",
                "敵1～2体のDEFと闇属性防御力をダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ピュリファイ・ラプラス",
            "支援",
            "闇",
            new[]
            {
                1662,
                4496,
                1647,
                3491
            },
            19,
            new Skill(
                "Sp.ダークパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと闇属性攻撃力をアップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ピュリファイ・ラプラス",
            "特殊範囲",
            "闇",
            new[]
            {
                1662,
                4496,
                1647,
                3491
            },
            19,
            new Skill(
                "Sp.ダークガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと闇属性防御力を小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ヘイムスクリングラ・シスターズ",
            "通常範囲",
            "風",
            new[]
            {
                4504,
                1679,
                3504,
                1650
            },
            19,
            new Skill(
                "WパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "ヘイムスクリングラ・シスターズ",
            "支援",
            "風",
            new[]
            {
                4504,
                1679,
                3504,
                1650
            },
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "黄昏の英雄たち",
            "妨害",
            "風",
            new[]
            {
                1672,
                1678,
                3477,
                4479
            },
            19,
            new Skill(
                "Sp.ガードフォールA Ⅳ",
                "敵1体のSp.DEFを超特大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "黄昏の英雄たち",
            "特殊範囲",
            "風",
            new[]
            {
                1672,
                1678,
                3477,
                4479
            },
            19,
            new Skill(
                "風：スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "お姉様の水難",
            "通常範囲",
            "闇",
            new[]
            {
                4498,
                1656,
                3498,
                1651
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "お姉様の水難",
            "妨害",
            "闇",
            new[]
            {
                4498,
                1656,
                3498,
                1651
            },
            19,
            new Skill(
                "マイトフォールB Ⅲ",
                "敵1～2体のATKとDEFを大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "この勝利が小さな一歩でも",
            "回復",
            "光",
            new[]
            {
                1649,
                1645,
                3503,
                4491
            },
            19,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "この勝利が小さな一歩でも",
            "特殊単体",
            "光",
            new[]
            {
                1649,
                1645,
                3503,
                4491
            },
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "巨影を討つ閃光",
            "妨害",
            "光",
            new[]
            {
                4490,
                1644,
                3484,
                1655
            },
            19,
            new Skill(
                "光：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "巨影を討つ閃光",
            "通常範囲",
            "光",
            new[]
            {
                4490,
                1644,
                3484,
                1655
            },
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "ここから先は通さない",
            "特殊範囲",
            "光",
            new[]
            {
                1674,
                4490,
                1651,
                3505
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ここから先は通さない",
            "支援",
            "光",
            new[]
            {
                1674,
                4490,
                1651,
                3505
            },
            19,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "猛る獅子の剣",
            "通常範囲",
            "闇",
            new[]
            {
                4137,
                2719,
                3657,
                2731
            },
            21,
            new Skill(
                "ガードブレイクD LG",
                "敵2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "クローバー・クラウン",
            "特殊範囲",
            "闇",
            new[]
            {
                1650,
                4494,
                1673,
                3473
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "クローバー・クラウン",
            "回復",
            "闇",
            new[]
            {
                1650,
                4494,
                1673,
                3473
            },
            19,
            new Skill(
                "WパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKとSp.ATKを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "華の休息",
            "妨害",
            "闇",
            new[]
            {
                1650,
                4484,
                1681,
                3473
            },
            19,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "華の休息",
            "特殊単体",
            "闇",
            new[]
            {
                1650,
                4484,
                1681,
                3473
            },
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ビーチでバカンス",
            "回復",
            "闇",
            new[]
            {
                1675,
                1671,
                4481,
                3483
            },
            19,
            new Skill(
                "パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "ビーチでバカンス",
            "通常単体",
            "闇",
            new[]
            {
                1675,
                1671,
                4481,
                3483
            },
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "乙女の非常事態",
            "特殊範囲",
            "闇",
            new[]
            {
                1681,
                4476,
                1661,
                3475
            },
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "乙女の非常事態",
            "支援",
            "闇",
            new[]
            {
                1681,
                4476,
                1661,
                3475
            },
            19,
            new Skill(
                "闇：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "M.V.P.オンステージ",
            "通常範囲",
            "闇",
            new[]
            {
                4503,
                1681,
                3488,
                1651
            },
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "M.V.P.オンステージ",
            "妨害",
            "闇",
            new[]
            {
                4503,
                1681,
                3488,
                1651
            },
            19,
            new Skill(
                "ディファーフォールB Ⅲ",
                "敵1～2体のSp.ATKとDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "勝利のファンファーレ",
            "通常範囲",
            "光",
            new[]
            {
                4505,
                1667,
                3504,
                1647
            },
            19,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "勝利のファンファーレ",
            "回復",
            "光",
            new[]
            {
                4505,
                1667,
                3504,
                1647
            },
            19,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "お手当マイスター",
            "妨害",
            "闇",
            new[]
            {
                1652,
                4485,
                1677,
                3479
            },
            19,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "お手当マイスター",
            "特殊範囲",
            "闇",
            new[]
            {
                1652,
                4485,
                1677,
                3479
            },
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "謳歌のミュージックアワー",
            "支援",
            "光",
            new[]
            {
                4506,
                1679,
                3492,
                1669
            },
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "謳歌のミュージックアワー",
            "通常範囲",
            "光",
            new[]
            {
                4506,
                1679,
                3492,
                1669
            },
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "メイクアップ！",
            "特殊範囲",
            "光",
            new[]
            {
                1650,
                4479,
                1680,
                3489
            },
            19,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "メイクアップ！",
            "回復",
            "光",
            new[]
            {
                1650,
                4479,
                1680,
                3489
            },
            19,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "回遊のススメ",
            "通常範囲",
            "闇",
            new[]
            {
                4487,
                1683,
                3490,
                1671
            },
            19,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "回遊のススメ",
            "妨害",
            "闇",
            new[]
            {
                4487,
                1683,
                3490,
                1671
            },
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "尊みの探求者",
            "妨害",
            "闇",
            new[]
            {
                1669,
                4500,
                1665,
                3466
            },
            19,
            new Skill(
                "闇：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "尊みの探求者",
            "特殊範囲",
            "闇",
            new[]
            {
                1669,
                4500,
                1665,
                3466
            },
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "生徒会のお仕事",
            "特殊範囲",
            "光",
            new[]
            {
                1668,
                4481,
                1678,
                3497
            },
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "生徒会のお仕事",
            "妨害",
            "光",
            new[]
            {
                1668,
                4481,
                1678,
                3497
            },
            19,
            new Skill(
                "光：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "美しき師弟関係",
            "支援",
            "光",
            new[]
            {
                4508,
                1710,
                3534,
                1691
            },
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "美しき師弟関係",
            "通常単体",
            "光",
            new[]
            {
                4508,
                1710,
                3534,
                1691
            },
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "最高のルームメイト",
            "回復",
            "光",
            new[]
            {
                1689,
                1676,
                4028,
                4028
            },
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "最高のルームメイト",
            "特殊単体",
            "光",
            new[]
            {
                1689,
                1676,
                4028,
                4028
            },
            19,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "BZのプロフェッショナル",
            "通常範囲",
            "光",
            new[]
            {
                4501,
                1681,
                3475,
                1667
            },
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "BZのプロフェッショナル",
            "支援",
            "光",
            new[]
            {
                4501,
                1681,
                3475,
                1667
            },
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "もう一度、何度でも",
            "通常単体",
            "闇",
            new[]
            {
                3492,
                1613,
                2978,
                1610
            },
            18,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。"
            )
        ),
        new(
            "もう一度、何度でも",
            "妨害",
            "闇",
            new[]
            {
                3492,
                1613,
                2978,
                1610
            },
            18,
            new Skill(
                "闇：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "相生の水先案内人",
            "通常範囲",
            "闇",
            new[]
            {
                3466,
                1601,
                2945,
                1587
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "相生の水先案内人",
            "回復",
            "闇",
            new[]
            {
                3466,
                1601,
                2945,
                1587
            },
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "心の炎は豪雨で消えず",
            "特殊範囲",
            "闇",
            new[]
            {
                1574,
                3467,
                1583,
                2965
            },
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "心の炎は豪雨で消えず",
            "支援",
            "闇",
            new[]
            {
                1574,
                3467,
                1583,
                2965
            },
            18,
            new Skill(
                "ディファーアシストB Ⅲ",
                "味方1～2体のSp.ATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "昼下がりのラプソディー",
            "特殊範囲",
            "光",
            new[]
            {
                1595,
                3459,
                1575,
                2935
            },
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "昼下がりのラプソディー",
            "回復",
            "光",
            new[]
            {
                1595,
                3459,
                1575,
                2935
            },
            18,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "私ヲ蝕ム悪イ夢",
            "妨害",
            "闇",
            new[]
            {
                1581,
                3443,
                1585,
                2959
            },
            18,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "私ヲ蝕ム悪イ夢",
            "特殊範囲",
            "闇",
            new[]
            {
                1581,
                3443,
                1585,
                2959
            },
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "だいすきをあげる",
            "通常範囲",
            "闇",
            new[]
            {
                3460,
                1569,
                2945,
                1572
            },
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "だいすきをあげる",
            "支援",
            "闇",
            new[]
            {
                3460,
                1569,
                2945,
                1572
            },
            18,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "希望の光",
            "特殊単体",
            "闇",
            new[]
            {
                1634,
                3491,
                1627,
                2973
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "希望の光",
            "回復",
            "闇",
            new[]
            {
                1634,
                3491,
                1627,
                2973
            },
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "思い出はカメラの中に",
            "支援",
            "光",
            new[]
            {
                3318,
                3324,
                1831,
                1833
            },
            20,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "ブレイク・タイム",
            "通常範囲",
            "闇",
            new[]
            {
                3648,
                1848,
                2966,
                1820
            },
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "ラブリーアンドピース",
            "妨害",
            "闇",
            new[]
            {
                3297,
                3307,
                1833,
                1826
            },
            20,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "すってんあかりん",
            "特殊範囲",
            "光",
            new[]
            {
                1821,
                3669,
                1856,
                2946
            },
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "獅子奮迅",
            "支援",
            "火",
            new[]
            {
                2722,
                2698,
                3639,
                4132
            },
            18,
            new Skill(
                "WガードアシストD LG",
                "味方2体のDEFとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP/Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。さらに、支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "せめて、この子だけは",
            "通常範囲",
            "水",
            new[]
            {
                3450,
                1583,
                2933,
                1602
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:マイトDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとDEFを大ダウンさせる。"
            )
        ),
        new(
            "せめて、この子だけは",
            "支援",
            "水",
            new[]
            {
                3450,
                1583,
                2933,
                1602
            },
            18,
            new Skill(
                "水：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:マイトUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとDEFを大アップさせる。"
            )
        ),
        new(
            "紅巴式夏祭りの楽しみ方",
            "特殊範囲",
            "水",
            new[]
            {
                1606,
                3442,
                1586,
                2941
            },
            18,
            new Skill(
                "Sp.ディファーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ディファーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "紅巴式夏祭りの楽しみ方",
            "妨害",
            "水",
            new[]
            {
                1606,
                3442,
                1586,
                2941
            },
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.マイトDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "世界を越えて",
            "通常範囲",
            "水",
            new[]
            {
                3457,
                1591,
                2958,
                1597
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "世界を越えて",
            "回復",
            "水",
            new[]
            {
                3457,
                1591,
                2958,
                1597
            },
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "小さなシュッツエンゲル",
            "妨害",
            "水",
            new[]
            {
                2703,
                3440,
                1573,
                1844
            },
            18,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "小さなシュッツエンゲル",
            "特殊範囲",
            "水",
            new[]
            {
                2703,
                3440,
                1573,
                1844
            },
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "束ねる絆の一夜",
            "通常単体",
            "水",
            new[]
            {
                3450,
                1599,
                2935,
                1594
            },
            18,
            new Skill(
                "Sp.ディファーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:WガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "束ねる絆の一夜",
            "支援",
            "水",
            new[]
            {
                3450,
                1599,
                2935,
                1594
            },
            18,
            new Skill(
                "水：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "救う願いの一閃",
            "特殊単体",
            "水",
            new[]
            {
                1601,
                3894,
                1601,
                2489
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "救う願いの一閃",
            "妨害",
            "水",
            new[]
            {
                1601,
                3894,
                1601,
                2489
            },
            18,
            new Skill(
                "水：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "はるかな空",
            "通常単体",
            "水",
            new[]
            {
                2278,
                1286,
                2122,
                1297
            },
            16,
            new Skill(
                "ヒールストライクA Ⅳ",
                "敵1体に通常特大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "はるかな空",
            "回復",
            "水",
            new[]
            {
                2278,
                1286,
                2122,
                1297
            },
            16,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "足踏み健康ロードの悲劇",
            "通常範囲",
            "闇",
            new[]
            {
                3464,
                1606,
                2960,
                1580
            },
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "足踏み健康ロードの悲劇",
            "妨害",
            "闇",
            new[]
            {
                3464,
                1606,
                2960,
                1580
            },
            18,
            new Skill(
                "闇：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "みんなを守るために",
            "回復",
            "光",
            new[]
            {
                1603,
                1584,
                3446,
                2969
            },
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "みんなを守るために",
            "通常単体",
            "光",
            new[]
            {
                1603,
                1584,
                3446,
                2969
            },
            18,
            new Skill(
                "Sp.ディファーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "狂化フルスロットル",
            "特殊範囲",
            "光",
            new[]
            {
                1585,
                3453,
                1605,
                2965
            },
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "狂化フルスロットル",
            "回復",
            "光",
            new[]
            {
                1585,
                3453,
                1605,
                2965
            },
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "友を守護する剣",
            "通常範囲",
            "光",
            new[]
            {
                3433,
                1601,
                2964,
                1571
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "友を守護する剣",
            "妨害",
            "光",
            new[]
            {
                3433,
                1601,
                2964,
                1571
            },
            18,
            new Skill(
                "チャージガードフォールB Ⅱ",
                "敵1～2体のDEFをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "不死身の刃",
            "支援",
            "光",
            new[]
            {
                1594,
                3439,
                1601,
                2957
            },
            18,
            new Skill(
                "ディファーアシストB Ⅲ",
                "味方1～2体のSp.ATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "不死身の刃",
            "特殊単体",
            "光",
            new[]
            {
                1594,
                3439,
                1601,
                2957
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "駆ける希望",
            "回復",
            "光",
            new[]
            {
                1302,
                1322,
                2268,
                2129
            },
            16,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "駆ける希望",
            "通常単体",
            "光",
            new[]
            {
                1302,
                1322,
                2268,
                2129
            },
            16,
            new Skill(
                "ガードブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "癒しの露天風呂",
            "特殊範囲",
            "光",
            new[]
            {
                1578,
                3442,
                1580,
                2972
            },
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "癒しの露天風呂",
            "支援",
            "光",
            new[]
            {
                1578,
                3442,
                1580,
                2972
            },
            18,
            new Skill(
                "光：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "アサルトリリィ ふるーつ",
            "特殊範囲",
            "光",
            new[]
            {
                1683,
                3524,
                1684,
                3034
            },
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "アサルトリリィ ふるーつ",
            "妨害",
            "光",
            new[]
            {
                1683,
                3524,
                1684,
                3034
            },
            18,
            new Skill(
                "光：WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ストームデュオ",
            "支援",
            "光",
            new[]
            {
                3434,
                1602,
                2963,
                1589
            },
            18,
            new Skill(
                "光：ガードアシストB Ⅲ",
                "味方1～2体のDEFを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "ストームデュオ",
            "通常単体",
            "光",
            new[]
            {
                3434,
                1602,
                2963,
                1589
            },
            18,
            new Skill(
                "マイトブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。"
            )
        ),
        new(
            "アクアストライク",
            "通常範囲",
            "光",
            new[]
            {
                3433,
                1595,
                2950,
                1606
            },
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "アクアストライク",
            "回復",
            "光",
            new[]
            {
                3433,
                1595,
                2950,
                1606
            },
            18,
            new Skill(
                "チャージヒールC Ⅱ",
                "味方1～3体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "雷光一閃",
            "特殊範囲",
            "光",
            new[]
            {
                1585,
                3472,
                1574,
                2962
            },
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "雷光一閃",
            "妨害",
            "光",
            new[]
            {
                1585,
                3472,
                1574,
                2962
            },
            18,
            new Skill(
                "光：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "フラガラッハの光",
            "回復",
            "光",
            new[]
            {
                2711,
                2718,
                3888,
                3892
            },
            21,
            new Skill(
                "WガードヒールE LG",
                "味方2～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "ハッピーを見つけたら☆",
            "回復",
            "風",
            new[]
            {
                1575,
                1589,
                2955,
                3466
            },
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "ハッピーを見つけたら☆",
            "通常単体",
            "風",
            new[]
            {
                1575,
                1589,
                2955,
                3466
            },
            18,
            new Skill(
                "パワーブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のATKをダウンさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。"
            )
        ),
        new(
            "戦いを終えて",
            "回復",
            "水",
            new[]
            {
                1595,
                1596,
                3438,
                2935
            },
            18,
            new Skill(
                "ガードヒールB Ⅲ+",
                "味方1～2体のHPを大回復する。さらに味方のDEFをアップする。"
            ),
            new Support(
                "回:WガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "戦いを終えて",
            "特殊範囲",
            "水",
            new[]
            {
                1595,
                1596,
                3438,
                2935
            },
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "通じ合うふたり",
            "特殊範囲",
            "火",
            new[]
            {
                1585,
                3918,
                1583,
                2507
            },
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "通じ合うふたり",
            "妨害",
            "火",
            new[]
            {
                1585,
                3918,
                1583,
                2507
            },
            18,
            new Skill(
                "火：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "麗しき出立",
            "支援",
            "光",
            new[]
            {
                3467,
                1590,
                2945,
                1595
            },
            18,
            new Skill(
                "光：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "麗しき出立",
            "通常範囲",
            "光",
            new[]
            {
                3467,
                1590,
                2945,
                1595
            },
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "飛翔迎撃",
            "特殊範囲",
            "光",
            new[]
            {
                1594,
                3432,
                1580,
                2972
            },
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "飛翔迎撃",
            "回復",
            "光",
            new[]
            {
                1594,
                3432,
                1580,
                2972
            },
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "不動劒の姫",
            "通常単体",
            "光",
            new[]
            {
                3895,
                1589,
                2499,
                1590
            },
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "不動劒の姫",
            "妨害",
            "光",
            new[]
            {
                3895,
                1589,
                2499,
                1590
            },
            18,
            new Skill(
                "チャージパワーフォールB Ⅱ",
                "敵1～2体のATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "そうさく倶楽部の活動",
            "特殊範囲",
            "光",
            new[]
            {
                1608,
                3454,
                1599,
                2952
            },
            18,
            new Skill(
                "スマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "そうさく倶楽部の活動",
            "妨害",
            "光",
            new[]
            {
                1608,
                3454,
                1599,
                2952
            },
            18,
            new Skill(
                "光：WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "愛しき人との待ち合わせ",
            "通常範囲",
            "光",
            new[]
            {
                3450,
                1601,
                2968,
                1605
            },
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "愛しき人との待ち合わせ",
            "回復",
            "光",
            new[]
            {
                3450,
                1601,
                2968,
                1605
            },
            18,
            new Skill(
                "パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "月下の傍観者",
            "支援",
            "光",
            new[]
            {
                1606,
                3469,
                1569,
                2962
            },
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "月下の傍観者",
            "特殊範囲",
            "光",
            new[]
            {
                1606,
                3469,
                1569,
                2962
            },
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "なかよしとわいらいと",
            "通常範囲",
            "光",
            new[]
            {
                3459,
                1596,
                2943,
                1595
            },
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "なかよしとわいらいと",
            "妨害",
            "光",
            new[]
            {
                3459,
                1596,
                2943,
                1595
            },
            18,
            new Skill(
                "光：WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "水流乱撃",
            "特殊範囲",
            "光",
            new[]
            {
                1595,
                3464,
                1594,
                2941
            },
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "水流乱撃",
            "支援",
            "光",
            new[]
            {
                1595,
                3464,
                1594,
                2941
            },
            18,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "踏み込む勇気",
            "妨害",
            "火",
            new[]
            {
                2583,
                1398,
                2818,
                1422
            },
            17,
            new Skill(
                "火：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "踏み込む勇気",
            "通常範囲",
            "火",
            new[]
            {
                2583,
                1398,
                2818,
                1422
            },
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "GO!GO!ミリアム",
            "通常単体",
            "火",
            new[]
            {
                2565,
                1235,
                2334,
                1245
            },
            16,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "GO!GO!ミリアム",
            "支援",
            "火",
            new[]
            {
                2565,
                1235,
                2334,
                1245
            },
            16,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "吸血鬼のたしなみ",
            "支援",
            "水",
            new[]
            {
                1593,
                1572,
                2946,
                3470
            },
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "吸血鬼のたしなみ",
            "特殊範囲",
            "水",
            new[]
            {
                1593,
                1572,
                2946,
                3470
            },
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.マイトDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKとSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "校舎屋上のストラグル",
            "支援",
            "風",
            new[]
            {
                2939,
                3468,
                1574,
                1572
            },
            18,
            new Skill(
                "風：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "校舎屋上のストラグル",
            "特殊単体",
            "風",
            new[]
            {
                2939,
                3468,
                1574,
                1572
            },
            18,
            new Skill(
                "WパワーバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のATKとSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:WパワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "清淑なる黒き槍",
            "通常単体",
            "火",
            new[]
            {
                3890,
                1588,
                2515,
                1582
            },
            18,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "清淑なる黒き槍",
            "支援",
            "火",
            new[]
            {
                3890,
                1588,
                2515,
                1582
            },
            18,
            new Skill(
                "火：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "世界を守る剣たち",
            "妨害",
            "光",
            new[]
            {
                3450,
                1603,
                2936,
                1589
            },
            18,
            new Skill(
                "光：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "世界を守る剣たち",
            "通常範囲",
            "光",
            new[]
            {
                3450,
                1603,
                2936,
                1589
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "悪夢との共闘",
            "特殊範囲",
            "光",
            new[]
            {
                1597,
                3437,
                1582,
                2966
            },
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "悪夢との共闘",
            "支援",
            "光",
            new[]
            {
                1597,
                3437,
                1582,
                2966
            },
            18,
            new Skill(
                "チャージSp.ガードアシストC Ⅲ",
                "味方1～3体のSp.DEFをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "扶翼の剣",
            "特殊範囲",
            "光",
            new[]
            {
                1575,
                3466,
                1575,
                2951
            },
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "扶翼の剣",
            "妨害",
            "光",
            new[]
            {
                1575,
                3466,
                1575,
                2951
            },
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "灼爛の一撃",
            "特殊単体",
            "光",
            new[]
            {
                1579,
                3458,
                1570,
                2935
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "灼爛の一撃",
            "回復",
            "光",
            new[]
            {
                1579,
                3458,
                1570,
                2935
            },
            18,
            new Skill(
                "Sp.パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "静寂の中で",
            "通常範囲",
            "光",
            new[]
            {
                3461,
                1595,
                2956,
                1583
            },
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "静寂の中で",
            "支援",
            "光",
            new[]
            {
                3461,
                1595,
                2956,
                1583
            },
            18,
            new Skill(
                "チャージガードアシストC Ⅲ",
                "味方1～3体のDEFをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "デートのプロフェッショナル",
            "通常単体",
            "光",
            new[]
            {
                2278,
                2108,
                1310,
                1318
            },
            16,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "デートのプロフェッショナル",
            "支援",
            "光",
            new[]
            {
                2278,
                2108,
                1310,
                1318
            },
            16,
            new Skill(
                "光：WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:WパワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとSp.ATKを大アップさせる。"
            )
        ),
        new(
            "キラキラアイスクリーム！",
            "通常範囲",
            "闇",
            new[]
            {
                3443,
                1571,
                2937,
                1603
            },
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "キラキラアイスクリーム！",
            "回復",
            "闇",
            new[]
            {
                3443,
                1571,
                2937,
                1603
            },
            18,
            new Skill(
                "チャージヒールD Ⅱ",
                "味方2体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "迎え撃つ勇士たち",
            "特殊範囲",
            "闇",
            new[]
            {
                1600,
                3469,
                1573,
                2933
            },
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "迎え撃つ勇士たち",
            "回復",
            "闇",
            new[]
            {
                1600,
                3469,
                1573,
                2933
            },
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "不撓不屈の心を胸に",
            "通常範囲",
            "闇",
            new[]
            {
                3446,
                1578,
                2962,
                1573
            },
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "不撓不屈の心を胸に",
            "妨害",
            "闇",
            new[]
            {
                3446,
                1578,
                2962,
                1573
            },
            18,
            new Skill(
                "闇：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ブレイブ・ショット",
            "支援",
            "闇",
            new[]
            {
                1575,
                3439,
                1607,
                2955
            },
            18,
            new Skill(
                "チャージSp.パワーアシストB Ⅱ",
                "味方1～2体のSp.ATKをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ブレイブ・ショット",
            "特殊単体",
            "闇",
            new[]
            {
                1575,
                3439,
                1607,
                2955
            },
            18,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "発進☆ユニコーン！",
            "特殊単体",
            "水",
            new[]
            {
                1578,
                3885,
                1570,
                2491
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。"
            ),
            new Support(
                "攻:Sp.マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "発進☆ユニコーン！",
            "支援",
            "水",
            new[]
            {
                1578,
                3885,
                1570,
                2491
            },
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.マイトUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "仮想訓練場の応酬",
            "通常範囲",
            "火",
            new[]
            {
                3912,
                1593,
                2489,
                1608
            },
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:WガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "仮想訓練場の応酬",
            "妨害",
            "火",
            new[]
            {
                3912,
                1593,
                2489,
                1608
            },
            18,
            new Skill(
                "火：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ディファーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとDEFを大ダウンさせる。"
            )
        ),
        new(
            "優しい夕暮れ",
            "通常単体",
            "水",
            new[]
            {
                3463,
                1596,
                2947,
                1586
            },
            18,
            new Skill(
                "ガードブレイクA Ⅴ",
                "敵1体に通常超特大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKとDEFを大アップさせる。"
            )
        ),
        new(
            "優しい夕暮れ",
            "妨害",
            "水",
            new[]
            {
                3463,
                1596,
                2947,
                1586
            },
            18,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "静かに肩を寄せて",
            "妨害",
            "闇",
            new[]
            {
                1587,
                3452,
                1599,
                2951
            },
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "静かに肩を寄せて",
            "特殊範囲",
            "闇",
            new[]
            {
                1587,
                3452,
                1599,
                2951
            },
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "レスキューキャット",
            "通常範囲",
            "闇",
            new[]
            {
                3463,
                1587,
                2935,
                1579
            },
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "レスキューキャット",
            "支援",
            "闇",
            new[]
            {
                3463,
                1587,
                2935,
                1579
            },
            18,
            new Skill(
                "闇：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "平穏を守るための哮り",
            "通常範囲",
            "火",
            new[]
            {
                3443,
                1603,
                2948,
                1588
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "平穏を守るための哮り",
            "回復",
            "火",
            new[]
            {
                3443,
                1603,
                2948,
                1588
            },
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅰ",
                "HP回復時、一定確率でHPの回復量をアップさせる。さらに、支援/妨害時、一定確率で支援/妨害時効果を小アップさせる。"
            )
        ),
        new(
            "ハッピー＆トリート",
            "特殊単体",
            "水",
            new[]
            {
                1589,
                3920,
                1609,
                2508
            },
            18,
            new Skill(
                "Sp.ガードバーストA Ⅴ",
                "敵1体に特殊超特大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "ハッピー＆トリート",
            "妨害",
            "水",
            new[]
            {
                1589,
                3920,
                1609,
                2508
            },
            18,
            new Skill(
                "Sp.パワーフォールC Ⅳ",
                "敵1～3体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.マイトDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "あなただけの守護天使",
            "特殊範囲",
            "闇",
            new[]
            {
                1593,
                3460,
                1586,
                2971
            },
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "あなただけの守護天使",
            "支援",
            "闇",
            new[]
            {
                1593,
                3460,
                1586,
                2971
            },
            18,
            new Skill(
                "闇：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "いつでも近くに",
            "通常単体",
            "闇",
            new[]
            {
                3463,
                1590,
                2960,
                1605
            },
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "いつでも近くに",
            "回復",
            "闇",
            new[]
            {
                3463,
                1590,
                2960,
                1605
            },
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "純白の想い",
            "妨害",
            "闇",
            new[]
            {
                3450,
                1583,
                2933,
                1602
            },
            18,
            new Skill(
                "闇：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。"
            )
        ),
        new(
            "純白の想い",
            "通常範囲",
            "闇",
            new[]
            {
                3450,
                1583,
                2933,
                1602
            },
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "蒼き月の御使い",
            "特殊範囲",
            "水",
            new[]
            {
                2736,
                4156,
                2736,
                3627
            },
            18,
            new Skill(
                "スマッシュD LG",
                "敵2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "放課後のミューズ",
            "回復",
            "闇",
            new[]
            {
                1596,
                3458,
                1591,
                2937
            },
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "放課後のミューズ",
            "特殊範囲",
            "闇",
            new[]
            {
                1596,
                3458,
                1591,
                2937
            },
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "小春日和",
            "通常範囲",
            "闇",
            new[]
            {
                3444,
                1578,
                2956,
                1595
            },
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "小春日和",
            "支援",
            "闇",
            new[]
            {
                3444,
                1578,
                2956,
                1595
            },
            18,
            new Skill(
                "WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。"
            )
        ),
        new(
            "真夜中のクリエイター",
            "特殊範囲",
            "闇",
            new[]
            {
                1601,
                3447,
                1583,
                2969
            },
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "真夜中のクリエイター",
            "支援",
            "闇",
            new[]
            {
                1601,
                3447,
                1583,
                2969
            },
            18,
            new Skill(
                "闇：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "信念と誇り",
            "妨害",
            "闇",
            new[]
            {
                1390,
                2693,
                1399,
                2528
            },
            16,
            new Skill(
                "闇：WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "信念と誇り",
            "特殊単体",
            "闇",
            new[]
            {
                1390,
                2693,
                1399,
                2528
            },
            16,
            new Skill(
                "WパワーバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のATKとSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:WパワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "遠い日の足跡",
            "妨害",
            "闇",
            new[]
            {
                1604,
                3436,
                1572,
                2959
            },
            18,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "遠い日の足跡",
            "特殊単体",
            "闇",
            new[]
            {
                1604,
                3436,
                1572,
                2959
            },
            18,
            new Skill(
                "チャージスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "先駆けプリンセス",
            "支援",
            "闇",
            new[]
            {
                3475,
                1589,
                2978,
                1601
            },
            18,
            new Skill(
                "チャージパワーアシストB Ⅱ",
                "味方1～2体のATKをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "先駆けプリンセス",
            "通常単体",
            "闇",
            new[]
            {
                3475,
                1589,
                2978,
                1601
            },
            18,
            new Skill(
                "チャージストライクA Ⅲ",
                "敵1体に通常大ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "寂寥の美",
            "通常範囲",
            "闇",
            new[]
            {
                3437,
                1596,
                2952,
                1592
            },
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "寂寥の美",
            "回復",
            "闇",
            new[]
            {
                3437,
                1596,
                2952,
                1592
            },
            18,
            new Skill(
                "チャージヒールC Ⅱ",
                "味方1～3体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。"
            ),
            new Support(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "劔の妖精",
            "特殊範囲",
            "火",
            new[]
            {
                1857,
                3650,
                1837,
                2949
            },
            20,
            new Skill(
                "WパワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.ATKを小ダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "セインツの宝石",
            "通常範囲",
            "火",
            new[]
            {
                3576,
                1697,
                3061,
                1667
            },
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "セインツの宝石",
            "支援",
            "火",
            new[]
            {
                3576,
                1697,
                3061,
                1667
            },
            18,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。"
            )
        ),
        new(
            "暴君の花嫁",
            "回復",
            "火",
            new[]
            {
                1859,
                1843,
                3328,
                3293
            },
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "約束の蕾",
            "支援",
            "水",
            new[]
            {
                2978,
                1614,
                3463,
                1584
            },
            18,
            new Skill(
                "水：ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "約束の蕾",
            "通常範囲",
            "水",
            new[]
            {
                2978,
                1614,
                3463,
                1584
            },
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のATKとDEFをアップさせる。"
            )
        ),
        new(
            "大切な存在",
            "特殊範囲",
            "風",
            new[]
            {
                1593,
                3446,
                1579,
                2943
            },
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "大切な存在",
            "妨害",
            "風",
            new[]
            {
                1593,
                3446,
                1579,
                2943
            },
            18,
            new Skill(
                "Sp.パワーフォールC Ⅳ",
                "敵1～3体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。"
            )
        ),
        new(
            "黄昏の研究者たち",
            "回復",
            "火",
            new[]
            {
                1487,
                1477,
                2828,
                3336
            },
            17,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "黄昏の研究者たち",
            "通常範囲",
            "火",
            new[]
            {
                1487,
                1477,
                2828,
                3336
            },
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "出逢いの約束",
            "回復",
            "風",
            new[]
            {
                1481,
                1473,
                2805,
                3328
            },
            17,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。"
            )
        ),
        new(
            "出逢いの約束",
            "特殊単体",
            "風",
            new[]
            {
                1481,
                1473,
                2805,
                3328
            },
            17,
            new Skill(
                "Sp.マイトバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "ワンマンアーミー",
            "支援",
            "風",
            new[]
            {
                3456,
                2599,
                2497,
                2126
            },
            18,
            new Skill(
                "WパワーアシストD LG",
                "味方2体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "暁に笑う少女",
            "妨害",
            "風",
            new[]
            {
                3456,
                1584,
                2966,
                1600
            },
            18,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "暁に笑う少女",
            "通常単体",
            "風",
            new[]
            {
                3456,
                1584,
                2966,
                1600
            },
            18,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "ハンドメイド・リリィ",
            "支援",
            "風",
            new[]
            {
                1575,
                3467,
                1601,
                2948
            },
            18,
            new Skill(
                "Sp.ガードアシストD Ⅲ",
                "味方2体のSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "ハンドメイド・リリィ",
            "特殊単体",
            "風",
            new[]
            {
                1575,
                3467,
                1601,
                2948
            },
            18,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "ハッピーバレンタインだにゃん♪",
            "回復",
            "風",
            new[]
            {
                1834,
                1821,
                3316,
                3316
            },
            20,
            new Skill(
                "WパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKとSp.ATKを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。"
            )
        ),
        new(
            "まごころをこめて！",
            "通常範囲",
            "風",
            new[]
            {
                3675,
                1857,
                2949,
                1843
            },
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。"
            )
        ),
        new(
            "ドキドキ・ショコラーデ",
            "特殊範囲",
            "風",
            new[]
            {
                1848,
                3674,
                1831,
                2953
            },
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。"
            )
        ),
        new(
            "煌めく花々",
            "回復",
            "水",
            new[]
            {
                1518,
                1527,
                3115,
                3094
            },
            18,
            new Skill(
                "ヒールD Ⅳ",
                "味方2体のHPを大回復する。"
            ),
            new Support(
                "回:WガードUP Ⅰ",
                "HP回復時、一定確率で味方前衛1体のDEFとSp.DEFをアップさせる。"
            )
        ),
        new(
            "煌めく花々",
            "通常範囲",
            "水",
            new[]
            {
                1518,
                1527,
                3115,
                3094
            },
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "一筆の心",
            "通常範囲",
            "水",
            new[]
            {
                3440,
                1597,
                2958,
                1569
            },
            18,
            new Skill(
                "WパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。"
            )
        ),
        new(
            "一筆の心",
            "回復",
            "水",
            new[]
            {
                3440,
                1597,
                2958,
                1569
            },
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。"
            )
        ),
        new(
            "飾らぬ想いに咲き誇る",
            "特殊範囲",
            "風",
            new[]
            {
                1602,
                3434,
                1585,
                2955
            },
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "飾らぬ想いに咲き誇る",
            "回復",
            "風",
            new[]
            {
                1602,
                3434,
                1585,
                2955
            },
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "ヘルヴォルのお嫁さん",
            "妨害",
            "水",
            new[]
            {
                1598,
                3453,
                1571,
                2960
            },
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "ヘルヴォルのお嫁さん",
            "特殊単体",
            "水",
            new[]
            {
                1598,
                3453,
                1571,
                2960
            },
            18,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "攻:Sp.マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFをアップさせる。"
            )
        ),
        new(
            "楽しい遊園地",
            "特殊単体",
            "火",
            new[]
            {
                1590,
                3471,
                1593,
                2959
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "楽しい遊園地",
            "支援",
            "火",
            new[]
            {
                1590,
                3471,
                1593,
                2959
            },
            18,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "リフレッシュ！エンジン",
            "通常単体",
            "火",
            new[]
            {
                3888,
                1587,
                2519,
                1576
            },
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "リフレッシュ！エンジン",
            "妨害",
            "火",
            new[]
            {
                3888,
                1587,
                2519,
                1576
            },
            18,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "夜の闇を切り拓く者たち",
            "支援",
            "火",
            new[]
            {
                1601,
                1606,
                3460,
                2943
            },
            18,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "夜の闇を切り拓く者たち",
            "通常範囲",
            "火",
            new[]
            {
                1601,
                1606,
                3460,
                2943
            },
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "エクストリームブースト",
            "支援",
            "火",
            new[]
            {
                3456,
                1583,
                2942,
                1580
            },
            18,
            new Skill(
                "ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。"
            )
        ),
        new(
            "エクストリームブースト",
            "通常単体",
            "火",
            new[]
            {
                3456,
                1583,
                2942,
                1580
            },
            18,
            new Skill(
                "パワーストライクA Ⅲ+",
                "敵1体に通常大ダメージを与え、自身のATKを大アップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。"
            )
        ),
        new(
            "ガーディアン・パワー",
            "特殊範囲",
            "火",
            new[]
            {
                1854,
                3672,
                1833,
                2958
            },
            20,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。"
            )
        ),
        new(
            "コール・ユア・ネーム",
            "回復",
            "火",
            new[]
            {
                1825,
                1855,
                3654,
                2973
            },
            20,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "ジャスト・ザ・ブレイブ",
            "通常範囲",
            "火",
            new[]
            {
                3666,
                1847,
                2946,
                1823
            },
            20,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。"
            )
        ),
        new(
            "戦いの旅路",
            "通常単体",
            "火",
            new[]
            {
                2506,
                1349,
                2691,
                1352
            },
            16,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "戦いの旅路",
            "回復",
            "火",
            new[]
            {
                2506,
                1349,
                2691,
                1352
            },
            16,
            new Skill(
                "ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "大切なあなたを想い",
            "回復",
            "風",
            new[]
            {
                1529,
                1517,
                2485,
                2212
            },
            18,
            new Skill(
                "パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "戦火の結束",
            "支援",
            "水",
            new[]
            {
                3439,
                1586,
                2945,
                1575
            },
            18,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "戦火の結束",
            "通常単体",
            "水",
            new[]
            {
                3439,
                1586,
                2945,
                1575
            },
            18,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "ラ・ピュセル",
            "特殊単体",
            "水",
            new[]
            {
                1821,
                4469,
                1856,
                2146
            },
            20,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "鬼神の意志を継ぐ者",
            "妨害",
            "水",
            new[]
            {
                3656,
                2976,
                1834,
                1845
            },
            20,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。"
            )
        ),
        new(
            "台場の白き魔女",
            "通常範囲",
            "水",
            new[]
            {
                3672,
                1857,
                2968,
                1831
            },
            20,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "狂乱の姫巫女",
            "特殊範囲",
            "水",
            new[]
            {
                1854,
                3655,
                1834,
                2977
            },
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。"
            )
        ),
        new(
            "親愛なる仲間",
            "特殊範囲",
            "水",
            new[]
            {
                1561,
                3005,
                2760,
                1531
            },
            17,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。"
            )
        ),
        new(
            "親愛なる仲間",
            "回復",
            "水",
            new[]
            {
                1561,
                3005,
                2760,
                1531
            },
            17,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "新しい可能性",
            "妨害",
            "火",
            new[]
            {
                2707,
                1349,
                2516,
                1384
            },
            16,
            new Skill(
                "火：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "新しい可能性",
            "通常範囲",
            "火",
            new[]
            {
                2707,
                1349,
                2516,
                1384
            },
            16,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "そこにある笑顔",
            "通常範囲",
            "風",
            new[]
            {
                2842,
                1386,
                2586,
                1397
            },
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "そこにある笑顔",
            "妨害",
            "風",
            new[]
            {
                2842,
                1386,
                2586,
                1397
            },
            17,
            new Skill(
                "マイトフォールA Ⅲ",
                "敵1体のATKとDEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "トリガーハッピー",
            "特殊範囲",
            "風",
            new[]
            {
                1231,
                2350,
                1256,
                2544
            },
            16,
            new Skill(
                "パワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のATKをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "トリガーハッピー",
            "支援",
            "風",
            new[]
            {
                1231,
                2350,
                1256,
                2544
            },
            16,
            new Skill(
                "風：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "ふたりのアーセナル",
            "支援",
            "火",
            new[]
            {
                1402,
                1422,
                1632,
                3793
            },
            17,
            new Skill(
                "火：Sp.ガードアシストB Ⅲ",
                "味方1～2体のSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "ふたりのアーセナル",
            "特殊範囲",
            "火",
            new[]
            {
                1402,
                1422,
                1632,
                3793
            },
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "猪突猛進！",
            "通常範囲",
            "火",
            new[]
            {
                3464,
                2137,
                2947,
                2156
            },
            18,
            new Skill(
                "ストライクD LG",
                "敵2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "パジャマパーティー",
            "特殊範囲",
            "風",
            new[]
            {
                1556,
                2977,
                1560,
                2493
            },
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ファイア・ダッシュ",
            "妨害",
            "風",
            new[]
            {
                3441,
                1579,
                2943,
                1609
            },
            18,
            new Skill(
                "風：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "ファイア・ダッシュ",
            "通常範囲",
            "風",
            new[]
            {
                3441,
                1579,
                2943,
                1609
            },
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "ミューチュアルプロテクション",
            "特殊範囲",
            "風",
            new[]
            {
                1607,
                3435,
                1608,
                2945
            },
            18,
            new Skill(
                "ヒールスマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ミューチュアルプロテクション",
            "支援",
            "風",
            new[]
            {
                1607,
                3435,
                1608,
                2945
            },
            18,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "海の世界に想いを馳せて",
            "回復",
            "風",
            new[]
            {
                1585,
                1590,
                2968,
                3465
            },
            18,
            new Skill(
                "ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。"
            )
        ),
        new(
            "海の世界に想いを馳せて",
            "特殊単体",
            "風",
            new[]
            {
                1585,
                1590,
                2968,
                3465
            },
            18,
            new Skill(
                "ディファースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "CHARMを絵筆に替えて",
            "特殊範囲",
            "水",
            new[]
            {
                1570,
                3464,
                1601,
                2934
            },
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "CHARMを絵筆に替えて",
            "妨害",
            "水",
            new[]
            {
                1570,
                3464,
                1601,
                2934
            },
            18,
            new Skill(
                "Sp.ディファーフォールB Ⅲ",
                "敵1～2体のATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "ボナペティ！",
            "通常範囲",
            "火",
            new[]
            {
                3457,
                1576,
                2964,
                1594
            },
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "ボナペティ！",
            "支援",
            "火",
            new[]
            {
                3457,
                1576,
                2964,
                1594
            },
            18,
            new Skill(
                "火：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "恋花様ダイエット大作戦",
            "妨害",
            "風",
            new[]
            {
                2663,
                1484,
                2928,
                1493
            },
            17,
            new Skill(
                "風：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "恋花様ダイエット大作戦",
            "通常範囲",
            "風",
            new[]
            {
                2663,
                1484,
                2928,
                1493
            },
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "復讐の炎",
            "特殊範囲",
            "風",
            new[]
            {
                1490,
                3288,
                1471,
                2337
            },
            17,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "復讐の炎",
            "支援",
            "風",
            new[]
            {
                1490,
                3288,
                1471,
                2337
            },
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "強くなるために",
            "回復",
            "風",
            new[]
            {
                1417,
                1423,
                2822,
                2569
            },
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "強くなるために",
            "通常単体",
            "風",
            new[]
            {
                1417,
                1423,
                2822,
                2569
            },
            17,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "疾風の救助者",
            "特殊単体",
            "風",
            new[]
            {
                1237,
                2340,
                1253,
                2535
            },
            16,
            new Skill(
                "WパワーバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のATKとSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "疾風の救助者",
            "妨害",
            "風",
            new[]
            {
                1237,
                2340,
                1253,
                2535
            },
            16,
            new Skill(
                "風：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "大切な貴女への贈り物",
            "妨害",
            "火",
            new[]
            {
                3434,
                1602,
                2963,
                1589
            },
            18,
            new Skill(
                "火：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "大切な貴女への贈り物",
            "通常単体",
            "火",
            new[]
            {
                3434,
                1602,
                2963,
                1589
            },
            18,
            new Skill(
                "パワーブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のATKをダウンさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "罰執行のお時間です",
            "回復",
            "水",
            new[]
            {
                3433,
                1595,
                2950,
                1606
            },
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。"
            )
        ),
        new(
            "罰執行のお時間です",
            "通常範囲",
            "水",
            new[]
            {
                3433,
                1595,
                2950,
                1606
            },
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "故郷へ想い馳せながら",
            "支援",
            "水",
            new[]
            {
                1836,
                2980,
                1825,
                3670
            },
            20,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "心弛ぶひととき",
            "特殊範囲",
            "水",
            new[]
            {
                1855,
                3661,
                1833,
                2966
            },
            20,
            new Skill(
                "水：スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "元日の決斗！",
            "特殊単体",
            "水",
            new[]
            {
                1845,
                3640,
                1831,
                2980
            },
            20,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "気高き錬金術師",
            "通常単体",
            "火",
            new[]
            {
                2993,
                1571,
                2477,
                1570
            },
            19,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "ガンズ・パーティー",
            "通常範囲",
            "火",
            new[]
            {
                2992,
                1576,
                2491,
                1553
            },
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "神獣鏡の輝き",
            "支援",
            "火",
            new[]
            {
                1562,
                1564,
                2996,
                2485
            },
            19,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "絆の歌",
            "特殊単体",
            "火",
            new[]
            {
                1544,
                3366,
                1530,
                2402
            },
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "絆の歌",
            "妨害",
            "火",
            new[]
            {
                1544,
                3366,
                1530,
                2402
            },
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "戦いの合間に",
            "特殊範囲",
            "火",
            new[]
            {
                1522,
                2462,
                1507,
                2241
            },
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "輝きの海岸線",
            "妨害",
            "風",
            new[]
            {
                1494,
                1459,
                2689,
                2913
            },
            17,
            new Skill(
                "WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "輝きの海岸線",
            "通常範囲",
            "風",
            new[]
            {
                1494,
                1459,
                2689,
                2913
            },
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "ざっぱ～～ん！",
            "支援",
            "水",
            new[]
            {
                1532,
                2766,
                1532,
                3001
            },
            17,
            new Skill(
                "水：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ざっぱ～～ん！",
            "特殊範囲",
            "水",
            new[]
            {
                1532,
                2766,
                1532,
                3001
            },
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "白花咲く港",
            "特殊単体",
            "風",
            new[]
            {
                1461,
                2901,
                2687,
                1481
            },
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "白花咲く港",
            "支援",
            "風",
            new[]
            {
                1461,
                2901,
                2687,
                1481
            },
            17,
            new Skill(
                "Sp.パワーアシストA Ⅳ",
                "味方1体のSp.ATKを超特大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "一柳隊、大特集！",
            "支援",
            "火",
            new[]
            {
                2857,
                1409,
                2606,
                1389
            },
            17,
            new Skill(
                "火：ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "一柳隊、大特集！",
            "通常範囲",
            "火",
            new[]
            {
                2857,
                1409,
                2606,
                1389
            },
            17,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "あなたに傘を",
            "妨害",
            "水",
            new[]
            {
                1469,
                2670,
                1473,
                2921
            },
            17,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "あなたに傘を",
            "特殊単体",
            "水",
            new[]
            {
                1469,
                2670,
                1473,
                2921
            },
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅲ+",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKを大アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "情熱の取材前夜！",
            "通常単体",
            "火",
            new[]
            {
                2939,
                1478,
                2680,
                1476
            },
            17,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "情熱の取材前夜！",
            "回復",
            "火",
            new[]
            {
                2939,
                1478,
                2680,
                1476
            },
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "大丈夫、みんながいるから",
            "妨害",
            "水",
            new[]
            {
                2984,
                2498,
                1580,
                1580
            },
            19,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "高らかと響き渡る歌声の中で",
            "特殊範囲",
            "水",
            new[]
            {
                1581,
                3003,
                1544,
                2496
            },
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "戦場のコンダクター",
            "特殊単体",
            "火",
            new[]
            {
                2156,
                3448,
                2139,
                2969
            },
            18,
            new Skill(
                "Sp.パワースマッシュA LG",
                "敵1体に特殊超特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "姫歌を脅かす2つの新星",
            "回復",
            "水",
            new[]
            {
                1421,
                1423,
                2827,
                2591
            },
            17,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "姫歌を脅かす2つの新星",
            "特殊単体",
            "水",
            new[]
            {
                1421,
                1423,
                2827,
                2591
            },
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ハッピー☆シューティングスター",
            "通常単体",
            "水",
            new[]
            {
                2842,
                1400,
                1396,
                2579
            },
            17,
            new Skill(
                "ヒールストライクA Ⅳ",
                "敵1体に通常特大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "ハッピー☆シューティングスター",
            "支援",
            "水",
            new[]
            {
                2842,
                1400,
                1396,
                2579
            },
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "プレゼントはお任せ♪",
            "通常範囲",
            "風",
            new[]
            {
                2906,
                1490,
                2656,
                1475
            },
            17,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "プレゼントはお任せ♪",
            "支援",
            "風",
            new[]
            {
                2906,
                1490,
                2656,
                1475
            },
            17,
            new Skill(
                "風：ガードアシストB Ⅲ",
                "味方1～2体のDEFを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※..."
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "スノーフレイク",
            "特殊単体",
            "風",
            new[]
            {
                1554,
                3642,
                1545,
                1819
            },
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "胸躍る聖夜",
            "妨害",
            "風",
            new[]
            {
                2995,
                2492,
                1568,
                1558
            },
            19,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "リリィのすべてを伝えるために",
            "特殊範囲",
            "風",
            new[]
            {
                1527,
                3358,
                1544,
                2419
            },
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "リリィのすべてを伝えるために",
            "回復",
            "風",
            new[]
            {
                1527,
                3358,
                1544,
                2419
            },
            17,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "キャッチ＆リリース＆イート",
            "支援",
            "風",
            new[]
            {
                2548,
                1410,
                2725,
                1425
            },
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "キャッチ＆リリース＆イート",
            "通常単体",
            "風",
            new[]
            {
                2548,
                1410,
                2725,
                1425
            },
            17,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "これまでも、これからも隣で",
            "通常範囲",
            "風",
            new[]
            {
                3170,
                1419,
                2249,
                1406
            },
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "これまでも、これからも隣で",
            "妨害",
            "風",
            new[]
            {
                3170,
                1419,
                2249,
                1406
            },
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "リリィになるために！",
            "特殊範囲",
            "水",
            new[]
            {
                1568,
                2970,
                1568,
                2498
            },
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "結梨の大好きな場所",
            "特殊単体",
            "水",
            new[]
            {
                1467,
                2902,
                1461,
                2684
            },
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "結梨の大好きな場所",
            "妨害",
            "水",
            new[]
            {
                1467,
                2902,
                1461,
                2684
            },
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "みんな、ガンバレー！",
            "通常範囲",
            "水",
            new[]
            {
                2997,
                1581,
                2470,
                1545
            },
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "冷たいラムネをどうぞ",
            "回復",
            "水",
            new[]
            {
                1558,
                1553,
                2496,
                2970
            },
            19,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "リワード・マイセルフ",
            "支援",
            "火",
            new[]
            {
                2906,
                1463,
                2688,
                1479
            },
            17,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "リワード・マイセルフ",
            "通常範囲",
            "火",
            new[]
            {
                2906,
                1463,
                2688,
                1479
            },
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "トライング・オン",
            "通常単体",
            "火",
            new[]
            {
                2931,
                1474,
                2653,
                1466
            },
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "トライング・オン",
            "妨害",
            "火",
            new[]
            {
                2931,
                1474,
                2653,
                1466
            },
            17,
            new Skill(
                "Sp.マイトフォールA Ⅲ",
                "敵1体のSp.ATKとSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "気まぐれのツーショット",
            "妨害",
            "火",
            new[]
            {
                1568,
                2994,
                1561,
                2505
            },
            19,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "藍の宝物",
            "支援",
            "水",
            new[]
            {
                1410,
                2383,
                2220,
                2249
            },
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "藍の宝物",
            "特殊範囲",
            "水",
            new[]
            {
                1410,
                2383,
                2220,
                2249
            },
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "指先と白いペン",
            "通常範囲",
            "火",
            new[]
            {
                3391,
                1743,
                2764,
                1750
            },
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "指先と白いペン",
            "回復",
            "火",
            new[]
            {
                3391,
                1743,
                2764,
                1750
            },
            17,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "エレクトロンバウト！",
            "特殊範囲",
            "火",
            new[]
            {
                1575,
                2987,
                1572,
                2502
            },
            19,
            new Skill(
                "ヒールスマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "一流の戦い",
            "妨害",
            "火",
            new[]
            {
                2363,
                1260,
                2559,
                1234
            },
            16,
            new Skill(
                "ガードフォールA Ⅳ",
                "敵1体のDEFを超特大ダウンさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "一流の戦い",
            "通常単体",
            "火",
            new[]
            {
                2363,
                1260,
                2559,
                1234
            },
            16,
            new Skill(
                "ストライクA Ⅳ",
                "敵1体に通常超特大ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "戦場を切り裂く閃光",
            "通常範囲",
            "火",
            new[]
            {
                2547,
                1244,
                2367,
                1256
            },
            16,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "戦場を切り裂く閃光",
            "回復",
            "火",
            new[]
            {
                2547,
                1244,
                2367,
                1256
            },
            16,
            new Skill(
                "ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "悲壮の華",
            "支援",
            "風",
            new[]
            {
                1464,
                1484,
                3276,
                2300
            },
            17,
            new Skill(
                "ガードアシストD Ⅲ",
                "味方2体のDEFを大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "悲壮の華",
            "通常範囲",
            "風",
            new[]
            {
                1464,
                1484,
                3276,
                2300
            },
            17,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "鬼さんズ、こちら",
            "通常単体",
            "火",
            new[]
            {
                3356,
                1554,
                2399,
                1565
            },
            17,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "鬼さんズ、こちら",
            "回復",
            "火",
            new[]
            {
                3356,
                1554,
                2399,
                1565
            },
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "優美なる舞",
            "特殊範囲",
            "火",
            new[]
            {
                1397,
                1652,
                2598,
                2600
            },
            17,
            new Skill(
                "WガードスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のDEFとSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "優美なる舞",
            "妨害",
            "火",
            new[]
            {
                1397,
                1652,
                2598,
                2600
            },
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "美しき鉄糸の舞",
            "支援",
            "水",
            new[]
            {
                1485,
                2900,
                1494,
                2685
            },
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "美しき鉄糸の舞",
            "特殊範囲",
            "水",
            new[]
            {
                1485,
                2900,
                1494,
                2685
            },
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "駆けろ！エージェント",
            "特殊単体",
            "水",
            new[]
            {
                1571,
                2992,
                1838,
                2471
            },
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFをアップさせる。"
            )
        ),
        new(
            "スピード☆スター",
            "支援",
            "風",
            new[]
            {
                2469,
                2216,
                1518,
                1520
            },
            18,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "電光石火でご到着！",
            "通常範囲",
            "風",
            new[]
            {
                2489,
                1509,
                2221,
                1531
            },
            18,
            new Skill(
                "風：ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。さらに味方がオーダースキル「風属性効果増加」を発動中は威力がアップする。※..."
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "街角の寡黙な花",
            "特殊範囲",
            "風",
            new[]
            {
                1574,
                2515,
                1542,
                2288
            },
            18,
            new Skill(
                "ディファースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFを小アップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "アクアプラクティス",
            "通常単体",
            "火",
            new[]
            {
                2925,
                1461,
                2678,
                1473
            },
            17,
            new Skill(
                "パワーストライクA Ⅲ+",
                "敵1体に通常大ダメージを与え、自身のATKを大アップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "アクアプラクティス",
            "支援",
            "火",
            new[]
            {
                2925,
                1461,
                2678,
                1473
            },
            17,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "二水のヘイムスクリングラ体験",
            "妨害",
            "風",
            new[]
            {
                1499,
                2484,
                1514,
                2230
            },
            18,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "勝利の女神が微笑む時",
            "回復",
            "風",
            new[]
            {
                1522,
                1509,
                2219,
                2482
            },
            18,
            new Skill(
                "パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "プレシャス・モーニング",
            "支援",
            "風",
            new[]
            {
                2954,
                1528,
                2710,
                1519
            },
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "プレシャス・モーニング",
            "通常範囲",
            "風",
            new[]
            {
                2954,
                1528,
                2710,
                1519
            },
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。"
            ),
            new Support(
                "攻:マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のATKとDEFをアップさせる。"
            )
        ),
        new(
            "シュッツエンゲルの誓い",
            "通常範囲",
            "風",
            new[]
            {
                2362,
                1241,
                2534,
                1233
            },
            16,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "シュッツエンゲルの誓い",
            "妨害",
            "風",
            new[]
            {
                2362,
                1241,
                2534,
                1233
            },
            16,
            new Skill(
                "ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "果てなき戦いの日々",
            "通常範囲",
            "火",
            new[]
            {
                2039,
                1147,
                1985,
                1113
            },
            15,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "果てなき戦いの日々",
            "支援",
            "火",
            new[]
            {
                2039,
                1147,
                1985,
                1113
            },
            15,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "そばにいるだけで",
            "特殊単体",
            "水",
            new[]
            {
                1119,
                1992,
                1142,
                2021
            },
            15,
            new Skill(
                "Sp.パワースマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅰ",
                "攻撃時、一定確率で攻撃ダメージをアップさせる。"
            )
        ),
        new(
            "そばにいるだけで",
            "妨害",
            "水",
            new[]
            {
                1119,
                1992,
                1142,
                2021
            },
            15,
            new Skill(
                "Sp.パワーフォールA Ⅲ",
                "敵1体のSp.ATKを特大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "ここから先へ",
            "特殊範囲",
            "火",
            new[]
            {
                1566,
                2997,
                1554,
                2482
            },
            19,
            new Skill(
                "Sp.パワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "神の子",
            "回復",
            "風",
            new[]
            {
                2153,
                2147,
                3453,
                2985
            },
            18,
            new Skill(
                "ヒールE LG",
                "味方2～3体のHPを回復する。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "無邪気な親近感",
            "支援",
            "火",
            new[]
            {
                2976,
                1572,
                2466,
                1577
            },
            19,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "フォール・ダウン・アタック",
            "回復",
            "火",
            new[]
            {
                1502,
                1508,
                2492,
                2228
            },
            18,
            new Skill(
                "ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。"
            )
        ),
        new(
            "ホワイト・ラビット・マジック！",
            "特殊範囲",
            "火",
            new[]
            {
                1506,
                2472,
                1499,
                2213
            },
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "プレ・ハロウィンパーティー！",
            "妨害",
            "火",
            new[]
            {
                1507,
                2485,
                1522,
                2244
            },
            18,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "ようこそ！ふしぎの国へ",
            "回復",
            "火",
            new[]
            {
                1569,
                1561,
                2471,
                2994
            },
            19,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "おいでよ☆ハロウィン",
            "通常範囲",
            "水",
            new[]
            {
                2971,
                1545,
                2489,
                1584
            },
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "一直線上のストラテジー",
            "特殊単体",
            "風",
            new[]
            {
                1532,
                2939,
                1515,
                1760
            },
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "この空の下で",
            "妨害",
            "風",
            new[]
            {
                1509,
                1506,
                2207,
                2493
            },
            18,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "追跡者",
            "通常範囲",
            "風",
            new[]
            {
                2468,
                2241,
                1524,
                1508
            },
            18,
            new Skill(
                "Sp.パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKをダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "花を束ねる者",
            "特殊範囲",
            "風",
            new[]
            {
                1578,
                2978,
                1547,
                2488
            },
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "姉妹の休息",
            "回復",
            "水",
            new[]
            {
                1509,
                1510,
                2226,
                2462
            },
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "凛々しい花々",
            "通常範囲",
            "風",
            new[]
            {
                2552,
                1587,
                2302,
                1605
            },
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "わたしたちの魔法",
            "支援",
            "水",
            new[]
            {
                1561,
                2504,
                1557,
                2981
            },
            19,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "貫く想いの一撃",
            "特殊範囲",
            "水",
            new[]
            {
                1549,
                2967,
                1581,
                2491
            },
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "プリンセスひめひめ",
            "妨害",
            "水",
            new[]
            {
                2913,
                1522,
                1786,
                1501
            },
            18,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "いつかみんなと見る景色",
            "通常範囲",
            "火",
            new[]
            {
                3006,
                1579,
                2492,
                1569
            },
            19,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "繋げたい言葉",
            "妨害",
            "火",
            new[]
            {
                1500,
                2470,
                1530,
                2230
            },
            18,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "夏祭りのスナイパー",
            "通常範囲",
            "水",
            new[]
            {
                2496,
                1517,
                2245,
                1497
            },
            18,
            new Skill(
                "ガードストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "もう何も奪わせない",
            "回復",
            "水",
            new[]
            {
                1476,
                1485,
                2128,
                2057
            },
            17,
            new Skill(
                "ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のDEFを小アップする。"
            ),
            new Support(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。"
            )
        ),
        new(
            "夜空に咲く約束の花",
            "特殊単体",
            "水",
            new[]
            {
                1568,
                2981,
                1578,
                2497
            },
            19,
            new Skill(
                "Sp.ガードスマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "のびのびトレーニング！",
            "支援",
            "風",
            new[]
            {
                1529,
                2494,
                1503,
                2230
            },
            18,
            new Skill(
                "Sp.マイトアシストA Ⅲ",
                "味方1体のSp.ATKとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "リリィたちの羽休め",
            "特殊単体",
            "風",
            new[]
            {
                1242,
                1889,
                1836,
                1252
            },
            16,
            new Skill(
                "ガードスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "神宿りの暴走",
            "通常単体",
            "風",
            new[]
            {
                3000,
                1576,
                2479,
                1562
            },
            19,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "プランセス",
            "通常範囲",
            "風",
            new[]
            {
                3390,
                2074,
                2873,
                2093
            },
            18,
            new Skill(
                "ストライクD LG",
                "敵2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "信頼の背中",
            "妨害",
            "水",
            new[]
            {
                1393,
                2036,
                1397,
                2002
            },
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "単騎無双",
            "通常範囲",
            "水",
            new[]
            {
                2485,
                1494,
                2220,
                1497
            },
            18,
            new Skill(
                "ヒールストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "ラプラスの発動",
            "通常範囲",
            "水",
            new[]
            {
                2059,
                1396,
                1989,
                1418
            },
            17,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "レギオン、集結",
            "妨害",
            "火",
            new[]
            {
                1879,
                1270,
                1825,
                1236
            },
            16,
            new Skill(
                "パワーフォールB Ⅱ",
                "敵1～2体のATKをダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅰ",
                "支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "顕現する脅威",
            "通常範囲",
            "火",
            new[]
            {
                2108,
                1480,
                2093,
                1457
            },
            17,
            new Skill(
                "ガードストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "スーパーかわいいジャンプ！",
            "支援",
            "風",
            new[]
            {
                2073,
                2128,
                1487,
                1484
            },
            17,
            new Skill(
                "WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "きみとぼくの創作世界",
            "特殊単体",
            "風",
            new[]
            {
                1382,
                2057,
                1413,
                1977
            },
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "グリーンライフ",
            "回復",
            "風",
            new[]
            {
                1406,
                1388,
                1657,
                2400
            },
            17,
            new Skill(
                "Sp.ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のSp.DEFを小アップする。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "馳せたる海辺",
            "特殊範囲",
            "風",
            new[]
            {
                1568,
                2994,
                1570,
                2524
            },
            19,
            new Skill(
                "スマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "エスコートナイト",
            "通常単体",
            "風",
            new[]
            {
                2027,
                1414,
                2006,
                1401
            },
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "おもちゃのプール",
            "特殊単体",
            "水",
            new[]
            {
                1469,
                2141,
                1455,
                2077
            },
            17,
            new Skill(
                "ヒールスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ランペイジクラフト",
            "通常範囲",
            "水",
            new[]
            {
                2467,
                1477,
                1708,
                1454
            },
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "あなたと甘いひとときを",
            "通常単体",
            "水",
            new[]
            {
                2133,
                1483,
                1476,
                2066
            },
            17,
            new Skill(
                "Sp.パワーブレイクA Ⅲ+",
                "敵1体に通常大ダメージを与え、敵のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "水の車窓",
            "妨害",
            "水",
            new[]
            {
                1409,
                1980,
                1394,
                2059
            },
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "ラ・ヴァカンス・パルフェ",
            "回復",
            "水",
            new[]
            {
                1410,
                1407,
                1999,
                2039
            },
            17,
            new Skill(
                "Sp.パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のSp.ATKを小アップする。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ウォーター・レイルウェイ",
            "通常単体",
            "水",
            new[]
            {
                2131,
                1455,
                2081,
                1483
            },
            17,
            new Skill(
                "Sp.ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "アナザーワールド",
            "特殊範囲",
            "水",
            new[]
            {
                1392,
                1977,
                1386,
                2059
            },
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "勝負の鍵は",
            "通常範囲",
            "水",
            new[]
            {
                2473,
                1491,
                1712,
                1481
            },
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "アンブレイカブル",
            "通常範囲",
            "水",
            new[]
            {
                2406,
                1399,
                1628,
                1391
            },
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "ひとりはみんなのために",
            "妨害",
            "風",
            new[]
            {
                1738,
                1488,
                2461,
                1452
            },
            17,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "文武両道の乙女",
            "通常単体",
            "水",
            new[]
            {
                2121,
                1460,
                2087,
                1455
            },
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "アーセナルの絆",
            "特殊範囲",
            "火",
            new[]
            {
                1469,
                2471,
                1452,
                1740
            },
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "未来を切り開く武器",
            "通常範囲",
            "火",
            new[]
            {
                2993,
                1570,
                2488,
                1549
            },
            19,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "百由の息抜き",
            "支援",
            "火",
            new[]
            {
                1311,
                1308,
                1902,
                1954
            },
            16,
            new Skill(
                "WガードアシストA Ⅲ",
                "味方1体のDEFとSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "この地にて芽吹く",
            "支援",
            "水",
            new[]
            {
                2010,
                1385,
                2028,
                1408
            },
            17,
            new Skill(
                "ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "心を満たす栄養食",
            "妨害",
            "水",
            new[]
            {
                1261,
                2009,
                1264,
                2121
            },
            16,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "おこづかいのゆくえ",
            "特殊単体",
            "水",
            new[]
            {
                1386,
                2052,
                1401,
                1998
            },
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "カワイイのシャッターチャンス",
            "通常範囲",
            "火",
            new[]
            {
                2013,
                1416,
                2063,
                1387
            },
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。"
            )
        ),
        new(
            "不滅のホワイトナイト",
            "特殊範囲",
            "水",
            new[]
            {
                1473,
                2488,
                1461,
                1723
            },
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "優麗なる白の姫騎士",
            "通常単体",
            "水",
            new[]
            {
                1888,
                1327,
                1928,
                1332
            },
            16,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "撃滅のブラックナイト",
            "回復",
            "水",
            new[]
            {
                1307,
                1313,
                1913,
                1949
            },
            16,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "果断なる漆黒の騎士",
            "妨害",
            "水",
            new[]
            {
                2468,
                1485,
                1723,
                1470
            },
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "不完全ゆえに愛おしく",
            "支援",
            "火",
            new[]
            {
                2008,
                1383,
                2041,
                1404
            },
            17,
            new Skill(
                "マイトアシストA Ⅲ",
                "味方1体のATKとDEFを大アップさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "甘いスイーツでおもてなし♪",
            "通常単体",
            "風",
            new[]
            {
                2404,
                1402,
                1627,
                1409
            },
            17,
            new Skill(
                "ヒールストライクA Ⅲ",
                "敵1体に通常大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "想いの輪唱",
            "通常範囲",
            "風",
            new[]
            {
                1736,
                1174,
                1804,
                1213
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "朋友",
            "支援",
            "風",
            new[]
            {
                1805,
                1176,
                1745,
                1211
            },
            16,
            new Skill(
                "ガードアシストA Ⅲ",
                "味方1体のDEFを特大アップさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "パーフェクトガード",
            "特殊単体",
            "風",
            new[]
            {
                1193,
                1819,
                1771,
                1199
            },
            16,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "空想と現実は陸続き",
            "回復",
            "火",
            new[]
            {
                1182,
                1176,
                1732,
                1790
            },
            16,
            new Skill(
                "ヒールA Ⅲ",
                "味方1体のHPを特大回復する。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "見切れ希望女子",
            "通常範囲",
            "火",
            new[]
            {
                2381,
                1401,
                1648,
                1415
            },
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "ハッピーハッピー☆タピオカ",
            "特殊単体",
            "水",
            new[]
            {
                1388,
                1661,
                2563,
                1202
            },
            17,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "レンズに咲く百合の花",
            "通常範囲",
            "火",
            new[]
            {
                2063,
                1413,
                1996,
                1384
            },
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。"
            )
        ),
        new(
            "不器用なお姉様",
            "支援",
            "火",
            new[]
            {
                1981,
                1642,
                1787,
                1416
            },
            17,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。"
            )
        ),
        new(
            "ワンショット",
            "回復",
            "火",
            new[]
            {
                1415,
                1389,
                1979,
                2037
            },
            17,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。"
            ),
            new Support(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。"
            )
        ),
        new(
            "ふたつのふれあい",
            "妨害",
            "水",
            new[]
            {
                2042,
                2013,
                1394,
                1396
            },
            17,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "星が見えない君へ",
            "通常単体",
            "風",
            new[]
            {
                1813,
                1185,
                1763,
                1180
            },
            16,
            new Skill(
                "ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "たった一歩の前進",
            "妨害",
            "風",
            new[]
            {
                1737,
                1201,
                1810,
                1180
            },
            16,
            new Skill(
                "ガードフォールA Ⅲ",
                "敵1体のDEFを特大ダウンさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "突き通す信念の剣",
            "回復",
            "風",
            new[]
            {
                1213,
                1208,
                1807,
                1756
            },
            16,
            new Skill(
                "ヒールB Ⅲ",
                "味方1～2体のHPを大回復する。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "ひめひめ・オン・ザ・ステージ！",
            "特殊単体",
            "水",
            new[]
            {
                1187,
                1732,
                1199,
                1807
            },
            16,
            new Skill(
                "Sp.ガードバーストA Ⅲ",
                "敵1体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "アイドルリリィをつかまえて",
            "通常範囲",
            "水",
            new[]
            {
                2321,
                1345,
                1554,
                1324
            },
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "光り輝くステージへ",
            "支援",
            "水",
            new[]
            {
                1739,
                1201,
                1199,
                1781
            },
            16,
            new Skill(
                "Sp.ガードアシストA Ⅲ",
                "味方1体のSp.DEFを特大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "アイドルリリィ★スマイル",
            "妨害",
            "水",
            new[]
            {
                1607,
                1413,
                1536,
                1399
            },
            16,
            new Skill(
                "Sp.パワーフォールA Ⅲ",
                "敵1体のSp.ATKを特大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "うさぎになったカメ",
            "特殊範囲",
            "水",
            new[]
            {
                1323,
                1554,
                1677,
                1970
            },
            17,
            new Skill(
                "Sp.ガードスマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "迷子のクマ",
            "通常単体",
            "風",
            new[]
            {
                2321,
                1311,
                1556,
                1314
            },
            17,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "クラシック・ホリデイ",
            "支援",
            "風",
            new[]
            {
                1131,
                1896,
                1134,
                1984
            },
            16,
            new Skill(
                "Sp.パワーアシストA Ⅲ",
                "味方1体のSp.ATKを特大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "晴れのちラムネ",
            "特殊範囲",
            "風",
            new[]
            {
                1387,
                2393,
                1418,
                1632
            },
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "湯けむりの園",
            "特殊単体",
            "風",
            new[]
            {
                1317,
                1978,
                1925,
                1327
            },
            17,
            new Skill(
                "ヒールスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。さらに自身のHPを回復する。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "姫歌イメチェン大作戦!!",
            "回復",
            "風",
            new[]
            {
                1316,
                1346,
                1907,
                1976
            },
            17,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ガラスの中の大切な世界",
            "妨害",
            "風",
            new[]
            {
                1346,
                1334,
                2321,
                1548
            },
            17,
            new Skill(
                "ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "あたしがこの手で守るもの",
            "支援",
            "水",
            new[]
            {
                2300,
                1560,
                1324,
                1317
            },
            17,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "CHARMにお疲れ様",
            "特殊範囲",
            "火",
            new[]
            {
                1312,
                2022,
                1347,
                1849
            },
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "一柳隊の知恵袋",
            "通常単体",
            "火",
            new[]
            {
                1930,
                1348,
                1965,
                1318
            },
            17,
            new Skill(
                "Sp.ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "ロイヤル・ホスピタリティ",
            "妨害",
            "火",
            new[]
            {
                1968,
                1345,
                1914,
                1311
            },
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "わたしにできること",
            "特殊単体",
            "火",
            new[]
            {
                1345,
                1558,
                1907,
                1748
            },
            17,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "安らぎの帰り道",
            "回復",
            "火",
            new[]
            {
                1312,
                1343,
                1925,
                1945
            },
            17,
            new Skill(
                "ヒールA Ⅲ",
                "味方1体のHPを特大回復する。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "猫の誘惑",
            "特殊範囲",
            "水",
            new[]
            {
                1320,
                2303,
                1322,
                1583
            },
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "ストロベリー・プリンセス",
            "通常単体",
            "風",
            new[]
            {
                1787,
                1207,
                1737,
                1192
            },
            16,
            new Skill(
                "パワーストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "都会の空を舞う天使",
            "回復",
            "風",
            new[]
            {
                1183,
                1199,
                1732,
                1784
            },
            16,
            new Skill(
                "ヒールB Ⅲ",
                "味方1～2体のHPを大回復する。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "わたしの秘密の記録",
            "支援",
            "風",
            new[]
            {
                1810,
                1407,
                1547,
                1187
            },
            16,
            new Skill(
                "パワーアシストA Ⅲ",
                "味方1体のATKを特大アップさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "貴方に贈る花",
            "妨害",
            "風",
            new[]
            {
                1197,
                2135,
                1197,
                1413
            },
            16,
            new Skill(
                "Sp.パワーフォールA Ⅲ",
                "敵1体のSp.ATKを特大ダウンさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "変わる世界と変わらぬ想い",
            "特殊範囲",
            "風",
            new[]
            {
                1184,
                1733,
                1178,
                1815
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "星降る夜の約束",
            "回復",
            "火",
            new[]
            {
                1349,
                1314,
                1582,
                2306
            },
            17,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "私たちの正義",
            "妨害",
            "水",
            new[]
            {
                1346,
                1914,
                1324,
                1969
            },
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:支援UP Ⅰ",
                "支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        ),
        new(
            "ヘルヴォルの戦乙女",
            "回復",
            "水",
            new[]
            {
                1204,
                1210,
                1785,
                1732
            },
            16,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "約束のピクニック",
            "支援",
            "水",
            new[]
            {
                1395,
                1182,
                1811,
                1527
            },
            16,
            new Skill(
                "ガードアシストA Ⅲ",
                "味方1体のDEFを特大アップさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "誇りの一閃",
            "特殊単体",
            "水",
            new[]
            {
                1179,
                1788,
                1211,
                1754
            },
            16,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "お気に入りのかわいい服",
            "妨害",
            "水",
            new[]
            {
                1812,
                1191,
                1734,
                1183
            },
            16,
            new Skill(
                "パワーフォールA Ⅲ",
                "敵1体のATKを特大ダウンさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "雨降って絆深まる",
            "通常範囲",
            "水",
            new[]
            {
                1805,
                1201,
                1748,
                1212
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:ダメージUP Ⅰ",
                "攻撃時、一定確率で攻撃ダメージをアップさせる。"
            )
        ),
        new(
            "楯の乙女",
            "通常単体",
            "水",
            new[]
            {
                1020,
                591,
                1093,
                570
            },
            15,
            new Skill(
                "パワーストライクA Ⅱ",
                "敵1体に通常ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅰ",
                "攻撃時、一定確率で敵のDEFをダウンさせる。"
            )
        ),
        new(
            "放課後ファンタズム",
            "通常範囲",
            "火",
            new[]
            {
                1970,
                1338,
                1904,
                1337
            },
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "雨上がりの朝稽古",
            "妨害",
            "火",
            new[]
            {
                1337,
                1925,
                1336,
                1952
            },
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "ニアレスト",
            "通常単体",
            "火",
            new[]
            {
                1399,
                1193,
                1811,
                1542
            },
            16,
            new Skill(
                "ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "歴戦の貫禄",
            "特殊範囲",
            "火",
            new[]
            {
                1191,
                1765,
                1177,
                1801
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "伝えたい言葉",
            "回復",
            "火",
            new[]
            {
                1201,
                1178,
                1744,
                1815
            },
            16,
            new Skill(
                "ヒールB Ⅲ",
                "味方1～2体のHPを大回復する。"
            ),
            new Support(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "夜に潜む",
            "支援",
            "火",
            new[]
            {
                569,
                591,
                1093,
                1021
            },
            15,
            new Skill(
                "Sp.ガードアシストA Ⅱ",
                "味方1体のSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅰ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFをダウンさせる。"
            )
        ),
        new(
            "アフタヌーンティー",
            "特殊単体",
            "火",
            new[]
            {
                1335,
                2297,
                1335,
                1575
            },
            17,
            new Skill(
                "Sp.ガードスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "帰り立つ少女",
            "特殊範囲",
            "風",
            new[]
            {
                578,
                980,
                582,
                1119
            },
            15,
            new Skill(
                "スマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅰ",
                "攻撃時、一定確率で敵のSp.DEFをダウンさせる。"
            )
        ),
        new(
            "Dear Schutzengel",
            "通常単体",
            "風",
            new[]
            {
                1954,
                1313,
                1898,
                1345
            },
            17,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "雪辱の一閃",
            "支援",
            "風",
            new[]
            {
                1737,
                1788,
                1211,
                1196
            },
            16,
            new Skill(
                "Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "やさしい黄昏",
            "支援",
            "火",
            new[]
            {
                1183,
                1201,
                1781,
                1764
            },
            16,
            new Skill(
                "ガードアシストA Ⅲ",
                "味方1体のDEFを特大アップさせる。"
            ),
            new Support(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "ねむねむにさようなら",
            "回復",
            "風",
            new[]
            {
                1181,
                1187,
                1766,
                1802
            },
            16,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "レンズにほころぶ百合の蕾",
            "通常範囲",
            "水",
            new[]
            {
                1813,
                1210,
                1759,
                1184
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "夢見る魔法少女",
            "妨害",
            "火",
            new[]
            {
                1812,
                1402,
                1551,
                1209
            },
            16,
            new Skill(
                "パワーフォールA Ⅲ",
                "敵1体のATKを特大ダウンさせる。"
            ),
            new Support(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。"
            )
        ),
        new(
            "踏み出す勇気",
            "回復",
            "水",
            new[]
            {
                1191,
                1187,
                1732,
                1814
            },
            16,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "可憐なる旗のもとに",
            "妨害",
            "水",
            new[]
            {
                1410,
                1731,
                1211,
                1608
            },
            16,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。"
            ),
            new Support(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "トレイン・コースター",
            "支援",
            "火",
            new[]
            {
                2158,
                1199,
                1403,
                1178
            },
            16,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。"
            ),
            new Support(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。"
            )
        ),
        new(
            "紅蓮を翔ける勇者",
            "回復",
            "風",
            new[]
            {
                1185,
                1182,
                1805,
                1757
            },
            16,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。"
            ),
            new Support(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "一柳隊の仲間として",
            "支援",
            "風",
            new[]
            {
                1205,
                1396,
                1537,
                1818
            },
            16,
            new Skill(
                "Sp.パワーアシストA Ⅲ",
                "味方1体のSp.ATKを特大アップさせる。"
            ),
            new Support(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "バトル・ウエイトレス",
            "妨害",
            "風",
            new[]
            {
                1208,
                1177,
                1734,
                1808
            },
            16,
            new Skill(
                "ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。"
            ),
            new Support(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。"
            )
        ),
        new(
            "孤高のリリィが生きる今",
            "回復",
            "火",
            new[]
            {
                1198,
                1178,
                1809,
                1748
            },
            16,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。"
            ),
            new Support(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。"
            )
        ),
        new(
            "お気に入りの隊服",
            "支援",
            "水",
            new[]
            {
                1178,
                1758,
                1193,
                1804
            },
            16,
            new Skill(
                "Sp.ガードアシストB Ⅲ",
                "味方1～2体のSp.DEFを大アップさせる。"
            ),
            new Support(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "花は優しく微笑む",
            "特殊単体",
            "風",
            new[]
            {
                1201,
                1796,
                1185,
                1756
            },
            16,
            new Skill(
                "Sp.ガードスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "エアリアル☆シューター",
            "特殊範囲",
            "水",
            new[]
            {
                1211,
                1816,
                1211,
                1737
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "努力は憧れを追って",
            "通常単体",
            "火",
            new[]
            {
                1814,
                1175,
                1747,
                1196
            },
            16,
            new Skill(
                "ガードブレイクA Ⅲ",
                "敵1体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "グラン・エプレの必勝戦術",
            "通常範囲",
            "火",
            new[]
            {
                1744,
                1210,
                1800,
                1201
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "その笑顔を守るために",
            "特殊単体",
            "風",
            new[]
            {
                1195,
                1762,
                1183,
                1797
            },
            16,
            new Skill(
                "Sp.パワースマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "秘密の魔法の唱え方",
            "特殊範囲",
            "火",
            new[]
            {
                1183,
                1759,
                1173,
                1814
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "言葉なく猛る",
            "特殊単体",
            "火",
            new[]
            {
                1181,
                1795,
                1208,
                1752
            },
            16,
            new Skill(
                "Sp.パワースマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "ヘルヴォルの名を冠する者",
            "通常範囲",
            "水",
            new[]
            {
                1742,
                1207,
                1797,
                1192
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "藍の一番長い日",
            "通常単体",
            "風",
            new[]
            {
                1793,
                1188,
                1736,
                1179
            },
            16,
            new Skill(
                "ガードブレイクA Ⅲ",
                "敵1体に通常大ダメージを与え、敵のDEFをダウンさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "全力少女",
            "特殊範囲",
            "水",
            new[]
            {
                1186,
                1759,
                1201,
                1818
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "先輩と後輩と仲間たち",
            "特殊範囲",
            "風",
            new[]
            {
                1198,
                1798,
                1178,
                1759
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。"
            )
        ),
        new(
            "淑やかなるスナイパー",
            "通常単体",
            "水",
            new[]
            {
                1765,
                1211,
                1796,
                1210
            },
            16,
            new Skill(
                "パワーストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "ふたりで守る灯り",
            "特殊単体",
            "風",
            new[]
            {
                1202,
                1752,
                1175,
                1809
            },
            16,
            new Skill(
                "Sp.ガードバーストA Ⅲ",
                "敵1体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。"
            )
        ),
        new(
            "吉村・Ｔｈｉ・梅の日常",
            "通常単体",
            "水",
            new[]
            {
                1736,
                1174,
                1804,
                1213
            },
            16,
            new Skill(
                "ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のDEFをアップさせる。"
            ),
            new Support(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。"
            )
        ),
        new(
            "紡ぐ未来、変わる今",
            "特殊範囲",
            "火",
            new[]
            {
                1197,
                1784,
                1187,
                1769
            },
            16,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。"
            ),
            new Support(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。"
            )
        ),
        new(
            "ふーみん、司令官になる",
            "通常範囲",
            "風",
            new[]
            {
                1801,
                1211,
                1771,
                1199
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。"
            )
        ),
        new(
            "完全無欠のお嬢様",
            "通常範囲",
            "火",
            new[]
            {
                1790,
                1176,
                1732,
                1182
            },
            16,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。"
            ),
            new Support(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。"
            )
        ),
        new(
            "世代を越え伝えるもの",
            "特殊単体",
            "水",
            new[]
            {
                1179,
                1751,
                1196,
                1815
            },
            16,
            new Skill(
                "Sp.ガードバーストA Ⅲ",
                "敵1体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。"
            ),
            new Support(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。"
            )
        ),
        new(
            "守りたいもの",
            "通常単体",
            "火",
            new[]
            {
                1796,
                1185,
                1731,
                1209
            },
            16,
            new Skill(
                "パワーストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のATKをアップさせる。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "焔の中の断罪者",
            "通常単体",
            "火",
            new[]
            {
                2142,
                1207,
                1397,
                1192
            },
            16,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。"
            ),
            new Support(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。"
            )
        ),
        new(
            "ふたりの約束",
            "回復",
            "水",
            new[]
            {
                569,
                591,
                1043,
                1071
            },
            15,
            new Skill(
                "ヒールB Ⅱ",
                "味方1～2体のHPを回復する。"
            ),
            new Support(
                "回:回復UP Ⅰ",
                "HP回復時、一定確率でHPの回復量をアップさせる。"
            )
        ),
        new(
            "祝!!リリース!!",
            "支援",
            "風",
            new[]
            {
                665,
                595,
                1046,
                965
            },
            15,
            new Skill(
                "ガードアシストA Ⅱ",
                "味方1体のDEFを大アップさせる。"
            ),
            new Support(
                "援:支援UP Ⅰ",
                "支援/妨害時、一定確率で支援/妨害効果をアップさせる。"
            )
        )
    };
}