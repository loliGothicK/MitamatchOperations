using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using mitama.Pages;

namespace mitama.Domain;

public abstract record Position
{
    internal abstract int GetCategory();

    public static Position FromStr(string pos) => pos switch
    {
        "Sp.Attacker" => new Front(FrontCategory.Normal),
        "N.Attacker" => new Front(FrontCategory.Special),
        "Buffer" => new Back(BackCategory.Buffer),
        "DeBuffer" => new Back(BackCategory.DeBuffer),
        "Healer" => new Back(BackCategory.Healer),
    };

}

public record Front(FrontCategory Category) : Position
{
    internal override int GetCategory() => (int)Category;
}

public enum FrontCategory
{
    Normal = 0,
    Special = 1
}

public record Back(BackCategory Category) : Position
{
    internal override int GetCategory() => (int)Category;
}

public enum BackCategory
{
    Buffer = 2,
    DeBuffer = 3,
    Healer = 4
}

public record Member(
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Name,
    Position Position,
    ushort[] OrderIndices
)
{
    internal string PositionInfo => Position switch
    {
        Front(var category) => category switch
        {
            FrontCategory.Normal => @"通常前衛",
            FrontCategory.Special => @"特殊前衛",
        },
        Back(var category) => category switch
        {
            BackCategory.Buffer => @"支援",
            BackCategory.DeBuffer => @"妨害",
            BackCategory.Healer => @"回復",
        },
    };

    internal static Member FromJson(string json) => JsonSerializer.Deserialize<MemberDto>(json);
    internal string ToJson() => JsonSerializer.Serialize(new MemberDto
    {
        CreatedAt = CreatedAt,
        UpdatedAt = UpdatedAt,
        Name = Name,
        Position = Position.GetCategory(),
        OrderIndices = OrderIndices,

    });
    private record struct MemberDto(
        DateTime CreatedAt,
        DateTime UpdatedAt,
        string Name,
        int Position,
        ushort[] OrderIndices
    )
    {
        public static implicit operator Member(MemberDto dto) => new Member(
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
            dto.OrderIndices
        );
    }

    internal static ObservableCollection<GroupInfoList> LoadMembersGrouped(string region)
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var members = new ObservableCollection<Member>(Directory.GetFiles(@$"{desktop}\MitamatchOperations\Regions\{region}", "*.json").Select(path =>
        {
            using var sr = new StreamReader(path, Encoding.GetEncoding("UTF-8"));
            var json = sr.ReadToEnd();
            return FromJson(json);
        }));

        var query = members
            .GroupBy(member => member.Position)
            .OrderBy(group => group.Key.GetCategory())
            .Select(group => new GroupInfoList(group) { Key = group.Key });

        return new ObservableCollection<GroupInfoList>(query);
    }
}