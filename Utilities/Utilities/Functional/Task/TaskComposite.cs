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

            AddTasks(tasks);
        }

        public TaskComposite(IEnumerable<ITask> tasks)
        {
            Contract.Requires(tasks != null);

            AddTasks(tasks);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(composedTasks != null);
        }

        public void Do()
        {
            while (!composedTasks.IsCompleted)
            {
                ITask task;
                if (composedTasks.TryTake(out task, TimeSpan.FromSeconds(1.0)))
                    if (task != null) task.Do();
            }
        }

        public void AddTask(ITask task)
        {
            if (task == null)
                return;
            composedTasks.Add(task);
        }

        public void AddTasks(IEnumerable<ITask> tasks)
        {
            if (tasks == null)
                return;
            foreach (var task in tasks)
                composedTasks.Add(task);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (composedTasks != null)
                    composedTasks.Dispose();
        }

        //~TaskComposite()
        //{
        //    Dispose(false);
        //}
    }
}