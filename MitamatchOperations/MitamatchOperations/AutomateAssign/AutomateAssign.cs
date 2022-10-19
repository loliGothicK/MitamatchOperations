using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using mitama.Pages.OrderConsole;
using mitama.Domain;
using System.IO;
using System.Text;
using WinRT;

namespace mitama.AutomateAssign;

internal abstract record AutomateAssignResult
{
    public static Success Success => new();
    public static Failure Failure(string msg) => new(msg);
}
internal record Success : AutomateAssignResult;
internal record Failure(string Msg) : AutomateAssignResult;

internal class AutomateAssign
{
    internal static AutomateAssignResult ExecAutoAssign(string region, ref ObservableCollection<TimeTableItem> timeTable)
    {
        var orderIndexToTimeTableIndexMap = new Dictionary<ushort, int>();
        var stat = new Dictionary<ushort, PicStat>();

        foreach (var (number, (item, index)) in timeTable.Select((item, index) => (item, index)).ToDictionary(tuple => tuple.item.Order.Index))
        {
            orderIndexToTimeTableIndexMap[number] = index;
            stat[number] = item.Pic == "" ? new NotAssigned() : new Assigned(item.Pic);
        }

        var orderList = timeTable.Select(item => item.Order).ToArray();

        if (orderList.Length == 0) return AutomateAssignResult.Success;

        var (result, msg) = orderList.Any(order => order.Name == "刻戻りのクロノグラフ") switch
        {
            true => Chronograph(region, stat, orderList.AsReadOnly()),
            false => NoChronograph(region, stat, orderList.AsReadOnly()),
        };

        if (msg != string.Empty) return AutomateAssignResult.Failure(msg);

        foreach (var (index, pic) in result)
        {
            var item = timeTable[orderIndexToTimeTableIndexMap[index]];
            timeTable[orderIndexToTimeTableIndexMap[index]] = pic switch
            {
                Assigned(var name) => item with { Pic = name },
                _ => throw new Exception("割当漏れが発生しました"),
            };
        }

        return AutomateAssignResult.Success;
    }

    private abstract record Assginability : IComparable<Assginability>
    {
        public abstract bool IsAssignableBefore();
        public abstract bool IsAssignableAfter();

        public static Assginability operator +(Assginability assginability, Before _) => assginability switch
        {
            Assignable => Assign.After,
            Before => new Unassignable(),
            After => Assign.After,
            Unassignable => new Unassignable(),
            _ => throw new ArgumentOutOfRangeException(nameof(assginability), assginability, null)
        };
        public static Assginability operator +(Assginability assginability, After _) => assginability switch
        {
            Assignable => Assign.Before,
            Before => Assign.Before,
            After => new Unassignable(),
            Unassignable => new Unassignable(),
            _ => throw new ArgumentOutOfRangeException(nameof(assginability), assginability, null)
        };

        public abstract int CompareTo(Assginability? other);
    }
    private record Assignable : Assginability, IComparable<Assginability>
    {
        public override bool IsAssignableBefore() => true;

        public override bool IsAssignableAfter() => true;
        public override int CompareTo(Assginability? other)
        {
            if (other == null) return 1;
            return other switch
            {
                Assignable => 0,
                _ => -1,
            };
        }
    }

    private record Before : Assginability, IComparable<Assginability>
    {
        public override bool IsAssignableBefore() => true;

        public override bool IsAssignableAfter() => false;
        public override int CompareTo(Assginability? other)
        {
            if (other == null) return 1;
            return other switch
            {
                Assignable => 1,
                Before => 0,
                _ => -1,
            };
        }
    }

    private record After : Assginability, IComparable<Assginability>
    {
        public override bool IsAssignableBefore() => false;

        public override bool IsAssignableAfter() => true;
        public override int CompareTo(Assginability? other)
        {
            if (other == null) return 1;
            return other switch
            {
                Assignable or Before => 1,
                After => 0,
                _ => -1,
            };
        }
    }

    private record Unassignable : Assginability, IComparable<Assginability>
    {
        public override bool IsAssignableBefore() => false;

        public override bool IsAssignableAfter() => false;
        public override int CompareTo(Assginability? other)
        {
            if (other == null) return 1;
            return other switch
            {
                Unassignable => 0,
                _ => 1,
            };
        }
    }

    private class Assign
    {
        public static Before Before => new();
        public static After After => new();
    }


    private static (Dictionary<ushort, PicStat>, string) Chronograph(string region, Dictionary<ushort, PicStat> stats, ReadOnlyCollection<Order> list)
    {
        var result = stats;

        // クロノグラフ: Index = 17
        var chronographIndex = list.IndexOf(Order.List[17]);

        // クロノグラフの前後に分割する
        var beforeReset = list
            .ToList()
            .GetRange(0, chronographIndex)
            .Where(o => stats[o.Index] is NotAssigned)
            .ToList();
        var afterReset = list
            .ToList()
            .GetRange(chronographIndex + 1, list.Count - chronographIndex - 1)
            .Where(o => stats[o.Index] is NotAssigned)
            .ToList();

        var members = LoadRegionMemberInformation(region);

        // オーダー担当可能数
        var assginabilities = new Dictionary<string, Assginability>();

        // オーダー担当可能数の初期化
        foreach (var member in members)
        {
            assginabilities[member.Name] = new Assignable();
        }

        foreach (var name in list
                     .ToList()
                     .GetRange(0, chronographIndex)
                     .Where(o => stats[o.Index] is Assigned)
                     .Select(o => stats[o.Index].As<Assigned>().Name))
        {
            assginabilities[name] += Assign.Before;
        }
        foreach (var name in list
                     .ToList()
                     .GetRange(chronographIndex + 1, list.Count - chronographIndex - 1)
                     .Where(o => stats[o.Index] is Assigned)
                     .Select(o => stats[o.Index].As<Assigned>().Name))
        {
            assginabilities[name] += Assign.After;
        }

        if (stats[17] is Assigned(var cName))
        {
            assginabilities[cName] = new Unassignable();
        }
        var nAttackers = members.Where(m => m.Position is Front { Category: FrontCategory.Normal } && assginabilities[m.Name] is not Unassignable).ToList();
        var spAttackers = members.Where(m => m.Position is Front { Category: FrontCategory.Special } && assginabilities[m.Name] is not Unassignable).ToList();
        var buffers = members.Where(m => m.Position is Back { Category: BackCategory.Buffer } && assginabilities[m.Name] is not Unassignable).ToList();
        var debuffers = members.Where(m => m.Position is Back { Category: BackCategory.DeBuffer } && assginabilities[m.Name] is not Unassignable).ToList();
        var healers = members.Where(m => m.Position is Back { Category: BackCategory.Healer } && assginabilities[m.Name] is not Unassignable).ToList();

        List<Domain.Member> Attackers() => nAttackers!.Concat(spAttackers!).ToList();
        List<Domain.Member> BuffDebuff() => buffers!.Concat(debuffers!).ToList();

        bool Check()
        {
            foreach (var healer in healers.Where(healer => assginabilities[healer.Name] is Unassignable).ToList())
            {
                healers.Remove(healer);
            }
            foreach (var nAttacker in nAttackers.Where(nAttacker => assginabilities[nAttacker.Name] is Unassignable).ToList())
            {
                nAttackers.Remove(nAttacker);
            }
            foreach (var spAttacker in spAttackers.Where(spAttacker => assginabilities[spAttacker.Name] is Unassignable).ToList())
            {
                spAttackers.Remove(spAttacker);
            }
            foreach (var buffer in buffers.Where(buffer => assginabilities[buffer.Name] is Unassignable).ToList())
            {
                buffers.Remove(buffer);
            }
            foreach (var debuffer in debuffers.Where(debuffer => assginabilities[debuffer.Name] is Unassignable).ToList())
            {
                debuffers.Remove(debuffer);
            }
            healers = healers.OrderBy(healer => assginabilities[healer.Name]).ToList();
            nAttackers = nAttackers.OrderBy(attacker => assginabilities[attacker.Name]).ToList();
            spAttackers = spAttackers.OrderBy(spAttacker => assginabilities[spAttacker.Name]).ToList();
            buffers = buffers.OrderBy(buffer => assginabilities[buffer.Name]).ToList();
            debuffers = debuffers.OrderBy(debuffer => assginabilities[debuffer.Name]).ToList();
            return result.Values.All(pic => pic is Assigned);
        }

        // 初手オーダーを優先して前衛に割り当てる
        if (result[list[0].Index] is NotAssigned)
        {
            var attacker = Attackers().FirstOrDefault(attacker => attacker.OrderIndices.Contains(list[0].Index));
            if (attacker == null) return (result, "理論的に不可能な編成をするな 1");
            beforeReset.Remove(list[0]);
            result[list[0].Index] = new Assigned(attacker.Name);
            assginabilities[attacker.Name] += Assign.Before;
            if (Check()) return (result, string.Empty);
        }

        // 回復はクロノグラフを割り当てるべき
        if (stats[17] is NotAssigned)
        {
            var healer = healers.FirstOrDefault(healer => assginabilities[healer.Name] is Assignable && healer.OrderIndices.Contains((ushort)17));
            if (healer == null) return (result, "回復にクロノグラフを割り当ててください（過激派）");
            assginabilities[healer.Name] = new Unassignable();
            healers.Remove(healer);
            result[17] = new Assigned(healer.Name);
            if (Check()) return (result, string.Empty);
        }

        // 最後のオーダーを優先して支援妨害に割り当てる
        if (result[list.Last().Index] is NotAssigned)
        {
            var other = BuffDebuff().FirstOrDefault(other => other.OrderIndices.Contains(list.Last().Index));
            if (other == null) return (result, "最後のオーダーは支援妨害に割り当ててください");
            afterReset.Remove(list.Last());
            result[list.Last().Index] = new Assigned(other.Name);
            assginabilities[other.Name] += Assign.After;
            if (Check()) return (result, string.Empty);
        }

        // 最後のオーダーの前が施術加速の場合も優先して妨害に割り当てる
        // Note (2022-10-19):
        //  戦術加速は現状総攻撃でトップだが、一応分けて記述しておく
        if (list[^2].Name == "戦術加速の陣" && result[list[list.Last().Index - 1].Index] is NotAssigned)
        {
            var debuffer = debuffers.FirstOrDefault(debuffer => debuffer.OrderIndices.Contains((ushort)52));
            if (debuffer == null) return (result, "最後の直前の戦術加速は支援妨害に割り当ててください");
            afterReset.Remove(list[^2]);
            result[list[^2].Index] = new Assigned(debuffer.Name);
            assginabilities[debuffer.Name] += Assign.After;
            if (Check()) return (result, string.Empty);
        }

        // 両攻撃系を妨害に割り当てる
        foreach (var order in beforeReset.Where(order => Order.AtkSumTop5.Contains(order) && result[order.Index] is NotAssigned).ToList())
        {
            var debuffer = debuffers.FirstOrDefault(debuffer => debuffer.OrderIndices.Contains(order.Index));
            if (debuffer == null) continue;
            beforeReset.Remove(order);
            result[order.Index] = new Assigned(debuffer.Name);
            assginabilities[debuffer.Name] += Assign.Before;
            if (Check()) return (result, string.Empty);
        }

        foreach (var order in afterReset.Where(order => Order.AtkSumTop5.Contains(order) && result[order.Index] is NotAssigned).ToList())
        {
            var debuffer = debuffers.FirstOrDefault(debuffer => debuffer.OrderIndices.Contains(order.Index));
            if (debuffer == null) continue;
            afterReset.Remove(order);
            result[order.Index] = new Assigned(debuffer.Name);
            assginabilities[debuffer.Name] += Assign.After;
            if (Check()) return (result, string.Empty);
        }

        #region 回復への割当
        // 次に御ステが最も高いオーダーをそれぞれ前後から回復に割り当てる
        for (var i = healers.Count; i > 0; i--)
        {
            foreach (var candidate in beforeReset.OrderByDescending(order => order.Status.Def + order.Status.SpDef).ToList())
            {
                var healer = healers.FirstOrDefault(healer => healer.OrderIndices.Contains(candidate.Index));
                if (healer == null) continue;
                beforeReset.Remove(candidate);
                result[candidate.Index] = new Assigned(healer.Name);
                assginabilities[healer.Name] += Assign.Before;
                if (Check()) return (result, string.Empty);
                break;
            }
        }

        for (var i = healers.Count; i > 0; i--)
        {
            foreach (var candidate in afterReset.OrderByDescending(order => order.Status.Def + order.Status.SpDef).ToList())
            {
                var healer = healers.FirstOrDefault(healer => healer.OrderIndices.Contains(candidate.Index));
                if (healer == null) continue;
                afterReset.Remove(candidate);
                result[candidate.Index] = new Assigned(healer.Name);
                assginabilities[healer.Name] += Assign.After;
                if (Check()) return (result, string.Empty);
                break;
            }
        }

        if (healers.Count > 0)
            return (result, @$"{string.Join("さん, ", healers.Select(healer => healer.Name))} はオーダーを買ってください");
        #endregion


        // 回復以外の後衛でオーダーをまかないきれない場合:
        // まかないきれないぶんは攻撃が高いものを優先して前衛に割り当てる

        if (beforeReset.Count > BuffDebuff().Count(p => assginabilities[p.Name].IsAssignableBefore()))
        {
            var required = beforeReset.Count - BuffDebuff().Count(p => assginabilities[p.Name].IsAssignableBefore());
            for (; required > 0; required--)
            {
                var attacker = Attackers().FirstOrDefault(
                    attacker => assginabilities[attacker.Name].IsAssignableBefore()
                );
                if (attacker == null) return (result, "理論的に不可能な編成をするな 2");
                var candidate = beforeReset.MaxBy(order => attacker.Position.As<Front>().Category switch
                {
                    FrontCategory.Normal => order.Status.Atk,
                    FrontCategory.Special => order.Status.SpAtk,
                });
                beforeReset.Remove(candidate);
                result[candidate.Index] = new Assigned(attacker.Name);
                assginabilities[attacker.Name] += Assign.Before;
                Check();
            }
        }

        if (afterReset.Count > BuffDebuff().Count(p => assginabilities[p.Name].IsAssignableAfter()))
        {
            var required = afterReset.Count - BuffDebuff().Count(p => assginabilities[p.Name].IsAssignableAfter());
            for (; required > 0; required--)
            {
                var attacker = Attackers().FirstOrDefault(attacker => assginabilities[attacker.Name].IsAssignableAfter());
                if (attacker == null) return (result, "理論的に不可能な編成をするな 3");
                var candidate = afterReset.MaxBy(order => attacker.Position.As<Front>().Category switch
                {
                    FrontCategory.Normal => order.Status.Atk,
                    FrontCategory.Special => order.Status.SpAtk,
                });
                afterReset.Remove(candidate);
                result[candidate.Index] = new Assigned(attacker.Name);
                assginabilities[attacker.Name] += Assign.After;
                Check();
            }
        }

        // 最後に支援妨害に適当に割り当ててみる
        while (beforeReset.Count > 0)
        {
            var other = BuffDebuff().FirstOrDefault(other => assginabilities[other.Name].IsAssignableBefore());
            if (other == null) return (result, "理論的に不可能な編成をするな 4");
            var candidate = beforeReset.First();
            beforeReset.Remove(candidate);
            result[candidate.Index] = new Assigned(other.Name);
            assginabilities[other.Name] += Assign.Before;
            Check();
        }

        while (afterReset.Count > 0)
        {
            var other = BuffDebuff().FirstOrDefault(other => assginabilities[other.Name].IsAssignableAfter());
            if (other == null) return (result, "理論的に不可能な編成をするな 5");
            var candidate = afterReset.First();
            afterReset.Remove(candidate);
            result[candidate.Index] = new Assigned(other.Name);
            assginabilities[other.Name] += Assign.After;
            Check();
        }

        return (result, Check() ? string.Empty : "");
    }

    private static Domain.Member[] LoadRegionMemberInformation(string region)
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        return Directory.GetFiles(@$"{desktop}\MitamatchOperations\Regions\{region}", "*.json").Select(path =>
        {
            using var sr = new StreamReader(path);
            var json = sr.ReadToEnd();
            return Domain.Member.FromJson(json);
        }).ToArray();
    }

    private static (Dictionary<ushort, PicStat>, string) NoChronograph(string region,
        Dictionary<ushort, PicStat> stats, ReadOnlyCollection<Order> list)
    {
        throw new NotImplementedException();
    }

    private abstract record PicStat;

    private record Assigned(string Name) : PicStat;
    private record NotAssigned : PicStat;
}