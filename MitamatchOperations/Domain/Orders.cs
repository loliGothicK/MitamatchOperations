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

    public readonly record struct BasicStatus(int Atk=0, int SpAtk=0, int Def=0, int SpDef=0)
    {
        public static implicit operator BasicStatus(ValueTuple<int, int, int, int> from) => new()
        {
            Atk = from.Item1,
            SpAtk = from.Item2,
            Def = from.Item3,
            SpDef = from.Item4,
        };

        public int ASA => Atk + SpAtk;
        public int DSD => Def + SpDef;

        public static BasicStatus operator+(BasicStatus a, BasicStatus b) => new()
        {
            Atk = a.Atk + b.Atk,
            SpAtk = a.SpAtk + b.SpAtk,
            Def = a.Def + b.Def,
            SpDef = a.SpDef + b.SpDef,
        };
    }

    public record struct OrderIndexAndTime(int Index, TimeOnly Time);

    public readonly record struct Order(
        ushort Index,
        string Name,
        BasicStatus Status,
        string Effect,
        string Description,
        int PrepareTime,
        int ActiveTime,
        bool Payed,
        Kind Kind,
        bool HasTemplate
    ) {
        public string Path => $@"/Assets/orders/{Name}.png";
        public Uri Uri => new($"ms-appx:///Assets/orders/{Name}.png");
        public Uri TemplateUri => new($"ms-appx:///Assets/OrderTemplates/{Name}.png");

        public string TimeFmt => ActiveTime switch {
            0 => $"({PrepareTime} sec)",
            _ => $"({PrepareTime} + {ActiveTime} sec)"
        };

        public static Order[] List = [
            new Order(
                157,
                "水刃縛りの大棘蔦",
                new(2684, 1866, 2681, 1902),
                "水属性攻撃力減少Lv.4",
                "敵前衛の水属性攻撃力を超特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                156,
                "水鎧強化の大城壁",
                new(1900, 2672, 1878, 2691),
                "水属性防御力増加Lv.4",
                "味方前衛の水属性防御力を超特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                155,
                "水刃縛りの棘蔦",
                new(2241, 1563, 2237, 1580),
                "水属性攻撃力減少Lv.4",
                "敵前衛の水属性攻撃力を超特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                154,
                "水鎧強化の城壁",
                new(1546, 2265, 1568, 2243),
                "水属性防御力増加Lv.4",
                "味方前衛の水属性防御力を超特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                153,
                "炎刃縛りの大棘蔦",
                new(2707, 1887, 2701, 1869),
                "火属性攻撃力減少Lv.4",
                "敵前衛の火属性攻撃力を超特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                152,
                "炎鎧強化の大城壁",
                new(1872, 2675, 1883, 2704),
                "火属性防御力増加Lv.4",
                "味方前衛の火属性防御力を超特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                151,
                "炎刃縛りの棘蔦",
                new(2262, 1546, 2263, 1543),
                "火属性攻撃力減少Lv.4",
                "敵前衛の火属性攻撃力を超特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                150,
                "炎鎧強化の城壁",
                new(1579, 2266, 1562, 2256),
                "火属性防御力増加Lv.4",
                "味方前衛の火属性防御力を超特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                149,
                "妨げの反動",
                new(1880, 2695, 1867, 2695),
                "妨害効果減少Lv.3",
                "敵が使用する妨害のメモリアのスキル効果が特大減少する。",
                20,
                80,
                true,
                Kinds.Other,
                true
            ),
            new Order(
                148,
                "支えの反動",
                new(2697, 1877, 2703, 1879),
                "支援効果減少Lv.3",
                "敵が使用する支援のメモリアのスキル効果が特大減少する。",
                20,
                80,
                true,
                Kinds.Other,
                true
            ),
            new Order(
                147,
                "妨げの破却",
                new(1569, 2274, 1540, 2271),
                "妨害効果減少Lv.3",
                "敵が使用する妨害のメモリアのスキル効果が特大減少する。",
                20,
                80,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                146,
                "支えの破却",
                new(2241, 1563, 2237, 1580),
                "支援効果減少Lv.3",
                "敵が使用する支援のメモリアのスキル効果が特大減少する。",
                20,
                80,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                145,
                "大天光の覚醒妨害",
                new(1896, 2676, 1890, 2708),
                "補助スキル発動率減少Lv.4",
                "敵全体のレギオンマッチ補助スキル発動率が超特大減少。レギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                20,
                90,
                true,
                Kinds.TriggerRateFluctuation,
                true
            ),
            new Order(
                144,
                "覚醒の大天光",
                new(2677, 1899, 2688, 1879),
                "補助スキル発動率増加Lv.4",
                "味方全体のレギオンマッチ補助スキル発動率が超特大増加。",
                20,
                90,
                true,
                Kinds.TriggerRateFluctuation,
                true
            ),
            new Order(
                143,
                "満月の覚醒妨害",
                new(1566, 2242, 1567, 2239),
                "補助スキル発動率減少Lv.4",
                "敵全体のレギオンマッチ補助スキル発動率が超特大減少。レギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                20,
                90,
                false,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                142,
                "覚醒の満月",
                new(2275, 1570, 2258, 1560),
                "補助スキル発動率増加Lv.4",
                "味方全体のレギオンマッチ補助スキル発動率が超特大増加。",
                20,
                90,
                false,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                141,
                "妨げの祝福",
                new(1890, 2679, 1880, 2680),
                "妨害効果増加Lv.3",
                "味方が使用する妨害のメモリアのスキル効果が特大増加する。",
                20,
                80,
                true,
                Kinds.Other,
                true
            ),
            new Order(
                140,
                "支えの祝福",
                new(2674, 1880, 2704, 1899),
                "支援効果増加Lv.3",
                "味方が使用する支援のメモリアのスキル効果が特大増加する。",
                20,
                80,
                true,
                Kinds.Other,
                true
            ),
            new Order(
                139,
                "妨げの波動",
                new(1546, 2265, 1568, 2243),
                "妨害効果増加Lv.3",
                "味方が使用する妨害のメモリアのスキル効果が特大増加する。",
                20,
                80,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                138,
                "支えの波動",
                new(2262, 1546, 2263, 1543),
                "支援効果増加Lv.3",
                "味方が使用する支援のメモリアのスキル効果が特大増加する。",
                20,
                80,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                137,
                "堅硬守勢の防壁",
                new(2079, 2948, 2086, 2935),
                "防御力増加Lv.4",
                "味方前衛のDEF/Sp.DEFを超特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                136,
                "炎刃縛りの大蔦",
                new(2456, 1713, 2416, 1706),
                "火属性攻撃力減少Lv.3",
                "敵前衛の火属性攻撃力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                135,
                "炎鎧強化の鉄壁",
                new(1716, 2463, 1736, 2424),
                "火属性防御力増加Lv.3",
                "味方前衛の火属性防御力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                134,
                "炎刃縛りの蔦",
                new(2029, 1419, 2000, 1416),
                "火属性攻撃力減少Lv.3",
                "敵前衛の火属性攻撃力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                133,
                "炎鎧強化の壁",
                new(1386, 2023, 1382, 2040),
                "火属性防御力増加Lv.3",
                "味方前衛の火属性防御力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                132,
                "炎鎧の鉄壁破壊",
                new(1712, 2465, 1711, 2426),
                "火属性防御力減少Lv.3",
                "敵前衛の火属性防御力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                131,
                "炎刃激化の聖剣",
                new(2481, 1728, 2436, 1711),
                "火属性攻撃力増加Lv.3",
                "味方前衛の火属性攻撃力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                130,
                "熾烈攻勢の聖剣",
                new(2918, 2110, 2930, 2080),
                "攻撃力増加Lv.4",
                "味方前衛のATK/Sp.ATKを超特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                129,
                "炎鎧の壁破壊",
                new(1420, 2030, 1403, 2020),
                "火属性防御力減少Lv.3",
                "敵前衛の火属性防御力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                128,
                "炎刃激化の豪剣",
                new(2006, 1410, 2028, 1388),
                "火属性攻撃力増加Lv.3",
                "味方前衛の火属性攻撃力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                127,
                "風刃縛りの大蔦",
                new(2449, 1732, 2431, 1725),
                "風属性攻撃力減少Lv.3",
                "敵前衛の風属性攻撃力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                126,
                "風鎧強化の鉄壁",
                new(1738, 2463, 1703, 2437),
                "風属性防御力増加Lv.3",
                "味方前衛の風属性防御力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                125,
                "定めの時辰儀",
                new(1891, 2697, 1871, 2686),
                "効果時間短縮Lv.3",
                "敵が次に使うオーダースキルの効果時間を60秒に変更する。",
                30,
                0,
                true,
                Kinds.Other,
                false
            ),
            new Order(
                124,
                "風刃縛りの蔦",
                new(2039, 1411, 2022, 1401),
                "風属性攻撃力減少Lv.3",
                "敵前衛の風属性攻撃力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                123,
                "風鎧強化の壁",
                new(1410, 2038, 1381, 2035),
                "風属性防御力増加Lv.3",
                "味方前衛の風属性防御力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                122,
                "操りの時計",
                new(1545, 2259, 1541, 2276),
                "効果時間短縮Lv.3",
                "敵が次に使うオーダースキルの効果時間を60秒に変更する。",
                30,
                0,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                121,
                "風鎧の鉄壁破壊",
                new(1701, 2456, 1732, 2426),
                "風属性防御力減少Lv.3",
                "敵前衛の風属性防御力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                120,
                "風刃激化の聖剣",
                new(2440, 1705, 2464, 1730),
                "風属性攻撃力増加Lv.3",
                "味方前衛の風属性攻撃力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                119,
                "風鎧の壁破壊",
                new(1407, 2006, 1408, 2003),
                "風属性防御力減少Lv.3",
                "敵前衛の風属性防御力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                118,
                "風刃激化の豪剣",
                new(2005, 1404, 2001, 1421),
                "風属性攻撃力増加Lv.3",
                "味方前衛の風属性攻撃力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                117,
                "戦術加速の陣",
                new(2675, 1903, 2687, 1873),
                "準備時間短縮Lv.3",
                "味方が次に使うオーダースキルの準備時間を5秒に変更する。",
                5,
                0,
                true,
                Kinds.Other,
                false
            ),
            new Order(
                116,
                "戦術加速の策",
                new(2249, 1559, 2273, 1568),
                "準備時間短縮Lv.3",
                "味方が次に使うオーダースキルの準備時間を5秒に変更する。",
                5,
                0,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                115,
                "水刃縛りの大蔦",
                new(2412, 1717, 2469, 1721),
                "水属性攻撃力減少Lv.3",
                "敵前衛の水属性攻撃力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                114,
                "水鎧強化の鉄壁",
                new(1718, 2435, 1732, 2485),
                "水属性防御力増加Lv.3",
                "味方前衛の水属性防御力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                113,
                "水鎧の鉄壁破壊",
                new(1735, 2412, 1721, 2475),
                "水属性防御力減少Lv.3",
                "敵前衛の水属性防御力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                112,
                "水刃激化の聖剣",
                new(2426, 1711, 2476, 1726),
                "水属性攻撃力増加Lv.3",
                "味方前衛の水属性攻撃力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                111,
                "水刃縛りの蔦",
                new(2026, 1387, 2027, 1384),
                "水属性攻撃力減少Lv.3",
                "敵前衛の水属性攻撃力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                110,
                "水鎧強化の壁",
                new(1420, 2030, 1403, 2020),
                "水属性防御力増加Lv.3",
                "味方前衛の水属性防御力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                109,
                "水鎧の壁破壊",
                new(1387, 2029, 1409, 2007),
                "水属性防御力減少Lv.3",
                "敵前衛の水属性防御力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                108,
                "水刃激化の豪剣",
                new(2029, 1419, 2000, 1416),
                "水属性攻撃力増加Lv.3",
                "味方前衛の水属性攻撃力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                107,
                "闇鎧の鉄壁破壊",
                new(1718, 2479, 1720, 2418),
                "闇属性防御力減少Lv.3",
                "敵前衛の闇属性防御力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                106,
                "闇刃縛りの大蔦",
                new(2477, 1713, 2404, 1734),
                "闇属性攻撃力減少Lv.3",
                "敵前衛の闇属性攻撃力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                105,
                "光鎧の鉄壁破壊",
                new(1710, 2400, 1726, 2452),
                "光属性防御力減少Lv.3",
                "敵前衛の光属性防御力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                104,
                "光刃縛りの大蔦",
                new(2423, 1724, 2453, 1732),
                "光属性攻撃力減少Lv.3",
                "敵前衛の光属性攻撃力を特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                103,
                "闇鎧強化の鉄壁",
                new(1731, 2453, 1737, 2400),
                "闇属性防御力増加Lv.3",
                "味方前衛の闇属性防御力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                102,
                "闇刃激化の聖剣",
                new(2455, 1738, 2406, 1723),
                "闇属性攻撃力増加Lv.3",
                "味方前衛の闇属性攻撃力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                101,
                "光鎧強化の鉄壁",
                new(1724, 2416, 1728, 2488),
                "光属性防御力増加Lv.3",
                "味方前衛の光属性防御力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                100,
                "光刃激化の聖剣",
                new(2417, 1701, 2463, 1733),
                "光属性攻撃力増加Lv.3",
                "味方前衛の光属性攻撃力を特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                99,
                "闇鎧の壁破壊",
                new(1386, 2023, 1382, 2040),
                "闇属性防御力減少Lv.3",
                "敵前衛の闇属性防御力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                98,
                "闇刃縛りの蔦",
                new(2006, 1410, 2028, 1388),
                "闇属性攻撃力減少Lv.3",
                "敵前衛の闇属性攻撃力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                97,
                "光鎧の壁破壊",
                new(1407, 2006, 1408, 2003),
                "光属性防御力減少Lv.3",
                "敵前衛の光属性防御力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                96,
                "光刃縛りの蔦",
                new(2039, 1411, 2022, 1401),
                "光属性攻撃力減少Lv.3",
                "敵前衛の光属性攻撃力を特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                95,
                "闇鎧強化の壁",
                new(1410, 2038, 1381, 2035),
                "闇属性防御力増加Lv.3",
                "味方前衛の闇属性防御力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                94,
                "闇刃激化の豪剣",
                new(2005, 1404, 2001, 1421),
                "闇属性攻撃力増加Lv.3",
                "味方前衛の闇属性攻撃力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                93,
                "光鎧強化の壁",
                new(1387, 2029, 1409, 2007),
                "光属性防御力増加Lv.3",
                "味方前衛の光属性防御力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                92,
                "光刃激化の豪剣",
                new(2026, 1387, 2027, 1384),
                "光属性攻撃力増加Lv.3",
                "味方前衛の光属性攻撃力を特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                91,
                "暗碧無双",
                new(2466, 1713, 2437, 1715),
                "闇属性効果増加Lv.3",
                "闇属性のメモリアのスキル効果が特大増加する。",
                30,
                120,
                true,
                Kinds.Elemental.Dark,
                false
            ),
            new Order(
                90,
                "玲瓏光艶",
                new(2467, 1703, 2429, 1725),
                "光属性効果増加Lv.3",
                "光属性のメモリアのスキル効果が特大増加する。",
                30,
                120,
                true,
                Kinds.Elemental.Light,
                false
            ),
            new Order(
                89,
                "宵闇の心得",
                new(2031, 1410, 2003, 1388),
                "闇属性効果増加Lv.3",
                "闇属性のメモリアのスキル効果が特大増加する。",
                30,
                120,
                false,
                Kinds.Elemental.Dark,
                false
            ),
            new Order(
                88,
                "神光の心得",
                new(2050, 1402, 2009, 1381),
                "光属性効果増加Lv.3",
                "光属性のメモリアのスキル効果が特大増加する。",
                30,
                120,
                false,
                Kinds.Elemental.Light,
                false
            ),
            new Order(
                87,
                "陰陽二律",
                new(2487, 1738, 2434, 1706),
                "[光闇]属性効果増加Lv.3",
                "光属性と闇属性のメモリアスキル効果が特大増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Special,
                false
            ),
            new Order(
                86,
                "光陰の心得",
                new(2050, 1402, 2009, 1381),
                "[光闇]属性効果増加Lv.3",
                "光属性と闇属性のメモリアスキル効果が特大増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Special,
                false
            ),
            new Order(
                85,
                "後衛再編の陣",
                new(1734, 2412, 1732, 2467),
                "後衛再編Lv.3",
                "味方後衛のユニットチェンジの使用回数がリセットされる。",
                15,
                0,
                true,
                Kinds.Formation,
                false
            ),
            new Order(
                84,
                "前衛再編の陣",
                new(1726, 2405, 1727, 2452),
                "前衛再編Lv.3",
                "味方前衛のユニットチェンジの使用回数がリセットされる。",
                15,
                0,
                true,
                Kinds.Formation,
                false
            ),
            new Order(
                83,
                "後衛再編の策",
                new(1406, 1996, 1415, 2025),
                "後衛再編Lv.3",
                "味方後衛のユニットチェンジの使用回数がリセットされる。",
                15,
                0,
                false,
                Kinds.Formation,
                false
            ),
            new Order(
                82,
                "前衛再編の策",
                new(1382, 2007, 1413, 2027),
                "前衛再編Lv.3",
                "味方前衛のユニットチェンジの使用回数がリセットされる。",
                15,
                0,
                false,
                Kinds.Formation,
                false
            ),
            new Order(
                81,
                "光華廻風",
                new(2454, 1723, 2400, 1740),
                "光・風属性効果増加Lv.3",
                "光属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Light,
                true
            ),
            new Order(
                80,
                "剣光と疾風の心得",
                new(2026, 1413, 2007, 1383),
                "光・風属性効果増加Lv.3",
                "光属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Light,
                false
            ),
            new Order(
                79,
                "清暉恒風",
                new(2274, 1559, 2248, 1568),
                "攻撃力増加Lv.3",
                "味方前衛のATK/Sp.ATKを特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                78,
                "天光銀波",
                new(1720, 1740, 2487, 2425),
                "光・水属性効果増加Lv.3",
                "光属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Light,
                true
            ),
            new Order(
                77,
                "光背火翼",
                new(2455, 2422, 1720, 1734),
                "光・火属性効果増加Lv.3",
                "光属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Light,
                true
            ),
            new Order(
                76,
                "陽光と漣の心得",
                new(1410, 1405, 2051, 1989),
                "光・水属性効果増加Lv.3",
                "光属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Light,
                false
            ),
            new Order(
                75,
                "極光と陽炎の心得",
                new(1988, 2044, 1418, 1409),
                "光・火属性効果増加Lv.3",
                "光属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Light,
                false
            ),
            new Order(
                74,
                "黒貂威風",
                new(2460, 1730, 2433, 1714),
                "闇・風属性効果増加Lv.3",
                "闇属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Dark,
                true
            ),
            new Order(
                73,
                "深闇と烈風の心得",
                new(2040, 1400, 1991, 1386),
                "闇・風属性効果増加Lv.3",
                "闇属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Dark,
                false
            ),
            new Order(
                72,
                "黒碑水鏡",
                new(1707, 1705, 2485, 2402),
                "闇・水属性効果増加Lv.3",
                "闇属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Dark,
                true
            ),
            new Order(
                71,
                "暗黒業火",
                new(2464, 2418, 1716, 1705),
                "闇・火属性効果増加Lv.3",
                "闇属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                15,
                60,
                true,
                Kinds.Elemental.Dark,
                true
            ),
            new Order(
                70,
                "石墨と奔流の心得",
                new(1406, 1402, 2059, 1975),
                "闇・水属性効果増加Lv.3",
                "闇属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Dark,
                false
            ),
            new Order(
                69,
                "漆黒と烈火の心得",
                new(1976, 2057, 1413, 1383),
                "闇・火属性効果増加Lv.3",
                "闇属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                15,
                60,
                false,
                Kinds.Elemental.Dark,
                false
            ),
            new Order(
                68,
                "魔縮領域",
                new(2481, 2431, 1712, 1707),
                "MP軽減Lv.3",
                "味方全体のメモリアスキルの消費MPが特大軽減。",
                20,
                80,
                true,
                Kinds.Mp,
                true
            ),
            new Order(
                67,
                "広域魔導凱旋",
                new(1724, 1718, 2449, 2410),
                "全体MP回復Lv.3",
                "味方全体のMPを40%回復。",
                20,
                0,
                true,
                Kinds.Mp,
                false
            ),
            new Order(
                66,
                "広域魔導復古の策",
                new(1406, 1402, 2059, 1975),
                "全体MP回復Lv.3",
                "味方全体のMPを40%回復。",
                20,
                0,
                false,
                Kinds.Mp,
                false
            ),
            new Order(
                65,
                "天翼の御盾",
                new(1706, 2481, 1735, 2415),
                "風属性効果減少Lv.3",
                "敵の使用した風属性のスキル効果が特大減少。",
                20,
                100,
                true,
                Kinds.Shield,
                true
            ),
            new Order(
                64,
                "水神の御盾",
                new(1735, 2408, 1737, 2472),
                "水属性効果減少Lv.3",
                "敵の使用した水属性のスキル効果が特大減少。",
                20,
                100,
                true,
                Kinds.Shield,
                true
            ),
            new Order(
                63,
                "朱雀の御盾",
                new(1708, 2475, 1728, 2399),
                "火属性効果減少Lv.3",
                "敵の使用した火属性のスキル効果が特大減少。",
                20,
                100,
                true,
                Kinds.Shield,
                true
            ),
            new Order(
                62,
                "絶風の盾",
                new(1410, 2049, 1407, 1989),
                "風属性効果減少Lv.3",
                "敵の使用した風属性のスキル効果が特大減少。",
                20,
                100,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                61,
                "氷結の盾",
                new(1394, 1994, 1418, 2053),
                "水属性効果減少Lv.3",
                "敵の使用した水属性のスキル効果が特大減少。",
                20,
                100,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                60,
                "火烈の盾",
                new(1396, 2044, 1397, 1980),
                "火属性効果減少Lv.3",
                "敵の使用した火属性のスキル効果が特大減少。",
                20,
                100,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                59,
                "広域再編の陣",
                new(1725, 2420, 1734, 2449),
                "全体再編Lv.3",
                "味方全体のユニットチェンジの使用回数がリセットされる。",
                30,
                0,
                true,
                Kinds.Formation,
                false
            ),
            new Order(
                58,
                "広域再編の策",
                new(901, 1442, 932, 1462),
                "全体再編Lv.3",
                "味方全体のユニットチェンジの使用回数がリセットされる。",
                30,
                0,
                false,
                Kinds.Formation,
                false
            ),
            new Order(
                57,
                "霊鳥の勇猛",
                new(2289, 2224, 1540, 1543),
                "通常:風属性効果増加Lv.3",
                "風属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。風属性メモリアの特殊攻撃はスキル効果が増加。",
                30,
                120,
                true,
                Kinds.Elemental.Wind,
                true
            ),
            new Order(
                56,
                "煌炎の神秘",
                new(1547, 1553, 2228, 2267),
                "特殊:火属性効果増加Lv.3",
                "火属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。火属性メモリアの通常攻撃はスキル効果が増加。",
                30,
                120,
                true,
                Kinds.Elemental.Fire,
                true
            ),
            new Order(
                55,
                "羽翼の勇猛",
                new(1489, 1434, 926, 914),
                "通常:風属性効果増加Lv.3",
                "風属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。風属性メモリアの特殊攻撃はスキル効果が増加。",
                30,
                120,
                false,
                Kinds.Elemental.Wind,
                false
            ),
            new Order(
                54,
                "輝炎の神秘",
                new(913, 919, 1447, 1488),
                "特殊:火属性効果増加Lv.3",
                "火属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。火属性メモリアの通常攻撃はスキル効果が増加。",
                30,
                120,
                false,
                Kinds.Elemental.Fire,
                false
            ),
            new Order(
                53,
                "溟海の勇猛",
                new(2277, 2225, 1576, 1565),
                "通常:水属性効果増加Lv.3",
                "水属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。水属性メモリアの特殊攻撃はスキル効果が増加。",
                30,
                120,
                true,
                Kinds.Elemental.Water,
                true
            ),
            new Order(
                52,
                "裂空の神秘",
                new(1574, 1554, 2226, 2298),
                "特殊:風属性効果増加Lv.3",
                "風属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。風属性メモリアの通常攻撃はスキル効果が増加。",
                30,
                120,
                true,
                Kinds.Elemental.Wind,
                true
            ),
            new Order(
                51,
                "大水の勇猛",
                new(1475, 1429, 916, 905),
                "通常:水属性効果増加Lv.3",
                "水属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。水属性メモリアの特殊攻撃はスキル効果が増加。",
                30,
                120,
                false,
                Kinds.Elemental.Water,
                false
            ),
            new Order(
                50,
                "風薙の神秘",
                new(925, 921, 1544, 1460),
                "特殊:風属性効果増加Lv.3",
                "風属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。風属性メモリアの通常攻撃はスキル効果が増加。",
                30,
                120,
                false,
                Kinds.Elemental.Wind,
                false
            ),
            new Order(
                49,
                "刻戻りのクロノグラフ",
                new(1558, 2215, 1574, 2281),
                "オーダー使用リセットLv.3",
                "自身を除く味方のオーダーの使用回数がリセットされる。",
                30,
                0,
                true,
                Kinds.Other,
                false
            ),
            new Order(
                48,
                "回帰の砂時計",
                new(915, 1429, 916, 1465),
                "オーダー使用リセットLv.3",
                "自身を除く味方のオーダーの使用回数がリセットされる。",
                30,
                0,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                47,
                "特異返しの鉄壁",
                new(2270, 2222, 1550, 1580),
                "特殊ダメージ軽減Lv.3",
                "敵から受ける特殊系攻撃のダメージを大軽減する。",
                30,
                90,
                true,
                Kinds.Shield,
                false
            ),
            new Order(
                46,
                "女帝蝶の火継",
                new(2026, 2007, 1413, 1383),
                "火属性効果増加Lv.3",
                "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                30,
                120,
                true,
                Kinds.Elemental.Fire,
                false
            ),
            new Order(
                45,
                "特異返しの盾",
                new(1489, 1434, 926, 914),
                "特殊ダメージ軽減Lv.3",
                "敵から受ける特殊系攻撃のダメージを大軽減する。",
                30,
                90,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                44,
                "劫火の勇猛",
                new(2286, 2222, 1540, 1580),
                "通常:火属性効果増加Lv.3",
                "火属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。火属性メモリアの特殊攻撃はスキル効果が増加。",
                30,
                120,
                true,
                Kinds.Elemental.Fire,
                true
            ),
            new Order(
                43,
                "焔の勇猛",
                new(1485, 1431, 934, 900),
                "通常:火属性効果増加Lv.3",
                "火属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。火属性メモリアの特殊攻撃はスキル効果が増加。",
                30,
                120,
                false,
                Kinds.Elemental.Fire,
                false
            ),
            new Order(
                42,
                "雪獄の息吹",
                new(1577, 1540, 2248, 2291),
                "特殊:水属性効果増加Lv.3",
                "水属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。水属性メモリアの通常攻撃はスキル効果が増加。",
                30,
                120,
                true,
                Kinds.Elemental.Water,
                true
            ),
            new Order(
                41,
                "神渡しの風巻き",
                new(2041, 1980, 1421, 1411),
                "風属性効果増加Lv.3",
                "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                30,
                120,
                true,
                Kinds.Elemental.Wind,
                false
            ),
            new Order(
                40,
                "衝撃返しの鉄壁",
                new(1566, 1565, 2293, 2220),
                "通常ダメージ軽減Lv.3",
                "敵から受ける通常系攻撃のダメージを大軽減する。",
                30,
                90,
                true,
                Kinds.Shield,
                false
            ),
            new Order(
                39,
                "衝撃返しの盾",
                new(913, 919, 1447, 1488),
                "通常ダメージ軽減Lv.3",
                "敵から受ける通常系攻撃のダメージを大軽減する。",
                30,
                90,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                38,
                "氷霧の息吹",
                new(929, 924, 1436, 1474),
                "特殊:水属性効果増加Lv.3",
                "水属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。水属性メモリアの通常攻撃はスキル効果が増加。",
                30,
                120,
                false,
                Kinds.Elemental.Water,
                false
            ),
            new Order(
                37,
                "敵城砦鉄壁破壊",
                new(2217, 1546, 2295, 1559),
                "防御力減少Lv.3",
                "敵前衛のDEF/Sp.DEFを特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                36,
                "天鳴雨の波紋",
                new(1420, 1411, 2047, 1995),
                "水属性効果増加Lv.3",
                "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                30,
                120,
                true,
                Kinds.Elemental.Water,
                false
            ),
            new Order(
                35,
                "敵城防壁破壊",
                new(1435, 921, 1494, 900),
                "防御力減少Lv.3",
                "敵前衛のDEF/Sp.DEFを特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                34,
                "守勢強化の鉄壁",
                new(1569, 2235, 1566, 2275),
                "防御力増加Lv.3",
                "味方前衛のDEF/Sp.DEFを特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                33,
                "聖剣縛りの蔦",
                new(1473, 2125, 1464, 2204),
                "攻撃力減少Lv.3",
                "敵前衛のATK/Sp.ATKを特大減少させる。",
                20,
                0,
                true,
                Kinds.DeBuff,
                false
            ),
            new Order(
                32,
                "守勢強化の壁",
                new(855, 1388, 886, 1408),
                "防御力増加Lv.3",
                "味方前衛のDEF/Sp.DEFを特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                31,
                "攻勢激化の聖剣",
                new(2134, 1492, 2179, 1466),
                "攻撃力増加Lv.3",
                "味方前衛のATK/Sp.ATKを特大上昇させる。",
                20,
                0,
                true,
                Kinds.Buff,
                false
            ),
            new Order(
                30,
                "豪剣縛りの蔦",
                new(929, 1484, 926, 1424),
                "攻撃力減少Lv.3",
                "敵前衛のATK/Sp.ATKを特大減少させる。",
                20,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                29,
                "日輪の覚醒妨害",
                new(1493, 2123, 1480, 2180),
                "補助スキル発動率減少Lv.3",
                "敵全体のレギオンマッチ補助スキル発動率が特大減少。レギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                20,
                80,
                true,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                28,
                "攻勢激化の豪剣",
                new(1473, 919, 1447, 928),
                "攻撃力増加Lv.3",
                "味方前衛のATK/Sp.ATKを特大上昇させる。",
                20,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                27,
                "覚醒の日輪",
                new(1493, 1490, 2141, 2206),
                "補助スキル発動率増加Lv.3",
                "味方全体のレギオンマッチ補助スキル発動率が特大増加。",
                20,
                80,
                true,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                26,
                "三日月の覚醒妨害",
                new(915, 1479, 916, 1415),
                "補助スキル発動率減少Lv.3",
                "敵全体のレギオンマッチ補助スキル発動率が特大減少。レギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                20,
                80,
                false,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                25,
                "覚醒の三日月",
                new(879, 875, 1390, 1406),
                "補助スキル発動率増加Lv.3",
                "味方全体のレギオンマッチ補助スキル発動率が特大増加。",
                20,
                80,
                false,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                24,
                "覚醒妨害",
                new(594, 997, 574, 955),
                "補助スキル発動率減少Lv.2",
                "敵全体のレギオンマッチ補助スキル発動率が大減少。レギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                30,
                70,
                false,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                23,
                "覚醒の明星",
                new(950, 592, 999, 575),
                "補助スキル発動率増加Lv.2",
                "味方全体のレギオンマッチ補助スキル発動率が大増加。",
                30,
                70,
                false,
                Kinds.TriggerRateFluctuation,
                false
            ),
            new Order(
                22,
                "後衛魔導強化の陣",
                new(592, 580, 954, 989),
                "後衛MP回復Lv.2",
                "味方後衛のMPを70%回復。",
                30,
                0,
                false,
                Kinds.Mp,
                false
            ),
            new Order(
                21,
                "前衛魔導強化の陣",
                new(947, 987, 573, 608),
                "前衛MP回復Lv.2",
                "味方前衛のMPを70%回復。",
                30,
                0,
                false,
                Kinds.Mp,
                false
            ),
            new Order(
                20,
                "広域魔導強化の陣",
                new(576, 576, 990, 958),
                "全体MP回復Lv.2",
                "味方全体のMPを30%回復。",
                30,
                0,
                false,
                Kinds.Mp,
                false
            ),
            new Order(
                19,
                "魔減の展開",
                new(594, 594, 927, 1005),
                "MP軽減Lv.2",
                "味方全体のメモリアスキルの消費MPが大軽減。",
                30,
                70,
                false,
                Kinds.Mp,
                false
            ),
            new Order(
                18,
                "風止めの盾",
                new(597, 1245, 596, 678),
                "風属性効果減少Lv.2",
                "敵の使用した風属性のスキル効果が大減少。",
                30,
                70,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                17,
                "水払いの盾",
                new(592, 683, 601, 1239),
                "水属性効果減少Lv.2",
                "敵の使用した水属性のスキル効果が大減少。",
                30,
                70,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                16,
                "火除けの盾",
                new(594, 1237, 573, 711),
                "火属性効果減少Lv.2",
                "敵の使用した火属性のスキル効果が大減少。",
                30,
                70,
                false,
                Kinds.Shield,
                false
            ),
            new Order(
                15,
                "革命の御旗",
                new(2132, 1486, 2179, 1488),
                "劣勢時攻撃効果増加Lv.3",
                "味方前衛全員のスキル攻撃の効果が特大増加。",
                30,
                120,
                true,
                Kinds.Other,
                false
            ),
            new Order(
                14,
                "不屈の御旗",
                new(1407, 886, 1388, 856),
                "劣勢時攻撃効果増加Lv.3",
                "味方前衛全員のスキル攻撃の効果が特大増加。",
                30,
                120,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                13,
                "水神の怒り",
                new(1467, 1485, 2171, 2160),
                "水属性効果増加Lv.3",
                "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                30,
                120,
                true,
                Kinds.Elemental.Water,
                false
            ),
            new Order(
                12,
                "氷結の布陣",
                new(855, 886, 1438, 1358),
                "水属性効果増加Lv.3",
                "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                30,
                120,
                false,
                Kinds.Elemental.Water,
                false
            ),
            new Order(
                11,
                "朱雀炎武",
                new(2129, 2171, 1465, 1471),
                "火属性効果増加Lv.3",
                "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                30,
                120,
                true,
                Kinds.Elemental.Fire,
                false
            ),
            new Order(
                10,
                "決死の御旗",
                new(991, 606, 955, 606),
                "劣勢時攻撃効果増加Lv.2",
                "味方前衛全員のスキル攻撃の効果が大増加。",
                40,
                110,
                false,
                Kinds.Other,
                false
            ),
            new Order(
                9,
                "火烈なる布陣",
                new(1435, 1394, 854, 889),
                "火属性効果増加Lv.3",
                "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                30,
                120,
                false,
                Kinds.Elemental.Fire,
                false
            ),
            new Order(
                8,
                "天翼のしらべ",
                new(2201, 2137, 1502, 1492),
                "風属性効果増加Lv.3",
                "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                30,
                120,
                true,
                Kinds.Elemental.Wind,
                false
            ),
            new Order(
                7,
                "絶風大破の陣",
                new(1362, 1435, 882, 861),
                "風属性効果増加Lv.3",
                "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                30,
                120,
                false,
                Kinds.Elemental.Wind,
                false
            ),
            new Order(
                6,
                "敵陣防壁破壊",
                new(578, 956, 589, 986),
                "防御力減少Lv.2",
                "敵前衛のDEF/Sp.DEFを大減少させる。",
                30,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                5,
                "刀縛りの蔦",
                new(997, 604, 938, 601),
                "攻撃力減少Lv.2",
                "敵前衛のATK/Sp.ATKを大減少させる。",
                30,
                0,
                false,
                Kinds.DeBuff,
                false
            ),
            new Order(
                4,
                "守勢強化の盾",
                new(582, 989, 581, 950),
                "防御力増加Lv.2",
                "味方前衛のDEF/Sp.DEFを大上昇させる。",
                30,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                3,
                "攻勢強化の剣",
                new(948, 602, 980, 595),
                "攻撃力増加Lv.2",
                "味方前衛のATK/Sp.ATKを大上昇させる。",
                30,
                0,
                false,
                Kinds.Buff,
                false
            ),
            new Order(
                2,
                "疾風の心得",
                new(947, 1007, 585, 601),
                "風属性効果増加Lv.2",
                "風属性のメモリアのスキル効果が大増加するが、敵から受ける火属性ダメージも増加。",
                40,
                110,
                false,
                Kinds.Elemental.Wind,
                false
            ),
            new Order(
                1,
                "流水の心得",
                new(588, 606, 1005, 959),
                "水属性効果増加Lv.2",
                "水属性のメモリアのスキル効果が大増加するが、敵から受ける風属性ダメージも増加。",
                40,
                110,
                false,
                Kinds.Elemental.Water,
                false
            ),
            new Order(
                0,
                "火炎の心得",
                new(995, 933, 601, 586),
                "火属性効果増加Lv.2",
                "火属性のメモリアのスキル効果が大増加するが、敵から受ける水属性ダメージも増加。",
                40,
                110,
                false,
                Kinds.Elemental.Fire,
                false
            ),
        ];

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
    }
}
