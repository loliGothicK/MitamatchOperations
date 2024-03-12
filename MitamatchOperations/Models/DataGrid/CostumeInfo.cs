using Mitama.Domain;
using Newtonsoft.Json.Linq;

namespace Mitama.Models.DataGrid;

public class CostumeInfo
{
    public int ID { get; init; }
    public string Lily { get; init; }
    public string Name { get; init; }
    public string RareSkill { get; init; }
    public string ExSkill { get; init; }
    public string Ex { get; init; }

    public CostumeInfo(CostumeIndexAndEx raw)
    {
        ID = raw.Index;
        var costume = Costume.Of(raw.Index);
        Lily = costume.Lily;
        Name = costume.Name;
        RareSkill = costume.RareSkill.Name;
        ExSkill = costume.ExSkill.HasValue ? costume.ExSkill.Value.Name : "-";
        Ex = raw.Ex switch
        {
            ExInfo.Active => "あり",
            ExInfo.Inactive => "なし",
            _ => "-"
        };
    }
}
