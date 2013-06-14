using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TaskHelper
    {
        public static Task<int> ReadAsyncTpl(this Stream stream, byte[] buffer, int offset, int count)
        {
            Contract.Requires(stream != null);
            Contract.Requires(buffer != null);
            Contract.Requires(offset >= 0);
            Contract.Requires(count >= 0);
            Contract.Requires(count <= (buffer.Length - offset));

            var tcs = new TaskCompletionSource<int>();

            stream.BeginRead(buffer, offset, count, iasr =>
            {
                try
                {
                    tcs.SetResult(stream.EndRead(iasr));
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);

            return tcs.Task;
        }
    }

    public class TplAsyncOperation<TArg, TResult>
    {
        private Func<TArg, Task<TResult>> operation;

        public TplAsyncOperation(Func<TArg, Task<TResult>> operation)
        {
            Contract.Requires(operation != null);

            this.operation = operation;
        }

        public IAsyncResult BeginInvoke(TArg arg, AsyncCallback callback)
        {
            Task<TResult> task = this.operation(arg);
            if (task != null) task.ContinueWith(_ => callback(task));
            return task;
        }

        public TResult EndInvoke(IAsyncResult result)
        {
            Contract.Requires(result != null);

            return ((Task<TResult>)result).Result;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.operation != null);
        }
    }
}