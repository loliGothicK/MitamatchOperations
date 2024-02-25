using System;
using System.Text.Json;

namespace Mitama.Domain;

public abstract record Position : IComparable<Position>
{
    internal abstract int GetCategory();
    public abstract string Display { get; }

    public static Position FromStr(string pos) => pos switch
    {
        "N.Attacker" => new Front(FrontCategory.Normal),
        "Sp.Attacker" => new Front(FrontCategory.Special),
        "Buffer" => new Back(BackCategory.Buffer),
        "DeBuffer" => new Back(BackCategory.DeBuffer),
        "Healer" => new Back(BackCategory.Healer),
        _ => throw new ArgumentOutOfRangeException(nameof(pos), pos, null)
    };

    public int CompareTo(Position other) => GetCategory().CompareTo(other?.GetCategory());
}

public record Front(FrontCategory Category) : Position
{
    internal override int GetCategory() => (int)Category;
    public override string Display
    {
        get
        {
            return Category switch
            {
                FrontCategory.Normal => "通常前衛",
                FrontCategory.Special => "特殊前衛",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

public enum FrontCategory
{
    Normal = 0,
    Special = 1
}

public record Back(BackCategory Category) : Position
{
    internal override int GetCategory() => (int)Category;

    public override string Display
    {
        get
        {
            return Category switch
            {
                BackCategory.Buffer => "支援",
                BackCategory.DeBuffer => "妨害",
                BackCategory.Healer => "回復",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

public enum BackCategory
{
    Buffer = 2,
    DeBuffer = 3,
    Healer = 4
}

public record struct MemoriaIdAndConcentration(int Id, int Concentration);

public enum ExInfo
{
    None,
    Active,
    Inactive,
}
public record struct CostumeIndexAndEx(int Index, ExInfo Ex);

public record MemberInfo(
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Name,
    Position Position,
    int[] OrderIndices,
    MemoriaIdAndConcentration[] Memorias,
    CostumeIndexAndEx[] Costumes,
    int? Version = 2
)
{
    internal string PositionInfo => Position switch
    {
        Front(var category) => category switch
        {
            FrontCategory.Normal => @"通常前衛",
            FrontCategory.Special => @"特殊前衛",
            _ => throw new ArgumentOutOfRangeException()
        },
        Back(var category) => category switch
        {
            BackCategory.Buffer => @"支援",
            BackCategory.DeBuffer => @"妨害",
            BackCategory.Healer => @"回復",
            _ => throw new ArgumentOutOfRangeException()
        },
        _ => throw new ArgumentOutOfRangeException()
    };

    internal static MemberInfo FromJson(string json)
        => JsonSerializer.Deserialize<MemberDto>(json.Replace("Concenration", "Concentration"));

    internal string ToJson()
    {
        var dto = new MemberDto
        {
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            Name = Name,
            Position = Position.GetCategory(),
            OrderIndices = OrderIndices,
            Memorias = Memorias,
            Costumes = Costumes
        };
        var json = JsonSerializer.Serialize(dto);
        return json;
    }
    private record struct MemberDto(
        DateTime CreatedAt,
        DateTime UpdatedAt,
        string Name,
        int Position,
        int[] OrderIndices,
        MemoriaIdAndConcentration[] Memorias,
        CostumeIndexAndEx[] Costumes,
        int? Version
    )
    {
        public static implicit operator MemberInfo(MemberDto dto) => new(
            dto.CreatedAt,
            dto.UpdatedAt,
            dto.Name,
            dto.Position switch
            {
                0 => new Front(FrontCategory.Normal),
                1 => new Front(FrontCategory.Special),
                2 => new Back(BackCategory.Buffer),
                3 => new Back(BackCategory.DeBuffer),
                4 => new Back(BackCategory.Healer),
                _ => throw new ArgumentOutOfRangeException(nameof(dto.Position)),
            },
            dto.OrderIndices,
            dto.Memorias,
            dto.Costumes,
            dto.Version
        );
    }
}
