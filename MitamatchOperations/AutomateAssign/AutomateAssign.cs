using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using mitama.Pages.OrderConsole;
using mitama.Pages.Common;

namespace mitama.AutomateAssign;

internal abstract record AutomateAssignResult {
    public static Success Success => new();
    public static Failure Failure(string msg) => new(msg);
}
internal record Success : AutomateAssignResult;
internal record Failure(string Msg) : AutomateAssignResult;


internal class AutomateAssign {
    internal static AutomateAssignResult ExecAutoAssign(string region, ref ObservableCollection<TimeTableItem> timeTable) {
        var list = timeTable.ToList();
        List<List<int>> result = [];

        var memberInfo = Util.LoadMembersInfo(region);
        var intoFlags = memberInfo
            .Select((item, index) => (item, index))
            .ToDictionary(
                x => x.index,
                x => list
                    .Select((item, index) => x.item.OrderIndices.Contains(item.Order.Index) ? (1 << index) : 0)
                    .Sum()
            );
        // クロノグラフ (Index = 17) の位置を取得
        var chronoIndex = timeTable.Select(item => item.Order.Index).ToList().IndexOf(17);

        // クロノグラフでオーダーを分割
        var beforeChrono = list.GetRange(0, chronoIndex);
        var afterChrono  = list.GetRange(chronoIndex + 1, list.Count - chronoIndex - 1);

        for (int chrono = 0; chrono < 9; chrono++)
        {
            List<List<int>> beforeCandidates = [];
            foreach (var pics in Permutation(Enumerable.Range(0, 9), beforeChrono.Count))
            {
                if (pics.Contains(chrono)) continue;
                var check = pics.Select((pic, index) => (pic, index)).All(x => (intoFlags[x.pic] & (1 << x.index)) != 0);
                if (check)
                {
                    beforeCandidates.Add([.. pics]);
                }
            }

            List<List<int>> afterCandidates = [];
            foreach (var pics in Permutation(Enumerable.Range(0, 9), afterChrono.Count))
            {
                if (pics.Contains(chrono)) continue;
                var check = pics.Select((pic, index) => (pic, index)).All(x => (intoFlags[x.pic] & (1 << (x.index + beforeChrono.Count + 1))) != 0);
                if (check)
                {
                    afterCandidates.Add([.. pics]);
                }
            }

            foreach (var before in beforeCandidates)
            {
                foreach (var after in afterCandidates)
                {
                    result.Add([.. before, chrono, .. after]);
                }
            }
        }
        Random engine = new();
        var picked = engine.Next(result.Count);
        foreach (var (pic, index) in result[picked].Select((x, i) => (x, i)))
        {
            timeTable[index] = timeTable[index] with { Pic = memberInfo[pic].Name };
        }
        return AutomateAssignResult.Success;
    }

    private static IEnumerable<T[]> Permutation<T>(IEnumerable<T> items, int k)
    {
        if (k == 1)
        {
            foreach (var item in items)
            {
                yield return new T[] { item };
            }
            yield break;
        }
        foreach (var item in items)
        {
            var leftside = new T[] { item };
            var unused = items.Except(leftside);
            foreach (var rightside in Permutation(unused, k - 1))
            {
                yield return leftside.Concat(rightside).ToArray();
            }
        }
    }
}
