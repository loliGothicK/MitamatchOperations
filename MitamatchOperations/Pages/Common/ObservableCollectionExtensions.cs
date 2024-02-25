using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace mitama.Pages.Common;

public static class ObservableCollectionExtensions
{
    public static void RemoveWhere<T>(this ObservableCollection<T> collection, Predicate<T> pred)
    {
        for (int i = collection.Count - 1; i >= 0; i--)
        {
            if (pred(collection[i]))
            {
                collection.RemoveAt(i);
            }
        }
    }

    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
