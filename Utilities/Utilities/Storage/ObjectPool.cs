using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Utilities.Functional;

namespace Utilities.Storage
{
    public sealed class ObjectPool<T> : IProducerConsumerCollection<T>
    {
        private readonly Func<T> _generator;
        private readonly IProducerConsumerCollection<T> _objects;

        public ObjectPool()
            : this(() => default(T), new ConcurrentQueue<T>(), null)
        {
        }

        public ObjectPool(Func<T> generator)
            : this(generator, new ConcurrentQueue<T>(), null)
        {
            Contract.Requires(generator != null);
        }

        public ObjectPool(Func<T> generator, IProducerConsumerCollection<T> objects)
            : this(generator, objects, null)
        {
            Contract.Requires(objects != null);
            Contract.Requires(generator != null);
        }

        public ObjectPool(IEnumerable<T> items)
            : this(() => default(T), new ConcurrentQueue<T>(), items)
        {
        }

        public ObjectPool(Func<T> generator, IProducerConsumerCollection<T> objects, IEnumerable<T> items)
        {
            Contract.Requires(objects != null);
            Contract.Requires(generator != null);

            _generator = generator;
            _objects = objects;
            if (items != null) Adds(items);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_generator != null);
            Contract.Invariant(_objects != null);
        }

        public ObjectPool<T> Add(T item)
        {
            _objects.TryAdd(item);
            return this;
        }

        public bool TryAdd(T item)
        {
            Add(item);
            return item != null;
        }

        public ObjectPool<T> Adds(IEnumerable<T> items)
        {
            Contract.Requires(items != null);

            items.ForEach(i => _objects.TryAdd(i));
            return this;
        }

        public bool TryAdds(IEnumerable<T> items)
        {
            Contract.Requires(items != null);

            return items.Select(TryAdd).All(i => i == true);
        }

        public T Take()
        {
            T value;
            return _objects.TryTake(out value) ? value : _generator();
        }

        public bool TryTake(out T item)
        {
            item = Take();
            return item != null;
        }

        public IEnumerable<T> Takes()
        {
            T value;
            while (_objects.TryTake(out value)) yield return value;
        }

        public T[] ToArray()
        {
            return _objects.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        public void CopyTo(T[] array, int index)
        {
            _objects.CopyTo(array, index);
        }

        public void CopyTo(Array array, int index)
        {
            _objects.CopyTo(array, index);
        }

        public int Count
        {
            get { return _objects.Count; }
        }

        public bool IsSynchronized
        {
            get { return _objects.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _objects.SyncRoot; }
        }
    }
}