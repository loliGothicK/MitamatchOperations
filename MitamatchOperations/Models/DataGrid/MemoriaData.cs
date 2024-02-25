using System.Diagnostics;
using mitama.Domain;

namespace mitama.Models.DataGrid;

public class MemoriaData(Memoria memoria)
{
    public int Id { get; } = memoria.Id;
    public string Link { get; } = memoria.Link;
    public string Name { get; } = memoria.Name;
    public string Kind { get; } = memoria.Kind switch
    {
        Vanguard(VanguardKind.NormalSingle)   => "通常単体",
        Vanguard(VanguardKind.NormalRange)    => "通常範囲",
        Vanguard(VanguardKind.SpecialSingle)  => "特殊単体",
        Vanguard(VanguardKind.SpecialRange)   => "特殊範囲",
        Rearguard(RearguardKind.Support)      => "支援",
        Rearguard(RearguardKind.Interference) => "妨害",
        Rearguard(RearguardKind.Recovery)     => "回復",
        _ => throw new UnreachableException(nameof(memoria.Kind)),
    };
    public string Element { get; } = memoria.Element switch
    {
        Domain.Element.Fire  => "火",
        Domain.Element.Water => "水",
        Domain.Element.Wind  => "風",
        Domain.Element.Light => "光",
        Domain.Element.Dark  => "闇",
        _ => throw new UnreachableException(nameof(memoria.Element)),
    };
    public int Cost { get; } = memoria.Cost;
    public string Skill { get; } = memoria.Skill.Name;
    public string SkillDescription { get; } = memoria.Skill.Description;
    public string SupportSkill { get; } = memoria.SupportSkill.Name;
    public string SupportSkillDescription { get; } = memoria.SupportSkill.Description;
}
