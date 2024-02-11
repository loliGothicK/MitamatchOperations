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
        var inCharge = list.Where(x => x.Pic != string.Empty).Select((_, index)=> index).ToList();
        List<List<int>> result = [];

        var memberInfo = Util.LoadMembersInfo(region);
        var memberToIndex = memberInfo
            .Select((item, index) => (item, index))
            .ToDictionary(x => x.item.Name, x => x.index);
        // 担当者のオーダー所持状況をビットフラグで表現
        var intoFlags = memberInfo
            .Select((item, index) => (item, index))
            .ToDictionary(
                x => x.index,
                x => list
                    .Select((item, index) => x.item.OrderIndices.Contains(item.Order.Index) ? (1 << index) : 0)
                    .Sum()
            );
        // クロノグラフ (Order Index = 17) の位置を取得
        var chronoIndex = timeTable.Select(item => item.Order.Index).ToList().IndexOf(17);

        // すで割当てられている担当者と一致しているかをチェックする関数
        static bool IsAlreadyInCharge(IEnumerable<int> pics, IEnumerable<int> inChages)
        {
            return pics
                .Select((pic, index) => (pic, index))
                .Where(x => inChages.ElementAt(x.index) != -1)
                .Any(x => x.pic != inChages.ElementAt(x.index));
        };

        if (int.IsPositive(chronoIndex)) // クロノグラフが存在する場合
        {
            // クロノグラフでオーダーを分割
            var beforeChrono = list.GetRange(0, chronoIndex);
            var afterChrono = list.GetRange(chronoIndex + 1, list.Count - chronoIndex - 1);
            var beforeInCharges = beforeChrono.Select(item => item.Pic == string.Empty ? -1 : memberToIndex[item.Pic]).ToList();
            var afterInCharges = afterChrono.Select(item => item.Pic == string.Empty ? -1 : memberToIndex[item.Pic]).ToList();

            if (inCharge.Contains(chronoIndex))
            {
                var chrono = memberToIndex[timeTable[chronoIndex].Pic];
                List<List<int>> beforeCandidates = [];
                if (!beforeInCharges.Contains(-1))
                {
                    beforeCandidates.Add([.. beforeChrono.Select(item => memberToIndex[item.Pic])]);
                }
                else
                {
                    foreach (var pics in Permutation(Enumerable.Range(0, 9), beforeChrono.Count))
                    {
                        if (pics.Contains(chrono)) continue;
                        else if (IsAlreadyInCharge(pics, beforeInCharges)) continue;
                        var check = pics
                            .Select((pic, index) => (pic, index))
                            .Where(x => beforeInCharges.ElementAt(x.index) == -1)
                            .All(x => (intoFlags[x.pic] & (1 << x.index)) != 0);
                        if (check)
                        {
                            beforeCandidates.Add([.. pics]);
                        }
                    }
                }

                List<List<int>> afterCandidates = [];
                if (!afterInCharges.Contains(-1))
                {
                    afterCandidates.Add([.. afterChrono.Select(item => memberToIndex[item.Pic])]);
                }
                else
                {
                    foreach (var pics in Permutation(Enumerable.Range(0, 9), afterChrono.Count))
                    {
                        if (pics.Contains(chrono)) continue;
                        else if (IsAlreadyInCharge(pics, afterInCharges)) continue;
                        var check = pics
                            .Select((pic, index) => (pic, index))
                            .Where(x => afterInCharges.ElementAt(x.index) == -1)
                            .All(x => (intoFlags[x.pic] & (1 << (x.index + beforeChrono.Count + 1))) != 0);
                        if (check)
                        {
                            afterCandidates.Add([.. pics]);
                        }
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
            else // クロノグラフが割り当てられていない場合
            {
                for (int chrono = 0; chrono < 9; chrono++)
                {
                    List<List<int>> beforeCandidates = [];
                    foreach (var pics in Permutation(Enumerable.Range(0, 9), beforeChrono.Count))
                    {
                        if (pics.Contains(chrono)) continue;
                        else if (IsAlreadyInCharge(pics, beforeInCharges)) continue;
                        var check = pics
                            .Select((pic, index) => (pic, index))
                            .Where(x => beforeInCharges.ElementAt(x.index) == -1)
                            .All(x => (intoFlags[x.pic] & (1 << x.index)) != 0);
                        if (check)
                        {
                            beforeCandidates.Add([.. pics]);
                        }
                    }

                    List<List<int>> afterCandidates = [];
                    foreach (var pics in Permutation(Enumerable.Range(0, 9), afterChrono.Count))
                    {
                        if (pics.Contains(chrono)) continue;
                        else if (IsAlreadyInCharge(pics, afterInCharges)) continue;
                        var check = pics
                            .Select((pic, index) => (pic, index))
                            .Where(x => afterInCharges.ElementAt(x.index) == -1)
                            .All(x => (intoFlags[x.pic] & (1 << (x.index + beforeChrono.Count + 1))) != 0);
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
            }
        }
        else
        {
            var inCharges = list.Select(item => item.Pic == string.Empty ? -1 : memberToIndex[item.Pic]).ToList();
            foreach (var pics in Permutation(Enumerable.Range(0, 9), list.Count))
            {
                if (IsAlreadyInCharge(pics, inCharges)) continue;
                var check = pics
                    .Select((pic, index) => (pic, index))
                    .Where(x => inCharges.ElementAt(x.index) == -1)
                    .All(x => (intoFlags[x.pic] & (1 << x.index)) != 0);
                if (check)
                {
                    result.Add([.. pics]);
                }
            }
        }

        if (result.Count == 0) return AutomateAssignResult.Failure("割当てが不可能です。");

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
                yield return [item];
            }
            yield break;
        }
        foreach (var item in items)
        {
            T[] leftside = [item];
            var unused = items.Except(leftside);
            foreach (var rightside in Permutation(unused, k - 1))
            {
                yield return [.. leftside.Concat(rightside)];
            }
        }
    }
}
