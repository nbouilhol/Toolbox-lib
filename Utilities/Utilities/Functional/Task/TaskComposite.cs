using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Utilities.Functional.Task
{
    public class TaskComposite : ITask, IDisposable
    {
        private BlockingCollection<ITask> composedTasks = new BlockingCollection<ITask>();

        public TaskComposite(params ITask[] tasks)
        {
            Contract.Requires(tasks != null);

            this.AddTasks(tasks);
        }

        public TaskComposite(IEnumerable<ITask> tasks)
        {
            Contract.Requires(tasks != null);

            this.AddTasks(tasks);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(composedTasks != null);
        }

        public void Do()
        {
            while (!this.composedTasks.IsCompleted)
            {
                ITask task;
                if (this.composedTasks.TryTake(out task, TimeSpan.FromSeconds(1.0)))
                    if (task != null) task.Do();
            }
        }

        public void AddTask(ITask task)
        {
            if (task == null)
                return;
            this.composedTasks.Add(task);
        }

        public void AddTasks(IEnumerable<ITask> tasks)
        {
            if (tasks == null)
                return;
            foreach (var task in tasks)
                this.composedTasks.Add(task);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (this.composedTasks != null)
                    this.composedTasks.Dispose();
        }

        //~TaskComposite()
        //{
        //    Dispose(false);
        //}
    }
}