using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace mitama.Domain;

public record struct Unit(string UnitName, bool IsFront, List<Memoria> Memorias)
{
    public string ToJson() =>
        JsonSerializer.Serialize(new UnitDto(UnitName, IsFront, Memorias.Select(m => m.Id).ToArray()));

    public static Unit FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<UnitDto>(json);
        var selector = Memoria.List.ToDictionary(m => m.Id);
        return new Unit(dto.UnitName, dto.IsFront, dto.Ids.Select(id => selector[id]).ToList());
    }
}

public record struct UnitDto(string UnitName, bool IsFront, int[] Ids);

public enum Type
{
    Normal,
    Special,
}

public enum Level
{
    One,
    Two,
    Three,
    ThreePlus,
    Four,
    FourPlus,
    Five,
    FivePlus,
    Lg,
    LgPlus,
}

public abstract record StatusChange;

public abstract record Stat;

public record Atk : Stat;

public record Def : Stat;

public record SpAtk : Stat;

public record SpDef : Stat;

public record ElementAttack(Element Element) : Stat;

public record ElementGuard(Element Element) : Stat;

public record Life : Stat;

public enum Amount
{
    // 小
    Small,

    // 中
    Medium,

    // 大
    Large,

    // 特大
    ExtraLarge,

    // 超特大
    SuperExtraLarge,
}

public record StatusUp(Stat Stat, Amount Amount) : StatusChange;

public record StatusDown(Stat Stat, Amount Amount) : StatusChange;

public enum Range
{
    A,
    B,
    C,
    D,
    E,
}

public abstract record SkillEffect;

public record ElementStimulation(Element Element) : SkillEffect;

public record Heal : SkillEffect;

public record Charge : SkillEffect;

public record Recover : SkillEffect;

public record ElementSpread(Element Element) : SkillEffect;

public record ElementStrengthen(Element Element) : SkillEffect;

public record ElementWeaken(Element Element) : SkillEffect;

public record Counter : SkillEffect;

public record Skill(
    string Name,
    string Description,
    SkillEffect[] Effects,
    // ステータス変動
    StatusChange[] StatusChanges,
    Level Level,
    Range Range
);

public enum Trigger
{
    // 攻
    Attack,

    // 援
    Support,

    // 回
    Recovery,

    // コ
    Command,
}

public abstract record SupportEffect;

public record NormalMatchPtUp : SupportEffect;

public record SpecialMatchPtUp : SupportEffect;

public record DamageUp : SupportEffect;

public record PowerUp(Type Type) : SupportEffect;

public record PowerDown(Type Type) : SupportEffect;

public record GuardUp(Type Type) : SupportEffect;

public record GuardDown(Type Type) : SupportEffect;

public record ElementPowerUp(Element Element) : SupportEffect;

public record ElementPowerDown(Element Element) : SupportEffect;

public record ElementGuardUp(Element Element) : SupportEffect;

public record ElementGuardDown(Element Element) : SupportEffect;

public record SupportUp : SupportEffect;

public record RecoveryUp : SupportEffect;

public record MpCostDown : SupportEffect;

public record SupportSkill(
    string Name,
    string Description,
    Trigger Trigger,
    SupportEffect[] Effects,
    Level Level
);

public abstract record MemoriaKind;

public enum VanguardKind
{
    NormalSingle,
    NormalRange,
    SpecialSingle,
    SpecialRange,
}

public enum RearguardKind
{
    Support,
    Interference,
    Recovery,
}

public record Vanguard(VanguardKind Kind) : MemoriaKind;

public record Rearguard(RearguardKind Kind) : MemoriaKind;

public record Memoria(
    int Id,
    string Link,
    string Name,
    MemoriaKind Kind,
    Element Element,
    Status Status,
    int Cost,
    Skill Skill,
    SupportSkill SupportSkill,
    bool IsLegendary = false
)
{
    public Uri Uri => new($"ms-appx:///Assets/memoria/{Name}.png");
    public string Path = $"/Assets/memoria/{Name}.png";
    public string ToJson()
    {
        return $@"{{ ""link"": ""{HttpUtility.UrlEncode(Link)}"", ""name"": ""{HttpUtility.UrlEncode(Name)}"" }}";
    }

    public virtual bool Equals(Memoria other) => Id == other?.Id;
    public override int GetHashCode() => Name.GetHashCode();

    public static readonly Memoria[] List =
    {
        new Memoria(
            13,
            "クリエイターズコラボ秋の日に描く面影",
            "秋の日に描く面影",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(7037, 2592, 5667, 2602),
            22,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP/副攻:火パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、火属性攻撃力を大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal), new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            12,
            "クリエイターズコラボ秋の日に描く面影",
            "秋の日に描く面影",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2585, 7044, 2615, 5654),
            22,
            new Skill(
                "火：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            11,
            "クリエイターズコラボ秋の日に描く面影",
            "秋の日に描く面影",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2585, 7044, 2615, 5654),
            22,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅳ",
                "味方1～3体のSp.ATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP/副援:火パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。さらに、火属性攻撃力を大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            10,
            "クリエイターズコラボ秋の日に描く面影",
            "秋の日に描く面影",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(7037, 2592, 5667, 2602),
            22,
            new Skill(
                "ファイアパワーフォールC Ⅳ",
                "敵1～3体のATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN/副援:火パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。さらに、火属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new ElementPowerDown(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            9,
            "クリエイターズコラボ小さな音楽会",
            "小さな音楽会",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(6902, 2768, 5505, 2756),
            22,
            new Skill(
                "ファイアガードヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP/副援:火ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の火属性防御力を大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new ElementGuardUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            8,
            "クリエイターズコラボ小さな音楽会",
            "小さな音楽会",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6902, 2768, 5505, 2756),
            22,
            new Skill(
                "火：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            7,
            "理が導く勝機",
            "理が導く勝機",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(7037, 2597, 5668, 2586),
            22,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            6,
            "理が導く勝機",
            "理が導く勝機",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2585, 7049, 2616, 5638),
            22,
            new Skill(
                "Sp.ウォーターパワーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.ATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            5,
            "理が導く勝機",
            "理が導く勝機",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(7037, 2597, 5668, 2586),
            22,
            new Skill(
                "水弱：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに敵の水属性攻撃力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            4,
            "理が導く勝機",
            "理が導く勝機",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2585, 2597, 5668, 7038),
            22,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special), new ElementGuardUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            3,
            "暮れなずむ廃址の佳人",
            "暮れなずむ廃址の佳人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6912, 2756, 5496, 2746),
            22,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            2,
            "暮れなずむ廃址の佳人",
            "暮れなずむ廃址の佳人",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6912, 2756, 5496, 2746),
            22,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP/副援:水パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。さらに、水属性攻撃力を大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal), new ElementPowerUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1,
            "夕映えに冴える一手",
            "夕映えに冴える一手",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2766, 6891, 2746, 5490),
            22,
            new Skill(
                "Sp.ウォーターパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと水属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP/副援:水パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体の水属性攻撃力を大ダウンさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp(), new ElementPowerDown(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            0,
            "夕映えに冴える一手",
            "夕映えに冴える一手",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2766, 6891, 2746, 5490),
            22,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1127,
            "フラガラッハの光",
            "フラガラッハの光",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(3620, 3595, 4935, 4964),
            21,
            new Skill(
                "WガードヒールE LG",
                "味方2～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Lg,
                Range.E
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            1126,
            "クリエイターズコラボかわいい狩りの秋",
            "かわいい狩りの秋",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7068, 2616, 5653, 2586),
            22,
            new Skill(
                "火：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1125,
            "クリエイターズコラボかわいい狩りの秋",
            "かわいい狩りの秋",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2616, 7068, 2601, 5638),
            22,
            new Skill(
                "Sp.カウンターファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1124,
            "クリエイターズコラボかわいい狩りの秋",
            "かわいい狩りの秋",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2616, 7068, 2601, 5638),
            22,
            new Skill(
                "Sp.カウンターファイアパワーフォールC Ⅳ",
                "敵1～3体のSp.ATKと火属性攻撃力を大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1123,
            "クリエイターズコラボかわいい狩りの秋",
            "かわいい狩りの秋",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2616, 2616, 7053, 5638),
            22,
            new Skill(
                "カウンターファイアガードヒールD Ⅳ",
                "味方2体のHPを大回復し、DEFと火属性防御力を小アップする。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            1122,
            "クリエイターズコラボ藍の心と秋の空",
            "藍の心と秋の空",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6920, 2752, 5519, 2732),
            22,
            new Skill(
                "カウンターファイアパワーアシストC Ⅳ",
                "味方1～3体のATKと火属性攻撃力を大アップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1121,
            "クリエイターズコラボ藍の心と秋の空",
            "藍の心と秋の空",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6920, 2752, 5519, 2732),
            22,
            new Skill(
                "カウンターファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1120,
            "神庭の花火師",
            "神庭の花火師",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(7056, 2613, 5652, 2579),
            22,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1119,
            "神庭の花火師",
            "神庭の花火師",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2604, 7065, 2600, 5631),
            22,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1118,
            "神庭の花火師",
            "神庭の花火師",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2604, 7065, 2600, 5631),
            22,
            new Skill(
                "水強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の水属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1117,
            "神庭の花火師",
            "神庭の花火師",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(7056, 2613, 5652, 2579),
            22,
            new Skill(
                "ウォーターパワーフォールB Ⅲ",
                "敵1～2体のATKと水属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1116,
            "シネマティックに煌めいて",
            "シネマティックに煌めいて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2753, 2744, 5515, 6912),
            22,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1115,
            "シネマティックに煌めいて",
            "シネマティックに煌めいて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2753, 2744, 5515, 6912),
            22,
            new Skill(
                "水弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1114,
            "秘め事☆プールサイド",
            "秘め事☆プールサイド",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(3184, 6901, 3210, 6082),
            24,
            new Skill(
                "水拡：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。オーダースキル「水属性効果増加」を発動中は敵2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [new ElementSpread(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            1113,
            "クリエイターズコラボはつらつハイタッチ",
            "はつらつハイタッチ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(7045, 2580, 5657, 2605),
            22,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1112,
            "クリエイターズコラボはつらつハイタッチ",
            "はつらつハイタッチ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2593, 7032, 2605, 5657),
            22,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1111,
            "クリエイターズコラボはつらつハイタッチ",
            "はつらつハイタッチ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2593, 7032, 2605, 5657),
            22,
            new Skill(
                "Sp.ウォーターパワーフォールC Ⅳ",
                "敵1～3体のSp.ATKと水属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1110,
            "クリエイターズコラボはつらつハイタッチ",
            "はつらつハイタッチ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2593, 2580, 7057, 5657),
            22,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1109,
            "クリエイターズコラボ夢の中で隠れんぼ",
            "夢の中で隠れんぼ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6916, 2766, 5490, 2752),
            22,
            new Skill(
                "ウォーターパワーフォールC Ⅳ",
                "敵1～3体のATKと水属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN/副援:水パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。さらに、水属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new ElementPowerDown(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1108,
            "クリエイターズコラボ夢の中で隠れんぼ",
            "夢の中で隠れんぼ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6916, 2766, 5490, 2752),
            22,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1107,
            "クリエイターズコラボフレフレリリィ",
            "フレフレリリィ！",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2757, 2763, 5492, 6894),
            22,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1106,
            "クリエイターズコラボフレフレリリィ",
            "フレフレリリィ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2757, 2763, 5492, 6894),
            22,
            new Skill(
                "水弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1105,
            "クリエイターズコラボひそひそナイト",
            "ひそひそナイト",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6889, 2757, 5507, 2729),
            22,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1104,
            "クリエイターズコラボひそひそナイト",
            "ひそひそナイト",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6889, 2757, 5507, 2729),
            22,
            new Skill(
                "ウォーターパワーアシストC Ⅳ",
                "味方1～3体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1103,
            "クリエイターズコラボ新涼灯火",
            "新涼灯火",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7055, 2593, 5641, 2590),
            22,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1102,
            "クリエイターズコラボ新涼灯火",
            "新涼灯火",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2603, 7045, 2589, 5642),
            22,
            new Skill(
                "Sp.カウンターファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1101,
            "クリエイターズコラボ新涼灯火",
            "新涼灯火",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2603, 7045, 2589, 5642),
            22,
            new Skill(
                "Sp.カウンターファイアパワーフォールC Ⅳ",
                "敵1～3体のSp.ATKと火属性攻撃力を大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1100,
            "クリエイターズコラボ新涼灯火",
            "新涼灯火",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2603, 2593, 6341, 6342),
            22,
            new Skill(
                "WガードヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            1099,
            "クリエイターズコラボ秋のそうさく",
            "秋のそうさく",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6915, 2761, 5494, 2747),
            22,
            new Skill(
                "カウンターファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1098,
            "クリエイターズコラボ秋のそうさく",
            "秋のそうさく",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6915, 2761, 5494, 2747),
            22,
            new Skill(
                "カウンターパワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1097,
            "かけがえのない時間",
            "かけがえのない時間",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7036, 2614, 5645, 2612),
            22,
            new Skill(
                "火弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1096,
            "かけがえのない時間",
            "かけがえのない時間",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2584, 7066, 2593, 5664),
            22,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1095,
            "かけがえのない時間",
            "かけがえのない時間",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2584, 7066, 2593, 5664),
            22,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅳ",
                "味方1～3体のSp.ATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP/副援:火パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。さらに、火属性攻撃力を大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            1094,
            "かけがえのない時間",
            "かけがえのない時間",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2584, 2614, 5645, 7064),
            22,
            new Skill(
                "Sp.ファイアガードヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:火ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の火属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            1093,
            "双子メイド風スタイル",
            "双子メイド風スタイル",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2755, 2756, 6887, 5484),
            22,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            1092,
            "双子メイド風スタイル",
            "双子メイド風スタイル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2755, 2756, 6887, 5484),
            22,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1091,
            "素敵なお茶会",
            "素敵なお茶会",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7043, 2590, 5647, 2606),
            22,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1090,
            "素敵なお茶会",
            "素敵なお茶会",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2591, 7042, 2595, 5658),
            22,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1089,
            "素敵なお茶会",
            "素敵なお茶会",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(7043, 2590, 5647, 2606),
            22,
            new Skill(
                "ファイアパワーアシストB Ⅲ",
                "味方1～2体のATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1088,
            "素敵なお茶会",
            "素敵なお茶会",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6343, 6342, 2595, 2606),
            22,
            new Skill(
                "火弱：WパワーフォールD Ⅲ",
                "敵2体のATKとSp.ATKを大ダウンさせる。さらに敵の火属性攻撃力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1087,
            "メイドのうたたね",
            "メイドのうたたね",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2757, 6881, 2742, 5484),
            22,
            new Skill(
                "Sp.ファイアパワーフォールC Ⅳ",
                "敵1～3体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1086,
            "メイドのうたたね",
            "メイドのうたたね",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2757, 6881, 2742, 5484),
            22,
            new Skill(
                "火弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1085,
            "メイドのご奉仕",
            "メイドのご奉仕",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6916, 2752, 5509, 2734),
            22,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1084,
            "メイドのご奉仕",
            "メイドのご奉仕",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6916, 2752, 5509, 2734),
            22,
            new Skill(
                "ファイアパワーフォールB Ⅲ",
                "敵1～2体のATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1083,
            "海風に吹かれて",
            "海風に吹かれて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2736, 6898, 2742, 5487),
            22,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1082,
            "海風に吹かれて",
            "海風に吹かれて",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2736, 6898, 2742, 5487),
            22,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1081,
            "クリエイターズコラボ落ち葉のリース",
            "落ち葉のリース",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2580, 7060, 2615, 5637),
            22,
            new Skill(
                "水弱：Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1080,
            "クリエイターズコラボ落ち葉のリース",
            "落ち葉のリース",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2580, 7060, 2615, 5637),
            22,
            new Skill(
                "Sp.カウンターウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1079,
            "クリエイターズコラボ落ち葉のリース",
            "落ち葉のリース",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2580, 7060, 2615, 5637),
            22,
            new Skill(
                "Sp.カウンターパワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1078,
            "クリエイターズコラボ落ち葉のリース",
            "落ち葉のリース",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2580, 2608, 5667, 7037),
            22,
            new Skill(
                "Sp.カウンターウォーターガードヒールD Ⅳ",
                "味方2体のHPを大回復し、Sp.DEFと水属性防御力を小アップする。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1077,
            "クリエイターズコラボ深まる秋の一幕",
            "深まる秋の一幕",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6897, 2765, 5495, 2754),
            22,
            new Skill(
                "カウンターウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1076,
            "クリエイターズコラボ深まる秋の一幕",
            "深まる秋の一幕",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6897, 2765, 5495, 2754),
            22,
            new Skill(
                "ウォーターパワーフォールC Ⅳ",
                "敵1～3体のATKと水属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN/副援:水パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。さらに、水属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new ElementPowerDown(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1075,
            "雹嵐の如く",
            "雹嵐の如く",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3639, 4135, 2699, 2731),
            21,
            new Skill(
                "WパワーフォールE LG",
                "敵2～3体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Lg,
                Range.E
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            1074,
            "瓦礫の街に咲いてこそ",
            "瓦礫の街に咲いてこそ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7063, 2616, 5659, 2590),
            22,
            new Skill(
                "火弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1073,
            "瓦礫の街に咲いてこそ",
            "瓦礫の街に咲いてこそ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2611, 7068, 2607, 5642),
            22,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1072,
            "瓦礫の街に咲いてこそ",
            "瓦礫の街に咲いてこそ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2611, 7068, 2607, 5642),
            22,
            new Skill(
                "火強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の火属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1071,
            "瓦礫の街に咲いてこそ",
            "瓦礫の街に咲いてこそ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2611, 2616, 7059, 5642),
            22,
            new Skill(
                "ファイアガードヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1070,
            "掲げし長針と短針",
            "掲げし長針と短針",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6915, 2744, 5495, 2766),
            22,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1069,
            "掲げし長針と短針",
            "掲げし長針と短針",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6915, 2744, 5495, 2766),
            22,
            new Skill(
                "ファイアパワーアシストC Ⅳ",
                "味方1～3体のATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1068,
            "廃墟のアンティークドール",
            "廃墟のアンティークドール",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2745, 6902, 2765, 5514),
            22,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1067,
            "廃墟のアンティークドール",
            "廃墟のアンティークドール",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2745, 6902, 2765, 5514),
            22,
            new Skill(
                "Sp.ファイアガードフォールC Ⅳ",
                "敵1～3体のSp.DEFと火属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN/副援:火ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。さらに、火属性防御力を大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special), new ElementGuardDown(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            1066,
            "クリエイターズコラボ転ばぬ先の一葉",
            "転ばぬ先の一葉",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2754, 6888, 2761, 5506),
            22,
            new Skill(
                "Sp.カウンターファイアパワーアシストC Ⅳ",
                "味方1～3体のSp.ATKと火属性攻撃力を大アップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1065,
            "クリエイターズコラボ転ばぬ先の一葉",
            "転ばぬ先の一葉",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2754, 6888, 2761, 5506),
            22,
            new Skill(
                "Sp.カウンターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1064,
            "クリエイターズコラボ秋空を見上げながら",
            "秋空を見上げながら",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(7040, 2589, 5642, 2619),
            22,
            new Skill(
                "火：ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP/副攻:火パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、火属性攻撃力を大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal), new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            1063,
            "クリエイターズコラボ秋空を見上げながら",
            "秋空を見上げながら",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7040, 2589, 5642, 2619),
            22,
            new Skill(
                "カウンターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1062,
            "クリエイターズコラボ秋空を見上げながら",
            "秋空を見上げながら",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(7040, 2589, 5642, 2619),
            22,
            new Skill(
                "ウィンドパワーフォールC Ⅳ",
                "敵1～3体のATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN/副援:風パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。さらに、風属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new ElementPowerDown(Element.Wind)],
                Level.Three
            )
        ),
        new Memoria(
            1061,
            "クリエイターズコラボ秋空を見上げながら",
            "秋空を見上げながら",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2588, 2589, 5642, 7071),
            22,
            new Skill(
                "Sp.カウンターガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFを小アップする。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            1060,
            "優美な魔女に魅せられて",
            "優美な魔女に魅せられて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(7069, 2582, 5670, 2592),
            22,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1059,
            "優美な魔女に魅せられて",
            "優美な魔女に魅せられて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2617, 7034, 2618, 5644),
            22,
            new Skill(
                "水弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1058,
            "優美な魔女に魅せられて",
            "優美な魔女に魅せられて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(7069, 2582, 5670, 2592),
            22,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP/副援:水パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。さらに、水属性攻撃力を大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal), new ElementPowerUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1057,
            "優美な魔女に魅せられて",
            "優美な魔女に魅せられて",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2617, 7034, 2618, 5644),
            22,
            new Skill(
                "水弱：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに敵の水属性防御力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1056,
            "這い寄る妖艶",
            "這い寄る妖艶",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2731, 6914, 2760, 5501),
            22,
            new Skill(
                "水：Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP/副攻:水パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、水属性攻撃力を大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1055,
            "這い寄る妖艶",
            "這い寄る妖艶",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2731, 6914, 2760, 5501),
            22,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1054,
            "健気×恥じらい＝",
            "健気×恥じらい＝",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3179, 3204, 6484, 6503),
            24,
            new Skill(
                "水：WガードヒールD Ⅳ",
                "味方2体のHPを大回復し、味方のDEFとSp.DEFを小アップする。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            1053,
            "百合ヶ丘ティーパーティ2023開催",
            "百合ヶ丘ティーパーティ2023開催！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1570, 2264, 1601, 2234),
            18,
            new Skill(
                "WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1052,
            "百合ヶ丘ティーパーティ2023開催",
            "百合ヶ丘ティーパーティ2023開催！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1570, 2264, 1601, 2234),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1051,
            "クリエイターズコラボもみじまんじゅう",
            "もみじまんじゅう",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2745, 6921, 2734, 5511),
            22,
            new Skill(
                "Sp.カウンターファイアパワーフォールC Ⅳ",
                "敵1～3体のSp.ATKと火属性攻撃力を大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1050,
            "クリエイターズコラボもみじまんじゅう",
            "もみじまんじゅう",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2745, 6921, 2734, 5511),
            22,
            new Skill(
                "Sp.カウンターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1049,
            "クリエイターズコラボ楓の頃",
            "楓の頃",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6916, 2750, 5494, 2755),
            22,
            new Skill(
                "カウンターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1048,
            "クリエイターズコラボ楓の頃",
            "楓の頃",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6916, 2750, 5494, 2755),
            22,
            new Skill(
                "ウォーターパワーアシストC Ⅳ",
                "味方1～3体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1047,
            "光射す守護者",
            "光射す守護者",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6906, 2729, 5492, 2769),
            22,
            new Skill(
                "火弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1046,
            "光射す守護者",
            "光射す守護者",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(6906, 2729, 5492, 2769),
            22,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1045,
            "緋染めの天使",
            "緋染めの天使",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2742, 6901, 2746, 5502),
            22,
            new Skill(
                "火弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1044,
            "緋染めの天使",
            "緋染めの天使",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2742, 6901, 2746, 5502),
            22,
            new Skill(
                "火強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の火属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1043,
            "誓いの剣閃",
            "誓いの剣閃",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6505, 6488, 3208, 3198),
            24,
            new Skill(
                "火：WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            1042,
            "emotionalmemoriaねこさんこちら",
            "ねこさんこちら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2229, 6255, 2247, 4909),
            21,
            new Skill(
                "Sp.カウンターファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1041,
            "emotionalmemoriaねこさんこちら",
            "ねこさんこちら",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2229, 6255, 2247, 4909),
            21,
            new Skill(
                "火強：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方の火属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1040,
            "たつき100％",
            "たつき100％",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2248, 2248, 4873, 6253),
            21,
            new Skill(
                "Sp.ファイアガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:火ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の火属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            1039,
            "たつき100％",
            "たつき100％",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2248, 2248, 4873, 6253),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP/副攻:水パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、水属性攻撃力を大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            1038,
            "彼方の宵空",
            "彼方の宵空",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6250, 2238, 4884, 2237),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1037,
            "彼方の宵空",
            "彼方の宵空",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6250, 2238, 4884, 2237),
            21,
            new Skill(
                "ウォーターガードフォールC Ⅳ",
                "敵1～3体のDEFと水属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1036,
            "荒ぶる銘花",
            "荒ぶる銘花",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2237, 6255, 2236, 4882),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1035,
            "荒ぶる銘花",
            "荒ぶる銘花",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2237, 6255, 2236, 4882),
            21,
            new Skill(
                "Sp.ウォーターパワーアシストC Ⅳ",
                "味方1～3体のSp.ATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1034,
            "クリエイターズコラボムーンリットワルツ",
            "ムーンリットワルツ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2229, 6243, 2241, 4892),
            21,
            new Skill(
                "Sp.カウンターファイアガードフォールC Ⅳ",
                "敵1～3体のSp.DEFと火属性防御力を大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1033,
            "クリエイターズコラボムーンリットワルツ",
            "ムーンリットワルツ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(2229, 6243, 2241, 4892),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP/副攻:火パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、火属性攻撃力を大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            1032,
            "クリエイターズコラボ月を見上げて",
            "月を見上げて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2229, 6257, 2215, 4893),
            21,
            new Skill(
                "Sp.カウンターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1031,
            "クリエイターズコラボ月を見上げて",
            "月を見上げて",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2229, 6257, 2215, 4893),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1030,
            "クリエイターズコラボお月さまの秘密",
            "お月さまの秘密",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6251, 2216, 4886, 2245),
            21,
            new Skill(
                "カウンターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1029,
            "クリエイターズコラボお月さまの秘密",
            "お月さまの秘密",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6251, 2216, 4886, 2245),
            21,
            new Skill(
                "ファイアパワーアシストB Ⅲ",
                "味方1～2体のATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1028,
            "2人でニューゲーム",
            "2人でニューゲーム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2217, 6257, 2217, 4892),
            21,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1027,
            "2人でニューゲーム",
            "2人でニューゲーム",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2217, 6257, 2217, 4892),
            21,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP/副援:風パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。さらに、風属性攻撃力を大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Wind)],
                Level.Three
            )
        ),
        new Memoria(
            1026,
            "クエレブレの正義",
            "クエレブレの正義",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(6233, 2237, 4874, 2214),
            21,
            new Skill(
                "ウォーターガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            1025,
            "クエレブレの正義",
            "クエレブレの正義",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6233, 2237, 4874, 2214),
            21,
            new Skill(
                "風弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の風属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1024,
            "ワイルドバンチ",
            "ワイルドバンチ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2240, 6249, 2235, 4887),
            21,
            new Skill(
                "風弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の風属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1023,
            "ワイルドバンチ",
            "ワイルドバンチ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2240, 6249, 2235, 4887),
            21,
            new Skill(
                "風強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の風属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Wind)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1022,
            "追憶の星空",
            "追憶の星空",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2235, 6227, 2235, 4905),
            21,
            new Skill(
                "風弱：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに敵の風属性防御力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1021,
            "追憶の星空",
            "追憶の星空",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2235, 6227, 2235, 4905),
            21,
            new Skill(
                "Sp.ウォーターパワーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.ATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1020,
            "シャナの剣",
            "シャナの剣",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6055, 2718, 5249, 2750),
            23,
            new Skill(
                "風拡：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。オーダースキル「風属性効果増加」を発動中は敵2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [new ElementSpread(Element.Wind)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            1019,
            "クリエイターズコラボ黄色い絨毯で",
            "黄色い絨毯で",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6254, 2248, 4877, 2212),
            21,
            new Skill(
                "カウンターファイアパワーフォールC Ⅳ",
                "敵1～3体のATKと火属性攻撃力を大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1018,
            "クリエイターズコラボ黄色い絨毯で",
            "黄色い絨毯で",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6254, 2248, 4877, 2212),
            21,
            new Skill(
                "ファイアパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1017,
            "クリエイターズコラボ朋友信之",
            "朋友信之",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6237, 2220, 4903, 2215),
            21,
            new Skill(
                "カウンターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1016,
            "クリエイターズコラボ朋友信之",
            "朋友信之",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6237, 2220, 4903, 2215),
            21,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            1015,
            "クリエイターズコラボコスモスコスメ",
            "コスモスコスメ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2217, 6230, 2249, 4896),
            21,
            new Skill(
                "Sp.カウンターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1014,
            "クリエイターズコラボコスモスコスメ",
            "コスモスコスメ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2217, 6230, 2249, 4896),
            21,
            new Skill(
                "Sp.ウォーターパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1013,
            "キラキラ★アイドル",
            "キラキラ★アイドル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6254, 2229, 4876, 2221),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1012,
            "キラキラ★アイドル",
            "キラキラ★アイドル",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6254, 2229, 4876, 2221),
            21,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            1011,
            "デュエットですの♥",
            "デュエットですの♥",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6247, 2239, 4890, 2250),
            21,
            new Skill(
                "水強：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方の水属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1010,
            "デュエットですの♥",
            "デュエットですの♥",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6247, 2239, 4890, 2250),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1009,
            "超電磁アイドル",
            "超電磁アイドル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2242, 6244, 2239, 4909),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1008,
            "超電磁アイドル",
            "超電磁アイドル",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2242, 6244, 2239, 4909),
            21,
            new Skill(
                "火弱：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに敵の火属性攻撃力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1007,
            "楯の乙女と超電磁砲",
            "楯の乙女と超電磁砲",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6241, 2225, 4874, 2244),
            21,
            new Skill(
                "火弱：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに敵の火属性攻撃力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            1006,
            "楯の乙女と超電磁砲",
            "楯の乙女と超電磁砲",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6241, 2225, 4874, 2244),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            1005,
            "さすが私ってところかしらぁ",
            "さすが私ってところかしらぁ？",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2240, 2211, 4911, 6258),
            21,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            1004,
            "さすが私ってところかしらぁ",
            "さすが私ってところかしらぁ？",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2240, 2211, 4911, 6258),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1003,
            "お姉様の露払い",
            "お姉様の露払い",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2238, 6249, 2233, 4878),
            21,
            new Skill(
                "水弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1002,
            "お姉様の露払い",
            "お姉様の露払い",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2238, 6249, 2233, 4878),
            21,
            new Skill(
                "水強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の水属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            1001,
            "これが私の、全力だぁ――",
            "これが私の、全力だぁ――！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6235, 2220, 4897, 2237),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            1000,
            "これが私の、全力だぁ――",
            "これが私の、全力だぁ――！！",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6235, 2220, 4897, 2237),
            21,
            new Skill(
                "水弱：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに敵の水属性防御力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            999,
            "プランセス",
            "プランセス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4152, 2719, 3649, 2734),
            18,
            new Skill(
                "ストライクD LG",
                "敵2体に通常大ダメージを与える。",
                [],
                [],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            998,
            "クリエイターズコラボお祭りを味わって",
            "お祭りを味わって",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2246, 2215, 4876, 6250),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅳ+",
                "味方1～3体のHPを大回復する。さらに味方のSp.DEFと水属性防御力をアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            997,
            "クリエイターズコラボお祭りを味わって",
            "お祭りを味わって",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2246, 2215, 4876, 6250),
            21,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            996,
            "クリエイターズコラボ瞳の中に映る花火",
            "瞳の中に映る花火",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(6248, 2216, 4901, 2228),
            21,
            new Skill(
                "カウンターウィンドパワーアシストC Ⅳ",
                "味方1～3体のATKと風属性攻撃力を大アップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            995,
            "クリエイターズコラボ瞳の中に映る花火",
            "瞳の中に映る花火",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6248, 2216, 4901, 2228),
            21,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            994,
            "クリエイターズコラボ射的の景品です",
            "射的の景品です！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2216, 6250, 2231, 4896),
            21,
            new Skill(
                "Sp.カウンターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            993,
            "クリエイターズコラボ射的の景品です",
            "射的の景品です！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2216, 6250, 2231, 4896),
            21,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅳ",
                "味方1～3体のSp.ATKと風属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            992,
            "クリエイターズコラボ切れた鼻緒と繋ぐ心",
            "切れた鼻緒と繋ぐ心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6251, 2226, 4885, 2236),
            21,
            new Skill(
                "カウンターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            991,
            "クリエイターズコラボ切れた鼻緒と繋ぐ心",
            "切れた鼻緒と繋ぐ心",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(6251, 2226, 4885, 2236),
            21,
            new Skill(
                "水強：ヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方の水属性防御力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Water)],
                [],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            990,
            "明朗たる戦乙女",
            "明朗たる戦乙女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6261, 2246, 4911, 2217),
            21,
            new Skill(
                "火弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            989,
            "明朗たる戦乙女",
            "明朗たる戦乙女",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6261, 2246, 4911, 2217),
            21,
            new Skill(
                "[火攻風防]マイトアシストC Ⅳ",
                "味方1～3体の火属性攻撃力と風属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium), new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:火パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体の火属性攻撃力を特大アップさせる。",
                Trigger.Support,
                [new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            988,
            "陽だまりの女神",
            "陽だまりの女神",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2244, 2213, 4889, 6246),
            21,
            new Skill(
                "風強：ヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方の風属性防御力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Wind)],
                [],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:風ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の風属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Wind)],
                Level.Three
            )
        ),
        new Memoria(
            987,
            "陽だまりの女神",
            "陽だまりの女神",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2244, 2213, 4889, 6246),
            21,
            new Skill(
                "火弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の火属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            986,
            "燦然たる騎士",
            "燦然たる騎士",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2729, 6081, 2735, 5272),
            23,
            new Skill(
                "火：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            985,
            "真実の刃＆リリコレ2023開演",
            "真実の刃＆リリコレ2023 開演！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1676, 4481, 1658, 3503),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            984,
            "真実の刃＆リリコレ2023開演",
            "真実の刃＆リリコレ2023 開演！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1676, 4481, 1658, 3503),
            19,
            new Skill(
                "WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            983,
            "神の子は、水面に踊る",
            "神の子は、水面に踊る",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2731, 4123, 2714, 3644),
            21,
            new Skill(
                "Sp.ガードバーストD LG",
                "敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            982,
            "夏尽くしの海",
            "夏尽くしの海",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2233, 6254, 2221, 4889),
            21,
            new Skill(
                "[風攻水防]マイトアシストC Ⅳ",
                "味方1～3体の風属性攻撃力と水属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium), new StatusUp(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:風パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体の風属性攻撃力を特大アップさせる。",
                Trigger.Support,
                [new ElementPowerUp(Element.Wind)],
                Level.Three
            )
        ),
        new Memoria(
            981,
            "夏尽くしの海",
            "夏尽くしの海",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2233, 6254, 2221, 4889),
            21,
            new Skill(
                "風弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の風属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            980,
            "壱撃必殺",
            "壱撃必殺！！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(2221, 6251, 2211, 4906),
            21,
            new Skill(
                "風弱：Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の風属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP/副攻:風パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、風属性攻撃力を大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new ElementPowerUp(Element.Wind)],
                Level.Three
            )
        ),
        new Memoria(
            979,
            "壱撃必殺",
            "壱撃必殺！！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2221, 6251, 2211, 4906),
            21,
            new Skill(
                "風弱：Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。さらに敵の風属性防御力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            978,
            "くすみんサマー",
            "くすみんサマー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6231, 2225, 4908, 2232),
            21,
            new Skill(
                "風弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の風属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Wind)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            977,
            "くすみんサマー",
            "くすみんサマー",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(6231, 2225, 4908, 2232),
            21,
            new Skill(
                "水強：ヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方の水属性防御力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Water)],
                [],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            976,
            "ultimatememoria双刃無双",
            "双刃無双",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4071, 4094, 7066, 7069),
            26,
            new Skill(
                "リカバーヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに自身のMPを60回復する。",
                [new Recover()],
                [],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ+",
                "HP回復時、中確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            975,
            "ultimatememoria双刃無双",
            "双刃無双",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(7405, 4094, 6722, 4079),
            26,
            new Skill(
                "ウォーターパワーストライクA Ⅴ+",
                "敵1体に通常超特大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Five,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            974,
            "ultimatememoria無二なる二刀",
            "無二なる二刀",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(7062, 7065, 4065, 4066),
            26,
            new Skill(
                "WパワーフォールD Ⅲ",
                "敵2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            973,
            "ultimatememoria無二なる二刀",
            "無二なる二刀",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7406, 4075, 6711, 4066),
            26,
            new Skill(
                "火：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            972,
            "ultimatememoria幻奏乙女",
            "幻奏乙女",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(7063, 7078, 4088, 4097),
            26,
            new Skill(
                "WパワーアシストD Ⅲ",
                "味方2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            971,
            "ultimatememoria幻奏乙女",
            "幻奏乙女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(4073, 7422, 4088, 6743),
            26,
            new Skill(
                "火：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            970,
            "ultimatememoria終曲のタクト",
            "終曲のタクト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(4085, 7411, 4065, 6734),
            26,
            new Skill(
                "水：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            969,
            "ultimatememoria終曲のタクト",
            "終曲のタクト",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4085, 4077, 7055, 7078),
            26,
            new Skill(
                "WガードアシストD Ⅲ",
                "味方2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            968,
            "ultimatememoriaヴィルトシュバイン",
            "ヴィルトシュバイン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(7428, 4098, 6721, 4097),
            26,
            new Skill(
                "水：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            967,
            "ultimatememoriaヴィルトシュバイン",
            "ヴィルトシュバイン",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4094, 4098, 7065, 7087),
            26,
            new Skill(
                "WガードフォールD Ⅲ",
                "敵2体のDEFとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            966,
            "待ち望んだパーティナイト",
            "待ち望んだパーティナイト",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6232, 2214, 4874, 2220),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            965,
            "待ち望んだパーティナイト",
            "待ち望んだパーティナイト",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6232, 2214, 4874, 2220),
            21,
            new Skill(
                "火強：ヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方の火属性防御力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Fire)],
                [],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            964,
            "焦がれる夜",
            "焦がれる夜",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6229, 2231, 4896, 2245),
            21,
            new Skill(
                "火弱：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに敵の火属性攻撃力が低いほどスキル効果が上昇する。",
                [new ElementWeaken(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            963,
            "焦がれる夜",
            "焦がれる夜",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6229, 2231, 4896, 2245),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            962,
            "いたずらトゥインクル",
            "いたずらトゥインクル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2226, 6235, 2211, 4909),
            21,
            new Skill(
                "水弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                [new ElementWeaken(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            961,
            "いたずらトゥインクル",
            "いたずらトゥインクル",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2226, 6235, 2211, 4909),
            21,
            new Skill(
                "水強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の水属性攻撃力が高いほどスキル効果が上昇する。",
                [new ElementStrengthen(Element.Water)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            960,
            "はにかみプールサイド",
            "はにかみプールサイド",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(5655, 5678, 2746, 2730),
            23,
            new Skill(
                "水：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            959,
            "クリエイターズコラボ真夏のステージ",
            "真夏のステージ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2240, 2232, 6225, 4901),
            21,
            new Skill(
                "WカウンターガードヒールC Ⅳ",
                "味方1～3体のHPを大回復し、DEFとSp.DEFを小アップする。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            958,
            "クリエイターズコラボ真夏のステージ",
            "真夏のステージ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2240, 2232, 6225, 4901),
            21,
            new Skill(
                "[風攻火防]マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵の風属性攻撃力と火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            957,
            "クリエイターズコラボ浮き輪でぷかぷか",
            "浮き輪でぷかぷか",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2216, 6224, 2234, 4913),
            21,
            new Skill(
                "WカウンターガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            956,
            "クリエイターズコラボ浮き輪でぷかぷか",
            "浮き輪でぷかぷか",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2216, 6224, 2234, 4913),
            21,
            new Skill(
                "[風攻火防]マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵の風属性攻撃力と火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            955,
            "クリエイターズコラボ向日葵の咲く園",
            "向日葵の咲く園",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2235, 6226, 2225, 4911),
            21,
            new Skill(
                "Sp.カウンターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            954,
            "クリエイターズコラボ向日葵の咲く園",
            "向日葵の咲く園",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2235, 6226, 2225, 4911),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            953,
            "クリエイターズコラボ水着をお披露目",
            "水着をお披露目",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6243, 2249, 4913, 2237),
            21,
            new Skill(
                "カウンターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            952,
            "クリエイターズコラボ水着をお披露目",
            "水着をお披露目",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6243, 2249, 4913, 2237),
            21,
            new Skill(
                "ファイアパワーアシストB Ⅲ",
                "味方1～2体のATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            951,
            "emotionalmemoria楽しいを探して",
            "楽しいを探して",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2218, 2247, 6228, 4876),
            21,
            new Skill(
                "WガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            950,
            "emotionalmemoria楽しいを探して",
            "楽しいを探して",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2218, 2247, 6228, 4876),
            21,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            949,
            "祝25周年リリサマ",
            "祝2.5周年 リリサマ!!",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(4500, 1656, 3469, 1666),
            19,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            948,
            "祝25周年リリサマ",
            "祝2.5周年 リリサマ!!",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4500, 1656, 3469, 1666),
            19,
            new Skill(
                "WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            947,
            "惹かれる手のひら",
            "惹かれる手のひら",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2249, 2243, 4894, 6226),
            21,
            new Skill(
                "WカウンターガードヒールC Ⅳ",
                "味方1～3体のHPを大回復し、DEFとSp.DEFを小アップする。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            946,
            "惹かれる手のひら",
            "惹かれる手のひら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2249, 2243, 4894, 6226),
            21,
            new Skill(
                "Sp.ウォーターパワーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            945,
            "王家の夏休み",
            "王家の夏休み",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6238, 2227, 4892, 2216),
            21,
            new Skill(
                "WカウンターパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            944,
            "王家の夏休み",
            "王家の夏休み",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6238, 2227, 4892, 2216),
            21,
            new Skill(
                "ウォーターパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            943,
            "アグレッシヴ・ヒロイン",
            "アグレッシヴ・ヒロイン",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2722, 6061, 2722, 5279),
            23,
            new Skill(
                "Sp.カウンターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            942,
            "夏色スライダー",
            "夏色スライダー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6069, 2738, 5270, 2744),
            23,
            new Skill(
                "カウンターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                [new Counter()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            941,
            "星月夜の指揮者",
            "星月夜の指揮者",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2245, 2219, 6227, 4885),
            21,
            new Skill(
                "ウィンドガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            940,
            "星月夜の指揮者",
            "星月夜の指揮者",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2245, 2219, 6227, 4885),
            21,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            939,
            "月下に舞うプランセス",
            "月下に舞うプランセス",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2228, 6261, 2224, 4888),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            938,
            "月下に舞うプランセス",
            "月下に舞うプランセス",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2228, 6261, 2224, 4888),
            21,
            new Skill(
                "[火攻風防]マイトアシストB Ⅲ",
                "味方1～2体の火属性攻撃力と風属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium), new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:火パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体の火属性攻撃力を特大アップさせる。",
                Trigger.Support,
                [new ElementPowerUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            937,
            "蒼き月の夜",
            "蒼き月の夜",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6241, 2241, 4876, 2236),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            936,
            "蒼き月の夜",
            "蒼き月の夜",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6241, 2241, 4876, 2236),
            21,
            new Skill(
                "ウィンドパワーフォールC Ⅳ",
                "敵1～3体のATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN/副援:風パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。さらに、風属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new ElementPowerDown(Element.Wind)],
                Level.Three
            )
        ),
        new Memoria(
            935,
            "クリエイターズコラボ非常事態のその後",
            "非常事態のその後",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2243, 2223, 4905, 6230),
            21,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            934,
            "クリエイターズコラボ非常事態のその後",
            "非常事態のその後",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2243, 2223, 4905, 6230),
            21,
            new Skill(
                "Sp.ファイアパワーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.ATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            933,
            "クリエイターズコラボ冷たい舌触り",
            "冷たい舌触り",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6228, 2212, 4910, 2236),
            21,
            new Skill(
                "ファイアパワーフォールC Ⅳ",
                "敵1～3体のATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            932,
            "クリエイターズコラボ冷たい舌触り",
            "冷たい舌触り",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6228, 2212, 4910, 2236),
            21,
            new Skill(
                "ファイアパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            931,
            "クリエイターズコラボ灯莉の貝殻アート☆",
            "灯莉の貝殻アート☆",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2235, 6223, 2229, 4884),
            21,
            new Skill(
                "水：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            930,
            "クリエイターズコラボ灯莉の貝殻アート☆",
            "灯莉の貝殻アート☆",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2235, 6223, 2229, 4884),
            21,
            new Skill(
                "[水攻火防]マイトアシストB Ⅲ",
                "味方1～2体の水属性攻撃力と火属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium), new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            929,
            "クリエイターズコラボ碧い海のふたり",
            "碧い海のふたり",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6253, 2235, 4880, 2250),
            21,
            new Skill(
                "水：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            928,
            "クリエイターズコラボ碧い海のふたり",
            "碧い海のふたり",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6253, 2235, 4880, 2250),
            21,
            new Skill(
                "ウォーターガードフォールB Ⅲ",
                "敵1～2体のDEFと水属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            927,
            "夢の果て、その先へ",
            "夢の果て、その先へ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2217, 6236, 2247, 4896),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            926,
            "夢の果て、その先へ",
            "夢の果て、その先へ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2217, 6236, 2247, 4896),
            21,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅳ",
                "敵1～2体のSp.ATKと火属性攻撃力を特大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN/副援:火パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。さらに、火属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special), new ElementPowerDown(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            925,
            "正義の咆哮",
            "正義の咆哮",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6244, 2250, 4897, 2238),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            924,
            "正義の咆哮",
            "正義の咆哮",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6244, 2250, 4897, 2238),
            21,
            new Skill(
                "ファイアガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:火ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の火属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Fire)],
                Level.Three
            )
        ),
        new Memoria(
            923,
            "深炎のスキャルドメール",
            "深炎のスキャルドメール",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5650, 5662, 2753, 2723),
            23,
            new Skill(
                "水：WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            922,
            "七頭龍幻想の担い手",
            "七頭龍幻想の担い手",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2242, 6250, 2226, 4890),
            21,
            new Skill(
                "Sp.ファイアガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            921,
            "七頭龍幻想の担い手",
            "七頭龍幻想の担い手",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2242, 6250, 2226, 4890),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            920,
            "竜のシャナ",
            "竜のシャナ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6260, 2221, 4890, 2220),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            919,
            "竜のシャナ",
            "竜のシャナ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6260, 2221, 4890, 2220),
            21,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            918,
            "ゴージャス☆おしゃ恋花",
            "ゴージャス☆おしゃ恋花",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2217, 6251, 2240, 4880),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            917,
            "ゴージャス☆おしゃ恋花",
            "ゴージャス☆おしゃ恋花",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2217, 6251, 2240, 4880),
            21,
            new Skill(
                "[火攻風防]マイトアシストB Ⅲ",
                "味方1～2体の火属性攻撃力と風属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium), new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            916,
            "枕投げチャンピオン",
            "枕投げチャンピオン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6231, 2230, 4906, 2224),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            915,
            "枕投げチャンピオン",
            "枕投げチャンピオン",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6231, 2230, 4906, 2224),
            21,
            new Skill(
                "ウィンドパワーフォールB Ⅲ",
                "敵1～2体のATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            914,
            "幸せな夢を見る前に",
            "幸せな夢を見る前に",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2251, 2246, 6249, 4898),
            21,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            913,
            "幸せな夢を見る前に",
            "幸せな夢を見る前に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2251, 2246, 6249, 4898),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            912,
            "心を鋼鉄に変えて",
            "心を鋼鉄に変えて",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2248, 6260, 2233, 4880),
            21,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            911,
            "心を鋼鉄に変えて",
            "心を鋼鉄に変えて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2248, 6260, 2233, 4880),
            21,
            new Skill(
                "[火防]Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKと火属性防御力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            910,
            "受け継がれし攻守の型",
            "受け継がれし攻守の型",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2250, 6246, 2249, 4876),
            21,
            new Skill(
                "水：Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            909,
            "受け継がれし攻守の型",
            "受け継がれし攻守の型",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2250, 6246, 2249, 4876),
            21,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            908,
            "煉獄の守護天使",
            "煉獄の守護天使",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2236, 6257, 2232, 4873),
            21,
            new Skill(
                "[水攻火防]マイトアシストB Ⅲ",
                "味方1～2体の水属性攻撃力と火属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium), new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            907,
            "煉獄の守護天使",
            "煉獄の守護天使",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2236, 6257, 2232, 4873),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            906,
            "祈りの声が届く時",
            "祈りの声が届く時",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6247, 2226, 4907, 2242),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            905,
            "祈りの声が届く時",
            "祈りの声が届く時",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6247, 2226, 4907, 2242),
            21,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            904,
            "クリエイターズコラボこれなんかどう",
            "これなんかどう？",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2217, 2246, 4905, 6239),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new ElementGuardUp(Element.Water)],
                Level.Three
            )
        ),
        new Memoria(
            903,
            "クリエイターズコラボこれなんかどう",
            "これなんかどう？",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2217, 2246, 4905, 6239),
            21,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            902,
            "クリエイターズコラボ日差しを見上げて",
            "日差しを見上げて",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6237, 2212, 4899, 2237),
            21,
            new Skill(
                "ウィンドガードフォールB Ⅲ",
                "敵1～2体のDEFと風属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            901,
            "クリエイターズコラボ日差しを見上げて",
            "日差しを見上げて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6237, 2212, 4899, 2237),
            21,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            900,
            "クリエイターズコラボ初夏の装い",
            "初夏の装い",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6258, 2248, 4882, 2234),
            21,
            new Skill(
                "風：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            899,
            "クリエイターズコラボ初夏の装い",
            "初夏の装い",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(6258, 2248, 4882, 2234),
            21,
            new Skill(
                "ウィンドパワーアシストB Ⅲ",
                "味方1～2体のATKと風属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            898,
            "クリエイターズコラボ水族館を探検",
            "水族館を探検",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2239, 6257, 2222, 4886),
            21,
            new Skill(
                "風：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            897,
            "クリエイターズコラボ水族館を探検",
            "水族館を探検",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2239, 6257, 2222, 4886),
            21,
            new Skill(
                "Sp.ウィンドガードフォールB Ⅲ",
                "敵1～2体のSp.DEFと風属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            896,
            "桜花爛漫",
            "桜花爛漫",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2219, 6251, 2237, 4873),
            21,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            895,
            "桜花爛漫",
            "桜花爛漫",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2219, 6251, 2237, 4873),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            894,
            "心の痛みを判る人",
            "心の痛みを判る人",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6247, 2225, 4883, 2222),
            21,
            new Skill(
                "[風防]マイトアシストB Ⅲ",
                "味方1～2体のATKと風属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            893,
            "心の痛みを判る人",
            "心の痛みを判る人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6247, 2225, 4883, 2222),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            892,
            "輝く心",
            "輝く心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6257, 2243, 4886, 2229),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            891,
            "輝く心",
            "輝く心",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6257, 2243, 4886, 2229),
            21,
            new Skill(
                "ファイアガードフォールB Ⅲ",
                "敵1～2体のDEFと火属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            890,
            "勇気の拳",
            "勇気の拳",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2216, 6258, 2225, 4906),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            889,
            "勇気の拳",
            "勇気の拳",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2216, 6258, 2225, 4906),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            888,
            "emotionalmemoria伝わる鼓動",
            "伝わる鼓動",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2237, 6250, 2217, 4876),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            887,
            "emotionalmemoria伝わる鼓動",
            "伝わる鼓動",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2237, 6250, 2217, 4876),
            21,
            new Skill(
                "水：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            886,
            "クリエイターズコラボ二度寝のいいわけ",
            "二度寝のいいわけ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6235, 2222, 4889, 2238),
            21,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            885,
            "クリエイターズコラボ二度寝のいいわけ",
            "二度寝のいいわけ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6235, 2222, 4889, 2238),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            884,
            "クリエイターズコラボ雨、舌戦のあと",
            "雨、舌戦のあと",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2239, 2211, 4886, 6226),
            21,
            new Skill(
                "水：Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFと火属性防御力を小アップする。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            883,
            "クリエイターズコラボ雨、舌戦のあと",
            "雨、舌戦のあと",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2239, 2211, 4886, 6226),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            882,
            "クリエイターズコラボ雨の日は紅茶を",
            "雨の日は紅茶を",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6258, 2234, 4901, 2216),
            21,
            new Skill(
                "水：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            881,
            "クリエイターズコラボ雨の日は紅茶を",
            "雨の日は紅茶を",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6258, 2234, 4901, 2216),
            21,
            new Skill(
                "ファイアガードアシストB Ⅲ",
                "味方1～2体のDEFと火属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            880,
            "クリエイターズコラボ紫陽花の咲く頃",
            "紫陽花の咲く頃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2218, 6240, 2224, 4879),
            21,
            new Skill(
                "水：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            879,
            "クリエイターズコラボ紫陽花の咲く頃",
            "紫陽花の咲く頃",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2218, 6240, 2224, 4879),
            21,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅲ",
                "敵1～2体のSp.DEFと水属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            878,
            "清純な心",
            "清純な心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6224, 2240, 4909, 2217),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            877,
            "清純な心",
            "清純な心",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(6224, 2240, 4909, 2217),
            21,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            876,
            "そよ風のシュッツエンゲル",
            "そよ風のシュッツエンゲル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2227, 6259, 2225, 4898),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            875,
            "そよ風のシュッツエンゲル",
            "そよ風のシュッツエンゲル",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2227, 6259, 2225, 4898),
            21,
            new Skill(
                "Sp.ファイアパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            874,
            "神の子",
            "神の子",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2722, 2698, 4139, 3632),
            18,
            new Skill(
                "ヒールE LG",
                "味方2～3体のHPを回復する。",
                [],
                [],
                Level.Lg,
                Range.E
            ),
            new SupportSkill(
                "回:回復UP/ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。さらに、HPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp(), new GuardUp(Type.Normal)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            873,
            "交差する勇み花",
            "交差する勇み花",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6255, 2248, 4901, 2222),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            872,
            "交差する勇み花",
            "交差する勇み花",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6255, 2248, 4901, 2222),
            21,
            new Skill(
                "ファイアパワーアシストB Ⅲ",
                "味方1～2体のATKと火属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            871,
            "情熱",
            "情熱",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2245, 6238, 2225, 4910),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            870,
            "情熱",
            "情熱",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2245, 6238, 2225, 4910),
            21,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            869,
            "愛情の絆",
            "愛情の絆",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2227, 2232, 4909, 6256),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            868,
            "愛情の絆",
            "愛情の絆",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2227, 2232, 4909, 6256),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            867,
            "貴方に微笑む",
            "貴方に微笑む",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6224, 2243, 4905, 2213),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            866,
            "貴方に微笑む",
            "貴方に微笑む",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6224, 2243, 4905, 2213),
            21,
            new Skill(
                "ファイアガードフォールB Ⅲ",
                "敵1～2体のDEFと火属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            865,
            "つきしーmax",
            "つきしーMAX!!",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2727, 2719, 3873, 3899),
            21,
            new Skill(
                "WガードヒールE LG",
                "味方2～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Lg,
                Range.E
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            ),
            true
        ),
        new Memoria(
            864,
            "g戦場の百合亜",
            "G戦場の百合亜",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(6248, 2218, 4905, 2236),
            21,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            863,
            "g戦場の百合亜",
            "G戦場の百合亜",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6248, 2218, 4905, 2236),
            21,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            862,
            "焼け焦げた土を踏んで",
            "焼け焦げた土を踏んで",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2220, 6233, 2222, 4913),
            21,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            861,
            "焼け焦げた土を踏んで",
            "焼け焦げた土を踏んで",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2220, 6233, 2222, 4913),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            860,
            "黒蝕の夢",
            "黒蝕の夢",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2249, 6226, 2250, 4886),
            21,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            859,
            "黒蝕の夢",
            "黒蝕の夢",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2249, 6226, 2250, 4886),
            21,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            858,
            "月光奏鳴",
            "月光奏鳴",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6225, 2244, 4904, 2231),
            21,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            857,
            "月光奏鳴",
            "月光奏鳴",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6225, 2244, 4904, 2231),
            21,
            new Skill(
                "ウィンドガードフォールB Ⅲ",
                "敵1～2体のDEFと風属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            856,
            "クリエイターズコラボ式場を決めましたわ",
            "式場を決めましたわ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2212, 2237, 6241, 4910),
            21,
            new Skill(
                "火：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            855,
            "クリエイターズコラボ式場を決めましたわ",
            "式場を決めましたわ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2212, 2237, 6241, 4910),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            854,
            "クリエイターズコラボ想像ウェディング",
            "想像ウェディング",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2227, 2251, 4878, 6253),
            21,
            new Skill(
                "火：Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFと風属性防御力を小アップする。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            853,
            "クリエイターズコラボ想像ウェディング",
            "想像ウェディング",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2227, 2251, 4878, 6253),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            852,
            "クリエイターズコラボウェディングベア",
            "ウェディングベア",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2246, 6244, 2224, 4899),
            21,
            new Skill(
                "火：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            851,
            "クリエイターズコラボウェディングベア",
            "ウェディングベア",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2246, 6244, 2224, 4899),
            21,
            new Skill(
                "Sp.ファイアガードフォールB Ⅲ",
                "敵1～2体のSp.DEFと火属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            850,
            "クリエイターズコラボ門出のブーケ・トス",
            "門出のブーケ・トス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6248, 2211, 4884, 2251),
            21,
            new Skill(
                "火：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            849,
            "クリエイターズコラボ門出のブーケ・トス",
            "門出のブーケ・トス",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(6248, 2211, 4884, 2251),
            21,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            848,
            "夢見る自分を、怖れずに",
            "夢見る自分を、怖れずに",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2224, 6243, 2228, 4894),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            847,
            "夢見る自分を、怖れずに",
            "夢見る自分を、怖れずに",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2224, 6243, 2228, 4894),
            21,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            846,
            "ウエディング・マーチ",
            "ウエディング・マーチ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6262, 2233, 4903, 2231),
            21,
            new Skill(
                "ファイアパワーフォールB Ⅲ",
                "敵1～2体のATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            845,
            "ウエディング・マーチ",
            "ウエディング・マーチ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6262, 2233, 4903, 2231),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            844,
            "エターナル・プロミス",
            "エターナル・プロミス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6062, 2748, 5280, 2752),
            23,
            new Skill(
                "水拡：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [new ElementSpread(Element.Water)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            843,
            "クリエイターズコラボピクニック日和",
            "ピクニック日和",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2049, 5285, 2012, 4178),
            20,
            new Skill(
                "Sp.ウォーターパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと水属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            842,
            "クリエイターズコラボピクニック日和",
            "ピクニック日和",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2049, 5285, 2012, 4178),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            841,
            "クリエイターズコラボなでなで連鎖",
            "なでなで連鎖",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2039, 2039, 5259, 4174),
            20,
            new Skill(
                "風：ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、DEFと水属性防御力を小アップする。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            840,
            "クリエイターズコラボなでなで連鎖",
            "なでなで連鎖",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2039, 2039, 5259, 4174),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            839,
            "クリエイターズコラボ尊さの不意打ち",
            "尊さの不意打ち",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2038, 5280, 2037, 4157),
            20,
            new Skill(
                "風：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            838,
            "クリエイターズコラボ尊さの不意打ち",
            "尊さの不意打ち",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2038, 5280, 2037, 4157),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            837,
            "クリエイターズコラボ皐月の頃に",
            "皐月の頃に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5266, 2032, 4178, 2031),
            20,
            new Skill(
                "風：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            836,
            "クリエイターズコラボ皐月の頃に",
            "皐月の頃に",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5266, 2032, 4178, 2031),
            20,
            new Skill(
                "ウィンドガードフォールB Ⅲ",
                "敵1～2体のDEFと風属性防御力を大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            835,
            "言葉無く吠える",
            "言葉無く吠える",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5266, 2046, 4152, 2032),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            834,
            "言葉無く吠える",
            "言葉無く吠える",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5266, 2046, 4152, 2032),
            20,
            new Skill(
                "ファイアパワーフォールB Ⅲ",
                "敵1～2体のATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            833,
            "戦乙女の誇り",
            "戦乙女の誇り",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2040, 5253, 2025, 4182),
            20,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            832,
            "戦乙女の誇り",
            "戦乙女の誇り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2040, 5253, 2025, 4182),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            831,
            "竜のシャナと楯の乙女",
            "竜のシャナと楯の乙女",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2018, 2046, 5254, 4167),
            20,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            830,
            "竜のシャナと楯の乙女",
            "竜のシャナと楯の乙女",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2018, 2046, 5254, 4167),
            20,
            new Skill(
                "ウォーターパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            829,
            "パーフェクトエイム",
            "パーフェクトエイム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5258, 2038, 4149, 2015),
            20,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            828,
            "パーフェクトエイム",
            "パーフェクトエイム",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(5258, 2038, 4149, 2015),
            20,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            827,
            "征くと決めたこの道を",
            "征くと決めたこの道を",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2041, 5274, 2036, 4162),
            20,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            826,
            "征くと決めたこの道を",
            "征くと決めたこの道を",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2041, 5274, 2036, 4162),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            825,
            "藍だけが使える魔法",
            "藍だけが使える魔法",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2404, 5451, 2404, 4679),
            22,
            new Skill(
                "水拡：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [new ElementSpread(Element.Water)],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            824,
            "エクセレントアイドル☆紗癒",
            "エクセレントアイドル☆紗癒",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(5259, 2014, 4153, 2046),
            20,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            823,
            "エクセレントアイドル☆紗癒",
            "エクセレントアイドル☆紗癒",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5259, 2014, 4153, 2046),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            822,
            "ゴージャスアイドル☆楓",
            "ゴージャスアイドル☆楓",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2043, 5285, 2016, 4149),
            20,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            821,
            "ゴージャスアイドル☆楓",
            "ゴージャスアイドル☆楓",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2043, 5285, 2016, 4149),
            20,
            new Skill(
                "水拡：Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は敵2体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                [new ElementSpread(Element.Water)],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            820,
            "荒ぶる魂",
            "荒ぶる魂",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1934, 5149, 1950, 4044),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            819,
            "荒ぶる魂",
            "荒ぶる魂",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1934, 5149, 1950, 4044),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            818,
            "閑かなること、幻想の如く",
            "閑かなること、幻想の如く",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5254, 2019, 4186, 2035),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            817,
            "閑かなること、幻想の如く",
            "閑かなること、幻想の如く",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5254, 2019, 4186, 2035),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅲ",
                "敵1～2体のATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            816,
            "猛禽の視点",
            "猛禽の視点",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2043, 5266, 2015, 4158),
            20,
            new Skill(
                "[風防]Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性防御力を大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            815,
            "猛禽の視点",
            "猛禽の視点",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2043, 5266, 2015, 4158),
            20,
            new Skill(
                "Sp.ディファーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            814,
            "戦場に差しこむ光",
            "戦場に差しこむ光",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2036, 2040, 5265, 4187),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            813,
            "戦場に差しこむ光",
            "戦場に差しこむ光",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2036, 2040, 5265, 4187),
            20,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            812,
            "クリエイターズコラボウィステリアの誘い",
            "ウィステリアの誘い",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2043, 5269, 2040, 4184),
            20,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            811,
            "クリエイターズコラボウィステリアの誘い",
            "ウィステリアの誘い",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2043, 5269, 2040, 4184),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            810,
            "クリエイターズコラボ花言葉のように",
            "花言葉のように",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5266, 2026, 4149, 2045),
            20,
            new Skill(
                "[風防]マイトアシストB Ⅲ",
                "味方1～2体のATKと風属性防御力を大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            809,
            "クリエイターズコラボ花言葉のように",
            "花言葉のように",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5266, 2026, 4149, 2045),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            808,
            "クリエイターズコラボ藤棚の下で",
            "藤棚の下で",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2041, 5248, 2050, 4183),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            807,
            "クリエイターズコラボ藤棚の下で",
            "藤棚の下で",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2041, 5248, 2050, 4183),
            20,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            806,
            "クリエイターズコラボ紫に酔い、白に想う",
            "紫に酔い、白に想う",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5275, 2038, 4170, 2017),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            805,
            "クリエイターズコラボ紫に酔い、白に想う",
            "紫に酔い、白に想う",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5275, 2038, 4170, 2017),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅲ",
                "敵1～2体のATKと風属性攻撃力を大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            804,
            "舞台「thegleamofdawn」開演",
            "舞台「The Gleam of Dawn」開演！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1583, 3441, 1599, 2936),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            803,
            "舞台「thegleamofdawn」開演",
            "舞台「The Gleam of Dawn」開演！",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1583, 3441, 1599, 2936),
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            802,
            "猪突猛進",
            "猪突猛進！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(4152, 2719, 3649, 2734),
            18,
            new Skill(
                "ストライクD LG",
                "敵2体に通常大ダメージを与える。",
                [],
                [],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            801,
            "さみしがりうさぎ",
            "さみしがりうさぎ",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5260, 2021, 4172, 2038),
            20,
            new Skill(
                "ウィンドパワーフォールC Ⅲ",
                "敵1～3体のATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            800,
            "さみしがりうさぎ",
            "さみしがりうさぎ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5260, 2021, 4172, 2038),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            799,
            "バニートラップ",
            "バニートラップ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2044, 5263, 2026, 4185),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            798,
            "バニートラップ",
            "バニートラップ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2044, 5263, 2026, 4185),
            20,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.マイトUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKとSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special), new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            797,
            "魅惑のセレクション",
            "魅惑のセレクション",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2415, 2384, 5050, 5074),
            22,
            new Skill(
                "WガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            796,
            "クリエイターズコラボイースターハント",
            "イースターハント",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2037, 5253, 2040, 4165),
            20,
            new Skill(
                "火：Sp.ファイアパワーフォールC Ⅲ",
                "敵1～3体のSp.ATKと火属性攻撃力をダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            795,
            "クリエイターズコラボイースターハント",
            "イースターハント",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2037, 5253, 2040, 4165),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            794,
            "クリエイターズコラボ花咲くイースター",
            "花咲くイースター",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2017, 2039, 5268, 4171),
            20,
            new Skill(
                "火拡：WガードアシストB Ⅳ",
                "味方1～2体のDEFとSp.DEFを特大アップさせる。オーダースキル「火属性効果増加」を発動中は味方2体のDEFとSp.DEFを特大アップさせる。",
                [new ElementSpread(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            793,
            "クリエイターズコラボ花咲くイースター",
            "花咲くイースター",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2017, 2039, 5268, 4171),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            792,
            "クリエイターズコラボイースターエッグ",
            "イースターエッグ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2040, 5263, 2024, 4173),
            20,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            791,
            "クリエイターズコラボイースターエッグ",
            "イースターエッグ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2040, 5263, 2024, 4173),
            20,
            new Skill(
                "Sp.ファイアガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            790,
            "クリエイターズコラボエッグロール開始",
            "エッグロール開始！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5286, 2047, 4186, 2018),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            789,
            "クリエイターズコラボエッグロール開始",
            "エッグロール開始！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5286, 2047, 4186, 2018),
            20,
            new Skill(
                "ファイアパワーアシストC Ⅲ",
                "味方1～3体のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            788,
            "天のアカリ目",
            "天のアカリ目！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2023, 5282, 2028, 4167),
            20,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            787,
            "天のアカリ目",
            "天のアカリ目！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2023, 5282, 2028, 4167),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            786,
            "これが、あたしの理",
            "これが、あたしの理！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5260, 2027, 4153, 2018),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            785,
            "これが、あたしの理",
            "これが、あたしの理！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5260, 2027, 4153, 2018),
            20,
            new Skill(
                "ファイアパワーフォールC Ⅲ",
                "敵1～3体のATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            784,
            "マルチカラード・ティアーズ",
            "マルチカラード・ティアーズ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2721, 2736, 5111, 4320),
            22,
            new Skill(
                "ファイアガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            783,
            "尊き花を守るために",
            "尊き花を守るために",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5273, 2029, 4153, 2040),
            20,
            new Skill(
                "ファイアパワーアシストC Ⅲ",
                "味方1～3体のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            782,
            "尊き花を守るために",
            "尊き花を守るために",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5273, 2029, 4153, 2040),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            781,
            "笑顔の夜明け",
            "笑顔の夜明け",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2046, 5286, 2027, 4185),
            20,
            new Skill(
                "Sp.ファイアパワーフォールC Ⅲ",
                "敵1～3体のSp.ATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            780,
            "笑顔の夜明け",
            "笑顔の夜明け",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(2046, 5286, 2027, 4185),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            779,
            "天使の左手、堕天使の右手",
            "天使の左手、堕天使の右手",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2041, 5269, 2014, 4176),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            778,
            "天使の左手、堕天使の右手",
            "天使の左手、堕天使の右手",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2041, 5269, 2014, 4176),
            20,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:WガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            777,
            "emotionalmemoria朝寝坊のススメ",
            "朝寝坊のススメ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5253, 2013, 4171, 2052),
            20,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            776,
            "emotionalmemoria朝寝坊のススメ",
            "朝寝坊のススメ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(5253, 2013, 4171, 2052),
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            775,
            "千香瑠のエクササイズ",
            "千香瑠のエクササイズ",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2741, 2710, 4299, 5106),
            22,
            new Skill(
                "風：Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFと風属性防御力を小アップする。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            774,
            "玲瓏玉の如し",
            "玲瓏玉の如し",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5096, 2745, 4302, 2736),
            22,
            new Skill(
                "風：ウィンドパワーフォールC Ⅲ",
                "敵1～3体のATKと風属性攻撃力をダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            773,
            "ぱーふぇくとアカリズム",
            "ぱーふぇくとアカリズム",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2730, 2739, 4693, 4699),
            22,
            new Skill(
                "風：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            772,
            "グラスにラムネを注いだら",
            "グラスにラムネを注いだら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2718, 5111, 2708, 4316),
            22,
            new Skill(
                "風拡：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。オーダースキル「風属性効果増加」を発動中は敵2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [new ElementSpread(Element.Wind)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            771,
            "ツインテじゃらし",
            "ツインテじゃらし",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5091, 2722, 4318, 2729),
            22,
            new Skill(
                "風拡：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。オーダースキル「風属性効果増加」を発動中は敵2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [new ElementSpread(Element.Wind)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            770,
            "反りし刃、誘うは棺",
            "反りし刃、誘うは棺",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(5151, 1954, 4070, 1934),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            769,
            "反りし刃、誘うは棺",
            "反りし刃、誘うは棺",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5151, 1954, 4070, 1934),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            768,
            "蒼き輝き、楯たる矜持",
            "蒼き輝き、楯たる矜持",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2046, 6060, 2015, 3370),
            20,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            767,
            "蒼き輝き、楯たる矜持",
            "蒼き輝き、楯たる矜持",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2046, 6060, 2015, 3370),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            766,
            "その心、炎よりも熱く",
            "その心、炎よりも熱く",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6055, 2048, 3353, 2015),
            20,
            new Skill(
                "ウィンドパワーフォールC Ⅲ",
                "敵1～3体のATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            765,
            "その心、炎よりも熱く",
            "その心、炎よりも熱く",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6055, 2048, 3353, 2015),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            764,
            "贖いの祈り",
            "贖いの祈り",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2050, 2044, 4169, 5251),
            20,
            new Skill(
                "Sp.ガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            763,
            "贖いの祈り",
            "贖いの祈り",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(2050, 2044, 4169, 5251),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            762,
            "クリエイターズコラボ桜と貴女を",
            "桜と貴女を",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2732, 2711, 5097, 4321),
            22,
            new Skill(
                "ガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            761,
            "クリエイターズコラボgogo新学期",
            "GOGO新学期！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2728, 5121, 2748, 4309),
            22,
            new Skill(
                "火：Sp.ファイアパワーフォールC Ⅲ",
                "敵1～3体のSp.ATKと火属性攻撃力をダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            760,
            "クリエイターズコラボ天に舞う花びら",
            "天に舞う花びら",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2717, 2711, 4684, 4692),
            22,
            new Skill(
                "火：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            759,
            "クリエイターズコラボお花見ティータイム",
            "お花見ティータイム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2714, 5103, 2731, 4317),
            22,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            758,
            "クリエイターズコラボ春風に吹かれて",
            "春風に吹かれて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5098, 2720, 4283, 2744),
            22,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            757,
            "堅固なる守り",
            "堅固なる守り",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2736, 2730, 3892, 3874),
            21,
            new Skill(
                "WガードアシストE LG",
                "味方2～3体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Lg,
                Range.E
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            756,
            "光咲く日々を抱いて",
            "光咲く日々を抱いて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5155, 1936, 4059, 1925),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            755,
            "光咲く日々を抱いて",
            "光咲く日々を抱いて",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5155, 1936, 4059, 1925),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            754,
            "泡立てチャレンジの結果",
            "泡立てチャレンジの結果",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1926, 5157, 1926, 4075),
            20,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            753,
            "泡立てチャレンジの結果",
            "泡立てチャレンジの結果",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1926, 5157, 1926, 4075),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            752,
            "ぎゅーっとしてあげる",
            "ぎゅーっとしてあげる",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4869, 4866, 2292, 2294),
            22,
            new Skill(
                "風：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            751,
            "クリエイターズコラボシルト餌付け実験",
            "シルト餌付け実験",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2613, 2587, 4935, 4143),
            22,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            750,
            "クリエイターズコラボ苺飴の味わい",
            "苺飴の味わい",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2596, 4969, 2592, 4146),
            22,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            749,
            "クリエイターズコラボ甘々苺クレープ",
            "甘々苺クレープ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4949, 2609, 4134, 2604),
            22,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            748,
            "クリエイターズコラボ籠いっぱいの幸せ",
            "籠いっぱいの幸せ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2611, 4943, 2611, 4138),
            22,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            747,
            "クリエイターズコラボ苺色に染めて",
            "苺色に染めて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4936, 2580, 4168, 2604),
            22,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            746,
            "差し出されたお菓子",
            "差し出されたお菓子",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1950, 5164, 1927, 4079),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            745,
            "差し出されたお菓子",
            "差し出されたお菓子",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1950, 5164, 1927, 4079),
            20,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            744,
            "共同戦線",
            "共同戦線！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5146, 1948, 4069, 1927),
            20,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            743,
            "共同戦線",
            "共同戦線！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5146, 1948, 4069, 1927),
            20,
            new Skill(
                "ファイアパワーフォールB Ⅱ",
                "敵1～2体のATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            742,
            "僕と契約して、魔法少女になってよ",
            "僕と契約して、魔法少女になってよ！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3456, 1569, 2950, 1580),
            18,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            741,
            "僕と契約して、魔法少女になってよ",
            "僕と契約して、魔法少女になってよ！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3456, 1569, 2950, 1580),
            18,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            740,
            "奇跡の出会い",
            "奇跡の出会い！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1928, 1939, 4073, 5153),
            20,
            new Skill(
                "Sp.ファイアガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            739,
            "奇跡の出会い",
            "奇跡の出会い！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1928, 1939, 4073, 5153),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            738,
            "頼れる先輩",
            "頼れる先輩",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5180, 1955, 4066, 1945),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            737,
            "頼れる先輩",
            "頼れる先輩",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5180, 1955, 4066, 1945),
            20,
            new Skill(
                "ファイアパワーアシストC Ⅲ",
                "味方1～3体のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            736,
            "謎めいた魔法少女",
            "謎めいた魔法少女",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1926, 5153, 1956, 4063),
            20,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            735,
            "謎めいた魔法少女",
            "謎めいた魔法少女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1926, 5153, 1956, 4063),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            734,
            "連携プレーの勝利",
            "連携プレーの勝利！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1941, 5179, 1944, 4067),
            20,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            733,
            "連携プレーの勝利",
            "連携プレーの勝利！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1941, 5179, 1944, 4067),
            20,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと火属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            732,
            "氷嵐を断つ劔",
            "氷嵐を断つ劔",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1959, 5163, 1958, 4043),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            731,
            "氷嵐を断つ劔",
            "氷嵐を断つ劔",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1959, 5163, 1958, 4043),
            20,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            730,
            "一意専心",
            "一意専心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5165, 1954, 4061, 1920),
            20,
            new Skill(
                "ファイアガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            729,
            "一意専心",
            "一意専心",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5165, 1954, 4061, 1920),
            20,
            new Skill(
                "ファイアガードフォールB Ⅱ",
                "敵1～2体のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            728,
            "六花、胡蝶の如く舞う",
            "六花、胡蝶の如く舞う",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1944, 1935, 5174, 4071),
            20,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            727,
            "六花、胡蝶の如く舞う",
            "六花、胡蝶の如く舞う",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(1944, 1935, 5174, 4071),
            20,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            726,
            "輝ける流星",
            "輝ける流星",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1926, 5175, 1952, 4056),
            20,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            725,
            "輝ける流星",
            "輝ける流星",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1926, 5175, 1952, 4056),
            20,
            new Skill(
                "Sp.ガードライフアシストD Ⅱ",
                "味方2体のSp.DEFと最大HPをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Two,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            724,
            "クリエイターズコラボとろけるハート",
            "とろけるハート",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2585, 2597, 4168, 4938),
            22,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            723,
            "クリエイターズコラボ甘いきらめき",
            "甘いきらめき",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2610, 4958, 2594, 4148),
            22,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            722,
            "クリエイターズコラボショコラのゆうわく",
            "ショコラのゆうわく",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2616, 4941, 2596, 4140),
            22,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            721,
            "クリエイターズコラボいただきだゾ♪",
            "いただきだゾ♪",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(4968, 2616, 4153, 2586),
            22,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            720,
            "あなたにお茶を",
            "あなたにお茶を",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5154, 1921, 4066, 1946),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            719,
            "あなたにお茶を",
            "あなたにお茶を",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5154, 1921, 4066, 1946),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            718,
            "安らぎをあなたに",
            "安らぎをあなたに",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1948, 5174, 1931, 4053),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            717,
            "安らぎをあなたに",
            "安らぎをあなたに",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1948, 5174, 1931, 4053),
            20,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            716,
            "触れ合う吐息",
            "触れ合う吐息",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1928, 5168, 1946, 4040),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            715,
            "触れ合う吐息",
            "触れ合う吐息",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1928, 5168, 1946, 4040),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            714,
            "チョコを知らない君へ",
            "チョコを知らない君へ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(5164, 1934, 4050, 1931),
            20,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            713,
            "チョコを知らない君へ",
            "チョコを知らない君へ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5164, 1934, 4050, 1931),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            712,
            "運命のトリニティ",
            "運命のトリニティ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1927, 5157, 1933, 4046),
            20,
            new Skill(
                "Sp.ファイアパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            711,
            "運命のトリニティ",
            "運命のトリニティ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1927, 5157, 1933, 4046),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            710,
            "貴女と共にあるために",
            "貴女と共にあるために",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(5141, 1949, 4076, 1926),
            20,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            709,
            "貴女と共にあるために",
            "貴女と共にあるために",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5141, 1949, 4076, 1926),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            708,
            "貴女の笑顔を守るために",
            "貴女の笑顔を守るために",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(4547, 4567, 2593, 2604),
            22,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WパワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKとSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            707,
            "光の盾",
            "光の盾",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1952, 5177, 1948, 4051),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            706,
            "光の盾",
            "光の盾",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1952, 5177, 1948, 4051),
            20,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            705,
            "ハルナストライク",
            "ハルナストライク！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5174, 1935, 4054, 1957),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            704,
            "ハルナストライク",
            "ハルナストライク！！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5174, 1935, 4054, 1957),
            20,
            new Skill(
                "ファイアガードフォールB Ⅱ",
                "敵1～2体のDEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            703,
            "emotionalmemoria吐息の距離",
            "吐息の距離",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1921, 5172, 1952, 4042),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            702,
            "emotionalmemoria吐息の距離",
            "吐息の距離",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1921, 5172, 1952, 4042),
            20,
            new Skill(
                "Sp.ファイアパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            701,
            "ultimatememoria縦横無尽、阻む者無し",
            "縦横無尽、阻む者無し",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(4085, 4067, 7082, 7075),
            26,
            new Skill(
                "リカバーヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに自身のMPを60回復する。",
                [new Recover()],
                [],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ+",
                "HP回復時、中確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            700,
            "ultimatememoria縦横無尽、阻む者無し",
            "縦横無尽、阻む者無し",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(4085, 7401, 4092, 6731),
            26,
            new Skill(
                "Sp.マイトスマッシュA Ⅴ+",
                "敵1体に特殊超特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Five,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            699,
            "ultimatememoriaシリウス・ロア",
            "シリウス・ロア",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(7059, 7060, 4071, 4100),
            26,
            new Skill(
                "WパワーアシストD Ⅲ",
                "味方2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            698,
            "ultimatememoriaシリウス・ロア",
            "シリウス・ロア",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(7403, 4070, 6717, 4100),
            26,
            new Skill(
                "風：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            697,
            "ultimatememoria蒼き月、満ちる時",
            "蒼き月、満ちる時",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(7088, 7053, 4099, 4073),
            26,
            new Skill(
                "WパワーフォールD Ⅲ",
                "敵2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            696,
            "ultimatememoria蒼き月、満ちる時",
            "蒼き月、満ちる時",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(4098, 7397, 4099, 6719),
            26,
            new Skill(
                "風：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            695,
            "ultimatememoriaフェノメノ",
            "フェノメノ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(4062, 7427, 4091, 6726),
            26,
            new Skill(
                "火：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            694,
            "ultimatememoriaフェノメノ",
            "フェノメノ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(4062, 4093, 7081, 7070),
            26,
            new Skill(
                "WガードフォールD Ⅲ",
                "敵2体のDEFとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            693,
            "ultimatememoria比類なき異能",
            "比類なき異能",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7395, 4086, 6724, 4097),
            26,
            new Skill(
                "火：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            )
        ),
        new Memoria(
            692,
            "ultimatememoria比類なき異能",
            "比類なき異能",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(4061, 4086, 7068, 7087),
            26,
            new Skill(
                "WガードアシストD Ⅲ",
                "味方2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            691,
            "ありのままのわたしで",
            "ありのままのわたしで",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1932, 5151, 1936, 4067),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Fire), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            690,
            "ありのままのわたしで",
            "ありのままのわたしで",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1932, 5151, 1936, 4067),
            20,
            new Skill(
                "Sp.ファイアガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと火属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            689,
            "いつも隣に",
            "いつも隣に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5168, 1920, 4053, 1923),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Fire), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            688,
            "いつも隣に",
            "いつも隣に",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5168, 1920, 4053, 1923),
            20,
            new Skill(
                "ファイアパワーアシストB Ⅱ",
                "味方1～2体のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            687,
            "蝶の夢",
            "蝶の夢",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1955, 1943, 5168, 4045),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            686,
            "蝶の夢",
            "蝶の夢",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1955, 1943, 5168, 4045),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            685,
            "湯けむりの向こう側",
            "湯けむりの向こう側",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1954, 1952, 4603, 4608),
            20,
            new Skill(
                "WガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            684,
            "湯けむりの向こう側",
            "湯けむりの向こう側",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1954, 1952, 4603, 4608),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            683,
            "宵に舞う華",
            "宵に舞う華",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5145, 1955, 4054, 1953),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            682,
            "宵に舞う華",
            "宵に舞う華",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5145, 1955, 4054, 1953),
            20,
            new Skill(
                "ウィンドガードフォールB Ⅱ",
                "敵1～2体のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            681,
            "好いも甘いも受け止めて",
            "好いも甘いも受け止めて",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1946, 5167, 1926, 4043),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            680,
            "好いも甘いも受け止めて",
            "好いも甘いも受け止めて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1946, 5167, 1926, 4043),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            679,
            "2周年祭り絆の彩り",
            "2周年祭り 絆の彩り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            678,
            "2周年祭り絆の彩り",
            "2周年祭り 絆の彩り",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            677,
            "ふたりのヒメゴト",
            "ふたりのヒメゴト",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2595, 2619, 4536, 4561),
            22,
            new Skill(
                "WガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Four
            )
        ),
        new Memoria(
            676,
            "変わらない絆",
            "変わらない絆",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(4566, 4552, 2592, 2605),
            22,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Four
            )
        ),
        new Memoria(
            675,
            "cherishing",
            "Cherishing",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5544, 2277, 4220, 2317),
            22,
            new Skill(
                "ファイアガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            674,
            "止めどない熱",
            "止めどない熱",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2290, 5539, 2294, 4230),
            22,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Fire), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Four
            ),
            true
        ),
        new Memoria(
            673,
            "戦場のコンダクター",
            "戦場のコンダクター",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(2736, 4156, 2736, 3627),
            18,
            new Skill(
                "Sp.パワースマッシュA LG",
                "敵1体に特殊超特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Lg,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            672,
            "引導を渡して差し上げますわ",
            "引導を渡して差し上げますわ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(5179, 1942, 4070, 1940),
            20,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            671,
            "引導を渡して差し上げますわ",
            "引導を渡して差し上げますわ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5179, 1942, 4070, 1940),
            20,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            670,
            "飢えし群れ、挑む狩人",
            "飢えし群れ、挑む狩人",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1938, 5172, 1956, 4076),
            20,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            669,
            "飢えし群れ、挑む狩人",
            "飢えし群れ、挑む狩人",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1938, 5172, 1956, 4076),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            668,
            "うりゃうりゃうりゃうりゃ",
            "うりゃうりゃうりゃうりゃ！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5177, 1957, 4040, 1950),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            667,
            "うりゃうりゃうりゃうりゃ",
            "うりゃうりゃうりゃうりゃ！！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5177, 1957, 4040, 1950),
            20,
            new Skill(
                "ファイアパワーアシストB Ⅱ",
                "味方1～2体のATKと火属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            666,
            "この手に劔がある限り",
            "この手に劔がある限り",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2606, 2606, 4542, 4557),
            22,
            new Skill(
                "WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            665,
            "クリエイターズコラボ今年もよろしくね",
            "今年もよろしくね",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2605, 2611, 4556, 4540),
            22,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            664,
            "クリエイターズコラボ謹賀新年です",
            "謹賀新年です！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2597, 4951, 2609, 4150),
            22,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            663,
            "クリエイターズコラボあけおめですっ♪",
            "あけおめですっ♪",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4549, 4565, 2583, 2599),
            22,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            662,
            "クリエイターズコラボ新年を祝すわ",
            "新年を祝すわ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2607, 4936, 2592, 4165),
            22,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            661,
            "クリエイターズコラボことよろなのじゃ",
            "ことよろなのじゃ！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4937, 2613, 4137, 2598),
            22,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            660,
            "賀正",
            "賀正！！",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1930, 1946, 5141, 4043),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            659,
            "賀正",
            "賀正！！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1930, 1946, 5141, 4043),
            20,
            new Skill(
                "ウィンドガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            658,
            "思い出を抱きしめて",
            "思い出を抱きしめて",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1949, 5166, 1944, 4054),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            657,
            "思い出を抱きしめて",
            "思い出を抱きしめて",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1949, 5166, 1944, 4054),
            20,
            new Skill(
                "Sp.ウィンドガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            656,
            "突きて返すは兎姉妹",
            "突きて返すは兎姉妹",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1944, 5144, 1944, 4072),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            655,
            "突きて返すは兎姉妹",
            "突きて返すは兎姉妹",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1944, 5144, 1944, 4072),
            20,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            654,
            "雪兎に会えた日",
            "雪兎に会えた日",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4942, 2581, 4136, 2613),
            22,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            653,
            "大丈夫、みんながいるから",
            "大丈夫、みんながいるから",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4484, 3498, 1680, 1680),
            19,
            new Skill(
                "水：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            652,
            "大丈夫、みんながいるから",
            "大丈夫、みんながいるから",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4484, 3498, 1680, 1680),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            651,
            "高らかと響き渡る歌声の中で",
            "高らかと響き渡る歌声の中で",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1681, 4503, 1644, 3496),
            19,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            650,
            "高らかと響き渡る歌声の中で",
            "高らかと響き渡る歌声の中で",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1681, 4503, 1644, 3496),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            649,
            "スノーフレイク",
            "スノーフレイク",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1654, 5142, 1645, 2819),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            648,
            "スノーフレイク",
            "スノーフレイク",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1654, 5142, 1645, 2819),
            19,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            647,
            "胸躍る聖夜",
            "胸躍る聖夜",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4495, 3492, 1668, 1658),
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            646,
            "胸躍る聖夜",
            "胸躍る聖夜",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4495, 3492, 1668, 1658),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            645,
            "ここから先へ",
            "ここから先へ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1938, 4225, 1926, 3210),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.ATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            644,
            "ここから先へ",
            "ここから先へ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1938, 4225, 1926, 3210),
            19,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            643,
            "無邪気な親近感",
            "無邪気な親近感",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(4476, 1672, 3466, 1677),
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            642,
            "無邪気な親近感",
            "無邪気な親近感",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(4476, 1672, 3466, 1677),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            641,
            "かがみもち、できました",
            "かがみもち、できました！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5171, 1957, 4044, 1921),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            640,
            "かがみもち、できました",
            "かがみもち、できました！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(5171, 1957, 4044, 1921),
            20,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            639,
            "兎であけおめですわ",
            "兎であけおめですわ！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5154, 1929, 4070, 1924),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅱ",
                "敵1～2体のATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            638,
            "兎であけおめですわ",
            "兎であけおめですわ！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(5154, 1929, 4070, 1924),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            637,
            "新年、はっじまっるよ～♪",
            "新年、はっじまっるよ～♪",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1926, 1927, 4078, 5163),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            636,
            "新年、はっじまっるよ～♪",
            "新年、はっじまっるよ～♪",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1926, 1927, 4078, 5163),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            635,
            "初春の宴に貴女を想う",
            "初春の宴に貴女を想う",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2610, 4949, 2582, 4141),
            22,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            634,
            "約束された勝利の剣",
            "約束された勝利の剣",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1944, 5168, 1937, 4079),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            633,
            "約束された勝利の剣",
            "約束された勝利の剣",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1944, 5168, 1937, 4079),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            632,
            "射殺す百頭",
            "射殺す百頭",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5171, 1941, 4068, 1956),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            631,
            "射殺す百頭",
            "射殺す百頭",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5171, 1941, 4068, 1956),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅱ",
                "敵1～2体のATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            630,
            "投影魔術",
            "投影魔術",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(5158, 1934, 4041, 1953),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            629,
            "投影魔術",
            "投影魔術",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5158, 1934, 4041, 1953),
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            628,
            "願いの魔法少女",
            "願いの魔法少女",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1949, 5140, 1958, 4075),
            20,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと風属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            627,
            "願いの魔法少女",
            "願いの魔法少女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1949, 5140, 1958, 4075),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            626,
            "空想魔法少女",
            "空想魔法少女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5167, 1946, 4062, 1925),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            625,
            "空想魔法少女",
            "空想魔法少女",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(5167, 1946, 4062, 1925),
            20,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            624,
            "クリエイターズコラボ膝の子猫と窓の雪",
            "膝の子猫と窓の雪",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2591, 2588, 4155, 4957),
            22,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            623,
            "クリエイターズコラボかずはをよしよし",
            "かずはをよしよし",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4963, 2594, 4145, 2616),
            22,
            new Skill(
                "ウォーターパワーフォールC Ⅲ",
                "敵1～3体のATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            622,
            "クリエイターズコラボゆー姉と一緒",
            "ゆー姉と一緒！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2614, 4935, 2582, 4158),
            22,
            new Skill(
                "風拡：Sp.パワーアシストB Ⅳ",
                "味方1～2体のSp.ATKを特大アップさせる。オーダースキル「風属性効果増加」を発動中は味方2体のSp.ATKを特大アップさせる。",
                [new ElementSpread(Element.Wind)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            621,
            "クリエイターズコラボあつあつの肉まん",
            "あつあつの肉まん",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2604, 4936, 2607, 4148),
            22,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            620,
            "クリエイターズコラボどんがらがっしゃん",
            "どんがらがっしゃん",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4936, 2606, 4151, 2602),
            22,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            619,
            "懐かしくて、優しい味",
            "懐かしくて、優しい味",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1948, 5155, 1932, 4065),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            618,
            "懐かしくて、優しい味",
            "懐かしくて、優しい味",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1948, 5155, 1932, 4065),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            617,
            "聞こえし者",
            "聞こえし者",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(4601, 2722, 3178, 2737),
            21,
            new Skill(
                "パワーストライクA LG+",
                "敵1体に通常超特大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Lg,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            616,
            "ふーみんにインタビュー",
            "ふーみんにインタビュー",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1953, 1922, 4056, 5163),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            615,
            "ふーみんにインタビュー",
            "ふーみんにインタビュー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1953, 1922, 4056, 5163),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            614,
            "歴戦の余裕",
            "歴戦の余裕",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1933, 5177, 1939, 4068),
            20,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            613,
            "歴戦の余裕",
            "歴戦の余裕",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1933, 5177, 1939, 4068),
            20,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            612,
            "雪原に火花散る",
            "雪原に火花散る",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5162, 1951, 4050, 1936),
            20,
            new Skill(
                "ウォーターガードフォールB Ⅱ",
                "敵1～2体のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            611,
            "雪原に火花散る",
            "雪原に火花散る",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5162, 1951, 4050, 1936),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            610,
            "縮地、友の元へ",
            "縮地、友の元へ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1930, 5168, 1920, 4073),
            20,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            609,
            "縮地、友の元へ",
            "縮地、友の元へ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1930, 5168, 1920, 4073),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new SpecialMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            608,
            "旋律に身を委ねて",
            "旋律に身を委ねて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1928, 5154, 1955, 4061),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            607,
            "旋律に身を委ねて",
            "旋律に身を委ねて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1928, 5154, 1955, 4061),
            20,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            606,
            "神琳これは違うの",
            "神琳！？　これは違うの！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4628, 4625, 1958, 1926),
            20,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WパワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal), new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            605,
            "神琳これは違うの",
            "神琳！？　これは違うの！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4628, 4625, 1958, 1926),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            604,
            "クリエイターズコラボサンタをつかまえて",
            "サンタをつかまえて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1931, 1954, 4056, 5159),
            20,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            603,
            "クリエイターズコラボサンタをつかまえて",
            "サンタをつかまえて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1931, 1954, 4056, 5159),
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            602,
            "クリエイターズコラボ聖夜のテラリウム",
            "聖夜のテラリウム",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5152, 1935, 4045, 1926),
            20,
            new Skill(
                "ウィンドガードフォールC Ⅲ",
                "敵1～3体のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            601,
            "クリエイターズコラボ聖夜のテラリウム",
            "聖夜のテラリウム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5152, 1935, 4045, 1926),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            600,
            "クリエイターズコラボ聖夜に乾杯",
            "聖夜に乾杯",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1933, 5168, 1948, 4077),
            20,
            new Skill(
                "Sp.ウォーターパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            599,
            "クリエイターズコラボ聖夜に乾杯",
            "聖夜に乾杯",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1933, 5168, 1948, 4077),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            598,
            "クリエイターズコラボ手作りクリスマス",
            "手作りクリスマス",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1945, 5157, 1925, 4068),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            597,
            "クリエイターズコラボ手作りクリスマス",
            "手作りクリスマス",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1945, 5157, 1925, 4068),
            20,
            new Skill(
                "Sp.ウォーターパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            596,
            "クリエイターズコラボひめひめ仕立て",
            "ひめひめ仕立て",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5174, 1958, 4055, 1957),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            595,
            "クリエイターズコラボひめひめ仕立て",
            "ひめひめ仕立て",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5174, 1958, 4055, 1957),
            20,
            new Skill(
                "風：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            594,
            "はっぴーらっきーとっきー",
            "はっぴーらっきーとっきー",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5169, 1941, 4042, 1948),
            20,
            new Skill(
                "ウォーターパワーフォールB Ⅱ",
                "敵1～2体のATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            593,
            "はっぴーらっきーとっきー",
            "はっぴーらっきーとっきー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5169, 1941, 4042, 1948),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            592,
            "サプライズゲーム",
            "サプライズゲーム",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4595, 1921, 4613, 1960),
            20,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            591,
            "サプライズゲーム",
            "サプライズゲーム",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4595, 1921, 4613, 1960),
            20,
            new Skill(
                "ウォーターパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ",
                Trigger.Attack,
                [new NormalMatchPtUp()],
                Level.Two
            )
        ),
        new Memoria(
            590,
            "帯びる熱と急接近",
            "帯びる熱と急接近",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1944, 5143, 1934, 4078),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            589,
            "帯びる熱と急接近",
            "帯びる熱と急接近",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1944, 5143, 1934, 4078),
            20,
            new Skill(
                "Sp.ウォーターパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            588,
            "レンズ越しの視点",
            "レンズ越しの視点",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1695, 4540, 1715, 3528),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            587,
            "レンズ越しの視点",
            "レンズ越しの視点",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1695, 4540, 1715, 3528),
            19,
            new Skill(
                "Sp.ガードライフアシストD Ⅱ",
                "味方2体のSp.DEFと最大HPをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Two,
                Range.D
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            586,
            "白に染まる世界",
            "白に染まる世界",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            585,
            "白に染まる世界",
            "白に染まる世界",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "風：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            584,
            "雪風と踊る少女",
            "雪風と踊る少女",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1650, 1664, 4489, 3500),
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            583,
            "雪風と踊る少女",
            "雪風と踊る少女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(1650, 1664, 4489, 3500),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            582,
            "雪原の白き魔女",
            "雪原の白き魔女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1659, 4478, 1644, 3502),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            581,
            "雪原の白き魔女",
            "雪原の白き魔女",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1659, 4478, 1644, 3502),
            19,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            580,
            "鳴り響く狂乱の連弾",
            "鳴り響く狂乱の連弾",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4477, 1678, 3496, 1658),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            579,
            "鳴り響く狂乱の連弾",
            "鳴り響く狂乱の連弾",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4477, 1678, 3496, 1658),
            19,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            578,
            "クリエイターズコラボご一緒にいかが",
            "ご一緒にいかが？",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1678, 1656, 3969, 3988),
            19,
            new Skill(
                "WガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            577,
            "クリエイターズコラボご一緒にいかが",
            "ご一緒にいかが？",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1678, 1656, 3969, 3988),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            576,
            "クリエイターズコラボいつものおやつ",
            "いつものおやつ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(3973, 4002, 1649, 1647),
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            575,
            "クリエイターズコラボいつものおやつ",
            "いつものおやつ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(3973, 4002, 1649, 1647),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            574,
            "クリエイターズコラボどたばたデイズ",
            "どたばたデイズ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            573,
            "クリエイターズコラボどたばたデイズ",
            "どたばたデイズ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            572,
            "クリエイターズコラボ思い出がもう一つ",
            "思い出がもう一つ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1659, 4482, 1663, 3471),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            571,
            "クリエイターズコラボ思い出がもう一つ",
            "思い出がもう一つ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1659, 4482, 1663, 3471),
            19,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            570,
            "クリエイターズコラボもふもふな時間",
            "もふもふな時間",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4472, 1661, 3472, 1679),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            569,
            "クリエイターズコラボもふもふな時間",
            "もふもふな時間",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4472, 1661, 3472, 1679),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            568,
            "覚醒の兆し",
            "覚醒の兆し",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1669, 4488, 1670, 3494),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            567,
            "覚醒の兆し",
            "覚醒の兆し",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1669, 4488, 1670, 3494),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            566,
            "騒がし乙女の凱旋",
            "騒がし乙女の凱旋",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4500, 1652, 3470, 1656),
            19,
            new Skill(
                "ウォーターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            565,
            "騒がし乙女の凱旋",
            "騒がし乙女の凱旋",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4500, 1652, 3470, 1656),
            19,
            new Skill(
                "ウォーターガードフォールB Ⅱ",
                "敵1～2体のDEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            564,
            "あなたとおそろい",
            "あなたとおそろい",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1661, 4504, 1657, 3481),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            563,
            "あなたとおそろい",
            "あなたとおそろい",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1661, 4504, 1657, 3481),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            562,
            "ワンマンアーミー",
            "ワンマンアーミー",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4127, 3182, 3177, 2717),
            18,
            new Skill(
                "WパワーアシストD LG",
                "味方2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP/パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp(), new PowerUp(Type.Normal)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            561,
            "縄跳びトレーニング",
            "縄跳びトレーニング",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4520, 1705, 3505, 1700),
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            560,
            "縄跳びトレーニング",
            "縄跳びトレーニング",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4520, 1705, 3505, 1700),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            559,
            "暮れなずむ空",
            "暮れなずむ空",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4534, 1687, 3534, 1682),
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            558,
            "暮れなずむ空",
            "暮れなずむ空",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4534, 1687, 3534, 1682),
            19,
            new Skill(
                "チャージガードフォールB Ⅱ",
                "敵1～2体のDEFをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            557,
            "かめ、のち、えがお",
            "かめ、のち、えがお",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1649, 1645, 4503, 3491),
            19,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            556,
            "かめ、のち、えがお",
            "かめ、のち、えがお",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1649, 1645, 4503, 3491),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを特大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            555,
            "ひめひめコールお願いっ",
            "ひめひめコールお願いっ！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1668, 4466, 1662, 3477),
            19,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            554,
            "ひめひめコールお願いっ",
            "ひめひめコールお願いっ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1668, 4466, 1662, 3477),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            553,
            "カワウソづくし",
            "カワウソづくし",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4496, 1668, 3473, 1683),
            19,
            new Skill(
                "水：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            552,
            "カワウソづくし",
            "カワウソづくし",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4496, 1668, 3473, 1683),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            551,
            "クリエイターズコラボ不動劔と至宝",
            "不動劔と至宝",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            550,
            "クリエイターズコラボ不動劔と至宝",
            "不動劔と至宝",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "Sp.ウォーターガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            549,
            "クリエイターズコラボねんねこぐろっぴ",
            "ねんねこぐろっぴ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4474, 1663, 3499, 1657),
            19,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            548,
            "クリエイターズコラボねんねこぐろっぴ",
            "ねんねこぐろっぴ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4474, 1663, 3499, 1657),
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            547,
            "charmという兵器",
            "CHARMという兵器",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            546,
            "charmという兵器",
            "CHARMという兵器",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            545,
            "ワーオエキサイティン",
            "ワーオ！　エキサイティン！！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4472, 1657, 3502, 1667),
            19,
            new Skill(
                "ウィンドガードフォールC Ⅲ",
                "敵1～3体のDEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            544,
            "ワーオエキサイティン",
            "ワーオ！　エキサイティン！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4472, 1657, 3502, 1667),
            19,
            new Skill(
                "ウィンドパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            543,
            "束の間の休息",
            "束の間の休息",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1665, 1683, 3490, 4493),
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            542,
            "束の間の休息",
            "束の間の休息",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1665, 1683, 3490, 4493),
            19,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Wind), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            541,
            "作戦会議です",
            "作戦会議です！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1462, 2918, 1493, 2657),
            17,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            540,
            "作戦会議です",
            "作戦会議です！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1462, 2918, 1493, 2657),
            17,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Wind), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            539,
            "予想外の事態",
            "予想外の事態",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4497, 1671, 3481, 1661),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            538,
            "予想外の事態",
            "予想外の事態",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(4497, 1671, 3481, 1661),
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            537,
            "優雅なティータイム",
            "優雅なティータイム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            536,
            "優雅なティータイム",
            "優雅なティータイム",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                [],
                [new StatusUp(new Life(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:WガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            535,
            "西住流の誇り",
            "西住流の誇り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1681, 4503, 1666, 3473),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            534,
            "西住流の誇り",
            "西住流の誇り",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1681, 4503, 1666, 3473),
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            533,
            "形勢逆転",
            "形勢逆転！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            532,
            "形勢逆転",
            "形勢逆転！！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            531,
            "ワイン色の思い出",
            "ワイン色の思い出",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            530,
            "ワイン色の思い出",
            "ワイン色の思い出",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "風：WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFとSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal), new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            529,
            "月夜に吠える天狼",
            "月夜に吠える天狼",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4160, 3644, 2736, 2701),
            21,
            new Skill(
                "WパワーフォールE LG",
                "敵2～3体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Lg,
                Range.E
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            528,
            "雲間から差し込む光",
            "雲間から差し込む光",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1672, 4500, 1655, 3479),
            19,
            new Skill(
                "Sp.ウォーターガードバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Water), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            527,
            "雲間から差し込む光",
            "雲間から差し込む光",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1672, 4500, 1655, 3479),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            526,
            "好機を待つのじゃ",
            "好機を待つのじゃ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4474, 1672, 3492, 1644),
            19,
            new Skill(
                "水：パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            525,
            "好機を待つのじゃ",
            "好機を待つのじゃ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4474, 1672, 3492, 1644),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            524,
            "ひめひめ華麗に参上",
            "ひめひめ華麗に参上！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1668, 4480, 1654, 3477),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            523,
            "ひめひめ華麗に参上",
            "ひめひめ華麗に参上！",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1668, 4480, 1654, 3477),
            19,
            new Skill(
                "水拡：Sp.パワーフォールB Ⅳ",
                "敵1～2体のSp.ATKを特大ダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体のSp.ATKを特大ダウンさせる。",
                [new ElementSpread(Element.Water)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            522,
            "クリエイターズコラボ紅葉の帳",
            "紅葉の帳",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            521,
            "クリエイターズコラボ紅葉の帳",
            "紅葉の帳",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            520,
            "クリエイターズコラボ紅葉も頬も色づいて",
            "紅葉も頬も色づいて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1650, 1679, 4498, 3482),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            519,
            "クリエイターズコラボ紅葉も頬も色づいて",
            "紅葉も頬も色づいて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(1650, 1679, 4498, 3482),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            518,
            "クリエイターズコラボ秋月夜の彩り",
            "秋月夜の彩り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1658, 4467, 1670, 3492),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            517,
            "クリエイターズコラボ秋月夜の彩り",
            "秋月夜の彩り",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1658, 4467, 1670, 3492),
            19,
            new Skill(
                "Sp.ウォーターパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            516,
            "クリエイターズコラボ秋の木漏れ日",
            "秋の木漏れ日",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            515,
            "クリエイターズコラボ秋の木漏れ日",
            "秋の木漏れ日",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            514,
            "はじらいマミー",
            "はじらいマミー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4357, 2093, 3536, 2079),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            513,
            "どきどきデビル",
            "どきどきデビル",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4471, 1679, 3480, 1677),
            19,
            new Skill(
                "Sp.ディファーアシストC Ⅳ",
                "味方1～3体のATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            512,
            "どきどきデビル",
            "どきどきデビル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4471, 1679, 3480, 1677),
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            511,
            "いたずらゴースト",
            "いたずらゴースト",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1670, 4493, 1650, 3469),
            19,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            510,
            "いたずらゴースト",
            "いたずらゴースト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1670, 4493, 1650, 3469),
            19,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            509,
            "夜空に響く勝利の歌",
            "夜空に響く勝利の歌",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4478, 1655, 3482, 1671),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            508,
            "夜空に響く勝利の歌",
            "夜空に響く勝利の歌",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4478, 1655, 3482, 1671),
            19,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            507,
            "陽だまりシュッツエンゲル",
            "陽だまりシュッツエンゲル",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1672, 4466, 1657, 3469),
            19,
            new Skill(
                "Sp.ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKと水属性攻撃力を小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            506,
            "陽だまりシュッツエンゲル",
            "陽だまりシュッツエンゲル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1672, 4466, 1657, 3469),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            505,
            "クリエイターズコラボくるくおーらんたん",
            "くるくおーらんたん",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4473, 1661, 3479, 1650),
            19,
            new Skill(
                "ウォーターパワーフォールC Ⅲ",
                "敵1～3体のATKと水属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            504,
            "クリエイターズコラボくるくおーらんたん",
            "くるくおーらんたん",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4473, 1661, 3479, 1650),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            503,
            "クリエイターズコラボこころにいたずら",
            "こころにいたずら",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1645, 4495, 1680, 3472),
            19,
            new Skill(
                "水：Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            502,
            "クリエイターズコラボこころにいたずら",
            "こころにいたずら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1645, 4495, 1680, 3472),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            501,
            "クリエイターズコラボearlytrick",
            "Early Trick",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1660, 4502, 1658, 3491),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            500,
            "クリエイターズコラボearlytrick",
            "Early Trick",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1660, 4502, 1658, 3491),
            19,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            499,
            "クリエイターズコラボジャックコーデ",
            "ジャックコーデ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4498, 1681, 3494, 1655),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            498,
            "クリエイターズコラボジャックコーデ",
            "ジャックコーデ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4498, 1681, 3494, 1655),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            497,
            "秋空ピクニック",
            "秋空ピクニック",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1679, 4489, 1672, 3471),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            496,
            "秋空ピクニック",
            "秋空ピクニック",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1679, 4489, 1672, 3471),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            495,
            "ソーイングマスター姫歌",
            "ソーイングマスター姫歌",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4500, 1659, 3480, 1681),
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            494,
            "ソーイングマスター姫歌",
            "ソーイングマスター姫歌",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4500, 1659, 3480, 1681),
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            493,
            "庭園の護り人",
            "庭園の護り人",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1660, 4487, 1680, 3499),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            492,
            "庭園の護り人",
            "庭園の護り人",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1660, 4487, 1680, 3499),
            19,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            491,
            "ふたりの距離",
            "ふたりの距離",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4467, 1676, 3498, 1646),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            490,
            "ふたりの距離",
            "ふたりの距離",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4467, 1676, 3498, 1646),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            489,
            "ダイスキをキャンバスに",
            "ダイスキをキャンバスに",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4491, 1651, 3498, 1669),
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            488,
            "ダイスキをキャンバスに",
            "ダイスキをキャンバスに",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4491, 1651, 3498, 1669),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            487,
            "らんとたづさのかくれんぼ",
            "らんとたづさのかくれんぼ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4482, 1684, 3471, 1674),
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            486,
            "らんとたづさのかくれんぼ",
            "らんとたづさのかくれんぼ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4482, 1684, 3471, 1674),
            19,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            485,
            "藍は舞い降りた",
            "藍は舞い降りた",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1679, 4487, 1657, 3492),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            484,
            "藍は舞い降りた",
            "藍は舞い降りた",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1679, 4487, 1657, 3492),
            19,
            new Skill(
                "ディファーアシストC Ⅳ",
                "味方1～3体のSp.ATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            483,
            "静寂に佇む狩人",
            "静寂に佇む狩人",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4491, 1644, 3477, 1684),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            482,
            "静寂に佇む狩人",
            "静寂に佇む狩人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4491, 1644, 3477, 1684),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            481,
            "クリエイターズコラボ好きなものを一緒に",
            "好きなものを一緒に",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2070, 2071, 3534, 4363),
            21,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            480,
            "クリエイターズコラボ星空のどうぶつ探し",
            "星空のどうぶつ探し",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2099, 2064, 4362, 3536),
            21,
            new Skill(
                "水：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            479,
            "クリエイターズコラボ二人の奏でる夜の歌",
            "二人の奏でる夜の歌",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2063, 4356, 2092, 3543),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            478,
            "クリエイターズコラボすすきの道しるべ",
            "すすきの道しるべ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4324, 2087, 3541, 2098),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            477,
            "息を潜めて",
            "息を潜めて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1657, 4486, 1661, 3487),
            19,
            new Skill(
                "Sp.ウォーターパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            476,
            "息を潜めて",
            "息を潜めて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1657, 4486, 1661, 3487),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            475,
            "ミッドナイトスティール",
            "ミッドナイトスティール",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4505, 1666, 3496, 1664),
            19,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            474,
            "ミッドナイトスティール",
            "ミッドナイトスティール",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4505, 1666, 3496, 1664),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            473,
            "真夜中の極秘作戦",
            "真夜中の極秘作戦",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2079, 2093, 3559, 4359),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            472,
            "誠実なる守護者",
            "誠実なる守護者",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4155, 2730, 3634, 2716),
            18,
            new Skill(
                "パワーストライクA LG",
                "敵1体に通常超特大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Lg,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            471,
            "一葉ののんびりタイム",
            "一葉ののんびりタイム",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4503, 1681, 3466, 1674),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            470,
            "一葉ののんびりタイム",
            "一葉ののんびりタイム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4503, 1681, 3466, 1674),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            469,
            "クリエイターズコラボペアトレ",
            "ペアトレ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2088, 2088, 4334, 3549),
            21,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            468,
            "クリエイターズコラボ先輩ふぁいと☆",
            "先輩ふぁいと☆",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3549, 4355, 2086, 2070),
            21,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            467,
            "クリエイターズコラボもっと優しく",
            "もっと優しく",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2079, 4343, 2091, 3542),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            466,
            "クリエイターズコラボりざるとちぇっく",
            "りざるとちぇっく",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4341, 2095, 3527, 2081),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            465,
            "親愛なるルームメイト",
            "親愛なるルームメイト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2089, 4328, 2074, 3557),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            464,
            "戦場のお色直し",
            "戦場のお色直し",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1650, 4500, 1650, 3485),
            19,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            463,
            "戦場のお色直し",
            "戦場のお色直し",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1650, 4500, 1650, 3485),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            462,
            "白鳥の姫騎士",
            "白鳥の姫騎士",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4476, 1670, 3467, 1647),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            461,
            "白鳥の姫騎士",
            "白鳥の姫騎士",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4476, 1670, 3467, 1647),
            19,
            new Skill(
                "水拡：パワーアシストB Ⅳ",
                "味方1～2体のATKを特大アップさせる。オーダースキル「水属性効果増加」を発動中は味方2体のATKを特大アップさせる。",
                [new ElementSpread(Element.Water)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            460,
            "氷帝",
            "氷帝",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1673, 1670, 4490, 3480),
            19,
            new Skill(
                "ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKと水属性攻撃力を小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            459,
            "氷帝",
            "氷帝",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(1673, 1670, 4490, 3480),
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            458,
            "その瞳に映るモノ",
            "その瞳に映るモノ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1668, 4470, 1668, 3498),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            457,
            "その瞳に映るモノ",
            "その瞳に映るモノ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1668, 4470, 1668, 3498),
            19,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと水属性防御力をダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium), new StatusDown(new ElementGuard(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            456,
            "アクロバット・シューター",
            "アクロバット・シューター",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1655, 4468, 1649, 3500),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            455,
            "アクロバット・シューター",
            "アクロバット・シューター",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1655, 4468, 1649, 3500),
            19,
            new Skill(
                "Sp.ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKと水属性攻撃力を小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            454,
            "天からの強襲",
            "天からの強襲",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4497, 1681, 3470, 1645),
            19,
            new Skill(
                "ウォーターパワーアシストB Ⅱ",
                "味方1～2体のATKと水属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Water), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            453,
            "天からの強襲",
            "天からの強襲",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4497, 1681, 3470, 1645),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Water), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            452,
            "ナイトガンスリンガー",
            "ナイトガンスリンガー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2703, 4156, 2712, 3654),
            21,
            new Skill(
                "Sp.ガードバーストD LG",
                "敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            451,
            "クリエイターズコラボ夏色日和",
            "夏色日和",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(2075, 2070, 3553, 4327),
            21,
            new Skill(
                "Sp.ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと光属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            450,
            "クリエイターズコラボ打ち上げ花火",
            "打ち上げ花火",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4329, 2068, 3561, 2084),
            21,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            449,
            "クリエイターズコラボ涼やかな響き",
            "涼やかな響き",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4354, 2079, 3526, 2071),
            21,
            new Skill(
                "ライトガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと光属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            448,
            "クリエイターズコラボ納涼かき氷",
            "納涼かき氷",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(2085, 4351, 2078, 3562),
            21,
            new Skill(
                "Sp.ダークガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと闇属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            447,
            "飛び出せミリアム",
            "飛び出せミリアム！",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1676, 1659, 4480, 3503),
            19,
            new Skill(
                "ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            446,
            "飛び出せミリアム",
            "飛び出せミリアム！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(1676, 1659, 4480, 3503),
            19,
            new Skill(
                "ライトパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと光属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Light), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            445,
            "ポイ越しの笑顔",
            "ポイ越しの笑顔",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1679, 4470, 1647, 3493),
            19,
            new Skill(
                "Sp.ライトパワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと光属性攻撃力を小ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Small), new StatusDown(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            444,
            "ポイ越しの笑顔",
            "ポイ越しの笑顔",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1679, 4470, 1647, 3493),
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            443,
            "祭囃子と恋の音",
            "祭囃子と恋の音",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4348, 2066, 3551, 2078),
            21,
            new Skill(
                "ライトパワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKと光属性攻撃力を小ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Small), new StatusDown(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            442,
            "クリエイターズコラボ想いを込めた歌声",
            "想いを込めた歌声",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(2092, 2082, 4351, 3559),
            21,
            new Skill(
                "ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと光属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            441,
            "クリエイターズコラボ花咲くハーモニー",
            "花咲くハーモニー",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4341, 2075, 3524, 2094),
            21,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            440,
            "クリエイターズコラボ躍動の旋律",
            "躍動の旋律",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4352, 2061, 3561, 2096),
            21,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            439,
            "クリエイターズコラボきらめきステージ",
            "きらめきステージ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2088, 4349, 2083, 3528),
            21,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            438,
            "夏の海とかき氷",
            "夏の海とかき氷",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4478, 1653, 3490, 1670),
            19,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            437,
            "夏の海とかき氷",
            "夏の海とかき氷",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4478, 1653, 3490, 1670),
            19,
            new Skill(
                "ダークガードフォールB Ⅱ",
                "敵1～2体のDEFと闇属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            436,
            "見返り美人",
            "見返り美人",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1657, 4503, 1663, 3494),
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            435,
            "見返り美人",
            "見返り美人",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1657, 4503, 1663, 3494),
            19,
            new Skill(
                "Sp.ダークパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと闇属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            434,
            "diverse",
            "Diverse",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1649, 4493, 1664, 3489),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            433,
            "diverse",
            "Diverse",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1649, 4493, 1664, 3489),
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            432,
            "cherish",
            "Cherish",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4494, 1659, 3478, 1669),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            431,
            "cherish",
            "Cherish",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4494, 1659, 3478, 1669),
            19,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            430,
            "激戦の終わりに",
            "激戦の終わりに",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2083, 4354, 2071, 3539),
            21,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            429,
            "極限コンビネーション",
            "極限コンビネーション",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1654, 1672, 3466, 4499),
            19,
            new Skill(
                "Sp.ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと光属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            428,
            "極限コンビネーション",
            "極限コンビネーション",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1654, 1672, 3466, 4499),
            19,
            new Skill(
                "Sp.ダークパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            427,
            "不屈の一太刀",
            "不屈の一太刀",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4474, 1658, 3501, 1665),
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            426,
            "不屈の一太刀",
            "不屈の一太刀",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4474, 1658, 3501, 1665),
            19,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            425,
            "クリエイターズコラボらぶらぶぴーす",
            "らぶらぶぴーす",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4334, 2095, 3539, 2080),
            21,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            424,
            "クリエイターズコラボひまわりとんだよ",
            "ひまわりとんだよ",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4335, 2076, 3528, 2067),
            21,
            new Skill(
                "ライトパワーフォールB Ⅱ",
                "敵1～2体のATKと光属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            423,
            "クリエイターズコラボレディーティータイム",
            "レディーティータイム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2074, 4351, 2089, 3560),
            21,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            422,
            "クリエイターズコラボ月に顔をそむけて",
            "月に顔をそむけて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4348, 2078, 3528, 2089),
            21,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            421,
            "バトル・デプロイメント",
            "バトル・デプロイメント",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1673, 4487, 1646, 3494),
            19,
            new Skill(
                "Sp.ダークパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            420,
            "バトル・デプロイメント",
            "バトル・デプロイメント",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1673, 4487, 1646, 3494),
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            419,
            "淀みを蹴って",
            "淀みを蹴って",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1649, 4467, 1667, 3506),
            19,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            418,
            "淀みを蹴って",
            "淀みを蹴って",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1649, 4467, 1667, 3506),
            19,
            new Skill(
                "Sp.ライトパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと光属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            417,
            "アンブッシュ",
            "アンブッシュ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4490, 1647, 3480, 1682),
            19,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            416,
            "アンブッシュ",
            "アンブッシュ",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(4490, 1647, 3480, 1682),
            19,
            new Skill(
                "ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと光属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            415,
            "emotionalmemoria晴れときどきサンオイル",
            "晴れときどきサンオイル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4486, 1682, 3506, 1670),
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            414,
            "emotionalmemoria晴れときどきサンオイル",
            "晴れときどきサンオイル",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(4486, 1682, 3506, 1670),
            19,
            new Skill(
                "ダークガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            413,
            "楽しいを探しに行こう",
            "楽しいを探しに行こう！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "ダークパワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKと闇属性攻撃力を小ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Small), new StatusDown(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            412,
            "楽しいを探しに行こう",
            "楽しいを探しに行こう！",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            411,
            "ダイビング・アタッカー",
            "ダイビング・アタッカー",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1650, 4486, 1667, 3500),
            19,
            new Skill(
                "Sp.ダークガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと闇属性防御力をアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            410,
            "ダイビング・アタッカー",
            "ダイビング・アタッカー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1650, 4486, 1667, 3500),
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            409,
            "ウッドクラフトに挑戦",
            "ウッドクラフトに挑戦",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4338, 2073, 3523, 2097),
            21,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            408,
            "炊事は任せた",
            "炊事は任せた！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(2072, 4357, 2091, 3537),
            21,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            407,
            "面目躍如のサバイバル",
            "面目躍如のサバイバル",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1678, 1656, 4469, 3488),
            19,
            new Skill(
                "ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            406,
            "面目躍如のサバイバル",
            "面目躍如のサバイバル",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(1678, 1656, 4469, 3488),
            19,
            new Skill(
                "ライトパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            405,
            "殲滅のシルバーバレット",
            "殲滅のシルバーバレット",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4473, 1680, 3471, 1647),
            19,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            404,
            "殲滅のシルバーバレット",
            "殲滅のシルバーバレット",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4473, 1680, 3471, 1647),
            19,
            new Skill(
                "ライトガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと光属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            403,
            "やめられない刺激",
            "やめられない刺激",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ライトパワーフォールB Ⅱ",
                "敵1～2体のATKと光属性攻撃力をダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new ElementAttack(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            402,
            "やめられない刺激",
            "やめられない刺激",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Small), new StatusDown(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            401,
            "光の注ぐ夜",
            "光の注ぐ夜",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1659, 1660, 3485, 4471),
            19,
            new Skill(
                "Sp.ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと闇属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            400,
            "光の注ぐ夜",
            "光の注ぐ夜",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1659, 1660, 3485, 4471),
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            399,
            "ゼロ距離のしあわせ",
            "ゼロ距離のしあわせ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1650, 4483, 1650, 3501),
            19,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            398,
            "ゼロ距離のしあわせ",
            "ゼロ距離のしあわせ",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1650, 4483, 1650, 3501),
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium), new StatusUp(new ElementGuard(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            397,
            "水も滴るいい乙女",
            "水も滴るいい乙女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4491, 1666, 3492, 1672),
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            396,
            "水も滴るいい乙女",
            "水も滴るいい乙女",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4491, 1666, 3492, 1672),
            19,
            new Skill(
                "ライトパワーアシストB Ⅱ",
                "味方1～2体のATKと光属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Light), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            395,
            "リトル・アークメイジ",
            "リトル・アークメイジ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2705, 2734, 4126, 3624),
            18,
            new Skill(
                "WガードフォールD LG",
                "敵2体のDEFとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP/ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp(), new GuardDown(Type.Normal)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            394,
            "信じる想いを力に変えて",
            "信じる想いを力に変えて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1678, 4474, 1648, 3478),
            19,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new ElementAttack(Element.Light), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            393,
            "信じる想いを力に変えて",
            "信じる想いを力に変えて",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1678, 4474, 1648, 3478),
            19,
            new Skill(
                "Sp.ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと闇属性防御力を小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small), new StatusUp(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            392,
            "絆のアルケミートレース",
            "絆のアルケミートレース",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4483, 1682, 3479, 1659),
            19,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new ElementAttack(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            391,
            "絆のアルケミートレース",
            "絆のアルケミートレース",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4483, 1682, 3479, 1659),
            19,
            new Skill(
                "ダークガードフォールB Ⅱ",
                "敵1～2体のDEFと闇属性防御力をダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new ElementGuard(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            390,
            "ピュリファイ・ラプラス",
            "ピュリファイ・ラプラス",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1662, 4496, 1647, 3491),
            19,
            new Skill(
                "Sp.ダークパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと闇属性攻撃力をアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new ElementAttack(Element.Dark), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            389,
            "ピュリファイ・ラプラス",
            "ピュリファイ・ラプラス",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1662, 4496, 1647, 3491),
            19,
            new Skill(
                "Sp.ダークガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと闇属性防御力を小ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Small), new StatusDown(new ElementGuard(Element.Dark), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            388,
            "ヘイムスクリングラ・シスターズ",
            "ヘイムスクリングラ・シスターズ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4504, 1679, 3504, 1650),
            19,
            new Skill(
                "WパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとSp.ATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            387,
            "ヘイムスクリングラ・シスターズ",
            "ヘイムスクリングラ・シスターズ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4504, 1679, 3504, 1650),
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            386,
            "黄昏の英雄たち",
            "黄昏の英雄たち",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1672, 1678, 3477, 4479),
            19,
            new Skill(
                "Sp.ガードフォールA Ⅳ",
                "敵1体のSp.DEFを超特大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            385,
            "黄昏の英雄たち",
            "黄昏の英雄たち",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1672, 1678, 3477, 4479),
            19,
            new Skill(
                "風：スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            384,
            "emotionalmemoriaお姉様の水難",
            "お姉様の水難",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4498, 1656, 3498, 1651),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            383,
            "emotionalmemoriaお姉様の水難",
            "お姉様の水難",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4498, 1656, 3498, 1651),
            19,
            new Skill(
                "マイトフォールB Ⅲ",
                "敵1～2体のATKとDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            382,
            "この勝利が小さな一歩でも",
            "この勝利が小さな一歩でも",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1649, 1645, 3503, 4491),
            19,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            381,
            "この勝利が小さな一歩でも",
            "この勝利が小さな一歩でも",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1649, 1645, 3503, 4491),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            380,
            "巨影を討つ閃光",
            "巨影を討つ閃光",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4490, 1644, 3484, 1655),
            19,
            new Skill(
                "光：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            379,
            "巨影を討つ閃光",
            "巨影を討つ閃光",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4490, 1644, 3484, 1655),
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            378,
            "ここから先は通さない",
            "ここから先は通さない",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1674, 4490, 1651, 3505),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            377,
            "ここから先は通さない",
            "ここから先は通さない",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1674, 4490, 1651, 3505),
            19,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            376,
            "猛る獅子の剣",
            "猛る獅子の剣",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4137, 2719, 3657, 2731),
            21,
            new Skill(
                "ガードブレイクD LG",
                "敵2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            ),
            true
        ),
        new Memoria(
            375,
            "クローバー・クラウン",
            "クローバー・クラウン",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            374,
            "クローバー・クラウン",
            "クローバー・クラウン",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "WパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKとSp.ATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            373,
            "華の休息",
            "華の休息",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1650, 4484, 1681, 3473),
            19,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            372,
            "華の休息",
            "華の休息",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1650, 4484, 1681, 3473),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            371,
            "ビーチでバカンス",
            "ビーチでバカンス",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1675, 1671, 4481, 3483),
            19,
            new Skill(
                "パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            370,
            "ビーチでバカンス",
            "ビーチでバカンス",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(1675, 1671, 4481, 3483),
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            369,
            "乙女の非常事態",
            "乙女の非常事態",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            368,
            "乙女の非常事態",
            "乙女の非常事態",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "闇：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            367,
            "m.v.p.オンステージ",
            "M.V.P.オンステージ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4503, 1681, 3488, 1651),
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            366,
            "m.v.p.オンステージ",
            "M.V.P.オンステージ",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4503, 1681, 3488, 1651),
            19,
            new Skill(
                "ディファーフォールB Ⅲ",
                "敵1～2体のSp.ATKとDEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            365,
            "勝利のファンファーレ",
            "勝利のファンファーレ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            364,
            "勝利のファンファーレ",
            "勝利のファンファーレ",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            363,
            "お手当マイスター",
            "お手当マイスター",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1652, 4485, 1677, 3479),
            19,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            362,
            "お手当マイスター",
            "お手当マイスター",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1652, 4485, 1677, 3479),
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            361,
            "謳歌のミュージックアワー",
            "謳歌のミュージックアワー",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            360,
            "謳歌のミュージックアワー",
            "謳歌のミュージックアワー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            359,
            "メイクアップ",
            "メイクアップ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1650, 4479, 1680, 3489),
            19,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            358,
            "メイクアップ",
            "メイクアップ！",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1650, 4479, 1680, 3489),
            19,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            357,
            "回遊のススメ",
            "回遊のススメ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4487, 1683, 3490, 1671),
            19,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            356,
            "回遊のススメ",
            "回遊のススメ",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4487, 1683, 3490, 1671),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            355,
            "尊みの探求者",
            "尊みの探求者",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "闇：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            354,
            "尊みの探求者",
            "尊みの探求者",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            353,
            "生徒会のお仕事",
            "生徒会のお仕事",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            352,
            "生徒会のお仕事",
            "生徒会のお仕事",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "光：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            351,
            "美しき師弟関係",
            "美しき師弟関係",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4508, 1710, 3534, 1691),
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            350,
            "美しき師弟関係",
            "美しき師弟関係",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(4508, 1710, 3534, 1691),
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            349,
            "最高のルームメイト",
            "最高のルームメイト",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1689, 1676, 4028, 4028),
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            348,
            "最高のルームメイト",
            "最高のルームメイト",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1689, 1676, 4028, 4028),
            19,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            347,
            "bzのプロフェッショナル",
            "BZのプロフェッショナル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            346,
            "bzのプロフェッショナル",
            "BZのプロフェッショナル",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            345,
            "もう一度、何度でも",
            "もう一度、何度でも",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(3492, 1613, 2978, 1610),
            18,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            344,
            "もう一度、何度でも",
            "もう一度、何度でも",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3492, 1613, 2978, 1610),
            18,
            new Skill(
                "闇：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            343,
            "相生の水先案内人",
            "相生の水先案内人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3466, 1601, 2945, 1587),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            342,
            "相生の水先案内人",
            "相生の水先案内人",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3466, 1601, 2945, 1587),
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            341,
            "心の炎は豪雨で消えず",
            "心の炎は豪雨で消えず",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1574, 3467, 1583, 2965),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            340,
            "心の炎は豪雨で消えず",
            "心の炎は豪雨で消えず",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1574, 3467, 1583, 2965),
            18,
            new Skill(
                "ディファーアシストB Ⅲ",
                "味方1～2体のSp.ATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            339,
            "昼下がりのラプソディー",
            "昼下がりのラプソディー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1595, 3459, 1575, 2935),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを特大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            338,
            "昼下がりのラプソディー",
            "昼下がりのラプソディー",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1595, 3459, 1575, 2935),
            18,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            337,
            "私ヲ蝕ム悪イ夢",
            "私ヲ蝕ム悪イ夢",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1581, 3443, 1585, 2959),
            18,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            336,
            "私ヲ蝕ム悪イ夢",
            "私ヲ蝕ム悪イ夢",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1581, 3443, 1585, 2959),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            335,
            "だいすきをあげる",
            "だいすきをあげる",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3460, 1569, 2945, 1572),
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            334,
            "だいすきをあげる",
            "だいすきをあげる",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3460, 1569, 2945, 1572),
            18,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            333,
            "希望の光",
            "希望の光",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1634, 3491, 1627, 2973),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            332,
            "希望の光",
            "希望の光",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1634, 3491, 1627, 2973),
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            331,
            "思い出はカメラの中に",
            "思い出はカメラの中に",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3318, 3324, 1831, 1833),
            20,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            330,
            "ブレイク・タイム",
            "ブレイク・タイム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3648, 1848, 2966, 1820),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            329,
            "ラブリーアンドピース",
            "ラブリーアンドピース",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3297, 3307, 1833, 1826),
            20,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            328,
            "すってんあかりん",
            "すってんあかりん",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1821, 3669, 1856, 2946),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            327,
            "獅子奮迅",
            "獅子奮迅",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2722, 2698, 3639, 4132),
            18,
            new Skill(
                "WガードアシストD LG",
                "味方2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "援:支援UP/Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp(), new GuardUp(Type.Special)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            326,
            "せめて、この子だけは",
            "せめて、この子だけは",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:マイトDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとDEFを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal), new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            325,
            "せめて、この子だけは",
            "せめて、この子だけは",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "水：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:マイトUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとDEFを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal), new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            324,
            "紅巴式夏祭りの楽しみ方",
            "紅巴式夏祭りの楽しみ方",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1606, 3442, 1586, 2941),
            18,
            new Skill(
                "Sp.ディファーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ディファーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal), new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            323,
            "紅巴式夏祭りの楽しみ方",
            "紅巴式夏祭りの楽しみ方",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1606, 3442, 1586, 2941),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.マイトDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special), new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            322,
            "世界を越えて",
            "世界を越えて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3457, 1591, 2958, 1597),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            321,
            "世界を越えて",
            "世界を越えて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3457, 1591, 2958, 1597),
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            320,
            "小さなシュッツエンゲル",
            "小さなシュッツエンゲル",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2703, 3440, 1573, 1844),
            18,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            319,
            "小さなシュッツエンゲル",
            "小さなシュッツエンゲル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2703, 3440, 1573, 1844),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            318,
            "束ねる絆の一夜",
            "束ねる絆の一夜",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(3450, 1599, 2935, 1594),
            18,
            new Skill(
                "Sp.ディファーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:WガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFとSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            317,
            "束ねる絆の一夜",
            "束ねる絆の一夜",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3450, 1599, 2935, 1594),
            18,
            new Skill(
                "水：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            316,
            "救う願いの一閃",
            "救う願いの一閃",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1601, 3894, 1601, 2489),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            315,
            "救う願いの一閃",
            "救う願いの一閃",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1601, 3894, 1601, 2489),
            18,
            new Skill(
                "水：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            314,
            "足踏み健康ロードの悲劇",
            "足踏み健康ロードの悲劇",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3464, 1606, 2960, 1580),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            313,
            "足踏み健康ロードの悲劇",
            "足踏み健康ロードの悲劇",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3464, 1606, 2960, 1580),
            18,
            new Skill(
                "闇：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            312,
            "みんなを守るために",
            "みんなを守るために",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1603, 1584, 3446, 2969),
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            311,
            "みんなを守るために",
            "みんなを守るために",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(1603, 1584, 3446, 2969),
            18,
            new Skill(
                "Sp.ディファーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            310,
            "狂化フルスロットル",
            "狂化フルスロットル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            309,
            "狂化フルスロットル",
            "狂化フルスロットル",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            308,
            "友を守護する剣",
            "友を守護する剣",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3433, 1601, 2964, 1571),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            307,
            "友を守護する剣",
            "友を守護する剣",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3433, 1601, 2964, 1571),
            18,
            new Skill(
                "チャージガードフォールB Ⅱ",
                "敵1～2体のDEFをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            306,
            "不死身の刃",
            "不死身の刃",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1594, 3439, 1601, 2957),
            18,
            new Skill(
                "ディファーアシストB Ⅲ",
                "味方1～2体のSp.ATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            305,
            "不死身の刃",
            "不死身の刃",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1594, 3439, 1601, 2957),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            304,
            "emotionalmemoria癒しの露天風呂",
            "癒しの露天風呂",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1578, 3442, 1580, 2972),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            303,
            "emotionalmemoria癒しの露天風呂",
            "癒しの露天風呂",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1578, 3442, 1580, 2972),
            18,
            new Skill(
                "光：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            302,
            "アサルトリリィふるーつ",
            "アサルトリリィ ふるーつ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1683, 3524, 1684, 3034),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            301,
            "アサルトリリィふるーつ",
            "アサルトリリィ ふるーつ",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1683, 3524, 1684, 3034),
            18,
            new Skill(
                "光：WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            300,
            "ストームデュオ",
            "ストームデュオ",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "光：ガードアシストB Ⅲ",
                "味方1～2体のDEFを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            299,
            "ストームデュオ",
            "ストームデュオ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "マイトブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            298,
            "アクアストライク",
            "アクアストライク",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            297,
            "アクアストライク",
            "アクアストライク",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "チャージヒールC Ⅱ",
                "味方1～3体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.C
            ),
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            296,
            "雷光一閃",
            "雷光一閃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1585, 3472, 1574, 2962),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            295,
            "雷光一閃",
            "雷光一閃",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1585, 3472, 1574, 2962),
            18,
            new Skill(
                "光：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            294,
            "ハッピーを見つけたら☆",
            "ハッピーを見つけたら☆",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1575, 1589, 2955, 3466),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            293,
            "ハッピーを見つけたら☆",
            "ハッピーを見つけたら☆",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1575, 1589, 2955, 3466),
            18,
            new Skill(
                "パワーブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            292,
            "戦いを終えて",
            "戦いを終えて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1595, 1596, 3438, 2935),
            18,
            new Skill(
                "ガードヒールB Ⅲ+",
                "味方1～2体のHPを大回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "回:WガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            291,
            "戦いを終えて",
            "戦いを終えて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1595, 1596, 3438, 2935),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            290,
            "通じ合うふたり",
            "通じ合うふたり",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1585, 3918, 1583, 2507),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            289,
            "通じ合うふたり",
            "通じ合うふたり",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1585, 3918, 1583, 2507),
            18,
            new Skill(
                "火：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            288,
            "麗しき出立",
            "麗しき出立",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3467, 1590, 2945, 1595),
            18,
            new Skill(
                "光：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            287,
            "麗しき出立",
            "麗しき出立",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3467, 1590, 2945, 1595),
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            286,
            "飛翔迎撃",
            "飛翔迎撃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1594, 3432, 1580, 2972),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            285,
            "飛翔迎撃",
            "飛翔迎撃",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1594, 3432, 1580, 2972),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            284,
            "不動劔の姫",
            "不動剣の姫",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(3895, 1589, 2499, 1590),
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            283,
            "不動劔の姫",
            "不動剣の姫",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3895, 1589, 2499, 1590),
            18,
            new Skill(
                "チャージパワーフォールB Ⅱ",
                "敵1～2体のATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            282,
            "そうさく倶楽部の活動",
            "そうさく倶楽部の活動",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1608, 3454, 1599, 2952),
            18,
            new Skill(
                "スマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            281,
            "そうさく倶楽部の活動",
            "そうさく倶楽部の活動",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1608, 3454, 1599, 2952),
            18,
            new Skill(
                "光：WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            280,
            "愛しき人との待ち合わせ",
            "愛しき人との待ち合わせ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3450, 1601, 2968, 1605),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            279,
            "愛しき人との待ち合わせ",
            "愛しき人との待ち合わせ",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(3450, 1601, 2968, 1605),
            18,
            new Skill(
                "パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            278,
            "月下の傍観者",
            "月下の傍観者",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1606, 3469, 1569, 2962),
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            277,
            "月下の傍観者",
            "月下の傍観者",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1606, 3469, 1569, 2962),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            276,
            "なかよしとわいらいと",
            "なかよしとわいらいと",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3459, 1596, 2943, 1595),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            275,
            "なかよしとわいらいと",
            "なかよしとわいらいと",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3459, 1596, 2943, 1595),
            18,
            new Skill(
                "光：WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            274,
            "水流乱撃",
            "水流乱撃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1595, 3464, 1594, 2941),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            273,
            "水流乱撃",
            "水流乱撃",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1595, 3464, 1594, 2941),
            18,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            272,
            "踏み込む勇気",
            "踏み込む勇気",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2583, 1398, 2818, 1422),
            17,
            new Skill(
                "火：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            271,
            "踏み込む勇気",
            "踏み込む勇気",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2583, 1398, 2818, 1422),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            270,
            "吸血鬼のたしなみ",
            "吸血鬼のたしなみ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1593, 1572, 2946, 3470),
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            269,
            "吸血鬼のたしなみ",
            "吸血鬼のたしなみ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1593, 1572, 2946, 3470),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.マイトDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKとSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special), new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            268,
            "校舎屋上のストラグル",
            "校舎屋上のストラグル",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2939, 3468, 1574, 1572),
            18,
            new Skill(
                "風：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            267,
            "校舎屋上のストラグル",
            "校舎屋上のストラグル",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(2939, 3468, 1574, 1572),
            18,
            new Skill(
                "WパワーバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のATKとSp.ATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:WパワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal), new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            266,
            "清淑なる黒き槍",
            "清淑なる黒き槍",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3890, 1588, 2515, 1582),
            18,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            265,
            "清淑なる黒き槍",
            "清淑なる黒き槍",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3890, 1588, 2515, 1582),
            18,
            new Skill(
                "火：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            264,
            "世界を守る剣たち",
            "世界を守る剣たち",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3450, 1603, 2936, 1589),
            18,
            new Skill(
                "光：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            263,
            "世界を守る剣たち",
            "世界を守る剣たち",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3450, 1603, 2936, 1589),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            262,
            "悪夢との共闘",
            "悪夢との共闘",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1597, 3437, 1582, 2966),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            261,
            "悪夢との共闘",
            "悪夢との共闘",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1597, 3437, 1582, 2966),
            18,
            new Skill(
                "チャージSp.ガードアシストC Ⅲ",
                "味方1～3体のSp.DEFをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            260,
            "扶翼の剣",
            "扶翼の剣",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1575, 3466, 1575, 2951),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            259,
            "扶翼の剣",
            "扶翼の剣",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1575, 3466, 1575, 2951),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            258,
            "灼爛の一撃",
            "灼爛の一撃",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1579, 3458, 1570, 2935),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            257,
            "灼爛の一撃",
            "灼爛の一撃",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1579, 3458, 1570, 2935),
            18,
            new Skill(
                "Sp.パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            256,
            "静寂の中で",
            "静寂の中で",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3461, 1595, 2956, 1583),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            255,
            "静寂の中で",
            "静寂の中で",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3461, 1595, 2956, 1583),
            18,
            new Skill(
                "チャージガードアシストC Ⅲ",
                "味方1～3体のDEFをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            254,
            "キラキラアイスクリーム",
            "キラキラアイスクリーム！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3443, 1571, 2937, 1603),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            253,
            "キラキラアイスクリーム",
            "キラキラアイスクリーム！",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3443, 1571, 2937, 1603),
            18,
            new Skill(
                "チャージヒールD Ⅱ",
                "味方2体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.D
            ),
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            252,
            "迎え撃つ勇士たち",
            "迎え撃つ勇士たち",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1600, 3469, 1573, 2933),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            251,
            "迎え撃つ勇士たち",
            "迎え撃つ勇士たち",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1600, 3469, 1573, 2933),
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            250,
            "不撓不屈の心を胸に",
            "不撓不屈の心を胸に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3446, 1578, 2962, 1573),
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            249,
            "不撓不屈の心を胸に",
            "不撓不屈の心を胸に",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3446, 1578, 2962, 1573),
            18,
            new Skill(
                "闇：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            248,
            "ブレイブ・ショット",
            "ブレイブ・ショット",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1575, 3439, 1607, 2955),
            18,
            new Skill(
                "チャージSp.パワーアシストB Ⅱ",
                "味方1～2体のSp.ATKをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            247,
            "ブレイブ・ショット",
            "ブレイブ・ショット",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1575, 3439, 1607, 2955),
            18,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            246,
            "発進☆ユニコーン",
            "発進☆ユニコーン！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1578, 3885, 1570, 2491),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            245,
            "発進☆ユニコーン",
            "発進☆ユニコーン！",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1578, 3885, 1570, 2491),
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.マイトUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKとSp.DEFを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            244,
            "仮想訓練場の応酬",
            "仮想訓練場の応酬",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3912, 1593, 2489, 1608),
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:WガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFとSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            243,
            "仮想訓練場の応酬",
            "仮想訓練場の応酬",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3912, 1593, 2489, 1608),
            18,
            new Skill(
                "火：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ディファーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとDEFを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special), new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            242,
            "優しい夕暮れ",
            "優しい夕暮れ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(3463, 1596, 2947, 1586),
            18,
            new Skill(
                "ガードブレイクA Ⅴ",
                "敵1体に通常超特大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Five,
                Range.A
            ),
            new SupportSkill(
                "攻:マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKとDEFを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal), new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            241,
            "優しい夕暮れ",
            "優しい夕暮れ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(3463, 1596, 2947, 1586),
            18,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal), new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            240,
            "静かに肩を寄せて",
            "静かに肩を寄せて",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1587, 3452, 1599, 2951),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            239,
            "静かに肩を寄せて",
            "静かに肩を寄せて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1587, 3452, 1599, 2951),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            238,
            "レスキューキャット",
            "レスキューキャット",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3463, 1587, 2935, 1579),
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            237,
            "レスキューキャット",
            "レスキューキャット",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3463, 1587, 2935, 1579),
            18,
            new Skill(
                "闇：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            236,
            "平穏を守るための哮り",
            "平穏を守るための哮り",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3443, 1603, 2948, 1588),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            235,
            "平穏を守るための哮り",
            "平穏を守るための哮り",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3443, 1603, 2948, 1588),
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅰ",
                "HP回復時、一定確率でHPの回復量をアップさせる。さらに、支援/妨害時、一定確率で支援/妨害時効果を小アップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.One
            )
        ),
        new Memoria(
            234,
            "ハッピー＆トリート",
            "ハッピー＆トリート",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1589, 3920, 1609, 2508),
            18,
            new Skill(
                "Sp.ガードバーストA Ⅴ",
                "敵1体に特殊超特大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Five,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            233,
            "ハッピー＆トリート",
            "ハッピー＆トリート",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1589, 3920, 1609, 2508),
            18,
            new Skill(
                "Sp.パワーフォールC Ⅳ",
                "敵1～3体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.マイトDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special), new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            232,
            "あなただけの守護天使",
            "あなただけの守護天使",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1593, 3460, 1586, 2971),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            231,
            "あなただけの守護天使",
            "あなただけの守護天使",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1593, 3460, 1586, 2971),
            18,
            new Skill(
                "闇：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            230,
            "いつでも近くに",
            "いつでも近くに",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(3463, 1590, 2960, 1605),
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            229,
            "いつでも近くに",
            "いつでも近くに",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3463, 1590, 2960, 1605),
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            228,
            "純白の想い",
            "純白の想い",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "闇：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            227,
            "純白の想い",
            "純白の想い",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            226,
            "蒼き月の御使い",
            "蒼き月の御使い",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2736, 4156, 2736, 3627),
            18,
            new Skill(
                "スマッシュD LG",
                "敵2体に特殊大ダメージを与える。",
                [],
                [],
                Level.Lg,
                Range.D
            ),
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp(), new PowerUp(Type.Special)],
                Level.Three
            ),
            true
        ),
        new Memoria(
            225,
            "放課後のミューズ",
            "放課後のミューズ",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1596, 3458, 1591, 2937),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new RecoveryUp(), new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            224,
            "放課後のミューズ",
            "放課後のミューズ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1596, 3458, 1591, 2937),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            223,
            "小春日和",
            "小春日和",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3444, 1578, 2956, 1595),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            222,
            "小春日和",
            "小春日和",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3444, 1578, 2956, 1595),
            18,
            new Skill(
                "WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            221,
            "真夜中のクリエイター",
            "真夜中のクリエイター",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1601, 3447, 1583, 2969),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            220,
            "真夜中のクリエイター",
            "真夜中のクリエイター",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1601, 3447, 1583, 2969),
            18,
            new Skill(
                "闇：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            219,
            "遠い日の足跡",
            "遠い日の足跡",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1604, 3436, 1572, 2959),
            18,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            218,
            "遠い日の足跡",
            "遠い日の足跡",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1604, 3436, 1572, 2959),
            18,
            new Skill(
                "チャージスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            217,
            "先駆けプリンセス",
            "先駆けプリンセス",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3475, 1589, 2978, 1601),
            18,
            new Skill(
                "チャージパワーアシストB Ⅱ",
                "味方1～2体のATKをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            216,
            "先駆けプリンセス",
            "先駆けプリンセス",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(3475, 1589, 2978, 1601),
            18,
            new Skill(
                "チャージストライクA Ⅲ",
                "敵1体に通常大ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            215,
            "寂寥の美",
            "寂寥の美",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3437, 1596, 2952, 1592),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            214,
            "寂寥の美",
            "寂寥の美",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3437, 1596, 2952, 1592),
            18,
            new Skill(
                "チャージヒールC Ⅱ",
                "味方1～3体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                [new Charge()],
                [],
                Level.Two,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            213,
            "劔の妖精",
            "劔の妖精",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1857, 3650, 1837, 2949),
            20,
            new Skill(
                "WパワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.ATKを小ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Small), new StatusDown(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            212,
            "セインツの宝石",
            "セインツの宝石",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3576, 1697, 3061, 1667),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            211,
            "セインツの宝石",
            "セインツの宝石",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3576, 1697, 3061, 1667),
            18,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            210,
            "暴君の花嫁",
            "暴君の花嫁",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1859, 1843, 3328, 3293),
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small), new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            209,
            "約束の蕾",
            "約束の蕾",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2978, 1614, 3463, 1584),
            18,
            new Skill(
                "水：ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            208,
            "約束の蕾",
            "約束の蕾",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2978, 1614, 3463, 1584),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のATKとDEFをアップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal), new GuardUp(Type.Normal)],
                Level.One
            )
        ),
        new Memoria(
            207,
            "大切な存在",
            "大切な存在",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1593, 3446, 1579, 2943),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            206,
            "大切な存在",
            "大切な存在",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1593, 3446, 1579, 2943),
            18,
            new Skill(
                "Sp.パワーフォールC Ⅳ",
                "敵1～3体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            205,
            "黄昏の研究者たち",
            "黄昏の研究者たち",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1487, 1477, 2828, 3336),
            17,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            204,
            "黄昏の研究者たち",
            "黄昏の研究者たち",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(1487, 1477, 2828, 3336),
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            203,
            "出逢いの約束",
            "出逢いの約束",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1481, 1473, 2805, 3328),
            17,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Two
            )
        ),
        new Memoria(
            202,
            "出逢いの約束",
            "出逢いの約束",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1481, 1473, 2805, 3328),
            17,
            new Skill(
                "Sp.マイトバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            201,
            "暁に笑う少女",
            "暁に笑う少女",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(3456, 1584, 2966, 1600),
            18,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            200,
            "暁に笑う少女",
            "暁に笑う少女",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(3456, 1584, 2966, 1600),
            18,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            199,
            "ハンドメイド・リリィ",
            "ハンドメイド・リリィ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1575, 3467, 1601, 2948),
            18,
            new Skill(
                "Sp.ガードアシストD Ⅲ",
                "味方2体のSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            198,
            "ハンドメイド・リリィ",
            "ハンドメイド・リリィ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1575, 3467, 1601, 2948),
            18,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            197,
            "ハッピーバレンタインだにゃん♪",
            "ハッピーバレンタインだにゃん♪",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1834, 1821, 3316, 3316),
            20,
            new Skill(
                "WパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKとSp.ATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small), new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Three
            )
        ),
        new Memoria(
            196,
            "まごころをこめて",
            "まごころをこめて！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3675, 1857, 2949, 1843),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            195,
            "ドキドキ・ショコラーデ",
            "ドキドキ・ショコラーデ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1848, 3674, 1831, 2953),
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            194,
            "煌めく花々",
            "煌めく花々",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1518, 1527, 3115, 3094),
            18,
            new Skill(
                "ヒールD Ⅳ",
                "味方2体のHPを大回復する。",
                [],
                [],
                Level.Four,
                Range.D
            ),
            new SupportSkill(
                "回:WガードUP Ⅰ",
                "HP回復時、一定確率で味方前衛1体のDEFとSp.DEFをアップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal), new GuardUp(Type.Special)],
                Level.One
            )
        ),
        new Memoria(
            193,
            "煌めく花々",
            "煌めく花々",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(1518, 1527, 3115, 3094),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            192,
            "一筆の心",
            "一筆の心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3440, 1597, 2958, 1569),
            18,
            new Skill(
                "WパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとSp.ATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            191,
            "一筆の心",
            "一筆の心",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3440, 1597, 2958, 1569),
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            190,
            "飾らぬ想いに咲き誇る",
            "飾らぬ想いに咲き誇る",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1602, 3434, 1585, 2955),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            189,
            "飾らぬ想いに咲き誇る",
            "飾らぬ想いに咲き誇る",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1602, 3434, 1585, 2955),
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            188,
            "ヘルヴォルのお嫁さん",
            "ヘルヴォルのお嫁さん",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1598, 3453, 1571, 2960),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            187,
            "ヘルヴォルのお嫁さん",
            "ヘルヴォルのお嫁さん",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1598, 3453, 1571, 2960),
            18,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFをアップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new GuardUp(Type.Special)],
                Level.One
            )
        ),
        new Memoria(
            186,
            "楽しい遊園地",
            "楽しい遊園地",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1590, 3471, 1593, 2959),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            185,
            "楽しい遊園地",
            "楽しい遊園地",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1590, 3471, 1593, 2959),
            18,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            184,
            "リフレッシュエンジン",
            "リフレッシュ！エンジン",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3888, 1587, 2519, 1576),
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            183,
            "リフレッシュエンジン",
            "リフレッシュ！エンジン",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3888, 1587, 2519, 1576),
            18,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            182,
            "夜の闇を切り拓く者たち",
            "夜の闇を切り拓く者たち",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1601, 1606, 3460, 2943),
            18,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            181,
            "夜の闇を切り拓く者たち",
            "夜の闇を切り拓く者たち",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(1601, 1606, 3460, 2943),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            180,
            "エクストリームブースト",
            "エクストリームブースト",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3456, 1583, 2942, 1580),
            18,
            new Skill(
                "ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            179,
            "エクストリームブースト",
            "エクストリームブースト",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3456, 1583, 2942, 1580),
            18,
            new Skill(
                "パワーストライクA Ⅲ+",
                "敵1体に通常大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            178,
            "ガーディアン・パワー",
            "ガーディアン・パワー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1854, 3672, 1833, 2958),
            20,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Three
            )
        ),
        new Memoria(
            177,
            "コール・ユア・ネーム",
            "コール・ユア・ネーム",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1825, 1855, 3654, 2973),
            20,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            176,
            "ジャスト・ザ・ブレイブ",
            "ジャスト・ザ・ブレイブ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3666, 1847, 2946, 1823),
            20,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            175,
            "anniversarymemoria大切なあなたを想い",
            "大切なあなたを想い",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1529, 1517, 2485, 2212),
            18,
            new Skill(
                "パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            174,
            "戦火の結束",
            "戦火の結束",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3439, 1586, 2945, 1575),
            18,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            173,
            "戦火の結束",
            "戦火の結束",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(3439, 1586, 2945, 1575),
            18,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            172,
            "ラ・ピュセル",
            "ラ・ピュセル",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1821, 4469, 1856, 2146),
            20,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            171,
            "鬼神の意志を継ぐ者",
            "鬼神の意志を継ぐ者",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(3656, 2976, 1834, 1845),
            20,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Three
            )
        ),
        new Memoria(
            170,
            "台場の白き魔女",
            "台場の白き魔女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3672, 1857, 2968, 1831),
            20,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            169,
            "狂乱の姫巫女",
            "狂乱の姫巫女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1854, 3655, 1834, 2977),
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Three
            )
        ),
        new Memoria(
            168,
            "親愛なる仲間",
            "親愛なる仲間",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1561, 3005, 2760, 1531),
            17,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Three
            )
        ),
        new Memoria(
            167,
            "親愛なる仲間",
            "親愛なる仲間",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1561, 3005, 2760, 1531),
            17,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            166,
            "そこにある笑顔",
            "そこにある笑顔",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2842, 1386, 2586, 1397),
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            165,
            "そこにある笑顔",
            "そこにある笑顔",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2842, 1386, 2586, 1397),
            17,
            new Skill(
                "マイトフォールA Ⅲ",
                "敵1体のATKとDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            164,
            "ふたりのアーセナル",
            "ふたりのアーセナル",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1402, 1422, 1632, 3793),
            17,
            new Skill(
                "火：Sp.ガードアシストB Ⅲ",
                "味方1～2体のSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            163,
            "ふたりのアーセナル",
            "ふたりのアーセナル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1402, 1422, 1632, 3793),
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            162,
            "パジャマパーティー",
            "パジャマパーティー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1556, 2977, 1560, 2493),
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            161,
            "ファイア・ダッシュ",
            "ファイア・ダッシュ",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(3441, 1579, 2943, 1609),
            18,
            new Skill(
                "風：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            160,
            "ファイア・ダッシュ",
            "ファイア・ダッシュ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3441, 1579, 2943, 1609),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            159,
            "ミューチュアルプロテクション",
            "ミューチュアルプロテクション",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1607, 3435, 1608, 2945),
            18,
            new Skill(
                "ヒールスマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            158,
            "ミューチュアルプロテクション",
            "ミューチュアルプロテクション",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1607, 3435, 1608, 2945),
            18,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            157,
            "海の世界に想いを馳せて",
            "海の世界に想いを馳せて",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1585, 1590, 2968, 3465),
            18,
            new Skill(
                "ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Two
            )
        ),
        new Memoria(
            156,
            "海の世界に想いを馳せて",
            "海の世界に想いを馳せて",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1585, 1590, 2968, 3465),
            18,
            new Skill(
                "ディファースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            155,
            "charmを絵筆に替えて",
            "CHARMを絵筆に替えて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1570, 3464, 1601, 2934),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Two
            )
        ),
        new Memoria(
            154,
            "charmを絵筆に替えて",
            "CHARMを絵筆に替えて",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1570, 3464, 1601, 2934),
            18,
            new Skill(
                "Sp.ディファーフォールB Ⅲ",
                "敵1～2体のATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            153,
            "ボナペティ",
            "ボナペティ！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3457, 1576, 2964, 1594),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            152,
            "ボナペティ",
            "ボナペティ！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3457, 1576, 2964, 1594),
            18,
            new Skill(
                "火：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            151,
            "恋花様ダイエット大作戦",
            "恋花様ダイエット大作戦",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2663, 1484, 2928, 1493),
            17,
            new Skill(
                "風：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            150,
            "恋花様ダイエット大作戦",
            "恋花様ダイエット大作戦",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2663, 1484, 2928, 1493),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            149,
            "復讐の炎",
            "復讐の炎",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1490, 3288, 1471, 2337),
            17,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            148,
            "復讐の炎",
            "復讐の炎",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1490, 3288, 1471, 2337),
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            147,
            "強くなるために",
            "強くなるために",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1417, 1423, 2822, 2569),
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            146,
            "強くなるために",
            "強くなるために",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1417, 1423, 2822, 2569),
            17,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            145,
            "大切な貴女への贈り物",
            "大切な貴女への贈り物",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "火：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            144,
            "大切な貴女への贈り物",
            "大切な貴女への贈り物",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "パワーブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            143,
            "罰執行のお時間です",
            "罰執行のお時間です",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Two
            )
        ),
        new Memoria(
            142,
            "罰執行のお時間です",
            "罰執行のお時間です",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            141,
            "故郷へ想い馳せながら",
            "故郷へ想い馳せながら",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1836, 2980, 1825, 3670),
            20,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            140,
            "心弛ぶひととき",
            "心弛ぶひととき",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1855, 3661, 1833, 2966),
            20,
            new Skill(
                "水：スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            139,
            "元日の決斗",
            "元日の決斗！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1845, 3640, 1831, 2980),
            20,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            138,
            "気高き錬金術師",
            "気高き錬金術師",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2993, 1571, 2477, 1570),
            19,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Two
            )
        ),
        new Memoria(
            137,
            "ガンズ・パーティー",
            "ガンズ・パーティー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2992, 1576, 2491, 1553),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            136,
            "神獣鏡の輝き",
            "神獣鏡の輝き",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1562, 1564, 2996, 2485),
            19,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            135,
            "絆の歌",
            "絆の歌",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1544, 3366, 1530, 2402),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            134,
            "絆の歌",
            "絆の歌",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1544, 3366, 1530, 2402),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            133,
            "戦いの合間に",
            "戦いの合間に",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1522, 2462, 1507, 2241),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            132,
            "輝きの海岸線",
            "輝きの海岸線",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1494, 1459, 2689, 2913),
            17,
            new Skill(
                "WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            131,
            "輝きの海岸線",
            "輝きの海岸線",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(1494, 1459, 2689, 2913),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            130,
            "ざっぱ～～ん",
            "ざっぱ～～ん！",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1532, 2766, 1532, 3001),
            17,
            new Skill(
                "水：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            129,
            "ざっぱ～～ん",
            "ざっぱ～～ん！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1532, 2766, 1532, 3001),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            128,
            "白花咲く港",
            "白花咲く港",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1461, 2901, 2687, 1481),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            127,
            "白花咲く港",
            "白花咲く港",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1461, 2901, 2687, 1481),
            17,
            new Skill(
                "Sp.パワーアシストA Ⅳ",
                "味方1体のSp.ATKを超特大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            126,
            "一柳隊、大特集",
            "一柳隊、大特集！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2857, 1409, 2606, 1389),
            17,
            new Skill(
                "火：ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            125,
            "一柳隊、大特集",
            "一柳隊、大特集！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2857, 1409, 2606, 1389),
            17,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            124,
            "あなたに傘を",
            "あなたに傘を",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1469, 2670, 1473, 2921),
            17,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            123,
            "あなたに傘を",
            "あなたに傘を",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1469, 2670, 1473, 2921),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅲ+",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            122,
            "情熱の取材前夜",
            "情熱の取材前夜！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2939, 1478, 2680, 1476),
            17,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            121,
            "情熱の取材前夜",
            "情熱の取材前夜！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2939, 1478, 2680, 1476),
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            120,
            "姫歌を脅かす2つの新星",
            "姫歌を脅かす2つの新星",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1421, 1423, 2827, 2591),
            17,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            119,
            "姫歌を脅かす2つの新星",
            "姫歌を脅かす2つの新星",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1421, 1423, 2827, 2591),
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            118,
            "ハッピー☆シューティングスター",
            "ハッピー☆シューティングスター",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2842, 1400, 1396, 2579),
            17,
            new Skill(
                "ヒールストライクA Ⅳ",
                "敵1体に通常特大ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            117,
            "ハッピー☆シューティングスター",
            "ハッピー☆シューティングスター",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2842, 1400, 1396, 2579),
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            116,
            "プレゼントはお任せ♪",
            "プレゼントはお任せ♪",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2906, 1490, 2656, 1475),
            17,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            115,
            "プレゼントはお任せ♪",
            "プレゼントはお任せ♪",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2906, 1490, 2656, 1475),
            17,
            new Skill(
                "風：ガードアシストB Ⅲ",
                "味方1～2体のDEFを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。",
                [new ElementStimulation(Element.Fire)],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            114,
            "リリィのすべてを伝えるために",
            "リリィのすべてを伝えるために",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1527, 3358, 1544, 2419),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            113,
            "リリィのすべてを伝えるために",
            "リリィのすべてを伝えるために",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1527, 3358, 1544, 2419),
            17,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            112,
            "キャッチ＆リリース＆イート",
            "キャッチ＆リリース＆イート",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2548, 1410, 2725, 1425),
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            111,
            "キャッチ＆リリース＆イート",
            "キャッチ＆リリース＆イート",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2548, 1410, 2725, 1425),
            17,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            110,
            "これまでも、これからも隣で",
            "これまでも、これからも隣で",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3170, 1419, 2249, 1406),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            109,
            "これまでも、これからも隣で",
            "これまでも、これからも隣で",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(3170, 1419, 2249, 1406),
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            108,
            "リリィになるために",
            "リリィになるために！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1568, 2970, 1568, 2498),
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            107,
            "結梨の大好きな場所",
            "結梨の大好きな場所",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1467, 2902, 1461, 2684),
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            106,
            "結梨の大好きな場所",
            "結梨の大好きな場所",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1467, 2902, 1461, 2684),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            105,
            "みんな、ガンバレー",
            "みんな、ガンバレー！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2997, 1581, 2470, 1545),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            104,
            "冷たいラムネをどうぞ",
            "冷たいラムネをどうぞ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1558, 1553, 2496, 2970),
            19,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            103,
            "リワード・マイセルフ",
            "リワード・マイセルフ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2906, 1463, 2688, 1479),
            17,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            102,
            "リワード・マイセルフ",
            "リワード・マイセルフ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2906, 1463, 2688, 1479),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            101,
            "トライング・オン",
            "トライング・オン",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2931, 1474, 2653, 1466),
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            100,
            "トライング・オン",
            "トライング・オン",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2931, 1474, 2653, 1466),
            17,
            new Skill(
                "Sp.マイトフォールA Ⅲ",
                "敵1体のSp.ATKとSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium), new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            99,
            "気まぐれのツーショット",
            "気まぐれのツーショット",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1568, 2994, 1561, 2505),
            19,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            98,
            "藍の宝物",
            "藍の宝物",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1410, 2383, 2220, 2249),
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            97,
            "藍の宝物",
            "藍の宝物",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1410, 2383, 2220, 2249),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            96,
            "指先と白いペン",
            "指先と白いペン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3391, 1743, 2764, 1750),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            ),
            true
        ),
        new Memoria(
            95,
            "指先と白いペン",
            "指先と白いペン",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3391, 1743, 2764, 1750),
            17,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Two
            ),
            true
        ),
        new Memoria(
            94,
            "エレクトロンバウト",
            "エレクトロンバウト！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1575, 2987, 1572, 2502),
            19,
            new Skill(
                "ヒールスマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            93,
            "悲壮の華",
            "悲壮の華",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1464, 1484, 3276, 2300),
            17,
            new Skill(
                "ガードアシストD Ⅲ",
                "味方2体のDEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            92,
            "悲壮の華",
            "悲壮の華",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(1464, 1484, 3276, 2300),
            17,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            91,
            "鬼さんズ、こちら",
            "鬼さんズ、こちら",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3356, 1554, 2399, 1565),
            17,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            90,
            "鬼さんズ、こちら",
            "鬼さんズ、こちら",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3356, 1554, 2399, 1565),
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            89,
            "優美なる舞",
            "優美なる舞",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1397, 1652, 2598, 2600),
            17,
            new Skill(
                "WガードスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のDEFとSp.DEFをアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            88,
            "優美なる舞",
            "優美なる舞",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1397, 1652, 2598, 2600),
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            87,
            "美しき鉄糸の舞",
            "美しき鉄糸の舞",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1485, 2900, 1494, 2685),
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            86,
            "美しき鉄糸の舞",
            "美しき鉄糸の舞",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1485, 2900, 1494, 2685),
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            85,
            "駆けろエージェント",
            "駆けろ！エージェント",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1571, 2992, 1838, 2471),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFをアップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special), new GuardUp(Type.Special)],
                Level.One
            )
        ),
        new Memoria(
            84,
            "スピード☆スター",
            "スピード☆スター",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2469, 2216, 1518, 1520),
            18,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            83,
            "電光石火でご到着",
            "電光石火でご到着！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2489, 1509, 2221, 1531),
            18,
            new Skill(
                "風：ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。さらに味方がオーダースキル「風属性効果増加」を発動中は威力がアップする。",
                [new ElementStimulation(Element.Fire)],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Two
            )
        ),
        new Memoria(
            82,
            "街角の寡黙な花",
            "街角の寡黙な花",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1574, 2515, 1542, 2288),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFを小アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small), new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            81,
            "アクアプラクティス",
            "アクアプラクティス",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2925, 1461, 2678, 1473),
            17,
            new Skill(
                "パワーストライクA Ⅲ+",
                "敵1体に通常大ダメージを与え、自身のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            80,
            "アクアプラクティス",
            "アクアプラクティス",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2925, 1461, 2678, 1473),
            17,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            79,
            "二水のヘイムスクリングラ体験",
            "二水のヘイムスクリングラ体験",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1499, 2484, 1514, 2230),
            18,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            78,
            "勝利の女神が微笑む時",
            "勝利の女神が微笑む時",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1522, 1509, 2219, 2482),
            18,
            new Skill(
                "パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            77,
            "プレシャス・モーニング",
            "プレシャス・モーニング",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2954, 1528, 2710, 1519),
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            76,
            "プレシャス・モーニング",
            "プレシャス・モーニング",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2954, 1528, 2710, 1519),
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のATKとDEFをアップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal), new GuardUp(Type.Normal)],
                Level.One
            )
        ),
        new Memoria(
            75,
            "フォール・ダウン・アタック",
            "フォール・ダウン・アタック",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1502, 1508, 2492, 2228),
            18,
            new Skill(
                "ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Two
            )
        ),
        new Memoria(
            74,
            "ホワイト・ラビット・マジック",
            "ホワイト・ラビット・マジック！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1506, 2472, 1499, 2213),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            73,
            "プレ・ハロウィンパーティー",
            "プレ・ハロウィンパーティー！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1507, 2485, 1522, 2244),
            18,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            72,
            "ようこそふしぎの国へ",
            "ようこそ！ふしぎの国へ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1569, 1561, 2471, 2994),
            19,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                [],
                [new StatusUp(new Atk(), Amount.Small)],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            71,
            "おいでよ☆ハロウィン",
            "おいでよ☆ハロウィン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2971, 1545, 2489, 1584),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            70,
            "一直線上のストラテジー",
            "一直線上のストラテジー",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1532, 2939, 1515, 1760),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Two
            )
        ),
        new Memoria(
            69,
            "この空の下で",
            "この空の下で",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1509, 1506, 2207, 2493),
            18,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            68,
            "追跡者",
            "追跡者",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2468, 2241, 1524, 1508),
            18,
            new Skill(
                "Sp.パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKをダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            67,
            "花を束ねる者",
            "花を束ねる者",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1578, 2978, 1547, 2488),
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            66,
            "姉妹の休息",
            "姉妹の休息",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1509, 1510, 2226, 2462),
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            65,
            "凛々しい花々",
            "凛々しい花々",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2552, 1587, 2302, 1605),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            64,
            "わたしたちの魔法",
            "わたしたちの魔法",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1561, 2504, 1557, 2981),
            19,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            63,
            "貫く想いの一撃",
            "貫く想いの一撃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1549, 2967, 1581, 2491),
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            62,
            "プリンセスひめひめ",
            "プリンセスひめひめ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2913, 1522, 1786, 1501),
            18,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            61,
            "いつかみんなと見る景色",
            "いつかみんなと見る景色",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3006, 1579, 2492, 1569),
            19,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            60,
            "繋げたい言葉",
            "繋げたい言葉",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1500, 2470, 1530, 2230),
            18,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            59,
            "夏祭りのスナイパー",
            "夏祭りのスナイパー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2496, 1517, 2245, 1497),
            18,
            new Skill(
                "ガードストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のDEFをアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            58,
            "もう何も奪わせない",
            "もう何も奪わせない",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1476, 1485, 2128, 2057),
            17,
            new Skill(
                "ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のDEFを小アップする。",
                [],
                [new StatusUp(new Def(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Two
            )
        ),
        new Memoria(
            57,
            "夜空に咲く約束の花",
            "夜空に咲く約束の花",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1568, 2981, 1578, 2497),
            19,
            new Skill(
                "Sp.ガードスマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            56,
            "のびのびトレーニング",
            "のびのびトレーニング！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1529, 2494, 1503, 2230),
            18,
            new Skill(
                "Sp.マイトアシストA Ⅲ",
                "味方1体のSp.ATKとSp.DEFを大アップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium), new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            55,
            "神宿りの暴走",
            "神宿りの暴走",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(3000, 1576, 2479, 1562),
            19,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            54,
            "信頼の背中",
            "信頼の背中",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1393, 2036, 1397, 2002),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            53,
            "単騎無双",
            "単騎無双",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2485, 1494, 2220, 1497),
            18,
            new Skill(
                "ヒールストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            52,
            "ラプラスの発動",
            "ラプラスの発動",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2059, 1396, 1989, 1418),
            17,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            51,
            "顕現する脅威",
            "顕現する脅威",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2108, 1480, 2093, 1457),
            17,
            new Skill(
                "ガードストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のDEFをアップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            50,
            "スーパーかわいいジャンプ",
            "スーパーかわいいジャンプ！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2073, 2128, 1487, 1484),
            17,
            new Skill(
                "WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            49,
            "きみとぼくの創作世界",
            "きみとぼくの創作世界",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1382, 2057, 1413, 1977),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            48,
            "グリーンライフ",
            "グリーンライフ",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1406, 1388, 1657, 2400),
            17,
            new Skill(
                "Sp.ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のSp.DEFを小アップする。",
                [],
                [new StatusUp(new SpDef(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            47,
            "馳せたる海辺",
            "馳せたる海辺",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1568, 2994, 1570, 2524),
            19,
            new Skill(
                "スマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            46,
            "エスコートナイト",
            "エスコートナイト",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2027, 1414, 2006, 1401),
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            45,
            "おもちゃのプール",
            "おもちゃのプール",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1469, 2141, 1455, 2077),
            17,
            new Skill(
                "ヒールスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            44,
            "ランペイジクラフト",
            "ランペイジクラフト",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2467, 1477, 1708, 1454),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            43,
            "あなたと甘いひとときを",
            "あなたと甘いひとときを",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2133, 1483, 1476, 2066),
            17,
            new Skill(
                "Sp.パワーブレイクA Ⅲ+",
                "敵1体に通常大ダメージを与え、敵のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            42,
            "水の車窓",
            "水の車窓",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1409, 1980, 1394, 2059),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            41,
            "ラ・ヴァカンス・パルフェ",
            "ラ・ヴァカンス・パルフェ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1410, 1407, 1999, 2039),
            17,
            new Skill(
                "Sp.パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のSp.ATKを小アップする。",
                [],
                [new StatusUp(new SpAtk(), Amount.Small)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            40,
            "ウォーター・レイルウェイ",
            "ウォーター・レイルウェイ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2131, 1455, 2081, 1483),
            17,
            new Skill(
                "Sp.ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            39,
            "アナザーワールド",
            "アナザーワールド",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1392, 1977, 1386, 2059),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                [],
                [new StatusUp(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            38,
            "勝負の鍵は",
            "勝負の鍵は",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2473, 1491, 1712, 1481),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            37,
            "アンブレイカブル",
            "アンブレイカブル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2406, 1399, 1628, 1391),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            36,
            "ひとりはみんなのために",
            "ひとりはみんなのために",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1738, 1488, 2461, 1452),
            17,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            35,
            "文武両道の乙女",
            "文武両道の乙女",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2121, 1460, 2087, 1455),
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            34,
            "アーセナルの絆",
            "アーセナルの絆",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1469, 2471, 1452, 1740),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Two
            )
        ),
        new Memoria(
            33,
            "未来を切り開く武器",
            "未来を切り開く武器",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2993, 1570, 2488, 1549),
            19,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            32,
            "この地にて芽吹く",
            "この地にて芽吹く",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2010, 1385, 2028, 1408),
            17,
            new Skill(
                "ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。",
                [],
                [new StatusUp(new Def(), Amount.Medium)],
                Level.Four,
                Range.C
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            31,
            "おこづかいのゆくえ",
            "おこづかいのゆくえ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1386, 2052, 1401, 1998),
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Four,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            30,
            "カワイイのシャッターチャンス",
            "カワイイのシャッターチャンス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2013, 1416, 2063, 1387),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                [new MpCostDown()],
                Level.Two
            )
        ),
        new Memoria(
            29,
            "不滅のホワイトナイト",
            "不滅のホワイトナイト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1473, 2488, 1461, 1723),
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            28,
            "果断なる漆黒の騎士",
            "果断なる漆黒の騎士",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2468, 1485, 1723, 1470),
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            27,
            "不完全ゆえに愛おしく",
            "不完全ゆえに愛おしく",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2008, 1383, 2041, 1404),
            17,
            new Skill(
                "マイトアシストA Ⅲ",
                "味方1体のATKとDEFを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium), new StatusUp(new Def(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            26,
            "甘いスイーツでおもてなし♪",
            "甘いスイーツでおもてなし♪",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2404, 1402, 1627, 1409),
            17,
            new Skill(
                "ヒールストライクA Ⅲ",
                "敵1体に通常大ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            25,
            "見切れ希望女子",
            "見切れ希望女子",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2381, 1401, 1648, 1415),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            24,
            "ハッピーハッピー☆タピオカ",
            "ハッピーハッピー☆タピオカ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1388, 1661, 2563, 1202),
            17,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            23,
            "レンズに咲く百合の花",
            "レンズに咲く百合の花",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2063, 1413, 1996, 1384),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                [new DamageUp()],
                Level.Two
            )
        ),
        new Memoria(
            22,
            "不器用なお姉様",
            "不器用なお姉様",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1981, 1642, 1787, 1416),
            17,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.Two
            )
        ),
        new Memoria(
            21,
            "ワンショット",
            "ワンショット",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1415, 1389, 1979, 2037),
            17,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                [new RecoveryUp()],
                Level.Two
            )
        ),
        new Memoria(
            20,
            "ふたつのふれあい",
            "ふたつのふれあい",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2042, 2013, 1394, 1396),
            17,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium), new StatusDown(new SpAtk(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            19,
            "アイドルリリィをつかまえて",
            "アイドルリリィをつかまえて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2321, 1345, 1554, 1324),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            18,
            "うさぎになったカメ",
            "うさぎになったカメ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1323, 1554, 1677, 1970),
            17,
            new Skill(
                "Sp.ガードスマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            17,
            "迷子のクマ",
            "迷子のクマ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2321, 1311, 1556, 1314),
            17,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            16,
            "晴れのちラムネ",
            "晴れのちラムネ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1387, 2393, 1418, 1632),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            15,
            "湯けむりの園",
            "湯けむりの園",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1317, 1978, 1925, 1327),
            17,
            new Skill(
                "ヒールスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。さらに自身のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                [new PowerDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            14,
            "姫歌イメチェン大作戦",
            "姫歌イメチェン大作戦!!",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1316, 1346, 1907, 1976),
            17,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.D
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            13,
            "ガラスの中の大切な世界",
            "ガラスの中の大切な世界",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1346, 1334, 2321, 1548),
            17,
            new Skill(
                "ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。",
                [],
                [new StatusDown(new Def(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            12,
            "あたしがこの手で守るもの",
            "あたしがこの手で守るもの",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2300, 1560, 1324, 1317),
            17,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。",
                [],
                [new StatusUp(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            11,
            "charmにお疲れ様",
            "CHARMにお疲れ様",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1312, 2022, 1347, 1849),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            10,
            "一柳隊の知恵袋",
            "一柳隊の知恵袋",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(1930, 1348, 1965, 1318),
            17,
            new Skill(
                "Sp.ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            9,
            "ロイヤル・ホスピタリティ",
            "ロイヤル・ホスピタリティ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1968, 1345, 1914, 1311),
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。",
                [],
                [new StatusDown(new Atk(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。",
                Trigger.Support,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            8,
            "わたしにできること",
            "わたしにできること",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1345, 1558, 1907, 1748),
            17,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            7,
            "安らぎの帰り道",
            "安らぎの帰り道",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1312, 1343, 1925, 1945),
            17,
            new Skill(
                "ヒールA Ⅲ",
                "味方1体のHPを特大回復する。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            6,
            "猫の誘惑",
            "猫の誘惑",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1320, 2303, 1322, 1583),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                [new GuardUp(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            5,
            "星降る夜の約束",
            "星降る夜の約束",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1349, 1314, 1582, 2306),
            17,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。",
                [],
                [],
                Level.Three,
                Range.C
            ),
            new SupportSkill(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Recovery,
                [new GuardUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            4,
            "私たちの正義",
            "私たちの正義",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1346, 1914, 1324, 1969),
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:支援UP Ⅰ",
                "支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                [new SupportUp()],
                Level.One
            )
        ),
        new Memoria(
            3,
            "放課後ファンタズム",
            "放課後ファンタズム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(1970, 1338, 1904, 1337),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
        new Memoria(
            2,
            "雨上がりの朝稽古",
            "雨上がりの朝稽古",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1337, 1925, 1336, 1952),
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                [],
                [new StatusDown(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.B
            ),
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            1,
            "アフタヌーンティー",
            "アフタヌーンティー",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1335, 2297, 1335, 1575),
            17,
            new Skill(
                "Sp.ガードスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。",
                [],
                [new StatusUp(new SpDef(), Amount.Medium)],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                [new PowerUp(Type.Special)],
                Level.Two
            )
        ),
        new Memoria(
            0,
            "dearschutzengel",
            "Dear Schutzengel",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1954, 1313, 1898, 1345),
            17,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。",
                [],
                [],
                Level.Three,
                Range.A
            ),
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                [new GuardDown(Type.Normal)],
                Level.Two
            )
        ),
    };
}