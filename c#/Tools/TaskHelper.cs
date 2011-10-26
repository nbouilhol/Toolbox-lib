using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BouilholLib.Helper
{
    public static class TaskHelper
    {
        public static Task<int> ReadAsyncTpl(this Stream stream, byte[] buffer, int offset, int count)
        {
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
        Func<TArg, Task<TResult>> operation;

        public TplAsyncOperation(Func<TArg, Task<TResult>> operation)
        {
            this.operation = operation;
        }

        public IAsyncResult BeginInvoke(TArg arg, AsyncCallback callback, object state)
        {
            var task = operation(arg);
            task.ContinueWith(_ => callback(task));
            return task;
        }

        public TResult EndInvoke(IAsyncResult result)
        {
            return ((Task<TResult>)result).Result;
        }
    }
}
