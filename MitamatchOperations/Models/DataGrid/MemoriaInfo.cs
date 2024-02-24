using mitama.Domain;

namespace mitama.Models.DataGrid;

public class OrderInfo(Order order)
{
    public int ID { get; set; } = order.Index;
    public string Name { get; set; } = order.Name;
    public int ATK { get; set; } = order.Status.Atk;
    public int DEF { get; set; } = order.Status.Def;
    public int SpATK { get; set; } = order.Status.SpAtk;
    public int SpDEF { get; set; } = order.Status.SpDef;
    public string Effect { get; set; } = order.Effect;
    public string Description { get; set; } = order.Description;
}
