using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Utilities.Functional
{
    public static class ObjectExtensions
    {
        public static Lazy<B> Select<A, B>(this Lazy<A> a, Func<A, B> f)
        {
            Contract.Requires(f != null);

            return new Lazy<B>(() => f(a.Value));
        }

        public static Lazy<B> SelectMany<A, B>(this Lazy<A> a, Func<A, Lazy<B>> f)
        {
            Contract.Requires(a != null);
            Contract.Requires(f != null);

            return f(a.Value);
        }

        public static Lazy<C> SelectMany<A, B, C>(this Lazy<A> k, Func<A, Lazy<B>> p, Func<A, B, C> f)
        {
            Contract.Requires(f != null);
            Contract.Requires(p != null);

            return SelectMany<A, C>(k, a => Select<B, C>(p(a), b => f(a, b)));
        }

        public static Lazy<A> StrictLazy<A>(this A a)
        {
            return new Lazy<A>(() => a);
        }

        public static Nullable<TInput> ToNullable<TInput>(this TInput a)
            where TInput : struct
        {
            return new Nullable<TInput>(a);
        }

        public static Func<TInput, TInput> Do<TInput>(Action<TInput> action)
            where TInput : class
        {
            return (a) =>
            {
                if (a != null)
                    action(a);
                return a;
            };
        }

        public static Func<TInput> Do<TInput>(this TInput a, Action<TInput> action)
            where TInput : class
        {
            return () => Do(action)(a);
        }

        public static Func<TInput> Do<TInput>(this Func<TInput> fa, Action<TInput> action)
            where TInput : class
        {
            return () => Do(action)(fa());
        }

        public static Func<TInput, TInput> If<TInput>(Predicate<TInput> evaluator)
            where TInput : class
        {
            return (a) =>
            {
                return a != null ? (evaluator(a) ? a : null) : a;
            };
        }

        public static Func<TInput> If<TInput>(this TInput a, Predicate<TInput> evaluator)
            where TInput : class
        {
            return () => If(evaluator)(a);
        }

        public static Func<TInput> If<TInput>(this Func<TInput> fa, Predicate<TInput> evaluator)
            where TInput : class
        {
            return () => If(evaluator)(fa());
        }

        public static Func<TInput, TInput> Unless<TInput>(Predicate<TInput> evaluator)
          where TInput : class
        {
            return (a) =>
            {
                return a != null ? (evaluator(a) ? null : a) : a;
            };
        }

        public static Func<TInput> Unless<TInput>(this TInput a, Predicate<TInput> evaluator)
          where TInput : class
        {
            return () => Unless(evaluator)(a);
        }

        public static Func<TInput> Unless<TInput>(this Func<TInput> fa, Predicate<TInput> evaluator)
          where TInput : class
        {
            return () => Unless(evaluator)(fa());
        }

        public static Func<TInput, TResult> Return<TInput, TResult>(Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return (a) =>
            {
                return a != null ? evaluator(a) : failureValue;
            };
        }

        public static Func<TResult> Return<TInput, TResult>(this TInput a, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return () => Return(evaluator, failureValue)(a);
        }

        public static Func<TResult> Return<TInput, TResult>(this Func<TInput> fa, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return () => Return(evaluator, failureValue)(fa());
        }

        public static Func<TInput, TResult> With<TInput, TResult>(Func<TInput, TResult> evaluator)
            where TInput : class
        {
            return (a) =>
            {
                return a != null ? evaluator(a) : default(TResult);
            };
        }

        public static Func<TResult> With<TInput, TResult>(this TInput a, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            return () => With(evaluator)(a);
        }

        public static Func<TResult> With<TInput, TResult>(this Func<TInput> fa, Func<TInput, TResult> evaluator)
            where TInput : class
        {
            return () => With(evaluator)(fa());
        }

        public static Func<TInput, TInput> DoOnEmpty<TInput>(Action<TInput> action)
            where TInput : class
        {
            return (a) =>
            {
                if (a == null)
                    action(a);
                return a;
            };
        }

        public static Func<TInput> DoOnEmpty<TInput>(this TInput a, Action<TInput> action)
            where TInput : class
        {
            return () => DoOnEmpty(action)(a);
        }

        public static Func<TInput> DoOnEmpty<TInput>(this Func<TInput> fa, Action<TInput> action)
            where TInput : class
        {
            return () => DoOnEmpty(action)(fa());
        }

        public static Func<TInput1, TResult> SelectMany<TInput1, TInput2, TResult>(Func<TInput1, TInput2> evaluator1, Func<TInput1, TInput2, TResult> evaluator2)
            where TInput1 : class
            where TInput2 : IComparable<TInput2>
        {
            return (a) =>
            {
                TInput2 b = a != null ? evaluator1(a) : default(TInput2);
                return b.CompareTo(default(TInput2)) != 0 ? evaluator2(a, b) : default(TResult);
            };
        }

        public static Func<TResult> SelectMany<TInput1, TInput2, TResult>(this TInput1 a, Func<TInput1, TInput2> evaluator1, Func<TInput1, TInput2, TResult> evaluator2)
            where TInput1 : class
            where TInput2 : IComparable<TInput2>
        {
            return () => SelectMany(evaluator1, evaluator2)(a);
        }

        public static Func<TResult> SelectMany<TInput1, TInput2, TResult>(this Func<TInput1> fa, Func<TInput1, TInput2> evaluator1, Func<TInput1, TInput2, TResult> evaluator2)
            where TInput1 : class
            where TInput2 : IComparable<TInput2>
        {
            return () => SelectMany(evaluator1, evaluator2)(fa());
        }

        public static TInput Result<TInput>(this Func<TInput> fa)
            where TInput : class
        {
            Contract.Requires(fa != null);

            return fa();
        }

        public static void Run<TInput>(this Func<TInput> fa)
            where TInput : class
        {
            Contract.Requires(fa != null);

            fa();
        }

        public static IEnumerable<T> Cons<T>(this IEnumerable<T> list, T item)
        {
            yield return item;
            foreach (var listItem in list)
                yield return listItem;
        }
    }
}
