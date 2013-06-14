using System;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class TaskExtension
    {
        public static Task SendAsync(this SynchronizationContext context, SendOrPostCallback sendOrPostCallback, object state)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            context.Post(s =>
            {
                try
                {
                    sendOrPostCallback(s);
                    taskCompletionSource.SetResult(true);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            }, state);

            return taskCompletionSource.Task;
        }
    }
}