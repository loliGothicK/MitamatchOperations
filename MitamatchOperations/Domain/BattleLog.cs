using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mitama.Pages.LegionConsole;

namespace Mitama.Domain;

public record struct Player(string Legion, string Name);
public record struct UnitChangePoint(string Name, TimeOnly Time);

public enum SourceKind
{
    Ally,
    Opponent,
}

public abstract record Source
{
    public virtual SourceKind Kind { get; }
    public virtual Player Content { get; }
}
public record Ally(Player Player) : Source
{
    public override SourceKind Kind => SourceKind.Ally;
    public override Player Content => Player;
}
public record Opponent(Player Player) : Source
{
    public override SourceKind Kind => SourceKind.Opponent;
    public override Player Content => Player;
}

public enum FragmentKind
{
    Event,
    Result,
    Support,
    Regendary,
}

public abstract record Fragment
{
    public virtual FragmentKind Kind { get; }
    public virtual string Content { get; }
}
public record Event(string Value) : Fragment
{
    public override FragmentKind Kind => FragmentKind.Event;
    public override string Content => Value;
}

public record Result(string Value) : Fragment
{
    public override FragmentKind Kind => FragmentKind.Result;
    public override string Content => Value;
}

public record Support(string Value) : Fragment
{
    public override FragmentKind Kind => FragmentKind.Support;
    public override string Content => Value;
}
public record Regendary(string Value) : Fragment
{
    public override FragmentKind Kind => FragmentKind.Regendary;
    public override string Content => Value;
}

public record BattleLogItem(Source Source, TimeOnly Time, List<Fragment> Fragments);

public partial record BattleLog(List<BattleLogItem> Data)
{
    private record struct SourceDto(SourceKind Kind, Player Content);
    private record struct FragmentDto(FragmentKind Kind, string Content);
    private record struct BattleLogItemDto(SourceDto Source, TimeOnly Time, List<FragmentDto> Fragments);

    private static BattleLogItem DtoToItem(BattleLogItemDto dto)
    {
        var fragments = dto.Fragments.Select<FragmentDto, Fragment>(fragment => fragment.Kind switch
        {
            FragmentKind.Event => new Event(fragment.Content),
            FragmentKind.Result => new Result(fragment.Content),
            FragmentKind.Support => new Support(fragment.Content),
            FragmentKind.Regendary => new Regendary(fragment.Content),
            _ => throw new UnreachableException("CRITICAL ERROR!"),
        }).ToList();

        return dto.Source.Kind switch
        {
            SourceKind.Ally => new BattleLogItem(new Ally(dto.Source.Content), dto.Time, fragments),
            SourceKind.Opponent => new BattleLogItem(new Opponent(dto.Source.Content), dto.Time, fragments),
            _ => throw new UnreachableException("CRITICAL ERROR!"),
        };
    }
    private record struct BattleLogDto(List<BattleLogItemDto> Data);

    public static BattleLog FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<BattleLogDto>(json);
        return new BattleLog(dto.Data.Select(DtoToItem).ToList());
    }

    public (Player[], Player[]) ExtractPlayers()
    {
        var ally = Data
            .Select(log => log.Source)
            .Where(src => src is Ally)
            .Select(src => src.Content)
            .Distinct()
            .ToArray();
        var opponent = Data
            .Select(log => log.Source)
            .Where(src => src is Opponent)
            .Select(src => src.Content)
            .Distinct()
            .ToArray();
        return (ally, opponent);
    }

    public Task<(List<OrderIndexAndTime>, List<OrderIndexAndTime>)> ExtractOrders()
    {
        List<OrderIndexAndTime> ally = [];
        List<OrderIndexAndTime> opponent = [];

        foreach (var (time, source, order) in Data
            .Select(item => (item.Time, item.Source, BattleLogParser.ParseEvent(item.Fragments[0].Content)))
            .Where(pair => pair.Item3 is PrepareOrder)
            .Select(pair => (pair.Time, pair.Source, (pair.Item3 as PrepareOrder).Order)))
        {
            switch (source.Kind)
            {
                case SourceKind.Ally:
                    ally.Add(new(order.Index, time));
                    break;
                case SourceKind.Opponent:
                    opponent.Add(new(order.Index, time));
                    break;
                default:
                    throw new UnreachableException("CRITICAL ERROR!");
            }
        }

        return Task.FromResult((
            ally.DistinctBy(item => item.Index).ToList(),
            opponent.DistinctBy(item => item.Index).ToList()
        ));
    }

    public Task<List<Unit>> ExtractUnits(string player)
    {
        var events = Data
            .Where(log => log.Source.Content.Name == player)
            .Where(log =>
            {
                return BattleLogParser.ParseEvent(log.Fragments[0].Content) is UseMemoria or UnitChange;
            })
            .ToList();

        var sprints = Helper.SplitList(events, e => e.Fragments[0].Content.Contains("ユニット"));
        return Task.FromResult(sprints
            .Select((sprint, index) =>
            {
                HashSet<MemoriaWithConcentration> memorias = [];
                foreach (var item in sprint.Select(item => BattleLogParser.ParseEvent(item.Fragments[0].Content)))
                {
                    switch (item)
                    {
                        case UseMemoria memoria:
                            memorias.Add(memoria.Memoria);
                            break;
                        default: continue;
                    }
                }
                var vanguard = 0;
                var rearguard = 0;
                foreach (var kind in memorias.Select(memoria => memoria.Memoria.Kind))
                {
                    switch (kind)
                    {
                        case Vanguard:
                            vanguard++;
                            break;
                        case Rearguard:
                            rearguard++;
                            break;
                        default:
                            throw new UnreachableException("CRITICAL ERROR!");
                    }
                }
                foreach (var memoria in sprint.SelectMany(e => BattleLogParser.ExtractMemoria(e, vanguard > rearguard)).Where(m => m.Memoria is not null))
                {
                    memorias.Add(memoria);
                }
                List<MemoriaWithConcentration> list = [.. memorias];
                return new Unit($"Unit-{index + 1}", vanguard > rearguard, [.. list.DistinctBy(m => m.Memoria.Name)]);
            }).ToList());
    }

    public Dictionary<string, (TimeOnly Time, StatusIncreaseDecrease Stat)[]> ExtractIncreaseDecrease()
    {
        var StatRegex = StatusRegex();
        var standbyRegex = StandByRegex();
        var data = Data
            .SelectMany(log => log.Fragments.Select(Fragment => (log.Time, Fragment)))
            .Where(item => item.Fragment.Kind is FragmentKind.Result or FragmentKind.Event)
            .Where(item => StatRegex.IsMatch(item.Fragment.Content) || standbyRegex.IsMatch(item.Fragment.Content))
            .Select(item =>
            {
                if (standbyRegex.IsMatch(item.Fragment.Content))
                {
                    var match = standbyRegex.Match(item.Fragment.Content);
                    var status = match.Groups["status"].Value;
                    var Player = "スタンバイフェーズ";
                    return (Player, item.Time, new StatusIncreaseDecrease(new StandByPhase(status switch
                    {
                        "ATK" => new Attack(5000),
                        "Sp.ATK" => new SpecialAttack(5000),
                        "DEF" => new Defense(5000),
                        "Sp.DEF" => new SpecialDefense(5000),
                        _ => throw new NotImplementedException(),
                    })));
                }
                else
                {
                    var match = StatRegex.Match(item.Fragment.Content);
                    var Player = match.Groups["player"].Value;
                    var status = match.Groups["status"].Value;
                    var value = match.Groups["value"].Value.Replace(",", string.Empty).Replace(".", string.Empty);
                    var iord = match.Groups["iord"].Value switch
                    {
                        "増加" => 1,
                        "減少" => -1,
                        _ => throw new UnreachableException("CRITICAL ERROR!"),
                    };

                    return (Player, item.Time, new StatusIncreaseDecrease(status switch
                    {
                        "ATK" => new Attack(iord * int.Parse(value)),
                        "Sp.ATK" => new SpecialAttack(iord * int.Parse(value)),
                        "DEF" => new Defense(iord * int.Parse(value)),
                        "Sp.DEF" => new SpecialDefense(iord * int.Parse(value)),
                        "風属性攻撃" => new WindAttack(iord * int.Parse(value)),
                        "風属性防御" => new WindDefense(iord * int.Parse(value)),
                        "火属性攻撃" => new FireAttack(iord * int.Parse(value)),
                        "火属性防御" => new FireDefense(iord * int.Parse(value)),
                        "水属性攻撃" => new WaterAttack(iord * int.Parse(value)),
                        "水属性防御" => new WaterDefense(iord * int.Parse(value)),
                        "光属性攻撃" => new LightAttack(iord * int.Parse(value)),
                        "光属性防御" => new LightDefense(iord * int.Parse(value)),
                        "闇属性攻撃" => new DarkAttack(iord * int.Parse(value)),
                        "闇属性防御" => new DarkDefense(iord * int.Parse(value)),
                        "最大HP" => new MaxHp(int.Parse(value)),
                        _ => new ParseError(iord * int.Parse(value)),
                    }));
                }
            })
            .ToArray();

        Dictionary<string, (TimeOnly, StatusIncreaseDecrease)[]> res = [];
        var (allies, opponents) = ExtractPlayers();
        foreach (var player in allies)
        {
            res.Add(player.Name, data.Where(datum => datum.Player == player.Name || datum.Player == "スタンバイフェーズ").Select(datum => (datum.Time, datum.Item3)).ToArray());
        }
        foreach (var player in opponents)
        {
            res.Add(player.Name, data.Where(datum => datum.Player == player.Name || datum.Player == "スタンバイフェーズ").Select(datum => (datum.Time, datum.Item3)).ToArray());
        }
        return res;
    }

    public List<UnitChangePoint> ExtractUnitChanges()
    {
        List<UnitChangePoint> res = [];

        foreach (var (time, source) in Data
            .Select(item => (item.Time, item.Source, BattleLogParser.ParseEvent(item.Fragments[0].Content)))
            .Where(pair => pair.Item3 is UnitChange)
            .Select(pair => (pair.Time, pair.Source)))
        {
            if (time.Minute > 15) continue;
            switch (source.Kind)
            {
                case SourceKind.Opponent:
                    res.Add(new(source.Content.Name, time));
                    break;
                default:
                    break;
            }
        }
        return res
            .DistinctBy(item => $@"{item.Time}{item.Name}")
            .ToList();
    }

    [GeneratedRegex(@"^ *スタンバイフェーズにて *(?<status>.+?) *を増加$")]
    private static partial Regex StandByRegex();

    [GeneratedRegex(@"^ *(?<player>.+?) *の *(?<status>.+?) *が *(?<value>\d+([,.]\d+)+?).*(?<iord>増加|減少)$")]
    private static partial Regex StatusRegex();
}

public abstract record EventDetail;

public record UnitChange : EventDetail;
public record Critical : EventDetail;
public record UseMemoria(MemoriaWithConcentration Memoria) : EventDetail;
public record PrepareOrder(Order Order) : EventDetail;
public record ActivateOrder(Order Order) : EventDetail;
public record UseSkill(string Skill) : EventDetail;
public record StandBy : EventDetail;
public record Revival : EventDetail;
public record NoenWelt(string Raw) : EventDetail;
public record Error : EventDetail;

public abstract record ResultDetail;
public record Damage(int Value) : ResultDetail;
public record Healing(int Value) : ResultDetail;
public record StatusIncreaseDecrease(Status Value) : ResultDetail;

public abstract record Status;
public record Attack(int Value) : Status;
public record SpecialAttack(int Value) : Status;
public record Defense(int Value) : Status;
public record SpecialDefense(int Value) : Status;
public record WindAttack(int Value) : Status;
public record WindDefense(int Value) : Status;
public record FireAttack(int Value) : Status;
public record FireDefense(int Value) : Status;
public record WaterAttack(int Value) : Status;
public record WaterDefense(int Value) : Status;
public record LightAttack(int Value) : Status;
public record LightDefense(int Value) : Status;
public record DarkAttack(int Value) : Status;
public record DarkDefense(int Value) : Status;
public record MaxHp(int Value) : Status;
public record ParseError(int Value) : Status;
public record StandByPhase(Status Status) : Status;

public record struct AllStatus(
    int Attack,
    int SpecialAttack,
    int Defense,
    int SpecialDefense,
    int WindAttack,
    int WindDefense,
    int FireAttack,
    int FireDefense,
    int WaterAttack,
    int WaterDefense,
    int LightAttack,
    int LightDefense,
    int DarkAttack,
    int DarkDefense,
    int MaxHp
)
{
    public readonly string ToCSV()
    {
        return $"{Attack},{SpecialAttack},{Defense},{SpecialDefense},{WindAttack},{WindDefense},{FireAttack},{FireDefense},{WaterAttack},{WaterDefense},{LightAttack},{LightDefense},{DarkAttack},{DarkDefense},{MaxHp}";
    }
}

internal class Helper
{
    public static List<List<T>> SplitList<T>(List<T> source, Predicate<T> condition)
    {
        List<List<T>> result = [];

        int index = source.FindIndex(condition);

        if (index != -1)
        {
            // 条件に一致する要素を含む前の部分を結果に追加
            List<T> beforePart = source.GetRange(0, index);
            result.Add(beforePart);

            // 条件に一致する要素を含む後の部分を再帰的に処理
            List<T> afterPart = source.GetRange(index + 1, source.Count - index - 1);
            result.AddRange(SplitList(afterPart, condition));
        }
        else
        {
            // 条件に一致する要素が見つからなかった場合、元のリストをそのまま結果に追加
            result.Add(source);
        }

        return result.Where(list => list.Count != 0).ToList();
    }
}
