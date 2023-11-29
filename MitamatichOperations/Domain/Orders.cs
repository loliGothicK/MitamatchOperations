using System;
using System.Linq;
using mitama.Domain.OrderKinds;

namespace mitama.Domain 
{
    public abstract record Kind;

    namespace OrderKinds
    {
        public record Elemental(Element Element) : Kind;
        public record Buff : Kind;
        public record DeBuff : Kind;
        public record Mp : Kind;
        public record TriggerRateFluctuation : Kind;
        public record Shield : Kind;
        public record Formation : Kind;
        public record Stack : Kind;
        public record Other : Kind;
    }

    public class Kinds
    {
        public class Elemental
        {
            public static OrderKinds.Elemental Fire => new(Element.Fire);
            public static OrderKinds.Elemental Water => new(Element.Water);
            public static OrderKinds.Elemental Wind => new(Element.Wind);
            public static OrderKinds.Elemental Light => new(Element.Light);
            public static OrderKinds.Elemental Dark => new(Element.Dark);
            public static OrderKinds.Elemental Special => new(Element.Special);
        }

        public static Buff Buff => new();
        public static DeBuff DeBuff => new();
        public static Mp Mp => new();
        public static TriggerRateFluctuation TriggerRateFluctuation => new();
        public static Shield Shield => new();
        public static Formation Formation => new();
        public static Stack Stack => new();
        public static Other Other => new();
    }

    public enum Element
    {
        Fire,
        Water,
        Wind,
        Light,
        Dark,
        Special
    }

    public readonly record struct Status(int Atk=0, int SpAtk=0, int Def=0, int SpDef=0)
    {
        public static implicit operator Status(ValueTuple<int, int, int, int> from) => new()
        {
            Atk = from.Item1,
            SpAtk = from.Item2,
            Def = from.Item3,
            SpDef = from.Item4,
        };

        public int ASA => Atk + SpAtk;
        public int DSD => Def + SpDef;

        public static Status operator+(Status a, Status b) => new()
        {
            Atk = a.Atk + b.Atk,
            SpAtk = a.SpAtk + b.SpAtk,
            Def = a.Def + b.Def,
            SpDef = a.SpDef + b.SpDef,
        };
    }

    public readonly record struct Order(
        ushort Index,
        string Name,
        string Effect,
        string Description,
        Status Status,
        int PrepareTime,
        int ActiveTime,
        Kind Kind
    ) {
        public string Path => $@"/Assets/orders/{Index}.png";
        public Uri Uri => new($"ms-appx:///Assets/orders/{Index}.png");

        public string TimeFmt => ActiveTime switch {
            0 => $"({PrepareTime} sec)",
            _ => $"({PrepareTime} + {ActiveTime} sec)"
        };

        public static readonly Order[] List = Init();
        public static readonly Order[] ElementalOrders = List.Where(order => order.Kind is Elemental).ToArray();
        public static readonly Order[] BuffOrders = List.Where(order => order.Kind is Buff).ToArray();
        public static readonly Order[] DeBuffOrders = List.Where(order => order.Kind is DeBuff).ToArray();
        public static readonly Order[] ShieldOrders = List.Where(order => order.Kind is Shield).ToArray();
        public static readonly Order[] MpOrders = List.Where(order => order.Kind is Mp).ToArray();
        public static readonly Order[] TriggerRateFluctuationOrders = List.Where(order => order.Kind is TriggerRateFluctuation).ToArray();
        public static readonly Order[] FormationOrders = List.Where(order => order.Kind is Formation).ToArray();
        public static readonly Order[] StackOrders = List.Where(order => order.Kind is Stack).ToArray();
        public static readonly Order[] OtherOrders = List.Where(order => order.Kind is Other).ToArray();

        // For Automate Assign
        public static readonly Order[] AtkTop5 = List.OrderByDescending(order => order.Status.Atk).Take(5).ToArray();
        public static readonly Order[] SpAtkTop5 = List.OrderByDescending(order => order.Status.SpAtk).Take(5).ToArray();
        public static readonly Order[] DefTop5 = List.OrderByDescending(order => order.Status.Def).Take(5).ToArray();
        public static readonly Order[] SpDefTop5 = List.OrderByDescending(order => order.Status.SpDef).Take(5).ToArray();
        public static readonly Order[] AtkSumTop5 = List.OrderByDescending(order => order.Status.Atk + order.Status.SpAtk).Take(5).ToArray();
        public static readonly Order[] DefSumTop5 = List.OrderByDescending(order => order.Status.Def + order.Status.SpDef).Take(5).ToArray();

        private Order((ushort, string, string, string, (int, int, int, int), (int, int), Kind) raw)
            : this(raw.Item1, raw.Item2, raw.Item3, raw.Item4, raw.Item5, raw.Item6.Item1, raw.Item6.Item2, raw.Item7) { }

        private static Order[] Init() {
            var orders = new ValueTuple<string, string, string, ValueTuple<int, int, int, int>, ValueTuple<int, int>, Kind>[]
            {
                (
                    "天翼のしらべ",
                    "レギオンマッチスキル風属性効果増加Lv.3",
                    "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                    (2201, 1502, 2137, 1492),
                    (30, 120),
                    Kinds.Elemental.Wind
                ),
                (
                    "朱雀炎武",
                    "レギオンマッチスキル火属性効果増加Lv.3",
                    "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                    (2129, 1465, 2171, 1471), (30, 120),
                    Kinds.Elemental.Fire
                ),
                (
                    "水神の怒り",
                    "レギオンマッチスキル水属性効果増加Lv.3",
                    "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                    (1467, 2171, 1485, 2160), (30, 120),
                    Kinds.Elemental.Water
                ),
                (
                    "革命の御旗",
                    "レギオンマッチスキル劣勢時攻撃効果増加Lv.3",
                    "味方前衛全員のスキル攻撃の効果が特大増加。",
                    (2132, 2179, 1486, 1488), (30, 120),
                    Kinds.Other
                ),
                (
                    "覚醒の日輪",
                    "レギオンマッチスキル補助スキル発動率増加Lv.3",
                    "味方全体のレギオンマッチ補助スキル発動率が特大増加。",
                    (1493, 2141, 1490, 2206),
                    (20, 80),
                    Kinds.TriggerRateFluctuation
                ),
                (
                    "日輪の覚醒妨害", "レギオンマッチスキル補助スキル発動率減少Lv.3",
                    "敵全体のレギオンマッチ補助スキル発動率が特大減少。\nレギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                    (1493, 1480, 2123, 2180),
                    (20, 80),
                    Kinds.TriggerRateFluctuation
                ),
                (
                    "攻勢激化の聖剣",
                    "レギオンマッチスキル攻撃力増加Lv.3",
                    "味方前衛のATK/Sp.ATKを特大上昇させる。",
                    (2134, 2179, 1492, 1466),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "聖剣縛りの蔦",
                    "レギオンマッチスキル攻撃力減少Lv.3",
                    "敵前衛のATK/Sp.ATKを特大減少させる。",
                    (1473, 1464, 2125, 2204),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "守勢強化の鉄壁",
                    "レギオンマッチスキル防御力増加Lv.3",
                    "味方前衛のDEF/Sp.DEFを特大上昇させる。",
                    (1569, 1566, 2235, 2275),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "天鳴雨の波紋",
                    "レギオンマッチスキル水属性効果増加Lv.3",
                    "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                    (1420, 2047, 1411, 1995),
                    (30, 120),
                    Kinds.Elemental.Water
                ),
                (
                    "敵城砦鉄壁破壊",
                    "レギオンマッチスキル防御力減少Lv.3",
                    "敵前衛のDEF/Sp.DEFを特大減少させる。",
                    (2217, 2295, 1546, 1559),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "衝撃返しの鉄壁",
                    "レギオンマッチスキル通常ダメージ軽減Lv.3",
                    "敵から受ける通常系攻撃のダメージを大軽減する。",
                    (1566, 2293, 1565, 2220),
                    (30, 90),
                    Kinds.Shield
                ),
                (
                    "神渡しの風巻き",
                    "レギオンマッチスキル風属性効果増加Lv.3",
                    "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                    (2041, 1421, 1980, 1411),
                    (30, 120),
                    Kinds.Elemental.Wind
                ),
                (
                    "雪獄の息吹",
                    "レギオンマッチスキル特殊水属性効果増加Lv.3",
                    "水属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。\n水属性メモリアの通常攻撃はスキル効果が増加。",
                    (1577, 2248, 1540, 2291),
                    (30, 120),
                    Kinds.Elemental.Water
                ),
                (
                    "劫火の勇猛",
                    "レギオンマッチスキル通常火属性効果増加Lv.3",
                    "火属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。\n火属性メモリアの特殊攻撃はスキル効果が増加。",
                    (2286, 1540, 2222, 1580),
                    (30, 120),
                    Kinds.Elemental.Fire
                ),
                (
                    "女帝蝶の火継",
                    "レギオンマッチスキル火属性効果増加Lv.3",
                    "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                    (2026, 1413, 2007, 1383),
                    (30, 120),
                    Kinds.Elemental.Fire
                ),
                (
                    "特異返しの鉄壁",
                    "レギオンマッチスキル特殊ダメージ軽減Lv.3",
                    "敵から受ける特殊系攻撃のダメージを大軽減する。",
                    (2270, 1550, 2222, 1580),
                    (30, 90),
                    Kinds.Shield
                ),
                (
                    "刻戻りのクロノグラフ",
                    "レギオンマッチスキルオーダー使用リセットLv.3",
                    "自身を除く味方のオーダーの使用回数がリセットされる。",
                    (1558, 1574, 2215, 2281),
                    (30, 0),
                    Kinds.Other
                ),
                (
                    "裂空の神秘",
                    "レギオンマッチスキル特殊風属性効果増加Lv.3",
                    "風属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。\n風属性メモリアの通常攻撃はスキル効果が増加。",
                    (1574, 2226, 1554, 2298),
                    (30, 120),
                    Kinds.Elemental.Wind
                ),
                (
                    "溟海の勇猛",
                    "レギオンマッチスキル通常水属性効果増加Lv.3",
                    "水属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。\n水属性メモリアの特殊攻撃はスキル効果が増加。",
                    (2277, 1576, 2225, 1565),
                    (30, 120),
                    Kinds.Elemental.Water
                ),
                (
                    "煌炎の神秘",
                    "レギオンマッチスキル特殊火属性効果増加Lv.3",
                    "火属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。\n火属性メモリアの通常攻撃はスキル効果が増加。",
                    (1547, 2228, 1553, 2267),
                    (30, 120),
                    Kinds.Elemental.Fire
                ),
                (
                    "霊鳥の勇猛",
                    "レギオンマッチスキル通常風属性効果増加Lv.3",
                    "風属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。\n風属性メモリアの特殊攻撃はスキル効果が増加。",
                    (2289, 1540, 2224, 1543),
                    (30, 120),
                    Kinds.Elemental.Wind
                ),
                (
                    "広域再編の陣",
                    "レギオンマッチスキル全体再編Lv.3",
                    "味方全体のユニットチェンジの使用回数がリセットされる。",
                    (1725, 1734, 2420, 2449),
                    (30, 0),
                    Kinds.Formation
                ),
                (
                    "朱雀の御盾",
                    "レギオンマッチスキル火属性効果減少Lv.3",
                    "敵の使用した火属性のスキル効果が特大減少。",
                    (1708, 1728, 2475, 2399),
                    (20, 100),
                    Kinds.Shield
                ),
                (
                    "水神の御盾",
                    "レギオンマッチスキル水属性効果減少Lv.3",
                    "敵の使用した水属性のスキル効果が特大減少。",
                    (1735, 1737, 2408, 2472),
                    (20, 100),
                    Kinds.Shield
                ),
                (
                    "天翼の御盾",
                    "レギオンマッチスキル風属性効果減少Lv.3",
                    "敵の使用した風属性のスキル効果が特大減少。",
                    (1706, 1735, 2481, 2415),
                    (20, 100),
                    Kinds.Shield
                ),
                (
                    "広域魔導凱旋",
                    "レギオンマッチスキル全体MP回復Lv.3",
                    "味方全体のMPを40%回復。",
                    (1724, 2449, 1718, 2410),
                    (20, 0),
                    Kinds.Mp
                ),
                (
                    "魔縮領域",
                    "レギオンマッチスキルMP軽減Lv.3",
                    "味方全体のメモリアスキルの消費MPが特大軽減。",
                    (2481, 1712, 2431, 1707),
                    (20, 80),
                    Kinds.Mp
                ),
                (
                    "暗黒業火",
                    "レギオンマッチスキル闇・火属性効果増加Lv.3",
                    "闇属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                    (2464, 1716, 2418, 1705),
                    (15, 60),
                    Kinds.Elemental.Dark
                ),
                (
                    "黒碑水鏡",
                    "レギオンマッチスキル闇・水属性効果増加Lv.3",
                    "闇属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                    (1707, 2485, 1705, 2402),
                    (15, 60),
                    Kinds.Elemental.Dark
                ),
                (
                    "黒貂威風",
                    "レギオンマッチスキル闇・風属性効果増加Lv.3",
                    "闇属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                    (2460, 2433, 1730, 1714),
                    (15, 60),
                    Kinds.Elemental.Dark
                ),
                (
                    "光背火翼",
                    "レギオンマッチスキル光・火属性効果増加Lv.3",
                    "光属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                    (2455, 1720, 2422, 1734),
                    (15, 60),
                    Kinds.Elemental.Light
                ),
                (
                    "天光銀波",
                    "レギオンマッチスキル光・水属性効果増加Lv.3",
                    "光属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                    (1720, 2487, 1740, 2425),
                    (15, 60),
                    Kinds.Elemental.Light
                ),
                (
                    "清暉恒風",
                    "レギオンマッチスキル攻撃力増加Lv.3",
                    "味方前衛のATK/Sp.ATKを特大上昇させる。",
                    (2274, 2248, 1559, 1568),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "光華廻風",
                    "レギオンマッチスキル光・風属性効果増加Lv.3",
                    "光属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                    (2454, 2400, 1723, 1740),
                    (15, 60),
                    Kinds.Elemental.Light
                ),
                (
                    "前衛再編の陣",
                    "レギオンマッチスキル前衛再編Lv.3",
                    "味方前衛のユニットチェンジの使用回数がリセットされる。",
                    (1726, 1727, 2405, 2452),
                    (15, 0),
                    Kinds.Formation
                ),
                (
                    "後衛再編の陣",
                    "レギオンマッチスキル後衛再編Lv.3",
                    "味方後衛のユニットチェンジの使用回数がリセットされる。",
                    (1734, 1732, 2412, 2467),
                    (15, 0),
                    Kinds.Formation
                ),
                (
                    "陰陽二律",
                    "レギオンマッチスキル[光闇]属性効果増加Lv.3",
                    "光属性と闇属性のメモリアスキル効果が特大増加する。",
                    (2487, 2434, 1738, 1706),
                    (15, 60),
                    Kinds.Elemental.Special
                ),
                (
                    "玲瓏光艶",
                    "レギオンマッチスキル光属性効果増加Lv.3",
                    "光属性のメモリアのスキル効果が特大増加する。",
                    (2467, 2429, 1703, 1725),
                    (30, 120),
                    Kinds.Elemental.Light
                ),
                (
                    "暗碧無双",
                    "レギオンマッチスキル闇属性効果増加Lv.3",
                    "闇属性のメモリアのスキル効果が特大増加する。",
                    (2466, 2437, 1713, 1715),
                    (30, 120),
                    Kinds.Elemental.Special
                ),
                (
                    "光刃激化の聖剣",
                    "レギオンマッチスキル光属性攻撃力増加Lv.3",
                    "味方前衛の光属性攻撃力を特大上昇させる。",
                    (2417, 2463, 1701, 1733),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "光鎧強化の鉄壁",
                    "レギオンマッチスキル光属性防御力増加Lv.3",
                    "味方前衛の光属性防御力を特大上昇させる。",
                    (1724, 1728, 2416, 2488),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "闇刃激化の聖剣",
                    "レギオンマッチスキル闇属性攻撃力増加Lv.3",
                    "味方前衛の闇属性攻撃力を特大上昇させる。",
                    (2455, 2406, 1738, 1723),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "闇鎧強化の鉄壁",
                    "レギオンマッチスキル闇属性防御力増加Lv.3",
                    "味方前衛の闇属性防御力を特大上昇させる。",
                    (1731, 1737, 2453, 2400),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "光刃縛りの大蔦",
                    "レギオンマッチスキル光属性攻撃力減少Lv.3",
                    "敵前衛の光属性攻撃力を特大減少させる。",
                    (2423, 2453, 1724, 1732),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "光鎧の鉄壁破壊",
                    "レギオンマッチスキル光属性防御力減少Lv.3",
                    "敵前衛の光属性防御力を特大減少させる。",
                    (1710, 1726, 2400, 2452),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "闇刃縛りの大蔦",
                    "レギオンマッチスキル闇属性攻撃力減少Lv.3",
                    "敵前衛の闇属性攻撃力を特大減少させる。",
                    (2477, 2404, 1713, 1734),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "闇鎧の鉄壁破壊",
                    "レギオンマッチスキル闇属性防御力減少Lv.3",
                    "敵前衛の闇属性防御力を特大減少させる。",
                    (1718, 1720, 2479, 2418),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "水刃激化の聖剣",
                    "レギオンマッチスキル水属性攻撃力増加Lv.3",
                    "味方前衛の水属性攻撃力を特大上昇させる。",
                    (2426, 2476, 1711, 1726),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "水鎧の鉄壁破壊",
                    "レギオンマッチスキル水属性防御力減少Lv.3",
                    "敵前衛の水属性防御力を特大減少させる。",
                    (1735, 1721, 2412, 2475),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "水鎧強化の鉄壁",
                    "レギオンマッチスキル水属性防御力増加Lv.3",
                    "味方前衛の水属性防御力を特大上昇させる。",
                    (1718, 1732, 2435, 2485),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "水刃縛りの大蔦",
                    "レギオンマッチスキル水属性攻撃力減少Lv.3",
                    "敵前衛の水属性攻撃力を特大減少させる。",
                    (2412, 2469, 1717, 1721),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "戦術加速の陣",
                    "レギオンマッチスキル準備時間短縮Lv.3",
                    "味方が次に使うオーダースキルの準備時間を5秒に変更する。",
                    (2675, 2687, 1903, 1873),
                    (5, 0),
                    Kinds.Stack
                ),
                (
                    "風刃激化の聖剣",
                    "風属性攻撃力増加Lv.3",
                    "味方前衛の風属性攻撃力を特大上昇させる。",
                    (2440, 2464, 1705, 1730),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "風鎧の鉄壁破壊",
                    "風属性防御力減少Lv.3",
                    "敵前衛の風属性防御力を特大減少させる。",
                    (1701, 1732, 2456, 2426),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "定めの時辰儀",
                    "効果時間短縮Lv.3",
                    "敵が次に使うオーダースキルの効果時間を60秒に変更する。",
                    (1891, 1871, 2697, 2686),
                    (30, 0),
                    Kinds.Stack
                ),
                (
                    "風鎧強化の鉄壁",
                    "風属性防御力増加Lv.3",
                    "味方前衛の風属性防御力を特大上昇させる。",
                    (1738, 1703, 2463, 2437),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "風刃縛りの大蔦",
                    "風属性攻撃力減少Lv.3",
                    "敵前衛の風属性攻撃力を特大減少させる。",
                    (2449, 2431, 1732, 1725),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "熾烈攻勢の聖剣",
                    "攻撃力増加Lv.4",
                    "味方前衛のATK/Sp.ATKを超特大上昇させる。",
                    (2918, 2930, 2110, 2080),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "炎刃激化の聖剣",
                    "火属性攻撃力増加Lv.3",
                    "味方前衛の火属性攻撃力を特大上昇させる。",
                    (2481, 2436, 1728, 1711),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "炎鎧の鉄壁破壊",
                    "火属性防御力減少Lv.3",
                    "敵前衛の火属性防御力を特大減少させる。",
                    (1712, 1711, 2465, 2426),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "炎鎧強化の鉄壁",
                    "火属性防御力増加Lv.3",
                    "味方前衛の火属性防御力を特大上昇させる。",
                    (1716, 1736, 2463, 2424),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "炎刃縛りの大蔦",
                    "火属性攻撃力減少Lv.3",
                    "敵前衛の火属性攻撃力を特大減少させる。",
                    (2456, 2416, 1713, 1706),
                    (20, 0),
                    Kinds.DeBuff
                ),
                (
                    "堅硬守勢の防壁",
                    "防御力増加Lv.4",
                    "味方前衛のDEF/Sp.DEFを超特大上昇させる。（40%）",
                    (2079, 2086, 2948, 2935),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "妨げの祝福",
                    "妨害効果増加Lv.3",
                    "味方が使用する妨害メモリアのスキル効果が特大増加する。（40%）",
                    (1890, 2265, 1880, 2680),
                    (20, 80),
                    Kinds.Other
                ),
                (
                    "支えの祝福",
                    "支援効果増加Lv.3",
                    "味方が使用する支援メモリアのスキル効果が特大増加する。（40%）",
                    (2674, 1880, 2704, 1899),
                    (20, 80),
                    Kinds.Other
                ),
                (
                    "覚醒の大天光",
                    "補助スキル発動率上昇Lv.4",
                    "味方全体のレギオンマッチ補助スキル発動率が超特大上昇（75%）。",
                    (2677, 1899, 2688, 1879),
                    (20, 90),
                    Kinds.TriggerRateFluctuation
                ),
                (
                    "大天光の覚醒妨害",
                    "補助スキル発動率減少Lv.4",
                    "敵全体のレギオンマッチ補助スキル発動率が超特大減少（65%）。",
                    (1896, 2676, 1890, 2708),
                    (20, 90),
                    Kinds.TriggerRateFluctuation
                ),
                (
                    "支えの反動",
                    "支援効果減少Lv.3",
                    "敵が使用する支援のメモリアのスキル効果が特大減少する（40%）。",
                    (2697, 2703, 1877, 1879),
                    (20, 80),
                    Kinds.Other
                ),
                (
                    "妨げの反動",
                    "妨害効果減少Lv.3",
                    "敵が使用する妨害のメモリアのスキル効果が特大減少する（40%）。",
                    (1880, 1867, 2695, 2695),
                    (20, 80),
                    Kinds.Other
                ),
                (
                    "炎鎧強化の大城壁",
                    "火属性防御力増加Lv.4",
                    "味方前衛の火属性防御力を超特大上昇させる。",
                    (1872, 1883, 2675, 2704),
                    (20, 0),
                    Kinds.Buff
                ),
                (
                    "炎刃縛りの大棘蔦",
                    "火属性攻撃力減少Lv.4",
                    "敵前衛の火属性攻撃力を超特大減少させる。",
                    (2707, 2701, 1887, 1869),
                    (20, 0),
                    Kinds.DeBuff
                )
            };

            return orders.Select((v, index) =>
                new Order(((ushort)index, v.Item1, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6))).ToArray();
        }
    }
}
