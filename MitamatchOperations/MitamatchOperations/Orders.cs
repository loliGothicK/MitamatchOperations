using System;
using System.Linq;
using mitama.OrderKinds;

namespace mitama
{
    public abstract record Kind();

    namespace OrderKinds
    {
        public record Elemental(Element Element) : Kind;
        public record Buff() : Kind;
        public record DeBuff() : Kind;
        public record Mp() : Kind;
        public record TriggerRateFluctuation() : Kind;
        public record Shield() : Kind;
        public record Formation() : Kind;
        public record Stack() : Kind;
        public record Other() : Kind;
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
        Special,
    }

    public readonly record struct Order(
        ushort Index,
        string Name,
        string Effect,
        string Description,
        uint PrepareTIme,
        uint ActiveTime,
        Kind Kind
    )
    {
        public static implicit operator Order(ValueTuple<ushort, string, string, string, uint, uint, Kind> from) =>
            new(from.Item1, from.Item2, from.Item3, from.Item4, from.Item5, from.Item6, from.Item7);

        public string Path => $@"Assets/orders/{Index}.png";

        public string TimeFmt => ActiveTime switch {
            0 => $"({PrepareTIme} sec)",
            _ => $"({PrepareTIme} + {ActiveTime} sec)",
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

        private static Order[] Init()
        {
            var orders = new ValueTuple<string, string, string, uint, uint, Kind>[]
            {
                (
                    "天翼のしらべ",
                    "レギオンマッチスキル風属性効果増加Lv.3",
                    "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                    30, 120,
                    Kinds.Elemental.Wind
                ),
                (
                    "朱雀炎武",
                    "レギオンマッチスキル火属性効果増加Lv.3",
                    "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                    30, 120,
                    Kinds.Elemental.Fire
                ),
                (
                    "水神の怒り",
                    "レギオンマッチスキル水属性効果増加Lv.3",
                    "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                    30, 120,
                    Kinds.Elemental.Water
                ),
                (
                    "革命の御旗",
                    "レギオンマッチスキル劣勢時攻撃効果増加Lv.3",
                    "味方前衛全員のスキル攻撃の効果が特大増加。",
                    30, 120,
                    Kinds.Other
                ),
                (
                    "覚醒の日輪",
                    "レギオンマッチスキル補助スキル発動率増加Lv.3",
                    "味方全体のレギオンマッチ補助スキル発動率が特大増加。",
                    20, 80,
                    Kinds.TriggerRateFluctuation
                ),
                (
                    "日輪の覚醒妨害", "レギオンマッチスキル補助スキル発動率減少Lv.3",
                    "敵全体のレギオンマッチ補助スキル発動率が特大減少。\nレギオンマッチの残り時間が2分未満になると、その時点でこのオーダーは終了する。",
                    20, 80,
                    Kinds.TriggerRateFluctuation
                ),
                (
                    "攻勢激化の聖剣",
                    "レギオンマッチスキル攻撃力増加Lv.3",
                    "味方前衛のATK/Sp.ATKを特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "聖剣縛りの蔦",
                    "レギオンマッチスキル攻撃力減少Lv.3",
                    "敵前衛のATK/Sp.ATKを特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "守勢強化の鉄壁",
                    "レギオンマッチスキル防御力増加Lv.3",
                    "味方前衛のDEF/Sp.DEFを特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "天鳴雨の波紋",
                    "レギオンマッチスキル水属性効果増加Lv.3",
                    "水属性のメモリアのスキル効果が特大増加するが、敵から受ける風属性ダメージも増加。",
                    30, 120,
                    Kinds.Elemental.Water
                ),
                (
                    "敵城砦鉄壁破壊",
                    "レギオンマッチスキル防御力減少Lv.3",
                    "敵前衛のDEF/Sp.DEFを特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "衝撃返しの鉄壁",
                    "レギオンマッチスキル通常ダメージ軽減Lv.3",
                    "敵から受ける通常系攻撃のダメージを大軽減する。",
                    30, 90,
                    Kinds.Shield
                ),
                (
                    "神渡しの風巻き",
                    "レギオンマッチスキル風属性効果増加Lv.3",
                    "風属性のメモリアのスキル効果が特大増加するが、敵から受ける火属性ダメージも増加。",
                    30, 120,
                    Kinds.Elemental.Wind
                ),
                (
                    "雪獄の息吹",
                    "レギオンマッチスキル特殊水属性効果増加Lv.3",
                    "水属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。水属性メモリアの通常攻撃はスキル効果が増加。",
                    30, 120,
                    Kinds.Elemental.Water
                ),
                (
                    "劫火の勇猛",
                    "レギオンマッチスキル通常火属性効果増加Lv.3",
                    "火属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。火属性メモリアの特殊攻撃はスキル効果が増加。",
                    30, 120,
                    Kinds.Elemental.Fire
                ),
                (
                    "女帝蝶の火継",
                    "レギオンマッチスキル火属性効果増加Lv.3",
                    "火属性のメモリアのスキル効果が特大増加するが、敵から受ける水属性ダメージも増加。",
                    30, 120,
                    Kinds.Elemental.Fire
                ),
                (
                    "特異返しの鉄壁",
                    "レギオンマッチスキル特殊ダメージ軽減Lv.3",
                    "敵から受ける特殊系攻撃のダメージを大軽減する。",
                    30, 90,
                    Kinds.Shield
                ),
                (
                    "刻戻りのクロノグラフ",
                    "レギオンマッチスキルオーダー使用リセットLv.3",
                    "自身を除く味方のオーダーの使用回数がリセットされる。",
                    30, 0,
                    Kinds.Other
                ),
                (
                    "裂空の神秘",
                    "レギオンマッチスキル特殊風属性効果増加Lv.3",
                    "風属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。\n風属性メモリアの通常攻撃はスキル効果が増加。",
                    30, 120,
                    Kinds.Elemental.Wind
                ),
                (
                    "溟海の勇猛",
                    "レギオンマッチスキル通常水属性効果増加Lv.3",
                    "水属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。水属性メモリアの特殊攻撃はスキル効果が増加。",
                    30, 120,
                    Kinds.Elemental.Water
                ),
                (
                    "煌炎の神秘",
                    "レギオンマッチスキル特殊火属性効果増加Lv.3",
                    "火属性メモリアの特殊攻撃、支援、妨害、回復のスキル効果が特大増加。火属性メモリアの通常攻撃はスキル効果が増加。",
                    30, 120,
                    Kinds.Elemental.Fire
                ),
                (
                    "霊鳥の勇猛",
                    "レギオンマッチスキル通常風属性効果増加Lv.3",
                    "風属性メモリアの通常攻撃、支援、妨害、回復のスキル効果が特大増加。\n風属性メモリアの特殊攻撃はスキル効果が増加。",
                    30, 120,
                    Kinds.Elemental.Wind
                ),
                (
                    "広域再編の陣",
                    "レギオンマッチスキル全体再編Lv.3",
                    "味方全体のユニットチェンジの使用回数がリセットされる。",
                    30, 0,
                    Kinds.Formation
                ),
                (
                    "朱雀の御盾",
                    "レギオンマッチスキル火属性効果減少Lv.3",
                    "敵の使用した火属性のスキル効果が特大減少。",
                    20, 100,
                    Kinds.Shield
                ),
                (
                    "水神の御盾",
                    "レギオンマッチスキル水属性効果減少Lv.3",
                    "敵の使用した水属性のスキル効果が特大減少。",
                    20, 100,
                    Kinds.Shield
                ),
                (
                    "天翼の御盾",
                    "レギオンマッチスキル風属性効果減少Lv.3",
                    "敵の使用した風属性のスキル効果が特大減少。",
                    20, 100,
                    Kinds.Shield
                ),
                (
                    "広域魔導凱旋",
                    "レギオンマッチスキル全体MP回復Lv.3",
                    "味方全体のMPを40%回復。",
                    20, 0,
                    Kinds.Mp
                ),
                (
                    "魔縮領域",
                    "レギオンマッチスキルMP軽減Lv.3",
                    "味方全体のメモリアスキルの消費MPが特大軽減。",
                    20, 80,
                    Kinds.Mp
                ),
                (
                    "暗黒業火",
                    "レギオンマッチスキル闇・火属性効果増加Lv.3",
                    "闇属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                    15, 60,
                    Kinds.Elemental.Dark
                ),
                (
                    "黒碑水鏡",
                    "レギオンマッチスキル闇・水属性効果増加Lv.3",
                    "闇属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                    15, 60,
                    Kinds.Elemental.Water
                ),
                (
                    "黒貂威風",
                    "レギオンマッチスキル闇・風属性効果増加Lv.3",
                    "闇属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                    15, 60,
                    Kinds.Elemental.Dark
                ),
                (
                    "光背火翼",
                    "レギオンマッチスキル光・火属性効果増加Lv.3",
                    "光属性メモリアのスキル効果が特大増加し、火属性メモリアのスキル効果が増加する。",
                    15, 60,
                    Kinds.Elemental.Light
                ),
                (
                    "天光銀波",
                    "レギオンマッチスキル光・水属性効果増加Lv.3",
                    "光属性メモリアのスキル効果が特大増加し、水属性メモリアのスキル効果が増加する。",
                    15, 60,
                    Kinds.Elemental.Light
                ),
                (
                    "清暉恒風",
                    "レギオンマッチスキル攻撃力増加Lv.3",
                    "味方前衛のATK/Sp.ATKを特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "光華廻風",
                    "レギオンマッチスキル光・風属性効果増加Lv.3",
                    "光属性メモリアのスキル効果が特大増加し、風属性メモリアのスキル効果が増加する。",
                    15, 60,
                    Kinds.Elemental.Light
                ),
                (
                    "前衛再編の陣",
                    "レギオンマッチスキル前衛再編Lv.3",
                    "味方前衛のユニットチェンジの使用回数がリセットされる。",
                    15, 0,
                    Kinds.Formation
                ),
                (
                    "後衛再編の陣",
                    "レギオンマッチスキル後衛再編Lv.3",
                    "味方後衛のユニットチェンジの使用回数がリセットされる。",
                    15, 0,
                    Kinds.Formation
                ),
                (
                    "陰陽二律",
                    "レギオンマッチスキル[光闇]属性効果増加Lv.3",
                    "光属性と闇属性のメモリアスキル効果が特大増加する。",
                    15, 60,
                    Kinds.Elemental.Special
                ),
                (
                    "玲瓏光艶",
                    "レギオンマッチスキル光属性効果増加Lv.3",
                    "光属性のメモリアのスキル効果が特大増加する。",
                    30, 120,
                    Kinds.Elemental.Light
                ),
                (
                    "暗碧無双",
                    "レギオンマッチスキル闇属性効果増加Lv.3",
                    "闇属性のメモリアのスキル効果が特大増加する。",
                    30, 120,
                    Kinds.Elemental.Special
                ),
                (
                    "光刃激化の聖剣",
                    "レギオンマッチスキル光属性攻撃力増加Lv.3",
                    "味方前衛の光属性攻撃力を特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "光鎧強化の鉄壁",
                    "レギオンマッチスキル光属性防御力増加Lv.3",
                    "味方前衛の光属性防御力を特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "闇刃激化の聖剣",
                    "レギオンマッチスキル闇属性攻撃力増加Lv.3",
                    "味方前衛の闇属性攻撃力を特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "闇鎧強化の鉄壁",
                    "レギオンマッチスキル闇属性防御力増加Lv.3",
                    "味方前衛の闇属性防御力を特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "光刃縛りの大蔦",
                    "レギオンマッチスキル光属性攻撃力減少Lv.3",
                    "敵前衛の光属性攻撃力を特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "光鎧の鉄壁破壊",
                    "レギオンマッチスキル光属性防御力減少Lv.3",
                    "敵前衛の光属性防御力を特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "闇刃縛りの大蔦",
                    "レギオンマッチスキル闇属性攻撃力減少Lv.3",
                    "敵前衛の闇属性攻撃力を特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "闇鎧の鉄壁破壊",
                    "レギオンマッチスキル闇属性防御力減少Lv.3",
                    "敵前衛の闇属性防御力を特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "水刃激化の聖剣",
                    "レギオンマッチスキル水属性攻撃力増加Lv.3",
                    "味方前衛の水属性攻撃力を特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "水鎧の鉄壁破壊",
                    "レギオンマッチスキル水属性防御力減少Lv.3",
                    "敵前衛の水属性防御力を特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "水鎧強化の鉄壁",
                    "レギオンマッチスキル水属性防御力増加Lv.3",
                    "味方前衛の水属性防御力を特大上昇させる。",
                    20, 0,
                    Kinds.Buff
                ),
                (
                    "水刃縛りの大蔦",
                    "レギオンマッチスキル水属性攻撃力減少Lv.3",
                    "敵前衛の水属性攻撃力を特大減少させる。",
                    20, 0,
                    Kinds.DeBuff
                ),
                (
                    "戦術加速の策",
                    "レギオンマッチスキル準備時間短縮Lv.3",
                    "味方が次に使うオーダースキルの準備時間を5秒に変更する。",
                    5, 0,
                    Kinds.Stack
                ),
            };

            return orders.Select((v, index) =>
                new Order((ushort)index, v.Item1, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6)).ToArray();
        }
    }
}
