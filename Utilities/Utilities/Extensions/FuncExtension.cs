using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class FuncExtension
    {
        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> f)
        {
            return f.Memoize(EqualityComparer<TKey>.Default);
        }

        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> f, IEqualityComparer<TKey> equalityComparer)
        {
            ConcurrentDictionary<TKey, Lazy<TResult>> cache = new ConcurrentDictionary<TKey, Lazy<TResult>>(equalityComparer);
            return key => cache.GetOrAdd(key, new Lazy<TResult>(() => f(key))).Value;
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