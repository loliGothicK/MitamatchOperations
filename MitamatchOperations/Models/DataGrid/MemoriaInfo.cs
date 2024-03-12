using Mitama.Domain;

namespace Mitama.Models.DataGrid;

public class MemoriaInfo(MemoriaIdAndConcentration raw)
{
    public int ID { get; set; } = raw.Id;
    public string Name { get; set; } = Memoria.Of(raw.Id).Name;
    public int Atk { get; set; } = Memoria.Of(raw.Id).Status[raw.Concentration].Atk;
    public int Def { get; set; } = Memoria.Of(raw.Id).Status[raw.Concentration].Def;
    public int SpAtk { get; set; } = Memoria.Of(raw.Id).Status[raw.Concentration].SpAtk;
    public int SpDef { get; set; } = Memoria.Of(raw.Id).Status[raw.Concentration].SpDef;
    public int Concentration { get; set; } = raw.Concentration;
}
