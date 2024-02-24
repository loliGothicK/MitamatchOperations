using mitama.Domain;

namespace mitama.Models.DataGrid;

public class MemoriaInfo(MemoriaIdAndConcentration raw)
{
    public int ID { get; set; } = raw.Id;
    public string Name { get; set; } = Memoria.Of(raw.Id).Name;
    public int Concentration { get; set; } = raw.Concentration;
}
