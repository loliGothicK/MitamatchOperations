using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using mitama.Pages.DeckBuilder;
using mitama.Pages.RegionConsole;
using SimdLinq;

namespace mitama.Domain;

public record struct Player(string Region, string Name);

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
    public override SourceKind Kind => SourceKind.Ally;
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

public record Support(string Value): Fragment
{
    public override FragmentKind Kind => FragmentKind.Support;
    public override string Content => Value;
}
public record Regendary(string Value): Fragment
{
    public override FragmentKind Kind => FragmentKind.Regendary;
    public override string Content => Value;
}

public record BattleLogItem(Source Source, TimeOnly Time, List<Fragment> Fragments);

public record BattleLog(List<BattleLogItem> Data)
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

    public Task<List<Unit>> ExtractUnits(string player)
    {
        var events = Data
            .Where(log => log.Source.Content.Name == player)
            .Where(log =>
            {
                return BttaleLogParser.ParseEvent(log.Fragments[0].Content) is UseMemoria or UnitChange;
            })
            .ToList();

        var sprints = Helper.SplitList(events, e => e.Fragments[0].Content.Contains("ユニットを変更"));
        return Task.FromResult(sprints
            .Select((sprint, index) =>
            {
                HashSet<MemoriaWithConcentration> memorias = [];
                foreach (var memoria in sprint.SelectMany(e => BttaleLogParser.ExtractMemoria(e)))
                {
                    memorias.Add(memoria);
                }
                List<MemoriaWithConcentration> list = [.. memorias];
                return new Unit($"Unit-{index+1}", Costume.DummyVanguard.CanBeEquipped(list[0].Memoria), [.. list.DistinctBy(m => m.Memoria.Name)]);
            }).ToList());
    }
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
public record NoenWelt(string Raw): EventDetail;
public record Error : EventDetail;

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