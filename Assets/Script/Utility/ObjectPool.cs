using System;
using System.Collections.Concurrent;

public class ObjectPool<T>
{
    private ConcurrentBag<T> _objects;
    private Func<T> _objectGenerator;

    public ObjectPool(Func<T> objectGenerator)
    {
        _objects = new ConcurrentBag<T>();
        _objectGenerator = objectGenerator ?? throw new ArgumentNullException("objectGenerator");
    }

    public T GetObject()
    {
        T item;
        if (_objects.TryTake(out item)) return item;
        return _objectGenerator();
    }

    public void PutObject(T item)
    {
        _objects.Add(item);
    }
}