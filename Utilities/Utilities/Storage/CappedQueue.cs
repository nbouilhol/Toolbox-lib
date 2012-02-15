using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Utilities.Storage
{
    public abstract class CappedQueue<T> : ConcurrentQueue<T> where T : class
    {
        private readonly int capLimit;
        private readonly int timeLimit;
        private readonly Timer timer;
        private TaskFactory factory;

        public event Action<IEnumerable<T>> OnPublish = delegate { };

        protected CappedQueue(int capLimit, int timeLimit)
        {
            this.capLimit = capLimit;
            this.timeLimit = timeLimit;
            this.factory = new TaskFactory();
            this.timer = new Timer();
            this.timer.AutoReset = false;
            this.timer.Interval = timeLimit * 1000;
            this.timer.Elapsed += new ElapsedEventHandler((s, e) => { Publish(); });
            this.timer.Start();
        }

        public virtual new void Enqueue(T item)
        {
            base.Enqueue(item);
            if (Count >= capLimit) Publish();
        }

        protected virtual void Publish()
        {
            factory.StartNew(t =>
            {
                ((Timer)t).Stop();
                OnPublish(Dequeue());
                ((Timer)t).Start();
            }, timer);
        }

        private IEnumerable<T> Dequeue()
        {
            T item;
            while (TryDequeue(out item))
                yield return item;
        }
    }
}
