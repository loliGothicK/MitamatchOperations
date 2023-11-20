using mitama.Domain;
using mitama.Pages.DeckBuilder;

internal static class BuilderPageHelpers
{
    public static KindType ToKindType(MemoriaKind kind)
    {
        return kind switch
        {
            Vanguard(VanguardKind.NormalSingle) => KindType.NormalSingle,
            Vanguard(VanguardKind.NormalRange) => KindType.NormalRange,
            Vanguard(VanguardKind.SpecialSingle) => KindType.SpecialSingle,
            Vanguard(VanguardKind.SpecialRange) => KindType.SpecialRange,
            Rearguard(RearguardKind.Support) => KindType.Support,
            Rearguard(RearguardKind.Interference) => KindType.Interference,
            Rearguard(RearguardKind.Recovery) => KindType.Recovery,
            _ => throw new System.NotImplementedException(),
        };
    }

    public static string KindTypeToString(KindType type)
    {
        return type switch
        {
            KindType.NormalSingle => "通常単体",
            KindType.NormalRange => "通常範囲",
            KindType.SpecialSingle => "特殊単体",
            KindType.SpecialRange => "特殊範囲",
            KindType.Support => "支援",
            KindType.Interference => "妨害",
            KindType.Recovery => "回復",
            _ => "その他",
        };
    }

    public static SkillType ToSkillType(StatusChange eff)
    {
        return eff switch
        {
            StatusUp c when c.Stat is Atk => SkillType.Au,
            StatusUp c when c.Stat is Def => SkillType.Du,
            StatusUp c when c.Stat is SpAtk => SkillType.SAu,
            StatusUp c when c.Stat is SpDef => SkillType.SDu,
            StatusDown c when c.Stat is Atk => SkillType.Ad,
            StatusDown c when c.Stat is Def => SkillType.Dd,
            StatusDown c when c.Stat is SpAtk => SkillType.SAd,
            StatusDown c when c.Stat is SpDef => SkillType.SDd,
            StatusUp(ElementAttack(Element.Fire), _) => SkillType.FPu,
            StatusUp(ElementAttack(Element.Water), _) => SkillType.WaPu,
            StatusUp(ElementAttack(Element.Wind), _) => SkillType.WiPu,
            StatusUp(ElementAttack(Element.Light), _) => SkillType.LPu,
            StatusUp(ElementAttack(Element.Dark), _) => SkillType.DPu,
            StatusUp(ElementGuard(Element.Fire), _) => SkillType.FGu,
            StatusUp(ElementGuard(Element.Water), _) => SkillType.WaGu,
            StatusUp(ElementGuard(Element.Wind), _) => SkillType.WiGu,
            StatusUp(ElementGuard(Element.Light), _) => SkillType.LGu,
            StatusUp(ElementGuard(Element.Dark), _) => SkillType.DGu,
            StatusDown(ElementAttack(Element.Fire), _) => SkillType.FPd,
            StatusDown(ElementAttack(Element.Water), _) => SkillType.WaPd,
            StatusDown(ElementAttack(Element.Wind), _) => SkillType.WiPd,
            StatusDown(ElementAttack(Element.Light), _) => SkillType.LPd,
            StatusDown(ElementAttack(Element.Dark), _) => SkillType.DPd,
            StatusDown(ElementGuard(Element.Fire), _) => SkillType.FGd,
            StatusDown(ElementGuard(Element.Water), _) => SkillType.WaGd,
            StatusDown(ElementGuard(Element.Wind), _) => SkillType.WiGd,
            StatusDown(ElementGuard(Element.Light), _) => SkillType.LGd,
            StatusDown(ElementGuard(Element.Dark), _) => SkillType.DGd,
            _ => SkillType.Other,
        };
    }
    public static string SkillTypeToString(SkillType type)
    {
        return type switch
        {
            SkillType.Au => "攻UP",
            SkillType.Du => "防UP",
            SkillType.SAu => "特攻UP",
            SkillType.SDu => "特防UP",
            SkillType.Ad => "攻DOWN",
            SkillType.Dd => "防DOWN",
            SkillType.SAd => "特攻DOWN",
            SkillType.SDd => "特防DOWN",
            SkillType.FPu => "火攻UP",
            SkillType.WaPu => "水攻UP",
            SkillType.WiPu => "風攻UP",
            SkillType.LPu => "光攻UP",
            SkillType.DPu => "闇攻UP",
            SkillType.FGu => "火防UP",
            SkillType.WaGu => "水防UP",
            SkillType.WiGu => "風防UP",
            SkillType.LGu => "光防UP",
            SkillType.DGu => "闇防UP",
            SkillType.FPd => "火攻DOWN",
            SkillType.WaPd => "水攻DOWN",
            SkillType.WiPd => "風攻DOWN",
            SkillType.LPd => "光攻DOWN",
            SkillType.DPd => "闇攻DOWN",
            SkillType.FGd => "火防DOWN",
            SkillType.WaGd => "水防DOWN",
            SkillType.WiGd => "風防DOWN",
            SkillType.LGd => "光防DOWN",
            SkillType.DGd => "闇防DOWN",
            _ => "その他",
        };
    }

    public static SupportType ToSupportType(SupportEffect eff)
    {
        return eff switch {
            NormalMatchPtUp => SupportType.NormalMatchPtUp,
            SpecialMatchPtUp => SupportType.SpecialMatchPtUp,
            DamageUp => SupportType.DamageUp,
            PowerUp(Type.Normal) => SupportType.PowerUp,
            PowerDown(Type.Normal) => SupportType.PowerDown,
            GuardUp(Type.Normal) => SupportType.GuardUp,
            GuardDown(Type.Normal) => SupportType.GuardDown,
            PowerUp(Type.Special) => SupportType.SpPowerUp,
            PowerDown(Type.Special) => SupportType.SpPowerDown,
            GuardUp(Type.Special) => SupportType.SpGuardUp,
            GuardDown(Type.Special) => SupportType.SpGuardDown,
            ElementPowerUp(Element.Fire) => SupportType.FirePowerUp,
            ElementPowerDown(Element.Fire) => SupportType.FirePowerDown,
            ElementGuardUp(Element.Fire) => SupportType.FireGuardUp,
            ElementGuardDown(Element.Fire) => SupportType.FireGuardDown,
            ElementPowerUp(Element.Water) => SupportType.WaterPowerUp,
            ElementPowerDown(Element.Water) => SupportType.WaterPowerDown,
            ElementGuardUp(Element.Water) => SupportType.WaterGuardUp,
            ElementGuardDown(Element.Water) => SupportType.WaterGuardDown,
            ElementPowerUp(Element.Wind) => SupportType.WindPowerUp,
            ElementPowerDown(Element.Wind) => SupportType.WindPowerDown,
            ElementGuardUp(Element.Wind) => SupportType.WindGuardUp,
            ElementGuardDown(Element.Wind) => SupportType.WindGuardDown,
            SupportUp => SupportType.SupportUp,
            RecoveryUp => SupportType.RecoveryUp,
            MpCostDown => SupportType.MpCostDown,
            _ => throw new System.NotImplementedException(),
        };
    }
    public static string SupportTypeToString(SupportType type)
    {
        return type switch
        {
            SupportType.NormalMatchPtUp => "通常マッチPt UP",
            SupportType.SpecialMatchPtUp => "特殊マッチPt UP",
            SupportType.DamageUp => "ダメUP",
            SupportType.PowerUp => "攻UP",
            SupportType.PowerDown => "攻DOWN",
            SupportType.GuardUp => "防UP",
            SupportType.GuardDown => "防DOWN",
            SupportType.SpPowerUp => "特攻UP",
            SupportType.SpPowerDown => "特攻DOWN",
            SupportType.SpGuardUp => "特防UP",
            SupportType.SpGuardDown => "特防DOWN",
            SupportType.FirePowerUp => "火攻UP",
            SupportType.FirePowerDown => "火攻DOWN",
            SupportType.FireGuardUp => "火防UP",
            SupportType.FireGuardDown => "火防DOWN",
            SupportType.WaterPowerUp => "水攻UP",
            SupportType.WaterPowerDown => "水攻DOWN",
            SupportType.WaterGuardUp => "水防UP",
            SupportType.WaterGuardDown => "水防DOWN",
            SupportType.WindPowerUp => "風攻UP",
            SupportType.WindPowerDown => "風攻DOWN",
            SupportType.WindGuardUp => "風防UP",
            SupportType.WindGuardDown => "風防DOWN",
            SupportType.SupportUp => "支援UP",
            SupportType.RecoveryUp => "回復UP",
            SupportType.MpCostDown => "MP軽減",
            _ => "その他",
        };
    }

}