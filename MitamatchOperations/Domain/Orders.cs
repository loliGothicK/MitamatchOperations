using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Mitama.Domain.OrderKinds;
using Mitama.Pages.Common;

namespace Mitama.Domain
{
    public abstract record Kind;

    namespace OrderKinds
    {
        public record Elemental(Element Element) : Kind;
        public record Buff : Kind;
        public record DeBuff : Kind;
        public record Mp : Kind;
        public record TriggerRateFluctuation : Kind;
        public record Shield : Kind;
        public record Formation : Kind;
        public record Stack : Kind;
        public record Other : Kind;
    }

    public class Kinds
    {
        public class Elemental
        {
            public static OrderKinds.Elemental Fire => new(Element.Fire);
            public static OrderKinds.Elemental Water => new(Element.Water);
            public static OrderKinds.Elemental Wind => new(Element.Wind);
            public static OrderKinds.Elemental Light => new(Element.Light);
            public static OrderKinds.Elemental Dark => new(Element.Dark);
            public static OrderKinds.Elemental Special => new(Element.Special);
        }

        public static Buff Buff => new();
        public static DeBuff DeBuff => new();
        public static Mp Mp => new();
        public static TriggerRateFluctuation TriggerRateFluctuation => new();
        public static Shield Shield => new();
        public static Formation Formation => new();
        public static Stack Stack => new();
        public static Other Other => new();
    }

    public enum Element
    {
        Fire,
        Water,
        Wind,
        Light,
        Dark,
        Special
    }

    public readonly record struct BasicStatus(int Atk = 0, int SpAtk = 0, int Def = 0, int SpDef = 0)
    {
        public int[] ToArray() => [Atk, SpAtk, Def, SpDef];

        public static implicit operator BasicStatus(ValueTuple<int, int, int, int> from) => new()
        {
            Atk = from.Item1,
            SpAtk = from.Item2,
            Def = from.Item3,
            SpDef = from.Item4,
        };

        public static BasicStatus FromRaw(int[] from) => new()
        {
            Atk = from[0],
            SpAtk = from[1],
            Def = from[2],
            SpDef = from[3],
        };

        public int ASA => Atk + SpAtk;
        public int DSD => Def + SpDef;

        public static BasicStatus operator +(BasicStatus a, BasicStatus b) => new()
        {
            Atk = a.Atk + b.Atk,
            SpAtk = a.SpAtk + b.SpAtk,
            Def = a.Def + b.Def,
            SpDef = a.SpDef + b.SpDef,
        };
    }

    public record struct OrderIndexAndTime(int Index, TimeOnly Time);

    public readonly record struct Order(
        int Index,
        string Name,
        BasicStatus Status,
        string Effect,
        string Description,
        int PrepareTime,
        int ActiveTime,
        bool Payed,
        Kind Kind,
        bool HasTemplate
    )
    {
        public string Path => $@"{Director.OrderImageDir()}\{Name}.png";
        public Uri TemplateUri => new($"ms-appx:///Assets/OrderTemplates/{Name}.png");

        public string TimeFmt => ActiveTime switch
        {
            0 => $"({PrepareTime} sec)",
            _ => $"({PrepareTime} + {ActiveTime} sec)"
        };

        public static Order Of(int index) => List.Value[^(index + 1)];

        public string ToPrettyJSON()
        {
            var json = new
            {
                id = Index,
                name = Name,
                status = Status.ToArray(),
                effect = Effect,
                description = Description,
                prepare_time = PrepareTime,
                active_time = ActiveTime,
                payed = Payed,
                kind = Kind switch
                {
                    Elemental(var elem) => $"Elemental/{elem}",
                    Buff => "Buff",
                    DeBuff => "DeBuff",
                    Mp => "Mp",
                    TriggerRateFluctuation => "TriggerRateFluctuation",
                    Shield => "Shield",
                    Formation => "Formation",
                    Stack => "Stack",
                    Other => "Other",
                    _ => throw new NotImplementedException($"{Kind}"),
                },
                has_template = HasTemplate,
            };
            // with indent
            return System.Text.Json.JsonSerializer.Serialize(json, options: new() { WriteIndented = true });
        }

        public static readonly Lazy<Order[]> List = new(() =>
        {
            List<Order> list = [];
            foreach (var poco in Repository.Repository.LiteDB.List<Repository.Order.POCO>())
            {
                list.Add(new Order(
                    poco.id - 1,
                    poco.name,
                    BasicStatus.FromRaw(poco.status),
                    poco.effect,
                    poco.description,
                    poco.prepare_time,
                    poco.active_time,
                    poco.payed,
                    poco.kind switch
                    {
                        "Elemental/Fire" => new Elemental(Element.Fire),
                        "Elemental/Water" => new Elemental(Element.Water),
                        "Elemental/Wind" => new Elemental(Element.Wind),
                        "Elemental/Light" => new Elemental(Element.Light),
                        "Elemental/Dark" => new Elemental(Element.Dark),
                        "Elemental/Special" => new Elemental(Element.Special),
                        "Buff" => new Buff(),
                        "DeBuff" => new DeBuff(),
                        "Mp" => new Mp(),
                        "TriggerRateFluctuation" => new TriggerRateFluctuation(),
                        "Shield" => new Shield(),
                        "Formation" => new Formation(),
                        "Stack" => new Stack(),
                        "Other" => new Other(),
                        _ => throw new NotImplementedException($"{poco.kind}"),
                    },
                    poco.has_template
                ));
            }
            list.Sort((a, b) => b.Index.CompareTo(a.Index));
            return [.. list];
        });

        public static readonly Order[] ElementalOrders = List.Value.Where(order => order.Kind is Elemental).ToArray();
        public static readonly Order[] BuffOrders = List.Value.Where(order => order.Kind is Buff).ToArray();
        public static readonly Order[] DeBuffOrders = List.Value.Where(order => order.Kind is DeBuff).ToArray();
        public static readonly Order[] ShieldOrders = List.Value.Where(order => order.Kind is Shield).ToArray();
        public static readonly Order[] MpOrders = List.Value.Where(order => order.Kind is Mp).ToArray();
        public static readonly Order[] TriggerRateFluctuationOrders = List.Value.Where(order => order.Kind is TriggerRateFluctuation).ToArray();
        public static readonly Order[] FormationOrders = List.Value.Where(order => order.Kind is Formation).ToArray();
        public static readonly Order[] StackOrders = List.Value.Where(order => order.Kind is Stack).ToArray();
        public static readonly Order[] OtherOrders = List.Value.Where(order => order.Kind is Other).ToArray();
    }
}
