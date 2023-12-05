using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DynamicData.Kernel;
using mitama.Algorithm;
using mitama.Domain;
using mitama.Pages.DeckBuilder;

namespace mitama.Pages.RegionConsole;

internal partial class BttaleLogParser
{
    internal record RegionHint(string Name, List<string> Members);

    internal record Hints(RegionHint Ally, RegionHint Opponent);

    private static Player ParsePlayer(string raw)
    {
        Regex regex = SourceRegex();
        return new Player(regex.Match(raw).Groups["region"].Value, regex.Match(raw).Groups["player"].Value);
    }

    internal static Optional<Fragment> ParseFragment(string[] line)
    {
        return line[7] switch
        {
            "行動および事象" => new Event(line[8]),
            "結果" => new Result(line[8]),
            "発動したレギオンマッチ補助スキル" => new Support(line[8]),
            "発動したレジェンダリースキル" => new Regendary(line[8]),
            _ => Optional<Fragment>.None,
        };
    }

    internal static Optional<BattleLogItem> ParseAll(string[] line, Hints hints)
    {
        var parsedPlayer = ParsePlayer(line[5]);
        if (parsedPlayer.Name == string.Empty)
        {
            return Optional<BattleLogItem>.None;
        }
        var allyCandidate = hints.Ally.Members.Select(player =>
        {
            var rate = 
                Algo.LevenshteinRate(player, parsedPlayer.Name)
              + Algo.LevenshteinRate(hints.Ally.Name, parsedPlayer.Region);
            return (rate, player);
        }).MinBy(item => item.rate);
        var opponentCandidate = hints.Opponent.Members.Select(player =>
        {
            var rate =
                Algo.LevenshteinRate(player, parsedPlayer.Name)
              + Algo.LevenshteinRate(hints.Opponent.Name, parsedPlayer.Region);
            return (rate, player);
        }).MinBy(item => item.rate);

        Source source = allyCandidate.rate < opponentCandidate.rate
            ? new Ally(new Player(hints.Ally.Name, allyCandidate.player))
            : new Opponent(new Player(hints.Opponent.Name, opponentCandidate.player));

        TimeOnly time = TimeOnly.Parse(line[6].Split(" ")[1]);
        return line[7] switch
        {
            "行動および事象" => new BattleLogItem(source, time, [new Event(line[8])]),
            "結果" => new BattleLogItem(source, time, [new Result(line[8])]),
            "発動したレギオンマッチ補助スキル" => new BattleLogItem(source, time, [new Support(line[8])]),
            "発動したレジェンダリースキル" => new BattleLogItem(source, time, [new Regendary(line[8])]),
            _ => Optional<BattleLogItem>.None,
        };
    }

    [GeneratedRegex(@"^\[(?<region>.+)\] (?<player>.*)$")]
    private static partial Regex SourceRegex();

    internal static EventDetail ParseEvent(string content)
    {
        var memoriaRegex = MemoriaRegex();
        var prepareOrderRegex = PrepareOrderRegex();
        var activateOrderRegex = ActivateOrderRegex();
        var rareSkillRegex = RareSkillRegex();

        if (content.Contains("ユニットを変更"))
        {
            return new UnitChange();
        }
        else if (content.Contains("クリティカル発生"))
        {
            return new Critical();
        }
        else if (memoriaRegex.IsMatch(content))
        {
            var match = memoriaRegex.Match(content);
            var name = match.Groups["memoria"].Value;
            var skill = match.Groups["skill"].Value;
            var memoria = Memoria
                    .List
                    .MinBy(memoria =>
                    {
                        return Algo.LevenshteinRate(memoria.Name, name)
                             + Algo.LevenshteinRate(memoria.Skill.Name, skill);
                    });
            var concentration = match.Groups["level"].Value switch
            {
                "15" => 0,
                "16" => 1,
                "17" => 2,
                "18" => 3,
                "20" => 4,
                _ => throw new UnreachableException("CRITICAL ERROR!"),
            };
            return new UseMemoria(new(memoria, concentration));
        }
        else if (prepareOrderRegex.IsMatch(content))
        {
            var match = prepareOrderRegex.Match(content);
            var order = match.Groups["order"].Value;
            return new PrepareOrder(Order.List.MinBy(o => Algo.LevenshteinRate(order, o.Name)));
        }
        else if (activateOrderRegex.IsMatch(content))
        {
            var match = activateOrderRegex.Match(content);
            var order = match.Groups["order"].Value;
            return new ActivateOrder(Order.List.MinBy(o => Algo.LevenshteinRate(order, o.Name)));
        }
        else if (rareSkillRegex.IsMatch(content))
        {
            var match = rareSkillRegex.Match(content);
            var skill = match.Groups["skill"].Value;
            return new UseSkill(skill);
        }
        else if (content.Contains("スタンバイフェーズ"))
        {
            return new StandBy();
        }
        else if (content.Contains("復活"))
        {
            return new Revival();
        }
        else if (content.Contains("マギ"))
        {
            return new NoenWelt(content);
        }
        else
        {
            return new Error();
        }
    }

    public static MemoriaWithConcentration[] ExtractMemoria(BattleLogItem item)
    {
        var memoriaRegex = MemoriaRegex();
        return item.Fragments
            .Where(fragment => memoriaRegex.IsMatch(fragment.Content))
            .Select(fragment =>
            {
                var match = memoriaRegex.Match(fragment.Content);
                var name = match.Groups["memoria"].Value;
                var skill = match.Groups["skill"].Value;
                var memoria = Memoria
                        .List
                        .MinBy(memoria =>
                        {
                            return Algo.LevenshteinRate(memoria.Name, Fix(name))
                                 + Algo.LevenshteinRate(memoria.Skill.Name, skill);
                        });
                var concentration = match.Groups["level"].Value switch
                {
                    "15" => 0,
                    "16" => 1,
                    "17" => 2,
                    "18" => 3,
                    "20" => 4,
                    _ => throw new UnreachableException("CRITICAL ERROR!"),
                };
                return new MemoriaWithConcentration(memoria, concentration);
            })
            .ToArray();
    }

    private static string Fix(string name)
    {
        var creatorRegex = CreatorRegex();
        var ultimateRegex = UltimateRegex();
        var emotionalRegex = EmotionalRegex();
        if (creatorRegex.IsMatch(name))
        {
            return creatorRegex.Match(name).Groups["name"].Value;
        }
        else if (ultimateRegex.IsMatch(name))
        {
            return ultimateRegex.Match(name).Groups["name"].Value;
        }
        else if (emotionalRegex.IsMatch(name))
        {
            return emotionalRegex.Match(name).Groups["name"].Value;
        }
        else
        {
            return name;
        }
    }

    [GeneratedRegex(@"^「(?<memoria>.+?)」 *の *「(?<skill>.+?)」 *Lv(?<level>\d{2}) *が発動$")]
    private static partial Regex MemoriaRegex();

    [GeneratedRegex(@"^.+が「(?<order>.+?)」 *の発動準備を開始$")]
    private static partial Regex PrepareOrderRegex();

    [GeneratedRegex(@"^.+が「(?<order>.+?)」 *を発動$")]
    private static partial Regex ActivateOrderRegex();

    [GeneratedRegex(@"^.+ *が レアスキル「(?<skill>.+?)」 *を発動$")]
    private static partial Regex RareSkillRegex();

    [GeneratedRegex(@"^クリエイターズコラボ *-(?<name>.+?)-$")]
    private static partial Regex CreatorRegex();

    [GeneratedRegex(@"^Ultimate Memoria *-(?<name>.+?)-$")]
    private static partial Regex UltimateRegex();

    [GeneratedRegex(@"^Emotional Memoria *-(?<name>.+?)-$")]
    private static partial Regex EmotionalRegex();
}
