using System;
using System.Linq;
using System.Text.Json;
using Mitama.Domain;
using Range = Mitama.Domain.Range;

namespace Mitama.Repository;

public record MemoriaPOCO(
    int id,
    string link,
    string name,
    string full_name,
    string kind,
    string element,
    int[][] status,
    int cost,
    string skill,
    string support,
    string[] labels
);

public record SkillDto(
    string name,
    string description,
    string stats,
    string amount,
    string effects,
    string level,
    string range
)
{
    public static SkillDto FromJson(string json)
    {
        return JsonSerializer.Deserialize<SkillDto>(json);
    }

    public Skill IntoSkill => new(
        name,
        description,
        effects == string.Empty ? [] : effects.Split(", ").Select<string, SkillEffect>(effect => effect switch
        {
            "ElementStimulation/Fire" => new ElementStimulation(Element.Fire),
            "ElementStimulation/Water" => new ElementStimulation(Element.Water),
            "ElementStimulation/Wind" => new ElementStimulation(Element.Wind),
            "ElementStimulation/Light" => new ElementStimulation(Element.Light),
            "ElementStimulation/Dark" => new ElementStimulation(Element.Dark),
            "ElementSpread/Fire" => new ElementSpread(Element.Fire),
            "ElementSpread/Water" => new ElementSpread(Element.Water),
            "ElementSpread/Wind" => new ElementSpread(Element.Wind),
            "ElementStrengthen/Fire" => new ElementStrengthen(Element.Fire),
            "ElementStrengthen/Water" => new ElementStrengthen(Element.Water),
            "ElementStrengthen/Wind" => new ElementStrengthen(Element.Wind),
            "ElementWeaken/Fire" => new ElementWeaken(Element.Fire),
            "ElementWeaken/Water" => new ElementWeaken(Element.Water),
            "ElementWeaken/Wind" => new ElementWeaken(Element.Wind),
            "Counter" => new Counter(),
            "Charge" => new Charge(),
            "Recover" => new Recover(),
            _ => throw new NotImplementedException($"{effect}"),
        }).ToArray(),
        stats == string.Empty ? [] : stats.Split(", ").Select<string, StatusChange>(stat =>
        {
            Stat status = stat switch
            {
                "Atk" => new Atk(),
                "SpAtk" => new SpAtk(),
                "Def" => new Def(),
                "SpDef" => new SpDef(),
                "Life" => new Life(),
                "ElementalAttack/Fire" => new ElementAttack(Element.Fire),
                "ElementalAttack/Water" => new ElementAttack(Element.Water),
                "ElementalAttack/Wind" => new ElementAttack(Element.Wind),
                "ElementalAttack/Light" => new ElementAttack(Element.Light),
                "ElementalAttack/Dark" => new ElementAttack(Element.Dark),
                "ElementalGuard/Fire" => new ElementGuard(Element.Fire),
                "ElementalGuard/Water" => new ElementGuard(Element.Water),
                "ElementalGuard/Wind" => new ElementGuard(Element.Wind),
                "ElementalGuard/Light" => new ElementGuard(Element.Light),
                "ElementalGuard/Dark" => new ElementGuard(Element.Dark),
                _ => throw new NotImplementedException($"{stat}"),
            };
            var level = int.Parse(amount.Split("/").Last());
            return amount.Split('/').First() switch
            {
                "Up" => new StatusUp(status, (Amount)level),
                "Down" => new StatusDown(status, (Amount)level),
                _ => throw new NotImplementedException($"{amount.Split('/').First()}"),
            };
        }).ToArray(),
        level switch
        {
            "One" => Level.One,
            "Two" => Level.Two,
            "Three" => Level.Three,
            "ThreePlus" => Level.ThreePlus,
            "Four" => Level.Four,
            "FourPlus" => Level.FourPlus,
            "Five" => Level.Five,
            "FivePlus" => Level.FivePlus,
            "LG" => Level.Lg,
            "LGPlus" => Level.LgPlus,
            _ => throw new NotImplementedException(),
        },
        range switch
        {
            "A" => Range.A,
            "B" => Range.B,
            "C" => Range.C,
            "D" => Range.D,
            "E" => Range.E,
            _ => throw new NotImplementedException(),
        }
    );
}

public record SupportDto(
    string name,
    string description,
    string effects,
    string trigger,
    string level
)
{
    public static SupportDto FromJson(string json)
    {
        return JsonSerializer.Deserialize<SupportDto>(json);
    }

    public SupportSkill IntoSupportSkill => new(
        name,
        description,
        trigger switch
        {
            "Attack" => Trigger.Attack,
            "Support" => Trigger.Support,
            "Recovery" => Trigger.Recovery,
            "Command" => Trigger.Command,
            _ => throw new NotImplementedException($"{trigger}"),
        },
        effects.Split(", ").Select<string, SupportEffect>(effect => effect switch
        {
            "NormalMatchPtUp" => new NormalMatchPtUp(),
            "SpecialMatchPtUp" => new SpecialMatchPtUp(),
            "PowerUp/Normal" => new PowerUp(Domain.Type.Normal),
            "PowerUp/Special" => new PowerUp(Domain.Type.Special),
            "GuardUp/Normal" => new GuardUp(Domain.Type.Normal),
            "GuardUp/Special" => new GuardUp(Domain.Type.Special),
            "PowerDown/Normal" => new PowerDown(Domain.Type.Normal),
            "PowerDown/Special" => new PowerDown(Domain.Type.Special),
            "GuardDown/Normal" => new GuardDown(Domain.Type.Normal),
            "GuardDown/Special" => new GuardDown(Domain.Type.Special),
            "ElementPowerUp/Fire" => new ElementPowerUp(Element.Fire),
            "ElementPowerUp/Water" => new ElementPowerUp(Element.Water),
            "ElementPowerUp/Wind" => new ElementPowerUp(Element.Wind),
            "ElementPowerUp/Light" => new ElementPowerUp(Element.Light),
            "ElementPowerUp/Dark" => new ElementPowerUp(Element.Dark),
            "ElementGuardUp/Fire" => new ElementGuardUp(Element.Fire),
            "ElementGuardUp/Water" => new ElementGuardUp(Element.Water),
            "ElementGuardUp/Wind" => new ElementGuardUp(Element.Wind),
            "ElementGuardUp/Light" => new ElementGuardUp(Element.Light),
            "ElementGuardUp/Dark" => new ElementGuardUp(Element.Dark),
            "ElementPowerDown/Fire" => new ElementPowerDown(Element.Fire),
            "ElementPowerDown/Water" => new ElementPowerDown(Element.Water),
            "ElementPowerDown/Wind" => new ElementPowerDown(Element.Wind),
            "ElementPowerDown/Light" => new ElementPowerDown(Element.Light),
            "ElementPowerDown/Dark" => new ElementPowerDown(Element.Dark),
            "ElementGuardDown/Fire" => new ElementGuardDown(Element.Fire),
            "ElementGuardDown/Water" => new ElementGuardDown(Element.Water),
            "ElementGuardDown/Wind" => new ElementGuardDown(Element.Wind),
            "ElementGuardDown/Light" => new ElementGuardDown(Element.Light),
            "ElementGuardDown/Dark" => new ElementGuardDown(Element.Dark),
            "DamageUp" => new DamageUp(),
            "SupportUp" => new SupportUp(),
            "RecoveryUp" => new RecoveryUp(),
            "MpCostDown" => new MpCostDown(),
            "RangeUp" => new RangeUp(1),
            _ => throw new NotImplementedException($"{effect}"),
        }).ToArray(),
        level switch
        {
            "NoLevel" => Level.NoLevel,
            "One" => Level.One,
            "Two" => Level.Two,
            "Three" => Level.Three,
            "ThreePlus" => Level.ThreePlus,
            "Four" => Level.Four,
            "FourPlus" => Level.FourPlus,
            "Five" => Level.Five,
            "FivePlus" => Level.FivePlus,
            "LG" => Level.Lg,
            "LG+" => Level.LgPlus,
            _ => throw new NotImplementedException(),
        }
    );
}

public record Data(MemoriaPOCO[] data);
