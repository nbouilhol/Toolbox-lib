using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities
{
    public static class FuncExtension
    {
        public static Func<TArg, TResult> Memoize<TArg, TResult>(this Func<TArg, TResult> f)
        {
            var map = new Dictionary<TArg, TResult>();
            return arg =>
            {
                TResult value;
                if (map.TryGetValue(arg, out value))
                    return value;
                value = f(arg);
                map.Add(arg, value);
                return value;
            };
        }

        public static System.Threading.Tasks.Task Using<T>(Func<T> disposableAcquisition, Action<T> action) where T : IDisposable
        {
            T disposable = disposableAcquisition();

            return System.Threading.Tasks.Task.Factory.StartNew(() => action(disposable))
                .ContinueWith(task =>
                {
                    if (!ReferenceEquals(disposable, null))
                    {
                        disposable.Dispose();
                    }
                    return task;
                });
        }

        public static System.Threading.Tasks.Task Using<T>(Func<T> disposableAcquisition, Func<T, System.Threading.Tasks.Task> taskFunc) where T : IDisposable
        {
            T instance = disposableAcquisition();

            return taskFunc(instance)
                .ContinueWith(task =>
                {
                    if (!ReferenceEquals(instance, null))
                    {
                        instance.Dispose();
                    }
                    return task;
                });
        }

        public static Task<TResult> Using<T, TResult>(Func<T> disposableAcquisition, Func<T, Task<TResult>> taskFunc) where T : IDisposable
        {
            T instance = disposableAcquisition();

            return taskFunc(instance)
                .ContinueWith(task =>
                {
                    if (!ReferenceEquals(instance, null))
                    {
                        instance.Dispose();
                    }
                    return task.Result;
                });
        }
    }
}
