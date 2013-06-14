using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace Utilities.Functional.Task
{
    public class TaskAsync : ITaskCancellable, IDisposable
    {
        private readonly System.Threading.Tasks.Task sysTask;
        private readonly CancellationTokenSource cancellationTokenSource;

        public TaskAsync(ITask task)
        {
            Contract.Requires(task != null);

            this.sysTask = new System.Threading.Tasks.Task(() => task.Do());
        }

        public TaskAsync(ITask task, CancellationTokenSource cancellationTokenSource)
        {
            Contract.Requires(task != null);
            Contract.Requires(cancellationTokenSource != null);

            this.cancellationTokenSource = cancellationTokenSource;
            CancellationToken token = cancellationTokenSource.Token;
            this.sysTask = new System.Threading.Tasks.Task(() =>
            {
                token.ThrowIfCancellationRequested();
                task.Do();
            }, token);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(sysTask != null);
        }

        public void Do()
        {
            this.sysTask.Start();
        }

        public void Cancel()
        {
            if (cancellationTokenSource != null) this.cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) if (this.sysTask != null) this.sysTask.Dispose();
        }

        //~TaskAsync()
        //{
        //    Dispose(false);
        //}
    }
}