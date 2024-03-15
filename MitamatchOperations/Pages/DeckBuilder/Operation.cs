using System.Collections.ObjectModel;
using System.Linq;
using Mitama.Domain;
using Mitama.Pages.Common;

namespace Mitama.Pages.DeckBuilder;

internal class OperationHistory
{
    private ObservableCollection<Operation> operations;
    private int cursor;

    public OperationHistory()
    {
        operations = [];
        cursor = -1;
    }

    public void Push(Operation operation)
    {
        operations = [.. operations.Take(cursor + 1)];
        operations.Add(operation);
        cursor++;
    }

    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        if (cursor == -1) return;
        operations[cursor].Undo(ref Deck, ref LegendaryDeck);
        cursor--;
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        if (cursor == operations.Count) return;
        operations[cursor + 1].Redo(ref Deck, ref LegendaryDeck);
        cursor++;
    }
}

internal interface Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck);

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck);
}

internal record AddMemoria(MemoriaIdAndConcentration[] items) : Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var item in items)
        {
            Deck.RemoveWhere(m =>  m.Memoria.Id == item.Id);
            LegendaryDeck.RemoveWhere(m => m.Memoria.Id == item.Id);
        }
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        var memorias = items.Select(item => new MemoriaWithConcentration(Memoria.Of(item.Id), item.Concentration));
        foreach (var item in memorias.Where(memoria => memoria.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(item);
        }
        foreach (var item in memorias.Where(memoria => !memoria.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(item);
        }
    }
}

internal record RemoveMemoria(MemoriaIdAndConcentration[] items) : Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        var memorias = items.Select(item => new MemoriaWithConcentration(Memoria.Of(item.Id), item.Concentration));
        foreach (var item in memorias.Where(memoria => memoria.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(item);
        }
        foreach (var item in memorias.Where(memoria => !memoria.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(item);
        }
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var item in items)
        {
            Deck.RemoveWhere(m => m.Memoria.Id == item.Id);
            LegendaryDeck.RemoveWhere(m => m.Memoria.Id == item.Id);
        }
    }
}

internal record Load(Unit Before, Unit After) : Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var memoria in Before.Memorias.Where(m => m.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(memoria);
        }
        foreach (var memoria in Before.Memorias.Where(m => !m.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(memoria);
        }
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var memoria in After.Memorias.Where(m => m.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(memoria);
        }
        foreach (var memoria in After.Memorias.Where(m => !m.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(memoria);
        }
    }
}

internal record Import(Unit Before, Unit After) : Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var memoria in Before.Memorias.Where(m => m.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(memoria);
        }
        foreach (var memoria in Before.Memorias.Where(m => !m.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(memoria);
        }
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var memoria in After.Memorias.Where(m => m.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(memoria);
        }
        foreach (var memoria in After.Memorias.Where(m => !m.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(memoria);
        }
    }
}
internal record Clear(Unit Before) : Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var memoria in Before.Memorias.Where(m => m.Memoria.Labels.Contains(Label.Legendary)))
        {
            LegendaryDeck.Add(memoria);
        }
        foreach (var memoria in Before.Memorias.Where(m => !m.Memoria.Labels.Contains(Label.Legendary)))
        {
            Deck.Add(memoria);
        }
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        Deck.Clear();
        LegendaryDeck.Clear();
    }

}

internal record ChangeConcentration(int Id, int Before, int After) : Operation
{
    public void Undo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var item in LegendaryDeck.ToList())
        {
            if (item.Memoria.Id == Id)
            {
                var newItem = item with { Concentration = Before };
                var idx = LegendaryDeck.IndexOf(item);
                LegendaryDeck[idx] = newItem;
            }
        }
        foreach (var item in Deck.ToList())
        {
            if (item.Memoria.Id == Id)
            {
                var newItem = item with { Concentration = Before };
                var idx = Deck.IndexOf(item);
                Deck[idx] = newItem;
            }
        }
    }

    public void Redo(
        ref ObservableCollection<MemoriaWithConcentration> Deck,
        ref ObservableCollection<MemoriaWithConcentration> LegendaryDeck)
    {
        foreach (var item in LegendaryDeck.ToList())
        {
            if (item.Memoria.Id == Id)
            {
                var newItem = item with { Concentration = After };
                var idx = LegendaryDeck.IndexOf(item);
                LegendaryDeck[idx] = newItem;
            }
        }
        foreach (var item in Deck.ToList())
        {
            if (item.Memoria.Id == Id)
            {
                var newItem = item with { Concentration = After };
                var idx = Deck.IndexOf(item);
                Deck[idx] = newItem;
            }
        }
    }
}
