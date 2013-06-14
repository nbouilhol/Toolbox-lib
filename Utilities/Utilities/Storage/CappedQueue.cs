using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Utilities.Storage
{
    public abstract class CappedQueue<T> : ConcurrentQueue<T> where T : class
    {
        private readonly int _capLimit;
        private readonly TaskFactory _factory;
        private readonly Timer _timer;

        protected CappedQueue(int capLimit, int timeLimit)
        {
            _capLimit = capLimit;
            _factory = new TaskFactory();
            _timer = new Timer {AutoReset = false, Interval = timeLimit*1000};
            _timer.Elapsed += (s, e) => Publish();
            _timer.Start();
        }

        public event Action<IEnumerable<T>> OnPublish = delegate { };

        public new virtual void Enqueue(T item)
        {
            base.Enqueue(item);
            if (Count >= _capLimit) Publish();
        }

        protected virtual void Publish()
        {
            _factory.StartNew(t =>
            {
                ((Timer) t).Stop();
                OnPublish(Dequeue());
                ((Timer) t).Start();
            }, _timer);
        }

        private IEnumerable<T> Dequeue()
        {
            T item;
            while (TryDequeue(out item))
                yield return item;
        }
    }
}