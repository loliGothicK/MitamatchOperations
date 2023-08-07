using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace mitama.Domain;

public record struct Unit(string UnitName, bool IsFront, List<Memoria> Memorias)
{
    public string ToJson() =>
        JsonSerializer.Serialize(new UnitDto(UnitName, IsFront, Memorias.Select(m => m.Name).ToArray()));

    public static Unit FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<UnitDto>(json);
        var dummyCostume = dto.IsFront ? Costume.List[0] : Costume.List[1];
        var selector = Memoria.List.Where(dummyCostume.CanBeEquipped).ToDictionary(m => m.Name);
        return new Unit(dto.UnitName, dto.IsFront, dto.Names.Select(name => selector[name]).ToList());
    }
}

public record struct UnitDto(string UnitName, bool IsFront, string[] Names);

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
    string Name,
    MemoriaKind Kind,
    Element Element,
    Status Status,
    int Cost,
    Skill Skill,
    SupportSkill SupportSkill
)
{
    public Uri Uri => new($"ms-appx:///Assets/memoria/{Name}.jpg");
    public string Path = $"/Assets/memoria/{Name}.jpg";

    public static Memoria[] List =
    {
        new Memoria(
            "神の子は、水面に踊る",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2731, 4123, 2714, 3644),
            21,
            new Skill(
                "Sp.ガードバーストD LG",
                "敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "双刃無双",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4071, 4094, 7066, 7069),
            26,
            new Skill(
                "リカバーヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに自身のMPを60回復する。",
                new SkillEffect[]
                {
                    new Recover()
                },
                new StatusChange[]
                {
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ+",
                "HP回復時、中確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "双刃無双",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(7405, 4094, 6722, 4079),
            26,
            new Skill(
                "ウォーターパワーストライクA Ⅴ+",
                "敵1体に通常超特大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Five,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "無二なる二刀",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(7062, 7065, 4065, 4066),
            26,
            new Skill(
                "WパワーフォールD Ⅲ",
                "敵2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "無二なる二刀",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(7406, 4075, 6711, 4066),
            26,
            new Skill(
                "火：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "幻奏乙女",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(7063, 7078, 4088, 4097),
            26,
            new Skill(
                "WパワーアシストD Ⅲ",
                "味方2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "幻奏乙女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(4073, 7422, 4088, 6743),
            26,
            new Skill(
                "火：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "終曲のタクト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(4085, 7411, 4065, 6734),
            26,
            new Skill(
                "水：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "終曲のタクト",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4085, 4077, 7055, 7078),
            26,
            new Skill(
                "WガードアシストD Ⅲ",
                "味方2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ヴィルトシュバイン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(7428, 4098, 6721, 4097),
            26,
            new Skill(
                "水：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ヴィルトシュバイン",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4094, 4098, 7065, 7087),
            26,
            new Skill(
                "WガードフォールD Ⅲ",
                "敵2体のDEFとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "待ち望んだパーティナイト",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6232, 2214, 4874, 2220),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                new SkillEffect[]
                {
                    new ElementWeaken(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "待ち望んだパーティナイト",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6232, 2214, 4874, 2220),
            21,
            new Skill(
                "火強：ヒールC Ⅳ",
                "味方1～3体のHPを大回復する。さらに味方の火属性防御力が高いほどスキル効果が上昇する。",
                new SkillEffect[]
                {
                    new ElementStrengthen(Element.Fire)
                },
                new StatusChange[]
                {
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "焦がれる夜",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6229, 2231, 4896, 2245),
            21,
            new Skill(
                "火弱：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに敵の火属性攻撃力が低いほどスキル効果が上昇する。",
                new SkillEffect[]
                {
                    new ElementWeaken(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "焦がれる夜",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6229, 2231, 4896, 2245),
            21,
            new Skill(
                "水弱：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                new SkillEffect[]
                {
                    new ElementWeaken(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いたずらトゥインクル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2226, 6235, 2211, 4909),
            21,
            new Skill(
                "水弱：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに敵の水属性防御力が低いほど与えるダメージが上昇する。",
                new SkillEffect[]
                {
                    new ElementWeaken(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いたずらトゥインクル",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2226, 6235, 2211, 4909),
            21,
            new Skill(
                "水強：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方の水属性攻撃力が高いほどスキル効果が上昇する。",
                new SkillEffect[]
                {
                    new ElementStrengthen(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "はにかみプールサイド",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(5655, 5678, 2746, 2730),
            23,
            new Skill(
                "水：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "真夏のステージ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2240, 2232, 6225, 4901),
            21,
            new Skill(
                "WカウンターガードヒールC Ⅳ",
                "味方1～3体のHPを大回復し、DEFとSp.DEFを小アップする。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "真夏のステージ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2240, 2232, 6225, 4901),
            21,
            new Skill(
                "[風攻火防]マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵の風属性攻撃力と火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "浮き輪でぷかぷか",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2216, 6224, 2234, 4913),
            21,
            new Skill(
                "WカウンターガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "浮き輪でぷかぷか",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2216, 6224, 2234, 4913),
            21,
            new Skill(
                "[風攻火防]マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵の風属性攻撃力と火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "向日葵の咲く園",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2235, 6226, 2225, 4911),
            21,
            new Skill(
                "Sp.カウンターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "向日葵の咲く園",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2235, 6226, 2225, 4911),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "水着をお披露目",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6243, 2249, 4913, 2237),
            21,
            new Skill(
                "カウンターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "水着をお披露目",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6243, 2249, 4913, 2237),
            21,
            new Skill(
                "ファイアパワーアシストB Ⅲ",
                "味方1～2体のATKと火属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "楽しいを探して",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2218, 2247, 6228, 4876),
            21,
            new Skill(
                "WガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new ElementGuardUp(Element.Water)
                },
                Level.Three
            )
        ),

        new Memoria(
            "楽しいを探して",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2218, 2247, 6228, 4876),
            21,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "祝2.5周年 リリサマ!!",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(4500, 1656, 3469, 1666),
            19,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "祝2.5周年 リリサマ!!",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4500, 1656, 3469, 1666),
            19,
            new Skill(
                "WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "惹かれる手のひら",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2249, 2243, 4894, 6226),
            21,
            new Skill(
                "WカウンターガードヒールC Ⅳ",
                "味方1～3体のHPを大回復し、DEFとSp.DEFを小アップする。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "惹かれる手のひら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2249, 2243, 4894, 6226),
            21,
            new Skill(
                "Sp.ウォーターパワーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと水属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "王家の夏休み",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6238, 2227, 4892, 2216),
            21,
            new Skill(
                "WカウンターパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "王家の夏休み",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6238, 2227, 4892, 2216),
            21,
            new Skill(
                "ウォーターパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKと水属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アグレッシヴ・ヒロイン",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2722, 6061, 2722, 5279),
            23,
            new Skill(
                "Sp.カウンターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "夏色スライダー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6069, 2738, 5270, 2744),
            23,
            new Skill(
                "カウンターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに劣勢時は効果が1.5倍になる。",
                new SkillEffect[]
                {
                    new Counter()
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "星月夜の指揮者",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2245, 2219, 6227, 4885),
            21,
            new Skill(
                "ウィンドガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "星月夜の指揮者",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2245, 2219, 6227, 4885),
            21,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "月下に舞うプランセス",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2228, 6261, 2224, 4888),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "月下に舞うプランセス",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2228, 6261, 2224, 4888),
            21,
            new Skill(
                "[火攻風防]マイトアシストB Ⅲ",
                "味方1～2体の火属性攻撃力と風属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:火パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体の火属性攻撃力を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new ElementPowerUp(Element.Fire)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蒼き月の夜",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6241, 2241, 4876, 2236),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蒼き月の夜",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6241, 2241, 4876, 2236),
            21,
            new Skill(
                "ウィンドパワーフォールC Ⅳ",
                "敵1～3体のATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーDOWN/副援:風パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。さらに、風属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new ElementPowerDown(Element.Wind)
                },
                Level.Three
            )
        ),

        new Memoria(
            "非常事態のその後",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2243, 2223, 4905, 6230),
            21,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "非常事態のその後",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2243, 2223, 4905, 6230),
            21,
            new Skill(
                "Sp.ファイアパワーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと火属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.ATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "冷たい舌触り",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6228, 2212, 4910, 2236),
            21,
            new Skill(
                "ファイアパワーフォールC Ⅳ",
                "敵1～3体のATKと火属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "冷たい舌触り",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6228, 2212, 4910, 2236),
            21,
            new Skill(
                "ファイアパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKと火属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "灯莉の貝殻アート☆",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2235, 6223, 2229, 4884),
            21,
            new Skill(
                "水：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "灯莉の貝殻アート☆",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2235, 6223, 2229, 4884),
            21,
            new Skill(
                "[水攻火防]マイトアシストB Ⅲ",
                "味方1～2体の水属性攻撃力と火属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "碧い海のふたり",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6253, 2235, 4880, 2250),
            21,
            new Skill(
                "水：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "碧い海のふたり",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6253, 2235, 4880, 2250),
            21,
            new Skill(
                "ウォーターガードフォールB Ⅲ",
                "敵1～2体のDEFと水属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夢の果て、その先へ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2217, 6236, 2247, 4896),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "夢の果て、その先へ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2217, 6236, 2247, 4896),
            21,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅳ",
                "敵1～2体のSp.ATKと火属性攻撃力を特大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN/副援:火パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。さらに、火属性攻撃力を大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special),
                    new ElementPowerDown(Element.Fire)
                },
                Level.Three
            )
        ),

        new Memoria(
            "正義の咆哮",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6244, 2250, 4897, 2238),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "正義の咆哮",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6244, 2250, 4897, 2238),
            21,
            new Skill(
                "ファイアガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:火ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の火属性防御力を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new ElementGuardUp(Element.Fire)
                },
                Level.Three
            )
        ),

        new Memoria(
            "深炎のスキャルドメール",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5650, 5662, 2753, 2723),
            23,
            new Skill(
                "水：WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "七頭龍幻想の担い手",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2242, 6250, 2226, 4890),
            21,
            new Skill(
                "Sp.ファイアガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "七頭龍幻想の担い手",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2242, 6250, 2226, 4890),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "竜のシャナ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6260, 2221, 4890, 2220),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "竜のシャナ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6260, 2221, 4890, 2220),
            21,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゴージャス☆おしゃ恋花",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2217, 6251, 2240, 4880),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゴージャス☆おしゃ恋花",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2217, 6251, 2240, 4880),
            21,
            new Skill(
                "[火攻風防]マイトアシストB Ⅲ",
                "味方1～2体の火属性攻撃力と風属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "枕投げチャンピオン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6231, 2230, 4906, 2224),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "枕投げチャンピオン",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6231, 2230, 4906, 2224),
            21,
            new Skill(
                "ウィンドパワーフォールB Ⅲ",
                "敵1～2体のATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "幸せな夢を見る前に",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2251, 2246, 6249, 4898),
            21,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "幸せな夢を見る前に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2251, 2246, 6249, 4898),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "心を鋼鉄に変えて",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2248, 6260, 2233, 4880),
            21,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "心を鋼鉄に変えて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2248, 6260, 2233, 4880),
            21,
            new Skill(
                "[火防]Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKと火属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "受け継がれし攻守の型",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(2250, 6246, 2249, 4876),
            21,
            new Skill(
                "水：Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "受け継がれし攻守の型",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2250, 6246, 2249, 4876),
            21,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "煉獄の守護天使",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2236, 6257, 2232, 4873),
            21,
            new Skill(
                "[水攻火防]マイトアシストB Ⅲ",
                "味方1～2体の水属性攻撃力と火属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "煉獄の守護天使",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2236, 6257, 2232, 4873),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "祈りの声が届く時",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6247, 2226, 4907, 2242),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "祈りの声が届く時",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(6247, 2226, 4907, 2242),
            21,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "これなんかどう？",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2217, 2246, 4905, 6239),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:水ガードUP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で味方前衛1体の水属性防御力を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new ElementGuardUp(Element.Water)
                },
                Level.Three
            )
        ),

        new Memoria(
            "これなんかどう？",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2217, 2246, 4905, 6239),
            21,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "日差しを見上げて",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6237, 2212, 4899, 2237),
            21,
            new Skill(
                "ウィンドガードフォールB Ⅲ",
                "敵1～2体のDEFと風属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "日差しを見上げて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6237, 2212, 4899, 2237),
            21,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "初夏の装い",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6258, 2248, 4882, 2234),
            21,
            new Skill(
                "風：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "初夏の装い",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(6258, 2248, 4882, 2234),
            21,
            new Skill(
                "ウィンドパワーアシストB Ⅲ",
                "味方1～2体のATKと風属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "水族館を探検",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2239, 6257, 2222, 4886),
            21,
            new Skill(
                "風：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "水族館を探検",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2239, 6257, 2222, 4886),
            21,
            new Skill(
                "Sp.ウィンドガードフォールB Ⅲ",
                "敵1～2体のSp.DEFと風属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "桜花爛漫",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2219, 6251, 2237, 4873),
            21,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "桜花爛漫",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2219, 6251, 2237, 4873),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "心の痛みを判る人",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6247, 2225, 4883, 2222),
            21,
            new Skill(
                "[風防]マイトアシストB Ⅲ",
                "味方1～2体のATKと風属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "心の痛みを判る人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6247, 2225, 4883, 2222),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "輝く心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6257, 2243, 4886, 2229),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "輝く心",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6257, 2243, 4886, 2229),
            21,
            new Skill(
                "ファイアガードフォールB Ⅲ",
                "敵1～2体のDEFと火属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "勇気の拳",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2216, 6258, 2225, 4906),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "勇気の拳",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2216, 6258, 2225, 4906),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "伝わる鼓動",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2237, 6250, 2217, 4876),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "伝わる鼓動",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2237, 6250, 2217, 4876),
            21,
            new Skill(
                "水：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "二度寝のいいわけ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6235, 2222, 4889, 2238),
            21,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "二度寝のいいわけ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6235, 2222, 4889, 2238),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雨、舌戦のあと",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2239, 2211, 4886, 6226),
            21,
            new Skill(
                "水：Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFと火属性防御力を小アップする。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "雨、舌戦のあと",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2239, 2211, 4886, 6226),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雨の日は紅茶を",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6258, 2234, 4901, 2216),
            21,
            new Skill(
                "水：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雨の日は紅茶を",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(6258, 2234, 4901, 2216),
            21,
            new Skill(
                "ファイアガードアシストB Ⅲ",
                "味方1～2体のDEFと火属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "紫陽花の咲く頃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2218, 6240, 2224, 4879),
            21,
            new Skill(
                "水：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "紫陽花の咲く頃",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2218, 6240, 2224, 4879),
            21,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅲ",
                "敵1～2体のSp.DEFと水属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "清純な心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6224, 2240, 4909, 2217),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "清純な心",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(6224, 2240, 4909, 2217),
            21,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "そよ風のシュッツエンゲル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2227, 6259, 2225, 4898),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "そよ風のシュッツエンゲル",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2227, 6259, 2225, 4898),
            21,
            new Skill(
                "Sp.ファイアパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと火属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "神の子",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2722, 2698, 4139, 3632),
            18,
            new Skill(
                "ヒールE LG",
                "味方2～3体のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Lg,
                Range.E
            )
            ,
            new SupportSkill(
                "回:回復UP/ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。さらに、HPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "交差する勇み花",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6255, 2248, 4901, 2222),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "交差する勇み花",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(6255, 2248, 4901, 2222),
            21,
            new Skill(
                "ファイアパワーアシストB Ⅲ",
                "味方1～2体のATKと火属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "情熱",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2245, 6238, 2225, 4910),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "情熱",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2245, 6238, 2225, 4910),
            21,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "愛情の絆",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2227, 2232, 4909, 6256),
            21,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "愛情の絆",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2227, 2232, 4909, 6256),
            21,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "貴方に微笑む",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6224, 2243, 4905, 2213),
            21,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "貴方に微笑む",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(6224, 2243, 4905, 2213),
            21,
            new Skill(
                "ファイアガードフォールB Ⅲ",
                "敵1～2体のDEFと火属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "つきしーMAX!!",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2727, 2719, 3873, 3899),
            21,
            new Skill(
                "WガードヒールE LG",
                "味方2～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Lg,
                Range.E
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "G戦場の百合亜",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(6248, 2218, 4905, 2236),
            21,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "G戦場の百合亜",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6248, 2218, 4905, 2236),
            21,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "焼け焦げた土を踏んで",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2220, 6233, 2222, 4913),
            21,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "焼け焦げた土を踏んで",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2220, 6233, 2222, 4913),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "黒蝕の夢",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2249, 6226, 2250, 4886),
            21,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "黒蝕の夢",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2249, 6226, 2250, 4886),
            21,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "月光奏鳴",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6225, 2244, 4904, 2231),
            21,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "月光奏鳴",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6225, 2244, 4904, 2231),
            21,
            new Skill(
                "ウィンドガードフォールB Ⅲ",
                "敵1～2体のDEFと風属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "式場を決めましたわ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2212, 2237, 6241, 4910),
            21,
            new Skill(
                "火：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "式場を決めましたわ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2212, 2237, 6241, 4910),
            21,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "想像ウェディング",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2227, 2251, 4878, 6253),
            21,
            new Skill(
                "火：Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFと風属性防御力を小アップする。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "想像ウェディング",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2227, 2251, 4878, 6253),
            21,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ウェディングベア",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2246, 6244, 2224, 4899),
            21,
            new Skill(
                "火：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ウェディングベア",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2246, 6244, 2224, 4899),
            21,
            new Skill(
                "Sp.ファイアガードフォールB Ⅲ",
                "敵1～2体のSp.DEFと火属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "門出のブーケ・トス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6248, 2211, 4884, 2251),
            21,
            new Skill(
                "火：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "門出のブーケ・トス",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(6248, 2211, 4884, 2251),
            21,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夢見る自分を、怖れずに",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2224, 6243, 2228, 4894),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夢見る自分を、怖れずに",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2224, 6243, 2228, 4894),
            21,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ウエディング・マーチ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(6262, 2233, 4903, 2231),
            21,
            new Skill(
                "ファイアパワーフォールB Ⅲ",
                "敵1～2体のATKと火属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ウエディング・マーチ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6262, 2233, 4903, 2231),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "エターナル・プロミス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(6062, 2748, 5280, 2752),
            23,
            new Skill(
                "水拡：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体に通常大ダメージを与え、敵のDEFをダウンさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ピクニック日和",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2049, 5285, 2012, 4178),
            20,
            new Skill(
                "Sp.ウォーターパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと水属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ピクニック日和",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2049, 5285, 2012, 4178),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "なでなで連鎖",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2039, 2039, 5259, 4174),
            20,
            new Skill(
                "風：ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、DEFと水属性防御力を小アップする。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "なでなで連鎖",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2039, 2039, 5259, 4174),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "尊さの不意打ち",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2038, 5280, 2037, 4157),
            20,
            new Skill(
                "風：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "尊さの不意打ち",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2038, 5280, 2037, 4157),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "皐月の頃に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5266, 2032, 4178, 2031),
            20,
            new Skill(
                "風：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "皐月の頃に",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5266, 2032, 4178, 2031),
            20,
            new Skill(
                "ウィンドガードフォールB Ⅲ",
                "敵1～2体のDEFと風属性防御力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "言葉無く吠える",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5266, 2046, 4152, 2032),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "言葉無く吠える",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5266, 2046, 4152, 2032),
            20,
            new Skill(
                "ファイアパワーフォールB Ⅲ",
                "敵1～2体のATKと火属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦乙女の誇り",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2040, 5253, 2025, 4182),
            20,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦乙女の誇り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2040, 5253, 2025, 4182),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "竜のシャナと楯の乙女",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2018, 2046, 5254, 4167),
            20,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "竜のシャナと楯の乙女",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2018, 2046, 5254, 4167),
            20,
            new Skill(
                "ウォーターパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "パーフェクトエイム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5258, 2038, 4149, 2015),
            20,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "パーフェクトエイム",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(5258, 2038, 4149, 2015),
            20,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "征くと決めたこの道を",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2041, 5274, 2036, 4162),
            20,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "征くと決めたこの道を",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2041, 5274, 2036, 4162),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "藍だけが使える魔法",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2404, 5451, 2404, 4679),
            22,
            new Skill(
                "水拡：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "エクセレントアイドル☆紗癒",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(5259, 2014, 4153, 2046),
            20,
            new Skill(
                "ウォーターパワーアシストB Ⅲ",
                "味方1～2体のATKと水属性攻撃力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "エクセレントアイドル☆紗癒",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5259, 2014, 4153, 2046),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゴージャスアイドル☆楓",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2043, 5285, 2016, 4149),
            20,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゴージャスアイドル☆楓",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2043, 5285, 2016, 4149),
            20,
            new Skill(
                "水拡：Sp.ファイアパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと火属性攻撃力を大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は敵2体のSp.ATKと火属性攻撃力を大ダウンさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "荒ぶる魂",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1934, 5149, 1950, 4044),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "荒ぶる魂",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1934, 5149, 1950, 4044),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "閑かなること、幻想の如く",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5254, 2019, 4186, 2035),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "閑かなること、幻想の如く",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5254, 2019, 4186, 2035),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅲ",
                "敵1～2体のATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "猛禽の視点",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2043, 5266, 2015, 4158),
            20,
            new Skill(
                "[風防]Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKと風属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "猛禽の視点",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2043, 5266, 2015, 4158),
            20,
            new Skill(
                "Sp.ディファーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦場に差しこむ光",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2036, 2040, 5265, 4187),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦場に差しこむ光",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2036, 2040, 5265, 4187),
            20,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ウィステリアの誘い",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2043, 5269, 2040, 4184),
            20,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅲ",
                "敵1～2体のSp.ATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ウィステリアの誘い",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2043, 5269, 2040, 4184),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "花言葉のように",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5266, 2026, 4149, 2045),
            20,
            new Skill(
                "[風防]マイトアシストB Ⅲ",
                "味方1～2体のATKと風属性防御力を大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "花言葉のように",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5266, 2026, 4149, 2045),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "藤棚の下で",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2041, 5248, 2050, 4183),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "藤棚の下で",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2041, 5248, 2050, 4183),
            20,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "紫に酔い、白に想う",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5275, 2038, 4170, 2017),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "紫に酔い、白に想う",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5275, 2038, 4170, 2017),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅲ",
                "敵1～2体のATKと風属性攻撃力を大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "舞台「The Gleam of Dawn」開演！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1583, 3441, 1599, 2936),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "舞台「The Gleam of Dawn」開演！",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1583, 3441, 1599, 2936),
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "猪突猛進！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(4152, 2719, 3649, 2734),
            18,
            new Skill(
                "ストライクD LG",
                "敵2体に通常大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "さみしがりうさぎ",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5260, 2021, 4172, 2038),
            20,
            new Skill(
                "ウィンドパワーフォールC Ⅲ",
                "敵1～3体のATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "さみしがりうさぎ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5260, 2021, 4172, 2038),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "バニートラップ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2044, 5263, 2026, 4185),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "バニートラップ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2044, 5263, 2026, 4185),
            20,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.マイトUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKとSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special),
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "魅惑のセレクション",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2415, 2384, 5050, 5074),
            22,
            new Skill(
                "WガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "イースターハント",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2037, 5253, 2040, 4165),
            20,
            new Skill(
                "火：Sp.ファイアパワーフォールC Ⅲ",
                "敵1～3体のSp.ATKと火属性攻撃力をダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "イースターハント",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2037, 5253, 2040, 4165),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "花咲くイースター",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2017, 2039, 5268, 4171),
            20,
            new Skill(
                "火拡：WガードアシストB Ⅳ",
                "味方1～2体のDEFとSp.DEFを特大アップさせる。オーダースキル「火属性効果増加」を発動中は味方2体のDEFとSp.DEFを特大アップさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "花咲くイースター",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2017, 2039, 5268, 4171),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "イースターエッグ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2040, 5263, 2024, 4173),
            20,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "イースターエッグ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2040, 5263, 2024, 4173),
            20,
            new Skill(
                "Sp.ファイアガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "エッグロール開始！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5286, 2047, 4186, 2018),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "エッグロール開始！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5286, 2047, 4186, 2018),
            20,
            new Skill(
                "ファイアパワーアシストC Ⅲ",
                "味方1～3体のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "天のアカリ目！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2023, 5282, 2028, 4167),
            20,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "天のアカリ目！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2023, 5282, 2028, 4167),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "これが、あたしの理！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5260, 2027, 4153, 2018),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "これが、あたしの理！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5260, 2027, 4153, 2018),
            20,
            new Skill(
                "ファイアパワーフォールC Ⅲ",
                "敵1～3体のATKと火属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "マルチカラード・ティアーズ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2721, 2736, 5111, 4320),
            22,
            new Skill(
                "ファイアガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "尊き花を守るために",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5273, 2029, 4153, 2040),
            20,
            new Skill(
                "ファイアパワーアシストC Ⅲ",
                "味方1～3体のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "尊き花を守るために",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5273, 2029, 4153, 2040),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "笑顔の夜明け",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2046, 5286, 2027, 4185),
            20,
            new Skill(
                "Sp.ファイアパワーフォールC Ⅲ",
                "敵1～3体のSp.ATKと火属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "笑顔の夜明け",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(2046, 5286, 2027, 4185),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "天使の左手、堕天使の右手",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2041, 5269, 2014, 4176),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "天使の左手、堕天使の右手",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2041, 5269, 2014, 4176),
            20,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "朝寝坊のススメ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5253, 2013, 4171, 2052),
            20,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "朝寝坊のススメ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(5253, 2013, 4171, 2052),
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "千香瑠のエクササイズ",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2741, 2710, 4299, 5106),
            22,
            new Skill(
                "風：Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復し、Sp.DEFと風属性防御力を小アップする。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "玲瓏玉の如し",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5096, 2745, 4302, 2736),
            22,
            new Skill(
                "風：ウィンドパワーフォールC Ⅲ",
                "敵1～3体のATKと風属性攻撃力をダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ぱーふぇくとアカリズム",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2730, 2739, 4693, 4699),
            22,
            new Skill(
                "風：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "グラスにラムネを注いだら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2718, 5111, 2708, 4316),
            22,
            new Skill(
                "風拡：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。オーダースキル「風属性効果増加」を発動中は敵2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Wind)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ツインテじゃらし",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5091, 2722, 4318, 2729),
            22,
            new Skill(
                "風拡：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。オーダースキル「風属性効果増加」を発動中は敵2体に通常大ダメージを与え、自身のATKをアップさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Wind)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "反りし刃、誘うは棺",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(5151, 1954, 4070, 1934),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "反りし刃、誘うは棺",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5151, 1954, 4070, 1934),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蒼き輝き、楯たる矜持",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2046, 6060, 2015, 3370),
            20,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蒼き輝き、楯たる矜持",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2046, 6060, 2015, 3370),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "その心、炎よりも熱く",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6055, 2048, 3353, 2015),
            20,
            new Skill(
                "ウィンドパワーフォールC Ⅲ",
                "敵1～3体のATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "その心、炎よりも熱く",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6055, 2048, 3353, 2015),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "贖いの祈り",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2050, 2044, 4169, 5251),
            20,
            new Skill(
                "Sp.ガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "贖いの祈り",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(2050, 2044, 4169, 5251),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "桜と貴女を",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2732, 2711, 5097, 4321),
            22,
            new Skill(
                "ガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "GOGO新学期！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2728, 5121, 2748, 4309),
            22,
            new Skill(
                "火：Sp.ファイアパワーフォールC Ⅲ",
                "敵1～3体のSp.ATKと火属性攻撃力をダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "天に舞う花びら",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2717, 2711, 4684, 4692),
            22,
            new Skill(
                "火：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "お花見ティータイム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2714, 5103, 2731, 4317),
            22,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "春風に吹かれて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5098, 2720, 4283, 2744),
            22,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "堅固なる守り",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2736, 2730, 3892, 3874),
            21,
            new Skill(
                "WガードアシストE LG",
                "味方2～3体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Lg,
                Range.E
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "光咲く日々を抱いて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5155, 1936, 4059, 1925),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "光咲く日々を抱いて",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5155, 1936, 4059, 1925),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "泡立てチャレンジの結果",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1926, 5157, 1926, 4075),
            20,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "泡立てチャレンジの結果",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1926, 5157, 1926, 4075),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ぎゅーっとしてあげる",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4869, 4866, 2292, 2294),
            22,
            new Skill(
                "風：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "シルト餌付け実験",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2613, 2587, 4935, 4143),
            22,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "苺飴の味わい",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2596, 4969, 2592, 4146),
            22,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "甘々苺クレープ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4949, 2609, 4134, 2604),
            22,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "籠いっぱいの幸せ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2611, 4943, 2611, 4138),
            22,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "苺色に染めて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4936, 2580, 4168, 2604),
            22,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "差し出されたお菓子",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1950, 5164, 1927, 4079),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "差し出されたお菓子",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1950, 5164, 1927, 4079),
            20,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "共同戦線！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5146, 1948, 4069, 1927),
            20,
            new Skill(
                "ファイアガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "共同戦線！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5146, 1948, 4069, 1927),
            20,
            new Skill(
                "ファイアパワーフォールB Ⅱ",
                "敵1～2体のATKと火属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "僕と契約して、魔法少女になってよ！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3456, 1569, 2950, 1580),
            18,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "僕と契約して、魔法少女になってよ！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3456, 1569, 2950, 1580),
            18,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "奇跡の出会い！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1928, 1939, 4073, 5153),
            20,
            new Skill(
                "Sp.ファイアガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "奇跡の出会い！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1928, 1939, 4073, 5153),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "頼れる先輩",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5180, 1955, 4066, 1945),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "頼れる先輩",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5180, 1955, 4066, 1945),
            20,
            new Skill(
                "ファイアパワーアシストC Ⅲ",
                "味方1～3体のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "謎めいた魔法少女",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1926, 5153, 1956, 4063),
            20,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "謎めいた魔法少女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1926, 5153, 1956, 4063),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "連携プレーの勝利！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1941, 5179, 1944, 4067),
            20,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "連携プレーの勝利！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1941, 5179, 1944, 4067),
            20,
            new Skill(
                "Sp.ファイアパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと火属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "氷嵐を断つ劔",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1959, 5163, 1958, 4043),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "氷嵐を断つ劔",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1959, 5163, 1958, 4043),
            20,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "一意専心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5165, 1954, 4061, 1920),
            20,
            new Skill(
                "ファイアガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "一意専心",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5165, 1954, 4061, 1920),
            20,
            new Skill(
                "ファイアガードフォールB Ⅱ",
                "敵1～2体のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "六花、胡蝶の如く舞う",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1944, 1935, 5174, 4071),
            20,
            new Skill(
                "ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "六花、胡蝶の如く舞う",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(1944, 1935, 5174, 4071),
            20,
            new Skill(
                "ファイアパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "輝ける流星",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1926, 5175, 1952, 4056),
            20,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "輝ける流星",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1926, 5175, 1952, 4056),
            20,
            new Skill(
                "Sp.ガードライフアシストD Ⅱ",
                "味方2体のSp.DEFと最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Two,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "とろけるハート",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2585, 2597, 4168, 4938),
            22,
            new Skill(
                "Sp.ファイアガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと火属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "甘いきらめき",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2610, 4958, 2594, 4148),
            22,
            new Skill(
                "Sp.ファイアパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ショコラのゆうわく",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2616, 4941, 2596, 4140),
            22,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "いただきだゾ♪",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(4968, 2616, 4153, 2586),
            22,
            new Skill(
                "ファイアパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "あなたにお茶を",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5154, 1921, 4066, 1946),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "あなたにお茶を",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5154, 1921, 4066, 1946),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "安らぎをあなたに",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1948, 5174, 1931, 4053),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "安らぎをあなたに",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1948, 5174, 1931, 4053),
            20,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "触れ合う吐息",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1928, 5168, 1946, 4040),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "触れ合う吐息",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1928, 5168, 1946, 4040),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "チョコを知らない君へ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(5164, 1934, 4050, 1931),
            20,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "チョコを知らない君へ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5164, 1934, 4050, 1931),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "運命のトリニティ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1927, 5157, 1933, 4046),
            20,
            new Skill(
                "Sp.ファイアパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "運命のトリニティ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1927, 5157, 1933, 4046),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "貴女と共にあるために",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(5141, 1949, 4076, 1926),
            20,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "貴女と共にあるために",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5141, 1949, 4076, 1926),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "貴女の笑顔を守るために",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(4547, 4567, 2593, 2604),
            22,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WパワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKとSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "光の盾",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1952, 5177, 1948, 4051),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "光の盾",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1952, 5177, 1948, 4051),
            20,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ハルナストライク！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5174, 1935, 4054, 1957),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ハルナストライク！！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(5174, 1935, 4054, 1957),
            20,
            new Skill(
                "ファイアガードフォールB Ⅱ",
                "敵1～2体のDEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "吐息の距離",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1921, 5172, 1952, 4042),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "吐息の距離",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1921, 5172, 1952, 4042),
            20,
            new Skill(
                "Sp.ファイアパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "縦横無尽、阻む者無し",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3505, 3487, 6312, 6305),
            24,
            new Skill(
                "リカバーヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに自身のMPを60回復する。",
                new SkillEffect[]
                {
                    new Recover()
                },
                new StatusChange[]
                {
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ+",
                "HP回復時、中確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "縦横無尽、阻む者無し",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(3505, 6702, 3512, 5890),
            24,
            new Skill(
                "Sp.マイトスマッシュA Ⅴ+",
                "敵1体に特殊超特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Five,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "シリウス・ロア",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(6289, 6290, 3491, 3520),
            24,
            new Skill(
                "WパワーアシストD Ⅲ",
                "味方2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "シリウス・ロア",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(6704, 3490, 5876, 3520),
            24,
            new Skill(
                "風：ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "蒼き月、満ちる時",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(6318, 6283, 3519, 3493),
            24,
            new Skill(
                "WパワーフォールD Ⅲ",
                "敵2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "蒼き月、満ちる時",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(3518, 6698, 3519, 5878),
            24,
            new Skill(
                "風：Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "フェノメノ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(3482, 6728, 3511, 5885),
            24,
            new Skill(
                "火：Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "フェノメノ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3482, 3513, 6311, 6300),
            24,
            new Skill(
                "WガードフォールD Ⅲ",
                "敵2体のDEFとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "比類なき異能",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(6696, 3506, 5883, 3517),
            24,
            new Skill(
                "火：パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ+",
                "攻撃時、中確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "比類なき異能",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3481, 3506, 6298, 6317),
            24,
            new Skill(
                "WガードアシストD Ⅲ",
                "味方2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ+",
                "支援/妨害時、中確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "ありのままのわたしで",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1932, 5151, 1936, 4067),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと火属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ありのままのわたしで",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1932, 5151, 1936, 4067),
            20,
            new Skill(
                "Sp.ファイアガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと火属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いつも隣に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5168, 1920, 4053, 1923),
            20,
            new Skill(
                "ファイアパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと火属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いつも隣に",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5168, 1920, 4053, 1923),
            20,
            new Skill(
                "ファイアパワーアシストB Ⅱ",
                "味方1～2体のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蝶の夢",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1955, 1943, 5168, 4045),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蝶の夢",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1955, 1943, 5168, 4045),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "湯けむりの向こう側",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1954, 1952, 4603, 4608),
            20,
            new Skill(
                "WガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "湯けむりの向こう側",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1954, 1952, 4603, 4608),
            20,
            new Skill(
                "Sp.ファイアパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "宵に舞う華",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5145, 1955, 4054, 1953),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "宵に舞う華",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5145, 1955, 4054, 1953),
            20,
            new Skill(
                "ウィンドガードフォールB Ⅱ",
                "敵1～2体のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "好いも甘いも受け止めて",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1946, 5167, 1926, 4043),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "好いも甘いも受け止めて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1946, 5167, 1926, 4043),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "2周年祭り 絆の彩り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "2周年祭り 絆の彩り",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ふたりのヒメゴト",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2595, 2619, 4536, 4561),
            22,
            new Skill(
                "WガードヒールD Ⅳ",
                "味方2体のHPを大回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅳ",
                "HP回復時、一定確率でHPの回復量を超特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "変わらない絆",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(4566, 4552, 2592, 2605),
            22,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅳ",
                "支援/妨害時、一定確率で支援/妨害効果を超特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "Cherishing",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5544, 2277, 4220, 2317),
            22,
            new Skill(
                "ファイアガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと火属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "止めどない熱",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(2290, 5539, 2294, 4230),
            22,
            new Skill(
                "Sp.ファイアガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと火属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Fire), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅳ",
                "攻撃時、一定確率で攻撃ダメージを超特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Four
            )
        ),

        new Memoria(
            "戦場のコンダクター",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(2736, 4156, 2736, 3627),
            18,
            new Skill(
                "Sp.パワースマッシュA LG",
                "敵1体に特殊超特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Lg,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "引導を渡して差し上げますわ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(5179, 1942, 4070, 1940),
            20,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "引導を渡して差し上げますわ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5179, 1942, 4070, 1940),
            20,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "飢えし群れ、挑む狩人",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1938, 5172, 1956, 4076),
            20,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "飢えし群れ、挑む狩人",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1938, 5172, 1956, 4076),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "うりゃうりゃうりゃうりゃ！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(5177, 1957, 4040, 1950),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "うりゃうりゃうりゃうりゃ！！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(5177, 1957, 4040, 1950),
            20,
            new Skill(
                "ファイアパワーアシストB Ⅱ",
                "味方1～2体のATKと火属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Fire), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "この手に劔がある限り",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2606, 2606, 4542, 4557),
            22,
            new Skill(
                "WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "今年もよろしくね",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2605, 2611, 4556, 4540),
            22,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "謹賀新年です！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2597, 4951, 2609, 4150),
            22,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "あけおめですっ♪",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4549, 4565, 2583, 2599),
            22,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "新年を祝すわ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2607, 4936, 2592, 4165),
            22,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ことよろなのじゃ！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4937, 2613, 4137, 2598),
            22,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "賀正！！",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1930, 1946, 5141, 4043),
            20,
            new Skill(
                "ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "賀正！！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1930, 1946, 5141, 4043),
            20,
            new Skill(
                "ウィンドガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "思い出を抱きしめて",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1949, 5166, 1944, 4054),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "思い出を抱きしめて",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1949, 5166, 1944, 4054),
            20,
            new Skill(
                "Sp.ウィンドガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "突きて返すは兎姉妹",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1944, 5144, 1944, 4072),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "突きて返すは兎姉妹",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1944, 5144, 1944, 4072),
            20,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雪兎に会えた日",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4942, 2581, 4136, 2613),
            22,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "大丈夫、みんながいるから",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4484, 3498, 1680, 1680),
            19,
            new Skill(
                "水：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "大丈夫、みんながいるから",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4484, 3498, 1680, 1680),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "高らかと響き渡る歌声の中で",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1681, 4503, 1644, 3496),
            19,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "高らかと響き渡る歌声の中で",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1681, 4503, 1644, 3496),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "スノーフレイク",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1654, 5142, 1645, 2819),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "スノーフレイク",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1654, 5142, 1645, 2819),
            19,
            new Skill(
                "Sp.ウィンドパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "胸躍る聖夜",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4495, 3492, 1668, 1658),
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "胸躍る聖夜",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4495, 3492, 1668, 1658),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ここから先へ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1938, 4225, 1926, 3210),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.ATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ここから先へ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1938, 4225, 1926, 3210),
            19,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "無邪気な親近感",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(4476, 1672, 3466, 1677),
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "無邪気な親近感",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(4476, 1672, 3466, 1677),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "かがみもち、できました！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5171, 1957, 4044, 1921),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "かがみもち、できました！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(5171, 1957, 4044, 1921),
            20,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "兎であけおめですわ！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5154, 1929, 4070, 1924),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅱ",
                "敵1～2体のATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "兎であけおめですわ！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(5154, 1929, 4070, 1924),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "新年、はっじまっるよ～♪",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1926, 1927, 4078, 5163),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "新年、はっじまっるよ～♪",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1926, 1927, 4078, 5163),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "初春の宴に貴女を想う",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2610, 4949, 2582, 4141),
            22,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "約束された勝利の剣",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1944, 5168, 1937, 4079),
            20,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "約束された勝利の剣",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1944, 5168, 1937, 4079),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "射殺す百頭",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5171, 1941, 4068, 1956),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "射殺す百頭",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5171, 1941, 4068, 1956),
            20,
            new Skill(
                "ウィンドパワーフォールB Ⅱ",
                "敵1～2体のATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "投影魔術",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(5158, 1934, 4041, 1953),
            20,
            new Skill(
                "ウィンドパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "投影魔術",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(5158, 1934, 4041, 1953),
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "願いの魔法少女",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1949, 5140, 1958, 4075),
            20,
            new Skill(
                "Sp.ウィンドパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと風属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "願いの魔法少女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1949, 5140, 1958, 4075),
            20,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "空想魔法少女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5167, 1946, 4062, 1925),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "空想魔法少女",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(5167, 1946, 4062, 1925),
            20,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "膝の子猫と窓の雪",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(2591, 2588, 4155, 4957),
            22,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "かずはをよしよし",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4963, 2594, 4145, 2616),
            22,
            new Skill(
                "ウォーターパワーフォールC Ⅲ",
                "敵1～3体のATKと水属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゆー姉と一緒！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2614, 4935, 2582, 4158),
            22,
            new Skill(
                "風拡：Sp.パワーアシストB Ⅳ",
                "味方1～2体のSp.ATKを特大アップさせる。オーダースキル「風属性効果増加」を発動中は味方2体のSp.ATKを特大アップさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Wind)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "あつあつの肉まん",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(2604, 4936, 2607, 4148),
            22,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "どんがらがっしゃん",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4936, 2606, 4151, 2602),
            22,
            new Skill(
                "ウォーターガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "懐かしくて、優しい味",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1948, 5155, 1932, 4065),
            20,
            new Skill(
                "Sp.ウィンドパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "懐かしくて、優しい味",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1948, 5155, 1932, 4065),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "聞こえし者",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(4601, 2722, 3178, 2737),
            21,
            new Skill(
                "パワーストライクA LG+",
                "敵1体に通常超特大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Lg,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ふーみんにインタビュー",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1953, 1922, 4056, 5163),
            20,
            new Skill(
                "Sp.ウィンドガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと風属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ふーみんにインタビュー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1953, 1922, 4056, 5163),
            20,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "歴戦の余裕",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1933, 5177, 1939, 4068),
            20,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "歴戦の余裕",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1933, 5177, 1939, 4068),
            20,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "雪原に火花散る",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5162, 1951, 4050, 1936),
            20,
            new Skill(
                "ウォーターガードフォールB Ⅱ",
                "敵1～2体のDEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雪原に火花散る",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5162, 1951, 4050, 1936),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "縮地、友の元へ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1930, 5168, 1920, 4073),
            20,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "縮地、友の元へ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1930, 5168, 1920, 4073),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/特殊単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new SpecialMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "旋律に身を委ねて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1928, 5154, 1955, 4061),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "旋律に身を委ねて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1928, 5154, 1955, 4061),
            20,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "神琳！？　これは違うの！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4628, 4625, 1958, 1926),
            20,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WパワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal),
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "神琳！？　これは違うの！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4628, 4625, 1958, 1926),
            20,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "サンタをつかまえて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1931, 1954, 4056, 5159),
            20,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "サンタをつかまえて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1931, 1954, 4056, 5159),
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "聖夜のテラリウム",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5152, 1935, 4045, 1926),
            20,
            new Skill(
                "ウィンドガードフォールC Ⅲ",
                "敵1～3体のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "聖夜のテラリウム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5152, 1935, 4045, 1926),
            20,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "聖夜に乾杯",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1933, 5168, 1948, 4077),
            20,
            new Skill(
                "Sp.ウォーターパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "聖夜に乾杯",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1933, 5168, 1948, 4077),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "手作りクリスマス",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1945, 5157, 1925, 4068),
            20,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "手作りクリスマス",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1945, 5157, 1925, 4068),
            20,
            new Skill(
                "Sp.ウォーターパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと水属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひめひめ仕立て",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(5174, 1958, 4055, 1957),
            20,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひめひめ仕立て",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(5174, 1958, 4055, 1957),
            20,
            new Skill(
                "風：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "はっぴーらっきーとっきー",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(5169, 1941, 4042, 1948),
            20,
            new Skill(
                "ウォーターパワーフォールB Ⅱ",
                "敵1～2体のATKと水属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "はっぴーらっきーとっきー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(5169, 1941, 4042, 1948),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "サプライズゲーム",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4595, 1921, 4613, 1960),
            20,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "サプライズゲーム",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4595, 1921, 4613, 1960),
            20,
            new Skill(
                "ウォーターパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:獲得マッチPtUP/通常単体 Ⅱ",
                "前衛から攻撃時、一定確率で自身のマッチPtの獲得量がアップする。 ※...",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new NormalMatchPtUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "帯びる熱と急接近",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1944, 5143, 1934, 4078),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "帯びる熱と急接近",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1944, 5143, 1934, 4078),
            20,
            new Skill(
                "Sp.ウォーターパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "レンズ越しの視点",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1695, 4540, 1715, 3528),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "レンズ越しの視点",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1695, 4540, 1715, 3528),
            19,
            new Skill(
                "Sp.ガードライフアシストD Ⅱ",
                "味方2体のSp.DEFと最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Two,
                Range.D
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "白に染まる世界",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "白に染まる世界",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "風：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雪風と踊る少女",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1650, 1664, 4489, 3500),
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雪風と踊る少女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(1650, 1664, 4489, 3500),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雪原の白き魔女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1659, 4478, 1644, 3502),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雪原の白き魔女",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1659, 4478, 1644, 3502),
            19,
            new Skill(
                "Sp.ウィンドガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "鳴り響く狂乱の連弾",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4477, 1678, 3496, 1658),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "鳴り響く狂乱の連弾",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4477, 1678, 3496, 1658),
            19,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ご一緒にいかが？",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1678, 1656, 3969, 3988),
            19,
            new Skill(
                "WガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ご一緒にいかが？",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1678, 1656, 3969, 3988),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いつものおやつ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(3973, 4002, 1649, 1647),
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "いつものおやつ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(3973, 4002, 1649, 1647),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "どたばたデイズ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "どたばたデイズ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "思い出がもう一つ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1659, 4482, 1663, 3471),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "思い出がもう一つ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1659, 4482, 1663, 3471),
            19,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "もふもふな時間",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4472, 1661, 3472, 1679),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "もふもふな時間",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4472, 1661, 3472, 1679),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "覚醒の兆し",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1669, 4488, 1670, 3494),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "覚醒の兆し",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1669, 4488, 1670, 3494),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "騒がし乙女の凱旋",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4500, 1652, 3470, 1656),
            19,
            new Skill(
                "ウォーターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "騒がし乙女の凱旋",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4500, 1652, 3470, 1656),
            19,
            new Skill(
                "ウォーターガードフォールB Ⅱ",
                "敵1～2体のDEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "あなたとおそろい",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1661, 4504, 1657, 3481),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "あなたとおそろい",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1661, 4504, 1657, 3481),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ワンマンアーミー",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4127, 3182, 3177, 2717),
            18,
            new Skill(
                "WパワーアシストD LG",
                "味方2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP/パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "縄跳びトレーニング",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4520, 1705, 3505, 1700),
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "縄跳びトレーニング",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4520, 1705, 3505, 1700),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "暮れなずむ空",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4534, 1687, 3534, 1682),
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "暮れなずむ空",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4534, 1687, 3534, 1682),
            19,
            new Skill(
                "チャージガードフォールB Ⅱ",
                "敵1～2体のDEFをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "かめ、のち、えがお",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1649, 1645, 4503, 3491),
            19,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "かめ、のち、えがお",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1649, 1645, 4503, 3491),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひめひめコールお願いっ！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1668, 4466, 1662, 3477),
            19,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひめひめコールお願いっ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1668, 4466, 1662, 3477),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "カワウソづくし",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4496, 1668, 3473, 1683),
            19,
            new Skill(
                "水：WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "カワウソづくし",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4496, 1668, 3473, 1683),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不動劔と至宝",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不動劔と至宝",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "Sp.ウォーターガードフォールC Ⅲ",
                "敵1～3体のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ねんねこぐろっぴ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4474, 1663, 3499, 1657),
            19,
            new Skill(
                "ウィンドパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ねんねこぐろっぴ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4474, 1663, 3499, 1657),
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "CHARMという兵器",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "CHARMという兵器",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "ウィンドガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ワーオ！　エキサイティン！！",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4472, 1657, 3502, 1667),
            19,
            new Skill(
                "ウィンドガードフォールC Ⅲ",
                "敵1～3体のDEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ワーオ！　エキサイティン！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4472, 1657, 3502, 1667),
            19,
            new Skill(
                "ウィンドパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと風属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "束の間の休息",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1665, 1683, 3490, 4493),
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "束の間の休息",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1665, 1683, 3490, 4493),
            19,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "作戦会議です！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1462, 2918, 1493, 2657),
            17,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "作戦会議です！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1462, 2918, 1493, 2657),
            17,
            new Skill(
                "Sp.ウィンドパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと風属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "予想外の事態",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4497, 1671, 3481, 1661),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "予想外の事態",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(4497, 1671, 3481, 1661),
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "優雅なティータイム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "Sp.ウィンドガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと風属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "優雅なティータイム",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "ライフアシストB Ⅱ",
                "味方1～2体の最大HPをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Life(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "西住流の誇り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1681, 4503, 1666, 3473),
            19,
            new Skill(
                "Sp.ウィンドパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "西住流の誇り",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1681, 4503, 1666, 3473),
            19,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "形勢逆転！！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "形勢逆転！！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "ウィンドパワーアシストC Ⅲ",
                "味方1～3体のATKと風属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Wind), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ワイン色の思い出",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ワイン色の思い出",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "風：WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFとSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal),
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "月夜に吠える天狼",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(4160, 3644, 2736, 2701),
            21,
            new Skill(
                "WパワーフォールE LG",
                "敵2～3体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Lg,
                Range.E
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "雲間から差し込む光",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1672, 4500, 1655, 3479),
            19,
            new Skill(
                "Sp.ウォーターガードバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雲間から差し込む光",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1672, 4500, 1655, 3479),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "好機を待つのじゃ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4474, 1672, 3492, 1644),
            19,
            new Skill(
                "水：パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "好機を待つのじゃ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4474, 1672, 3492, 1644),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひめひめ華麗に参上！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1668, 4480, 1654, 3477),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひめひめ華麗に参上！",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1668, 4480, 1654, 3477),
            19,
            new Skill(
                "水拡：Sp.パワーフォールB Ⅳ",
                "敵1～2体のSp.ATKを特大ダウンさせる。オーダースキル「水属性効果増加」を発動中は敵2体のSp.ATKを特大ダウンさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "紅葉の帳",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "紅葉の帳",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "紅葉も頬も色づいて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1650, 1679, 4498, 3482),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "紅葉も頬も色づいて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(1650, 1679, 4498, 3482),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "秋月夜の彩り",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1658, 4467, 1670, 3492),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "秋月夜の彩り",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1658, 4467, 1670, 3492),
            19,
            new Skill(
                "Sp.ウォーターパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "秋の木漏れ日",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "秋の木漏れ日",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "はじらいマミー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4357, 2093, 3536, 2079),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "どきどきデビル",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4471, 1679, 3480, 1677),
            19,
            new Skill(
                "Sp.ディファーアシストC Ⅳ",
                "味方1～3体のATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "どきどきデビル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4471, 1679, 3480, 1677),
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いたずらゴースト",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1670, 4493, 1650, 3469),
            19,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いたずらゴースト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1670, 4493, 1650, 3469),
            19,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夜空に響く勝利の歌",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4478, 1655, 3482, 1671),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夜空に響く勝利の歌",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4478, 1655, 3482, 1671),
            19,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "陽だまりシュッツエンゲル",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1672, 4466, 1657, 3469),
            19,
            new Skill(
                "Sp.ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKと水属性攻撃力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "陽だまりシュッツエンゲル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1672, 4466, 1657, 3469),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "くるくおーらんたん",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4473, 1661, 3479, 1650),
            19,
            new Skill(
                "ウォーターパワーフォールC Ⅲ",
                "敵1～3体のATKと水属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "くるくおーらんたん",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4473, 1661, 3479, 1650),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "こころにいたずら",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1645, 4495, 1680, 3472),
            19,
            new Skill(
                "水：Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "こころにいたずら",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1645, 4495, 1680, 3472),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "Early Trick",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1660, 4502, 1658, 3491),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "Early Trick",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1660, 4502, 1658, 3491),
            19,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ジャックコーデ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4498, 1681, 3494, 1655),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ジャックコーデ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4498, 1681, 3494, 1655),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "秋空ピクニック",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1679, 4489, 1672, 3471),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "秋空ピクニック",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1679, 4489, 1672, 3471),
            19,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ソーイングマスター姫歌",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4500, 1659, 3480, 1681),
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ソーイングマスター姫歌",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4500, 1659, 3480, 1681),
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "庭園の護り人",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1660, 4487, 1680, 3499),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "庭園の護り人",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1660, 4487, 1680, 3499),
            19,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ふたりの距離",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4467, 1676, 3498, 1646),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ふたりの距離",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4467, 1676, 3498, 1646),
            19,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ダイスキをキャンバスに",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4491, 1651, 3498, 1669),
            19,
            new Skill(
                "ウォーターパワーアシストC Ⅲ",
                "味方1～3体のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ダイスキをキャンバスに",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4491, 1651, 3498, 1669),
            19,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "らんとたづさのかくれんぼ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4482, 1684, 3471, 1674),
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "らんとたづさのかくれんぼ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(4482, 1684, 3471, 1674),
            19,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "藍は舞い降りた",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1679, 4487, 1657, 3492),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "藍は舞い降りた",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1679, 4487, 1657, 3492),
            19,
            new Skill(
                "ディファーアシストC Ⅳ",
                "味方1～3体のSp.ATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "静寂に佇む狩人",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4491, 1644, 3477, 1684),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "静寂に佇む狩人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4491, 1644, 3477, 1684),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "好きなものを一緒に",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2070, 2071, 3534, 4363),
            21,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "星空のどうぶつ探し",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2099, 2064, 4362, 3536),
            21,
            new Skill(
                "水：WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "二人の奏でる夜の歌",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2063, 4356, 2092, 3543),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "すすきの道しるべ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4324, 2087, 3541, 2098),
            21,
            new Skill(
                "ウォーターパワーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "息を潜めて",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1657, 4486, 1661, 3487),
            19,
            new Skill(
                "Sp.ウォーターパワーアシストC Ⅲ",
                "味方1～3体のSp.ATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "息を潜めて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1657, 4486, 1661, 3487),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ミッドナイトスティール",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4505, 1666, 3496, 1664),
            19,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ミッドナイトスティール",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4505, 1666, 3496, 1664),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "真夜中の極秘作戦",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2079, 2093, 3559, 4359),
            21,
            new Skill(
                "Sp.ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "誠実なる守護者",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(4155, 2730, 3634, 2716),
            18,
            new Skill(
                "パワーストライクA LG",
                "敵1体に通常超特大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Lg,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "一葉ののんびりタイム",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(4503, 1681, 3466, 1674),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "一葉ののんびりタイム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4503, 1681, 3466, 1674),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ペアトレ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(2088, 2088, 4334, 3549),
            21,
            new Skill(
                "ウォーターガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと水属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "先輩ふぁいと☆",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3549, 4355, 2086, 2070),
            21,
            new Skill(
                "WパワーアシストC Ⅳ",
                "味方1～3体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "もっと優しく",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2079, 4343, 2091, 3542),
            21,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "りざるとちぇっく",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4341, 2095, 3527, 2081),
            21,
            new Skill(
                "ウォーターガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと水属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "親愛なるルームメイト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2089, 4328, 2074, 3557),
            21,
            new Skill(
                "Sp.ウォーターガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと水属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦場のお色直し",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1650, 4500, 1650, 3485),
            19,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "戦場のお色直し",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1650, 4500, 1650, 3485),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "白鳥の姫騎士",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4476, 1670, 3467, 1647),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "白鳥の姫騎士",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4476, 1670, 3467, 1647),
            19,
            new Skill(
                "水拡：パワーアシストB Ⅳ",
                "味方1～2体のATKを特大アップさせる。オーダースキル「水属性効果増加」を発動中は味方2体のATKを特大アップさせる。※...",
                new SkillEffect[]
                {
                    new ElementSpread(Element.Water)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "氷帝",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1673, 1670, 4490, 3480),
            19,
            new Skill(
                "ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKと水属性攻撃力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "氷帝",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(1673, 1670, 4490, 3480),
            19,
            new Skill(
                "ウォーターパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "その瞳に映るモノ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1668, 4470, 1668, 3498),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "その瞳に映るモノ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1668, 4470, 1668, 3498),
            19,
            new Skill(
                "Sp.ウォーターガードフォールB Ⅱ",
                "敵1～2体のSp.DEFと水属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アクロバット・シューター",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1655, 4468, 1649, 3500),
            19,
            new Skill(
                "Sp.ウォーターパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アクロバット・シューター",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1655, 4468, 1649, 3500),
            19,
            new Skill(
                "Sp.ウォーターパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKと水属性攻撃力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "天からの強襲",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(4497, 1681, 3470, 1645),
            19,
            new Skill(
                "ウォーターパワーアシストB Ⅱ",
                "味方1～2体のATKと水属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "天からの強襲",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(4497, 1681, 3470, 1645),
            19,
            new Skill(
                "ウォーターパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと水属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Water), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ナイトガンスリンガー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2703, 4156, 2712, 3654),
            21,
            new Skill(
                "Sp.ガードバーストD LG",
                "敵2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "夏色日和",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(2075, 2070, 3553, 4327),
            21,
            new Skill(
                "Sp.ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと光属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "打ち上げ花火",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4329, 2068, 3561, 2084),
            21,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "涼やかな響き",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4354, 2079, 3526, 2071),
            21,
            new Skill(
                "ライトガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと光属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "納涼かき氷",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(2085, 4351, 2078, 3562),
            21,
            new Skill(
                "Sp.ダークガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと闇属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "飛び出せミリアム！",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1676, 1659, 4480, 3503),
            19,
            new Skill(
                "ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "飛び出せミリアム！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(1676, 1659, 4480, 3503),
            19,
            new Skill(
                "ライトパワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKと光属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ポイ越しの笑顔",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1679, 4470, 1647, 3493),
            19,
            new Skill(
                "Sp.ライトパワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKと光属性攻撃力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Small),
                    new StatusDown(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ポイ越しの笑顔",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1679, 4470, 1647, 3493),
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "祭囃子と恋の音",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4348, 2066, 3551, 2078),
            21,
            new Skill(
                "ライトパワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKと光属性攻撃力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Small),
                    new StatusDown(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "想いを込めた歌声",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(2092, 2082, 4351, 3559),
            21,
            new Skill(
                "ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと光属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "花咲くハーモニー",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4341, 2075, 3524, 2094),
            21,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "躍動の旋律",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4352, 2061, 3561, 2096),
            21,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "きらめきステージ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2088, 4349, 2083, 3528),
            21,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夏の海とかき氷",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4478, 1653, 3490, 1670),
            19,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "夏の海とかき氷",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4478, 1653, 3490, 1670),
            19,
            new Skill(
                "ダークガードフォールB Ⅱ",
                "敵1～2体のDEFと闇属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "見返り美人",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1657, 4503, 1663, 3494),
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "見返り美人",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1657, 4503, 1663, 3494),
            19,
            new Skill(
                "Sp.ダークパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと闇属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "Diverse",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1649, 4493, 1664, 3489),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "Diverse",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1649, 4493, 1664, 3489),
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "Cherish",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4494, 1659, 3478, 1669),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "Cherish",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4494, 1659, 3478, 1669),
            19,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "激戦の終わりに",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2083, 4354, 2071, 3539),
            21,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "極限コンビネーション",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1654, 1672, 3466, 4499),
            19,
            new Skill(
                "Sp.ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと光属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "極限コンビネーション",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1654, 1672, 3466, 4499),
            19,
            new Skill(
                "Sp.ダークパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不屈の一太刀",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4474, 1658, 3501, 1665),
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不屈の一太刀",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4474, 1658, 3501, 1665),
            19,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "らぶらぶぴーす",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4334, 2095, 3539, 2080),
            21,
            new Skill(
                "ダークガードアシストB Ⅱ",
                "味方1～2体のDEFと闇属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ひまわりとんだよ",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4335, 2076, 3528, 2067),
            21,
            new Skill(
                "ライトパワーフォールB Ⅱ",
                "敵1～2体のATKと光属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "レディーティータイム",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(2074, 4351, 2089, 3560),
            21,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "月に顔をそむけて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4348, 2078, 3528, 2089),
            21,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "バトル・デプロイメント",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1673, 4487, 1646, 3494),
            19,
            new Skill(
                "Sp.ダークパワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "バトル・デプロイメント",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1673, 4487, 1646, 3494),
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "淀みを蹴って",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1649, 4467, 1667, 3506),
            19,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "淀みを蹴って",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1649, 4467, 1667, 3506),
            19,
            new Skill(
                "Sp.ライトパワーフォールB Ⅱ",
                "敵1～2体のSp.ATKと光属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アンブッシュ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4490, 1647, 3480, 1682),
            19,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アンブッシュ",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(4490, 1647, 3480, 1682),
            19,
            new Skill(
                "ライトガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと光属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "晴れときどきサンオイル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4486, 1682, 3506, 1670),
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "晴れときどきサンオイル",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(4486, 1682, 3506, 1670),
            19,
            new Skill(
                "ダークガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "楽しいを探しに行こう！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "ダークパワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKと闇属性攻撃力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Small),
                    new StatusDown(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "楽しいを探しに行こう！",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4475, 1647, 3467, 1653),
            19,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ダイビング・アタッカー",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1650, 4486, 1667, 3500),
            19,
            new Skill(
                "Sp.ダークガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと闇属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ダイビング・アタッカー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1650, 4486, 1667, 3500),
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ウッドクラフトに挑戦",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4338, 2073, 3523, 2097),
            21,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "炊事は任せた！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(2072, 4357, 2091, 3537),
            21,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "面目躍如のサバイバル",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1678, 1656, 4469, 3488),
            19,
            new Skill(
                "ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFと闇属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "面目躍如のサバイバル",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(1678, 1656, 4469, 3488),
            19,
            new Skill(
                "ライトパワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "殲滅のシルバーバレット",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4473, 1680, 3471, 1647),
            19,
            new Skill(
                "ダークパワーフォールB Ⅱ",
                "敵1～2体のATKと闇属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "殲滅のシルバーバレット",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4473, 1680, 3471, 1647),
            19,
            new Skill(
                "ライトガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと光属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "やめられない刺激",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ライトパワーフォールB Ⅱ",
                "敵1～2体のATKと光属性攻撃力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new ElementAttack(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "やめられない刺激",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4504, 1676, 3487, 1647),
            19,
            new Skill(
                "ダークガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFと闇属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "光の注ぐ夜",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1659, 1660, 3485, 4471),
            19,
            new Skill(
                "Sp.ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと闇属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "光の注ぐ夜",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1659, 1660, 3485, 4471),
            19,
            new Skill(
                "Sp.ライトガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと光属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゼロ距離のしあわせ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1650, 4483, 1650, 3501),
            19,
            new Skill(
                "Sp.ダークパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ゼロ距離のしあわせ",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1650, 4483, 1650, 3501),
            19,
            new Skill(
                "Sp.ライトガードアシストB Ⅱ",
                "味方1～2体のSp.DEFと光属性防御力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium),
                    new StatusUp(new ElementGuard(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "水も滴るいい乙女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4491, 1666, 3492, 1672),
            19,
            new Skill(
                "ライトパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "水も滴るいい乙女",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4491, 1666, 3492, 1672),
            19,
            new Skill(
                "ライトパワーアシストB Ⅱ",
                "味方1～2体のATKと光属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "リトル・アークメイジ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2705, 2734, 4126, 3624),
            18,
            new Skill(
                "WガードフォールD LG",
                "敵2体のDEFとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP/ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp(),
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "信じる想いを力に変えて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1678, 4474, 1648, 3478),
            19,
            new Skill(
                "Sp.ライトパワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKと光属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Light), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "信じる想いを力に変えて",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1678, 4474, 1648, 3478),
            19,
            new Skill(
                "Sp.ダークガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFと闇属性防御力を小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small),
                    new StatusUp(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "絆のアルケミートレース",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4483, 1682, 3479, 1659),
            19,
            new Skill(
                "ダークパワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKと闇属性攻撃力を小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "絆のアルケミートレース",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4483, 1682, 3479, 1659),
            19,
            new Skill(
                "ダークガードフォールB Ⅱ",
                "敵1～2体のDEFと闇属性防御力をダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ピュリファイ・ラプラス",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1662, 4496, 1647, 3491),
            19,
            new Skill(
                "Sp.ダークパワーアシストB Ⅱ",
                "味方1～2体のSp.ATKと闇属性攻撃力をアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new ElementAttack(Element.Dark), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ピュリファイ・ラプラス",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1662, 4496, 1647, 3491),
            19,
            new Skill(
                "Sp.ダークガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFと闇属性防御力を小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Small),
                    new StatusDown(new ElementGuard(Element.Dark), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ヘイムスクリングラ・シスターズ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(4504, 1679, 3504, 1650),
            19,
            new Skill(
                "WパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとSp.ATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ヘイムスクリングラ・シスターズ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(4504, 1679, 3504, 1650),
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "黄昏の英雄たち",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1672, 1678, 3477, 4479),
            19,
            new Skill(
                "Sp.ガードフォールA Ⅳ",
                "敵1体のSp.DEFを超特大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "黄昏の英雄たち",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1672, 1678, 3477, 4479),
            19,
            new Skill(
                "風：スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "お姉様の水難",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4498, 1656, 3498, 1651),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "お姉様の水難",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4498, 1656, 3498, 1651),
            19,
            new Skill(
                "マイトフォールB Ⅲ",
                "敵1～2体のATKとDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "この勝利が小さな一歩でも",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1649, 1645, 3503, 4491),
            19,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "この勝利が小さな一歩でも",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1649, 1645, 3503, 4491),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "巨影を討つ閃光",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(4490, 1644, 3484, 1655),
            19,
            new Skill(
                "光：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "巨影を討つ閃光",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4490, 1644, 3484, 1655),
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ここから先は通さない",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1674, 4490, 1651, 3505),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ここから先は通さない",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1674, 4490, 1651, 3505),
            19,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "猛る獅子の剣",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4137, 2719, 3657, 2731),
            21,
            new Skill(
                "ガードブレイクD LG",
                "敵2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "クローバー・クラウン",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "クローバー・クラウン",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1650, 4494, 1673, 3473),
            19,
            new Skill(
                "WパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKとSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "華の休息",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1650, 4484, 1681, 3473),
            19,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "華の休息",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1650, 4484, 1681, 3473),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ビーチでバカンス",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1675, 1671, 4481, 3483),
            19,
            new Skill(
                "パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ビーチでバカンス",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(1675, 1671, 4481, 3483),
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "乙女の非常事態",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "乙女の非常事態",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1681, 4476, 1661, 3475),
            19,
            new Skill(
                "闇：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "M.V.P.オンステージ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4503, 1681, 3488, 1651),
            19,
            new Skill(
                "ディファーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "M.V.P.オンステージ",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4503, 1681, 3488, 1651),
            19,
            new Skill(
                "ディファーフォールB Ⅲ",
                "敵1～2体のSp.ATKとDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "勝利のファンファーレ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "勝利のファンファーレ",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(4505, 1667, 3504, 1647),
            19,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "お手当マイスター",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1652, 4485, 1677, 3479),
            19,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "お手当マイスター",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1652, 4485, 1677, 3479),
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "謳歌のミュージックアワー",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "謳歌のミュージックアワー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4506, 1679, 3492, 1669),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "メイクアップ！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1650, 4479, 1680, 3489),
            19,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "メイクアップ！",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1650, 4479, 1680, 3489),
            19,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "回遊のススメ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(4487, 1683, 3490, 1671),
            19,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "回遊のススメ",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(4487, 1683, 3490, 1671),
            19,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "尊みの探求者",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "闇：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "尊みの探求者",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1669, 4500, 1665, 3466),
            19,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "生徒会のお仕事",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "生徒会のお仕事",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1668, 4481, 1678, 3497),
            19,
            new Skill(
                "光：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "美しき師弟関係",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4508, 1710, 3534, 1691),
            19,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "美しき師弟関係",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(4508, 1710, 3534, 1691),
            19,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "最高のルームメイト",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1689, 1676, 4028, 4028),
            19,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "最高のルームメイト",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1689, 1676, 4028, 4028),
            19,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "BZのプロフェッショナル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "BZのプロフェッショナル",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(4501, 1681, 3475, 1667),
            19,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "もう一度、何度でも",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(3492, 1613, 2978, 1610),
            18,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "もう一度、何度でも",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3492, 1613, 2978, 1610),
            18,
            new Skill(
                "闇：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "相生の水先案内人",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3466, 1601, 2945, 1587),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "相生の水先案内人",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3466, 1601, 2945, 1587),
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "心の炎は豪雨で消えず",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1574, 3467, 1583, 2965),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "心の炎は豪雨で消えず",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1574, 3467, 1583, 2965),
            18,
            new Skill(
                "ディファーアシストB Ⅲ",
                "味方1～2体のSp.ATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "昼下がりのラプソディー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1595, 3459, 1575, 2935),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "昼下がりのラプソディー",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1595, 3459, 1575, 2935),
            18,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "私ヲ蝕ム悪イ夢",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1581, 3443, 1585, 2959),
            18,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "私ヲ蝕ム悪イ夢",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1581, 3443, 1585, 2959),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "だいすきをあげる",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3460, 1569, 2945, 1572),
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "だいすきをあげる",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3460, 1569, 2945, 1572),
            18,
            new Skill(
                "Sp.ディファーアシストB Ⅲ",
                "味方1～2体のATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "希望の光",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1634, 3491, 1627, 2973),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "希望の光",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1634, 3491, 1627, 2973),
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "思い出はカメラの中に",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3318, 3324, 1831, 1833),
            20,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ブレイク・タイム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3648, 1848, 2966, 1820),
            20,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ラブリーアンドピース",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3297, 3307, 1833, 1826),
            20,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "すってんあかりん",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1821, 3669, 1856, 2946),
            20,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "獅子奮迅",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2722, 2698, 3639, 4132),
            18,
            new Skill(
                "WガードアシストD LG",
                "味方2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "援:支援UP/Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。さらに、支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp(),
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "せめて、この子だけは",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:マイトDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "せめて、この子だけは",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "水：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:マイトUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKとDEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal),
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "紅巴式夏祭りの楽しみ方",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1606, 3442, 1586, 2941),
            18,
            new Skill(
                "Sp.ディファーバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ディファーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "紅巴式夏祭りの楽しみ方",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1606, 3442, 1586, 2941),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.マイトDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special),
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "世界を越えて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3457, 1591, 2958, 1597),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "世界を越えて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3457, 1591, 2958, 1597),
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "小さなシュッツエンゲル",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2703, 3440, 1573, 1844),
            18,
            new Skill(
                "WパワーフォールB Ⅲ",
                "敵1～2体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "小さなシュッツエンゲル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2703, 3440, 1573, 1844),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "束ねる絆の一夜",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(3450, 1599, 2935, 1594),
            18,
            new Skill(
                "Sp.ディファーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:WガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFとSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "束ねる絆の一夜",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3450, 1599, 2935, 1594),
            18,
            new Skill(
                "水：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "救う願いの一閃",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1601, 3894, 1601, 2489),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "救う願いの一閃",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1601, 3894, 1601, 2489),
            18,
            new Skill(
                "水：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "足踏み健康ロードの悲劇",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3464, 1606, 2960, 1580),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "足踏み健康ロードの悲劇",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3464, 1606, 2960, 1580),
            18,
            new Skill(
                "闇：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "みんなを守るために",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1603, 1584, 3446, 2969),
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "みんなを守るために",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(1603, 1584, 3446, 2969),
            18,
            new Skill(
                "Sp.ディファーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "狂化フルスロットル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "狂化フルスロットル",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1585, 3453, 1605, 2965),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "友を守護する剣",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3433, 1601, 2964, 1571),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "友を守護する剣",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3433, 1601, 2964, 1571),
            18,
            new Skill(
                "チャージガードフォールB Ⅱ",
                "敵1～2体のDEFをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不死身の刃",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1594, 3439, 1601, 2957),
            18,
            new Skill(
                "ディファーアシストB Ⅲ",
                "味方1～2体のSp.ATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不死身の刃",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1594, 3439, 1601, 2957),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "癒しの露天風呂",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1578, 3442, 1580, 2972),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "癒しの露天風呂",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1578, 3442, 1580, 2972),
            18,
            new Skill(
                "光：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アサルトリリィ ふるーつ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1683, 3524, 1684, 3034),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アサルトリリィ ふるーつ",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1683, 3524, 1684, 3034),
            18,
            new Skill(
                "光：WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ストームデュオ",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "光：ガードアシストB Ⅲ",
                "味方1～2体のDEFを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ストームデュオ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "マイトブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アクアストライク",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "アクアストライク",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "チャージヒールC Ⅱ",
                "味方1～3体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.C
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雷光一閃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1585, 3472, 1574, 2962),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "雷光一閃",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1585, 3472, 1574, 2962),
            18,
            new Skill(
                "光：Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "フラガラッハの光",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(2711, 2718, 3888, 3892),
            21,
            new Skill(
                "WガードヒールE LG",
                "味方2～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Lg,
                Range.E
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ハッピーを見つけたら☆",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1575, 1589, 2955, 3466),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ハッピーを見つけたら☆",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1575, 1589, 2955, 3466),
            18,
            new Skill(
                "パワーブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦いを終えて",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1595, 1596, 3438, 2935),
            18,
            new Skill(
                "ガードヒールB Ⅲ+",
                "味方1～2体のHPを大回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "回:WガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "戦いを終えて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1595, 1596, 3438, 2935),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "通じ合うふたり",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1585, 3918, 1583, 2507),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "通じ合うふたり",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1585, 3918, 1583, 2507),
            18,
            new Skill(
                "火：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "麗しき出立",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3467, 1590, 2945, 1595),
            18,
            new Skill(
                "光：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "麗しき出立",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3467, 1590, 2945, 1595),
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "飛翔迎撃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1594, 3432, 1580, 2972),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "飛翔迎撃",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1594, 3432, 1580, 2972),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不動劒の姫",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Light,
            new Status(3895, 1589, 2499, 1590),
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "不動劒の姫",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3895, 1589, 2499, 1590),
            18,
            new Skill(
                "チャージパワーフォールB Ⅱ",
                "敵1～2体のATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "そうさく倶楽部の活動",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1608, 3454, 1599, 2952),
            18,
            new Skill(
                "スマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "そうさく倶楽部の活動",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1608, 3454, 1599, 2952),
            18,
            new Skill(
                "光：WガードフォールA Ⅲ",
                "敵1体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "愛しき人との待ち合わせ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3450, 1601, 2968, 1605),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "愛しき人との待ち合わせ",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(3450, 1601, 2968, 1605),
            18,
            new Skill(
                "パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "月下の傍観者",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1606, 3469, 1569, 2962),
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "月下の傍観者",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1606, 3469, 1569, 2962),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "なかよしとわいらいと",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3459, 1596, 2943, 1595),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "なかよしとわいらいと",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3459, 1596, 2943, 1595),
            18,
            new Skill(
                "光：WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "水流乱撃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1595, 3464, 1594, 2941),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "水流乱撃",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1595, 3464, 1594, 2941),
            18,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "踏み込む勇気",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2583, 1398, 2818, 1422),
            17,
            new Skill(
                "火：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "踏み込む勇気",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2583, 1398, 2818, 1422),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "吸血鬼のたしなみ",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1593, 1572, 2946, 3470),
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "吸血鬼のたしなみ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1593, 1572, 2946, 3470),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.マイトDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKとSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special),
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "校舎屋上のストラグル",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2939, 3468, 1574, 1572),
            18,
            new Skill(
                "風：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "校舎屋上のストラグル",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(2939, 3468, 1574, 1572),
            18,
            new Skill(
                "WパワーバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のATKとSp.ATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:WパワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKとSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "清淑なる黒き槍",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3890, 1588, 2515, 1582),
            18,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "清淑なる黒き槍",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3890, 1588, 2515, 1582),
            18,
            new Skill(
                "火：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "世界を守る剣たち",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(3450, 1603, 2936, 1589),
            18,
            new Skill(
                "光：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「光属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "世界を守る剣たち",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3450, 1603, 2936, 1589),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "悪夢との共闘",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1597, 3437, 1582, 2966),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "悪夢との共闘",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(1597, 3437, 1582, 2966),
            18,
            new Skill(
                "チャージSp.ガードアシストC Ⅲ",
                "味方1～3体のSp.DEFをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "扶翼の剣",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Light,
            new Status(1575, 3466, 1575, 2951),
            18,
            new Skill(
                "Sp.マイトバーストB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "扶翼の剣",
            new Rearguard(RearguardKind.Interference),
            Element.Light,
            new Status(1575, 3466, 1575, 2951),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "灼爛の一撃",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Light,
            new Status(1579, 3458, 1570, 2935),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "灼爛の一撃",
            new Rearguard(RearguardKind.Recovery),
            Element.Light,
            new Status(1579, 3458, 1570, 2935),
            18,
            new Skill(
                "Sp.パワーヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "静寂の中で",
            new Vanguard(VanguardKind.NormalRange),
            Element.Light,
            new Status(3461, 1595, 2956, 1583),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "静寂の中で",
            new Rearguard(RearguardKind.Support),
            Element.Light,
            new Status(3461, 1595, 2956, 1583),
            18,
            new Skill(
                "チャージガードアシストC Ⅲ",
                "味方1～3体のDEFをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "キラキラアイスクリーム！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3443, 1571, 2937, 1603),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "キラキラアイスクリーム！",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3443, 1571, 2937, 1603),
            18,
            new Skill(
                "チャージヒールD Ⅱ",
                "味方2体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.D
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "迎え撃つ勇士たち",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1600, 3469, 1573, 2933),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "迎え撃つ勇士たち",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1600, 3469, 1573, 2933),
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "不撓不屈の心を胸に",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3446, 1578, 2962, 1573),
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "不撓不屈の心を胸に",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3446, 1578, 2962, 1573),
            18,
            new Skill(
                "闇：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ブレイブ・ショット",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1575, 3439, 1607, 2955),
            18,
            new Skill(
                "チャージSp.パワーアシストB Ⅱ",
                "味方1～2体のSp.ATKをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ブレイブ・ショット",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1575, 3439, 1607, 2955),
            18,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "発進☆ユニコーン！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1578, 3885, 1570, 2491),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "発進☆ユニコーン！",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1578, 3885, 1570, 2491),
            18,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.マイトUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKとSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "仮想訓練場の応酬",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3912, 1593, 2489, 1608),
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:WガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFとSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "仮想訓練場の応酬",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3912, 1593, 2489, 1608),
            18,
            new Skill(
                "火：Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ディファーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとDEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special),
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "優しい夕暮れ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(3463, 1596, 2947, 1586),
            18,
            new Skill(
                "ガードブレイクA Ⅴ",
                "敵1体に通常超特大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Five,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKとDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal),
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "優しい夕暮れ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(3463, 1596, 2947, 1586),
            18,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:WパワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKとSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal),
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "静かに肩を寄せて",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1587, 3452, 1599, 2951),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "静かに肩を寄せて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1587, 3452, 1599, 2951),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "レスキューキャット",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3463, 1587, 2935, 1579),
            18,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "レスキューキャット",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3463, 1587, 2935, 1579),
            18,
            new Skill(
                "闇：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "平穏を守るための哮り",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3443, 1603, 2948, 1588),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "平穏を守るための哮り",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3443, 1603, 2948, 1588),
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅰ",
                "HP回復時、一定確率でHPの回復量をアップさせる。さらに、支援/妨害時、一定確率で支援/妨害時効果を小アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.One
            )
        ),

        new Memoria(
            "ハッピー＆トリート",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1589, 3920, 1609, 2508),
            18,
            new Skill(
                "Sp.ガードバーストA Ⅴ",
                "敵1体に特殊超特大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Five,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.マイトUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ハッピー＆トリート",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1589, 3920, 1609, 2508),
            18,
            new Skill(
                "Sp.パワーフォールC Ⅳ",
                "敵1～3体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.マイトDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKとSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special),
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "あなただけの守護天使",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1593, 3460, 1586, 2971),
            18,
            new Skill(
                "チャージスマッシュB Ⅱ",
                "敵1～2体に特殊ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "あなただけの守護天使",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1593, 3460, 1586, 2971),
            18,
            new Skill(
                "闇：WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "いつでも近くに",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(3463, 1590, 2960, 1605),
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "いつでも近くに",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3463, 1590, 2960, 1605),
            18,
            new Skill(
                "ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のDEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "純白の想い",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "闇：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "純白の想い",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3450, 1583, 2933, 1602),
            18,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "蒼き月の御使い",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(2736, 4156, 2736, 3627),
            18,
            new Skill(
                "スマッシュD LG",
                "敵2体に特殊大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "攻:ダメージUP/Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。さらに、攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp(),
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "放課後のミューズ",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(1596, 3458, 1591, 2937),
            18,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP/副援:支援UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。さらに、支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new RecoveryUp(),
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "放課後のミューズ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1596, 3458, 1591, 2937),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "小春日和",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3444, 1578, 2956, 1595),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "小春日和",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3444, 1578, 2956, 1595),
            18,
            new Skill(
                "WガードアシストC Ⅳ",
                "味方1～3体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:WガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFとSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "真夜中のクリエイター",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Dark,
            new Status(1601, 3447, 1583, 2969),
            18,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "真夜中のクリエイター",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(1601, 3447, 1583, 2969),
            18,
            new Skill(
                "闇：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「闇属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "遠い日の足跡",
            new Rearguard(RearguardKind.Interference),
            Element.Dark,
            new Status(1604, 3436, 1572, 2959),
            18,
            new Skill(
                "チャージSp.パワーフォールB Ⅱ",
                "敵1～2体のSp.ATKをダウンさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "遠い日の足跡",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Dark,
            new Status(1604, 3436, 1572, 2959),
            18,
            new Skill(
                "チャージスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "先駆けプリンセス",
            new Rearguard(RearguardKind.Support),
            Element.Dark,
            new Status(3475, 1589, 2978, 1601),
            18,
            new Skill(
                "チャージパワーアシストB Ⅱ",
                "味方1～2体のATKをアップさせる。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "先駆けプリンセス",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Dark,
            new Status(3475, 1589, 2978, 1601),
            18,
            new Skill(
                "チャージストライクA Ⅲ",
                "敵1体に通常大ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "寂寥の美",
            new Vanguard(VanguardKind.NormalRange),
            Element.Dark,
            new Status(3437, 1596, 2952, 1592),
            18,
            new Skill(
                "チャージストライクB Ⅱ",
                "敵1～2体に通常ダメージを与える。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "寂寥の美",
            new Rearguard(RearguardKind.Recovery),
            Element.Dark,
            new Status(3437, 1596, 2952, 1592),
            18,
            new Skill(
                "チャージヒールC Ⅱ",
                "味方1～3体のHPを小回復する。バトル時間60秒経過ごとにスキル効果がアップし、600秒経過で最大になる。",
                new SkillEffect[]
                {
                    new Charge()
                },
                new StatusChange[]
                {
                },
                Level.Two,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "劔の妖精",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1857, 3650, 1837, 2949),
            20,
            new Skill(
                "WパワーバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のATKとSp.ATKを小ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Small),
                    new StatusDown(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "セインツの宝石",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3576, 1697, 3061, 1667),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "セインツの宝石",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3576, 1697, 3061, 1667),
            18,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のATKを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "暴君の花嫁",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1859, 1843, 3328, 3293),
            20,
            new Skill(
                "WガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFとSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small),
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "約束の蕾",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2978, 1614, 3463, 1584),
            18,
            new Skill(
                "水：ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "約束の蕾",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2978, 1614, 3463, 1584),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のATKとDEFをアップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal),
                    new GuardUp(Type.Normal)
                },
                Level.One
            )
        ),

        new Memoria(
            "大切な存在",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1593, 3446, 1579, 2943),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "大切な存在",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1593, 3446, 1579, 2943),
            18,
            new Skill(
                "Sp.パワーフォールC Ⅳ",
                "敵1～3体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅲ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを特大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "黄昏の研究者たち",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1487, 1477, 2828, 3336),
            17,
            new Skill(
                "Sp.ガードヒールC Ⅲ+",
                "味方1～3体のHPを回復する。さらに味方のSp.DEFをアップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "黄昏の研究者たち",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(1487, 1477, 2828, 3336),
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "出逢いの約束",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1481, 1473, 2805, 3328),
            17,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "出逢いの約束",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1481, 1473, 2805, 3328),
            17,
            new Skill(
                "Sp.マイトバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.ATKとSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "暁に笑う少女",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(3456, 1584, 2966, 1600),
            18,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "暁に笑う少女",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(3456, 1584, 2966, 1600),
            18,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ハンドメイド・リリィ",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1575, 3467, 1601, 2948),
            18,
            new Skill(
                "Sp.ガードアシストD Ⅲ",
                "味方2体のSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ハンドメイド・リリィ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1575, 3467, 1601, 2948),
            18,
            new Skill(
                "Sp.マイトスマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ハッピーバレンタインだにゃん♪",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1834, 1821, 3316, 3316),
            20,
            new Skill(
                "WパワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKとSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small),
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅲ",
                "HP回復時、一定確率でHPの回復量を特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "まごころをこめて！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3675, 1857, 2949, 1843),
            20,
            new Skill(
                "Sp.ディファーストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ドキドキ・ショコラーデ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1848, 3674, 1831, 2953),
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のSp.DEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "煌めく花々",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1518, 1527, 3115, 3094),
            18,
            new Skill(
                "ヒールD Ⅳ",
                "味方2体のHPを大回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Four,
                Range.D
            )
            ,
            new SupportSkill(
                "回:WガードUP Ⅰ",
                "HP回復時、一定確率で味方前衛1体のDEFとSp.DEFをアップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal),
                    new GuardUp(Type.Special)
                },
                Level.One
            )
        ),

        new Memoria(
            "煌めく花々",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(1518, 1527, 3115, 3094),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "一筆の心",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3440, 1597, 2958, 1569),
            18,
            new Skill(
                "WパワーブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとSp.ATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "一筆の心",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3440, 1597, 2958, 1569),
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅲ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを特大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "飾らぬ想いに咲き誇る",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1602, 3434, 1585, 2955),
            18,
            new Skill(
                "Sp.マイトスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "飾らぬ想いに咲き誇る",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1602, 3434, 1585, 2955),
            18,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ヘルヴォルのお嫁さん",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1598, 3453, 1571, 2960),
            18,
            new Skill(
                "Sp.マイトフォールB Ⅲ",
                "敵1～2体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ヘルヴォルのお嫁さん",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1598, 3453, 1571, 2960),
            18,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFをアップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special),
                    new GuardUp(Type.Special)
                },
                Level.One
            )
        ),

        new Memoria(
            "楽しい遊園地",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1590, 3471, 1593, 2959),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "楽しい遊園地",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1590, 3471, 1593, 2959),
            18,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "リフレッシュ！エンジン",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3888, 1587, 2519, 1576),
            18,
            new Skill(
                "パワーストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "リフレッシュ！エンジン",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3888, 1587, 2519, 1576),
            18,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "夜の闇を切り拓く者たち",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1601, 1606, 3460, 2943),
            18,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "夜の闇を切り拓く者たち",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(1601, 1606, 3460, 2943),
            18,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "エクストリームブースト",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3456, 1583, 2942, 1580),
            18,
            new Skill(
                "ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅲ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "エクストリームブースト",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3456, 1583, 2942, 1580),
            18,
            new Skill(
                "パワーストライクA Ⅲ+",
                "敵1体に通常大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のDEFを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "ガーディアン・パワー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1854, 3672, 1833, 2958),
            20,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅲ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Three
            )
        ),

        new Memoria(
            "コール・ユア・ネーム",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1825, 1855, 3654, 2973),
            20,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ジャスト・ザ・ブレイブ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3666, 1847, 2946, 1823),
            20,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅲ",
                "攻撃時、一定確率で敵のDEFを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "大切なあなたを想い",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1529, 1517, 2485, 2212),
            18,
            new Skill(
                "パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "戦火の結束",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(3439, 1586, 2945, 1575),
            18,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "戦火の結束",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(3439, 1586, 2945, 1575),
            18,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "ラ・ピュセル",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1821, 4469, 1856, 2146),
            20,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "鬼神の意志を継ぐ者",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(3656, 2976, 1834, 1845),
            20,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅲ",
                "支援/妨害時、一定確率で支援/妨害効果を特大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "台場の白き魔女",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3672, 1857, 2968, 1831),
            20,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "狂乱の姫巫女",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1854, 3655, 1834, 2977),
            20,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅲ",
                "攻撃時、一定確率で攻撃ダメージを特大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Three
            )
        ),

        new Memoria(
            "親愛なる仲間",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1561, 3005, 2760, 1531),
            17,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅲ",
                "攻撃時、一定確率で敵のATKを特大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Three
            )
        ),

        new Memoria(
            "親愛なる仲間",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1561, 3005, 2760, 1531),
            17,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "そこにある笑顔",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2842, 1386, 2586, 1397),
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "そこにある笑顔",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2842, 1386, 2586, 1397),
            17,
            new Skill(
                "マイトフォールA Ⅲ",
                "敵1体のATKとDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ふたりのアーセナル",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1402, 1422, 1632, 3793),
            17,
            new Skill(
                "火：Sp.ガードアシストB Ⅲ",
                "味方1～2体のSp.DEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ふたりのアーセナル",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1402, 1422, 1632, 3793),
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "パジャマパーティー",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1556, 2977, 1560, 2493),
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ファイア・ダッシュ",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(3441, 1579, 2943, 1609),
            18,
            new Skill(
                "風：ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ファイア・ダッシュ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3441, 1579, 2943, 1609),
            18,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ミューチュアルプロテクション",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1607, 3435, 1608, 2945),
            18,
            new Skill(
                "ヒールスマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ミューチュアルプロテクション",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1607, 3435, 1608, 2945),
            18,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "海の世界に想いを馳せて",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1585, 1590, 2968, 3465),
            18,
            new Skill(
                "ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "海の世界に想いを馳せて",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1585, 1590, 2968, 3465),
            18,
            new Skill(
                "ディファースマッシュA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "CHARMを絵筆に替えて",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1570, 3464, 1601, 2934),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "CHARMを絵筆に替えて",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1570, 3464, 1601, 2934),
            18,
            new Skill(
                "Sp.ディファーフォールB Ⅲ",
                "敵1～2体のATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ボナペティ！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3457, 1576, 2964, 1594),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ボナペティ！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(3457, 1576, 2964, 1594),
            18,
            new Skill(
                "火：パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "恋花様ダイエット大作戦",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(2663, 1484, 2928, 1493),
            17,
            new Skill(
                "風：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "恋花様ダイエット大作戦",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2663, 1484, 2928, 1493),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "復讐の炎",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1490, 3288, 1471, 2337),
            17,
            new Skill(
                "ディファースマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "復讐の炎",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1490, 3288, 1471, 2337),
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "強くなるために",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1417, 1423, 2822, 2569),
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "強くなるために",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1417, 1423, 2822, 2569),
            17,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "大切な貴女への贈り物",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "火：パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "大切な貴女への贈り物",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3434, 1602, 2963, 1589),
            18,
            new Skill(
                "パワーブレイクA Ⅳ",
                "敵1体に通常特大ダメージを与え、敵のATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "罰執行のお時間です",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "罰執行のお時間です",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(3433, 1595, 2950, 1606),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "故郷へ想い馳せながら",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1836, 2980, 1825, 3670),
            20,
            new Skill(
                "Sp.マイトアシストB Ⅲ",
                "味方1～2体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "心弛ぶひととき",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1855, 3661, 1833, 2966),
            20,
            new Skill(
                "水：スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "元日の決斗！",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1845, 3640, 1831, 2980),
            20,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "気高き錬金術師",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2993, 1571, 2477, 1570),
            19,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ガンズ・パーティー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2992, 1576, 2491, 1553),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "神獣鏡の輝き",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1562, 1564, 2996, 2485),
            19,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "絆の歌",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1544, 3366, 1530, 2402),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "絆の歌",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1544, 3366, 1530, 2402),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "戦いの合間に",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1522, 2462, 1507, 2241),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "輝きの海岸線",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1494, 1459, 2689, 2913),
            17,
            new Skill(
                "WガードフォールB Ⅲ",
                "敵1～2体のDEFとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "輝きの海岸線",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(1494, 1459, 2689, 2913),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ざっぱ～～ん！",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1532, 2766, 1532, 3001),
            17,
            new Skill(
                "水：Sp.パワーアシストB Ⅲ",
                "味方1～2体のSp.ATKを大アップさせる。さらに味方がオーダースキル「水属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ざっぱ～～ん！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1532, 2766, 1532, 3001),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "白花咲く港",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1461, 2901, 2687, 1481),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "白花咲く港",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1461, 2901, 2687, 1481),
            17,
            new Skill(
                "Sp.パワーアシストA Ⅳ",
                "味方1体のSp.ATKを超特大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "一柳隊、大特集！",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2857, 1409, 2606, 1389),
            17,
            new Skill(
                "火：ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。さらに味方がオーダースキル「火属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "一柳隊、大特集！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2857, 1409, 2606, 1389),
            17,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "あなたに傘を",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1469, 2670, 1473, 2921),
            17,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "あなたに傘を",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1469, 2670, 1473, 2921),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅲ+",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "情熱の取材前夜！",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2939, 1478, 2680, 1476),
            17,
            new Skill(
                "マイトストライクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "情熱の取材前夜！",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(2939, 1478, 2680, 1476),
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "姫歌を脅かす2つの新星",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1421, 1423, 2827, 2591),
            17,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "姫歌を脅かす2つの新星",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1421, 1423, 2827, 2591),
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ハッピー☆シューティングスター",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2842, 1400, 1396, 2579),
            17,
            new Skill(
                "ヒールストライクA Ⅳ",
                "敵1体に通常特大ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ハッピー☆シューティングスター",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2842, 1400, 1396, 2579),
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プレゼントはお任せ♪",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2906, 1490, 2656, 1475),
            17,
            new Skill(
                "マイトブレイクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、敵のATKとDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プレゼントはお任せ♪",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2906, 1490, 2656, 1475),
            17,
            new Skill(
                "風：ガードアシストB Ⅲ",
                "味方1～2体のDEFを大アップさせる。さらに味方がオーダースキル「風属性効果増加」を発動中は効果がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "リリィのすべてを伝えるために",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1527, 3358, 1544, 2419),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "リリィのすべてを伝えるために",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1527, 3358, 1544, 2419),
            17,
            new Skill(
                "Sp.パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "キャッチ＆リリース＆イート",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2548, 1410, 2725, 1425),
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "キャッチ＆リリース＆イート",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2548, 1410, 2725, 1425),
            17,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "これまでも、これからも隣で",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3170, 1419, 2249, 1406),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "これまでも、これからも隣で",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(3170, 1419, 2249, 1406),
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "リリィになるために！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1568, 2970, 1568, 2498),
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "結梨の大好きな場所",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1467, 2902, 1461, 2684),
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ+",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "結梨の大好きな場所",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1467, 2902, 1461, 2684),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "みんな、ガンバレー！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2997, 1581, 2470, 1545),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "冷たいラムネをどうぞ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1558, 1553, 2496, 2970),
            19,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "リワード・マイセルフ",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2906, 1463, 2688, 1479),
            17,
            new Skill(
                "パワーアシストC Ⅳ",
                "味方1～3体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "リワード・マイセルフ",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2906, 1463, 2688, 1479),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "トライング・オン",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2931, 1474, 2653, 1466),
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "トライング・オン",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(2931, 1474, 2653, 1466),
            17,
            new Skill(
                "Sp.マイトフォールA Ⅲ",
                "敵1体のSp.ATKとSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium),
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "気まぐれのツーショット",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1568, 2994, 1561, 2505),
            19,
            new Skill(
                "Sp.ガードフォールC Ⅳ",
                "敵1～3体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "藍の宝物",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1410, 2383, 2220, 2249),
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "藍の宝物",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1410, 2383, 2220, 2249),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "指先と白いペン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3391, 1743, 2764, 1750),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "指先と白いペン",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3391, 1743, 2764, 1750),
            17,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "エレクトロンバウト！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1575, 2987, 1572, 2502),
            19,
            new Skill(
                "ヒールスマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "悲壮の華",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1464, 1484, 3276, 2300),
            17,
            new Skill(
                "ガードアシストD Ⅲ",
                "味方2体のDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "悲壮の華",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(1464, 1484, 3276, 2300),
            17,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "鬼さんズ、こちら",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(3356, 1554, 2399, 1565),
            17,
            new Skill(
                "ガードブレイクA Ⅳ+",
                "敵1体に通常特大ダメージを与え、敵のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "鬼さんズ、こちら",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(3356, 1554, 2399, 1565),
            17,
            new Skill(
                "ガードヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "優美なる舞",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1397, 1652, 2598, 2600),
            17,
            new Skill(
                "WガードスマッシュB Ⅲ+",
                "敵1～2体に特殊大ダメージを与え、自身のDEFとSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "優美なる舞",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1397, 1652, 2598, 2600),
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "美しき鉄糸の舞",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1485, 2900, 1494, 2685),
            17,
            new Skill(
                "Sp.パワーアシストC Ⅳ",
                "味方1～3体のSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "美しき鉄糸の舞",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1485, 2900, 1494, 2685),
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "駆けろ！エージェント",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1571, 2992, 1838, 2471),
            19,
            new Skill(
                "Sp.パワースマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のSp.ATKとSp.DEFをアップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special),
                    new GuardUp(Type.Special)
                },
                Level.One
            )
        ),

        new Memoria(
            "スピード☆スター",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2469, 2216, 1518, 1520),
            18,
            new Skill(
                "WパワーアシストB Ⅲ",
                "味方1～2体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "電光石火でご到着！",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2489, 1509, 2221, 1531),
            18,
            new Skill(
                "風：ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。さらに味方がオーダースキル「風属性効果増加」を発動中は威力がアップする。※...",
                new SkillEffect[]
                {
                    new ElementStimulation(Element.Fire)
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "街角の寡黙な花",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1574, 2515, 1542, 2288),
            18,
            new Skill(
                "ディファースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKとDEFを小アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small),
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アクアプラクティス",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(2925, 1461, 2678, 1473),
            17,
            new Skill(
                "パワーストライクA Ⅲ+",
                "敵1体に通常大ダメージを与え、自身のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アクアプラクティス",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2925, 1461, 2678, 1473),
            17,
            new Skill(
                "WガードアシストB Ⅲ",
                "味方1～2体のDEFとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のDEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "二水のヘイムスクリングラ体験",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1499, 2484, 1514, 2230),
            18,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "勝利の女神が微笑む時",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1522, 1509, 2219, 2482),
            18,
            new Skill(
                "パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プレシャス・モーニング",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2954, 1528, 2710, 1519),
            17,
            new Skill(
                "マイトアシストB Ⅲ",
                "味方1～2体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プレシャス・モーニング",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2954, 1528, 2710, 1519),
            17,
            new Skill(
                "マイトストライクB Ⅲ+",
                "敵1～2体に通常大ダメージを与え、自身のATKとDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:マイトUP Ⅰ",
                "前衛から攻撃時、一定確率で自身のATKとDEFをアップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal),
                    new GuardUp(Type.Normal)
                },
                Level.One
            )
        ),

        new Memoria(
            "フォール・ダウン・アタック",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1502, 1508, 2492, 2228),
            18,
            new Skill(
                "ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ホワイト・ラビット・マジック！",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1506, 2472, 1499, 2213),
            18,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プレ・ハロウィンパーティー！",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1507, 2485, 1522, 2244),
            18,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ようこそ！ふしぎの国へ",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1569, 1561, 2471, 2994),
            19,
            new Skill(
                "パワーヒールC Ⅲ",
                "味方1～3体のHPを回復する。さらに味方のATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Small)
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "おいでよ☆ハロウィン",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2971, 1545, 2489, 1584),
            19,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "一直線上のストラテジー",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1532, 2939, 1515, 1760),
            18,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "この空の下で",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1509, 1506, 2207, 2493),
            18,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "追跡者",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2468, 2241, 1524, 1508),
            18,
            new Skill(
                "Sp.パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のSp.ATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "花を束ねる者",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1578, 2978, 1547, 2488),
            19,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "姉妹の休息",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1509, 1510, 2226, 2462),
            18,
            new Skill(
                "Sp.ガードヒールD Ⅲ",
                "味方2体のHPを回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "凛々しい花々",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(2552, 1587, 2302, 1605),
            18,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "わたしたちの魔法",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(1561, 2504, 1557, 2981),
            19,
            new Skill(
                "Sp.ガードアシストC Ⅳ",
                "味方1～3体のSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:Sp.ガードUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "貫く想いの一撃",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1549, 2967, 1581, 2491),
            19,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プリンセスひめひめ",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2913, 1522, 1786, 1501),
            18,
            new Skill(
                "パワーフォールC Ⅳ",
                "敵1～3体のATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "いつかみんなと見る景色",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(3006, 1579, 2492, 1569),
            19,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "繋げたい言葉",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1500, 2470, 1530, 2230),
            18,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.DEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "夏祭りのスナイパー",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2496, 1517, 2245, 1497),
            18,
            new Skill(
                "ガードストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "もう何も奪わせない",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1476, 1485, 2128, 2057),
            17,
            new Skill(
                "ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のDEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "夜空に咲く約束の花",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1568, 2981, 1578, 2497),
            19,
            new Skill(
                "Sp.ガードスマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "のびのびトレーニング！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(1529, 2494, 1503, 2230),
            18,
            new Skill(
                "Sp.マイトアシストA Ⅲ",
                "味方1体のSp.ATKとSp.DEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium),
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "神宿りの暴走",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(3000, 1576, 2479, 1562),
            19,
            new Skill(
                "ガードストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "プランセス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Wind,
            new Status(3390, 2074, 2873, 2093),
            18,
            new Skill(
                "ストライクD LG",
                "敵2体に通常大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Lg,
                Range.D
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "信頼の背中",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1393, 2036, 1397, 2002),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "単騎無双",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2485, 1494, 2220, 1497),
            18,
            new Skill(
                "ヒールストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ラプラスの発動",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2059, 1396, 1989, 1418),
            17,
            new Skill(
                "ストライクC Ⅲ",
                "敵1～3体に通常ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "顕現する脅威",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2108, 1480, 2093, 1457),
            17,
            new Skill(
                "ガードストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のDEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "スーパーかわいいジャンプ！",
            new Rearguard(RearguardKind.Support),
            Element.Wind,
            new Status(2073, 2128, 1487, 1484),
            17,
            new Skill(
                "WパワーアシストA Ⅲ",
                "味方1体のATKとSp.ATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "きみとぼくの創作世界",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1382, 2057, 1413, 1977),
            17,
            new Skill(
                "Sp.パワースマッシュA Ⅳ",
                "敵1体に特殊特大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "グリーンライフ",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1406, 1388, 1657, 2400),
            17,
            new Skill(
                "Sp.ガードヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のSp.DEFを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "馳せたる海辺",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1568, 2994, 1570, 2524),
            19,
            new Skill(
                "スマッシュC Ⅲ",
                "敵1～3体に特殊ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "エスコートナイト",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2027, 1414, 2006, 1401),
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "おもちゃのプール",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1469, 2141, 1455, 2077),
            17,
            new Skill(
                "ヒールスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ランペイジクラフト",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2467, 1477, 1708, 1454),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "あなたと甘いひとときを",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2133, 1483, 1476, 2066),
            17,
            new Skill(
                "Sp.パワーブレイクA Ⅲ+",
                "敵1体に通常大ダメージを与え、敵のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.ATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "水の車窓",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1409, 1980, 1394, 2059),
            17,
            new Skill(
                "Sp.パワーフォールB Ⅲ",
                "敵1～2体のSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のSp.ATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ラ・ヴァカンス・パルフェ",
            new Rearguard(RearguardKind.Recovery),
            Element.Water,
            new Status(1410, 1407, 1999, 2039),
            17,
            new Skill(
                "Sp.パワーヒールB Ⅲ",
                "味方1～2体のHPを大回復する。さらに味方のSp.ATKを小アップする。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Small)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ウォーター・レイルウェイ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2131, 1455, 2081, 1483),
            17,
            new Skill(
                "Sp.ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アナザーワールド",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1392, 1977, 1386, 2059),
            17,
            new Skill(
                "Sp.パワースマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.ATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "勝負の鍵は",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2473, 1491, 1712, 1481),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アンブレイカブル",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2406, 1399, 1628, 1391),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ひとりはみんなのために",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1738, 1488, 2461, 1452),
            17,
            new Skill(
                "ガードフォールC Ⅳ",
                "敵1～3体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "文武両道の乙女",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Water,
            new Status(2121, 1460, 2087, 1455),
            17,
            new Skill(
                "パワーストライクA Ⅳ",
                "敵1体に通常特大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アーセナルの絆",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1469, 2471, 1452, 1740),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "未来を切り開く武器",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2993, 1570, 2488, 1549),
            19,
            new Skill(
                "パワーブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のATKをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "この地にて芽吹く",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2010, 1385, 2028, 1408),
            17,
            new Skill(
                "ガードアシストC Ⅳ",
                "味方1～3体のDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Four,
                Range.C
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "おこづかいのゆくえ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1386, 2052, 1401, 1998),
            17,
            new Skill(
                "Sp.ガードバーストA Ⅳ",
                "敵1体に特殊特大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Four,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "カワイイのシャッターチャンス",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2013, 1416, 2063, 1387),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "コ:MP消費DOWN Ⅱ",
                "コマンド実行時、一定確率でMP消費を抑える。",
                Trigger.Command,
                new SupportEffect[]
                {
                    new MpCostDown()
                },
                Level.Two
            )
        ),

        new Memoria(
            "不滅のホワイトナイト",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1473, 2488, 1461, 1723),
            17,
            new Skill(
                "Sp.ガードバーストB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、敵のSp.DEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "果断なる漆黒の騎士",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2468, 1485, 1723, 1470),
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "不完全ゆえに愛おしく",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(2008, 1383, 2041, 1404),
            17,
            new Skill(
                "マイトアシストA Ⅲ",
                "味方1体のATKとDEFを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium),
                    new StatusUp(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "甘いスイーツでおもてなし♪",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2404, 1402, 1627, 1409),
            17,
            new Skill(
                "ヒールストライクA Ⅲ",
                "敵1体に通常大ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "見切れ希望女子",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2381, 1401, 1648, 1415),
            17,
            new Skill(
                "ガードブレイクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、敵のDEFをダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ハッピーハッピー☆タピオカ",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Water,
            new Status(1388, 1661, 2563, 1202),
            17,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "レンズに咲く百合の花",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(2063, 1413, 1996, 1384),
            17,
            new Skill(
                "パワーストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与え、自身のATKをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ダメージUP Ⅱ",
                "攻撃時、一定確率で攻撃ダメージを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new DamageUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "不器用なお姉様",
            new Rearguard(RearguardKind.Support),
            Element.Fire,
            new Status(1981, 1642, 1787, 1416),
            17,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅱ",
                "支援/妨害時、一定確率で支援/妨害効果を大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ワンショット",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1415, 1389, 1979, 2037),
            17,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:回復UP Ⅱ",
                "HP回復時、一定確率でHPの回復量を大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new RecoveryUp()
                },
                Level.Two
            )
        ),

        new Memoria(
            "ふたつのふれあい",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(2042, 2013, 1394, 1396),
            17,
            new Skill(
                "WパワーフォールA Ⅲ",
                "敵1体のATKとSp.ATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium),
                    new StatusDown(new SpAtk(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "援:パワーDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のATKを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アイドルリリィをつかまえて",
            new Vanguard(VanguardKind.NormalRange),
            Element.Water,
            new Status(2321, 1345, 1554, 1324),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "うさぎになったカメ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1323, 1554, 1677, 1970),
            17,
            new Skill(
                "Sp.ガードスマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "迷子のクマ",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(2321, 1311, 1556, 1314),
            17,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "晴れのちラムネ",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Wind,
            new Status(1387, 2393, 1418, 1632),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "湯けむりの園",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Wind,
            new Status(1317, 1978, 1925, 1327),
            17,
            new Skill(
                "ヒールスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与える。さらに自身のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:パワーDOWN Ⅱ",
                "攻撃時、一定確率で敵のATKを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "姫歌イメチェン大作戦!!",
            new Rearguard(RearguardKind.Recovery),
            Element.Wind,
            new Status(1316, 1346, 1907, 1976),
            17,
            new Skill(
                "ヒールD Ⅲ",
                "味方2体のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.D
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ガラスの中の大切な世界",
            new Rearguard(RearguardKind.Interference),
            Element.Wind,
            new Status(1346, 1334, 2321, 1548),
            17,
            new Skill(
                "ガードフォールB Ⅲ",
                "敵1～2体のDEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Def(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "あたしがこの手で守るもの",
            new Rearguard(RearguardKind.Support),
            Element.Water,
            new Status(2300, 1560, 1324, 1317),
            17,
            new Skill(
                "パワーアシストB Ⅲ",
                "味方1～2体のATKを大アップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "CHARMにお疲れ様",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Fire,
            new Status(1312, 2022, 1347, 1849),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のSp.DEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "一柳隊の知恵袋",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Fire,
            new Status(1930, 1348, 1965, 1318),
            17,
            new Skill(
                "Sp.ガードストライクA Ⅲ",
                "敵1体に通常大ダメージを与え、自身のSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.DEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "ロイヤル・ホスピタリティ",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1968, 1345, 1914, 1311),
            17,
            new Skill(
                "パワーフォールB Ⅲ",
                "敵1～2体のATKを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new Atk(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:ガードDOWN Ⅱ",
                "支援/妨害時、一定確率で敵前衛1体のDEFを大ダウンさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "わたしにできること",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1345, 1558, 1907, 1748),
            17,
            new Skill(
                "スマッシュA Ⅲ",
                "敵1体に特殊特大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "安らぎの帰り道",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1312, 1343, 1925, 1945),
            17,
            new Skill(
                "ヒールA Ⅲ",
                "味方1体のHPを特大回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "回:Sp.パワーUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "猫の誘惑",
            new Vanguard(VanguardKind.SpecialRange),
            Element.Water,
            new Status(1320, 2303, 1322, 1583),
            17,
            new Skill(
                "スマッシュB Ⅲ",
                "敵1～2体に特殊大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のDEFを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardUp(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "星降る夜の約束",
            new Rearguard(RearguardKind.Recovery),
            Element.Fire,
            new Status(1349, 1314, 1582, 2306),
            17,
            new Skill(
                "ヒールC Ⅲ",
                "味方1～3体のHPを回復する。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.C
            )
            ,
            new SupportSkill(
                "回:Sp.ガードUP Ⅱ",
                "HP回復時、一定確率で味方前衛1体のSp.DEFを大アップさせる。",
                Trigger.Recovery,
                new SupportEffect[]
                {
                    new GuardUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "私たちの正義",
            new Rearguard(RearguardKind.Interference),
            Element.Water,
            new Status(1346, 1914, 1324, 1969),
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:支援UP Ⅰ",
                "支援/妨害時、一定確率で支援/妨害効果をアップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new SupportUp()
                },
                Level.One
            )
        ),

        new Memoria(
            "放課後ファンタズム",
            new Vanguard(VanguardKind.NormalRange),
            Element.Fire,
            new Status(1970, 1338, 1904, 1337),
            17,
            new Skill(
                "ストライクB Ⅲ",
                "敵1～2体に通常大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),

        new Memoria(
            "雨上がりの朝稽古",
            new Rearguard(RearguardKind.Interference),
            Element.Fire,
            new Status(1337, 1925, 1336, 1952),
            17,
            new Skill(
                "Sp.ガードフォールB Ⅲ",
                "敵1～2体のSp.DEFを大ダウンさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusDown(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.B
            )
            ,
            new SupportSkill(
                "援:Sp.パワーUP Ⅱ",
                "支援/妨害時、一定確率で味方前衛1体のSp.ATKを大アップさせる。",
                Trigger.Support,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "アフタヌーンティー",
            new Vanguard(VanguardKind.SpecialSingle),
            Element.Fire,
            new Status(1335, 2297, 1335, 1575),
            17,
            new Skill(
                "Sp.ガードスマッシュA Ⅲ",
                "敵1体に特殊大ダメージを与え、自身のSp.DEFをアップさせる。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                    new StatusUp(new SpDef(), Amount.Medium)
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:Sp.パワーUP Ⅱ",
                "前衛から攻撃時、一定確率で自身のSp.ATKを大アップさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new PowerUp(Type.Special)
                },
                Level.Two
            )
        ),

        new Memoria(
            "Dear Schutzengel",
            new Vanguard(VanguardKind.NormalSingle),
            Element.Wind,
            new Status(1954, 1313, 1898, 1345),
            17,
            new Skill(
                "ストライクA Ⅲ",
                "敵1体に通常特大ダメージを与える。",
                new SkillEffect[]
                {
                },
                new StatusChange[]
                {
                },
                Level.Three,
                Range.A
            )
            ,
            new SupportSkill(
                "攻:ガードDOWN Ⅱ",
                "攻撃時、一定確率で敵のDEFを大ダウンさせる。",
                Trigger.Attack,
                new SupportEffect[]
                {
                    new GuardDown(Type.Normal)
                },
                Level.Two
            )
        ),
    };
}