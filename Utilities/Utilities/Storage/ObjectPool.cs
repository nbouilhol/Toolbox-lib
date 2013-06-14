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
        private readonly Func<T> generator;
        private readonly IProducerConsumerCollection<T> objects;

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

            this.generator = generator;
            this.objects = objects;
            if (items != null) Adds(items);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.generator != null);
            Contract.Invariant(this.objects != null);
        }

        public ObjectPool<T> Add(T item)
        {
            objects.TryAdd(item);
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

            items.ForEach(i => objects.TryAdd(i));
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
            return objects.TryTake(out value) ? value : generator();
        }

        public bool TryTake(out T item)
        {
            item = Take();
            return item != null;
        }

        public IEnumerable<T> Takes()
        {
            T value;
            while (objects.TryTake(out value)) yield return value;
        }

        public T[] ToArray()
        {
            return objects.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        public void CopyTo(T[] array, int index)
        {
            objects.CopyTo(array, index);
        }

        public void CopyTo(Array array, int index)
        {
            objects.CopyTo(array, index);
        }

        public int Count
        {
            get { return objects.Count; }
        }

        public bool IsSynchronized
        {
            get { return objects.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return objects.SyncRoot; }
        }
    }
}