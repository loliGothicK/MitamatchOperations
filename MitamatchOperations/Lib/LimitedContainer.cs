using System;
using System.Collections.Generic;

public class LimitedContainer<T>
{
    private readonly int _capacity;
    private readonly Queue<T> _queue = [];

    public int Length => _queue.Count;

    public LimitedContainer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be a positive integer.");

        _capacity = capacity;
    }

    public void Add(T item)
    {
        if (_queue.Count >= _capacity)
        {
            _queue.Dequeue(); // Remove oldest item if capacity is reached
        }
        _queue.Enqueue(item); // Add new item
    }

    public IEnumerable<T> GetItems()
    {
        return [.. _queue];
    }
}