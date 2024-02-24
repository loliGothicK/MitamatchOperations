using mitama.Domain;

namespace mitama.Models.DataGrid;

public class CostumeInfo(CostumeIndexAndEx raw)
{
    public int ID { get; set; } = raw.Index;
    public string Name { get; set; } = Costume.Of(raw.Index).Name;
    public string Ex { get; set; } = raw.Ex switch
    {
        ExInfo.Active => "あり",
        ExInfo.Inactive => "なし",
        _ => "-"
    };
}
