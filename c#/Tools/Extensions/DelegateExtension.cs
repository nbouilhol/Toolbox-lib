namespace BouilholLib.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Collection;

    public static class DelegateExtension
    {
        /// <summary>
        /// F# - Currying
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            Guard.AgainstNull(func, "func");

            return a => b => c => func(a, b, c);
        }

        /// <summary>
        /// F# - Currying
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
        {
            Guard.AgainstNull(func, "func");

            return a => b => c => d => func(a, b, c, d);
        }

        /// <summary>
        /// F# - DeCurrying
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<T1, T2, TResult> DeCurry<T1, T2, TResult>(this Func<T1, Func<T2, TResult>> func)
        {
            Guard.AgainstNull(func, "func");

            return (a, b) => func(a)(b);
        }

        /// <summary>
        /// F# - DeCurrying
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, TResult> DeCurry<T1, T2, T3, TResult>(this Func<T1, Func<T2, Func<T3, TResult>>> func)
        {
            Guard.AgainstNull(func, "func");

            return (a, b, c) => func(a)(b)(c);
        }

        /// <summary>
        /// F# - DeCurrying
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<T1, T2, T3, T4, TResult> DeCurry<T1, T2, T3, T4, TResult>(this Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> func)
        {
            Guard.AgainstNull(func, "func");

            return (a, b, c, d) => func(a)(b)(c)(d);
        }

        /// <summary>
        /// F# - Currying
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<TArg1, Func<TArg2, TResult>> Curry<TArg1, TArg2, TResult>(this Func<TArg1, TArg2, TResult> func)
        {
            Guard.AgainstNull(func, "func");

            return a1 => a2 => func(a1, a2);
        }

        /// <summary>
        /// F# - Currying
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<TArg1, Action<TArg2>> Curry<TArg1, TArg2>(this Action<TArg1, TArg2> action)
        {
            Guard.AgainstNull(action, "action");

            return a1 => a2 => action(a1, a2);
        }

        /// <summary>
        /// F# - Memoize Function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> func)
        {
            var map = new Dictionary<T, TResult>();
            return key =>
            {
                if (map.ContainsKey(key)) return map[key];
                var result = func(key);
                map.Add(key, result);
                return result;
            };
        }

        /// <summary>
        /// F# - Seq.unfold
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="generator"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Unfold<T, TResult>(this Func<T, Option<Tuple<TResult, T>>> generator, T start)
        {
            T next = start;

            while (true)
            {
                Option<Tuple<TResult, T>> res = generator(next);
                if (res.IsNone)
                    yield break;

                yield return res.Value.Item1;

                next = res.Value.Item2;
            }
        }

        /// <summary>
        /// F# - Seq.init_infinite
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static IEnumerable<T> InitializeInfinite<T>(this Func<int, T> f)
        {
            return Unfold(s => Option.Some(Tuple.Create(f(s), s + 1)), 0);
        }

        /// <summary>
        /// F# - Seq.init_finite
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="f">
        /// </param>
        /// <param name="count">
        /// </param>
        /// <returns>
        /// </returns>
        public static IEnumerable<T> InitializeFinite<T>(this Func<int, T> f, int count)
        {
            return Unfold(s => s < count ? Option.Some(Tuple.Create(f(s), s + 1)) : Option<Tuple<T, int>>.None, 0);
        }

        /// <summary>
        /// F# - Seq.generate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="opener"></param>
        /// <param name="generator"></param>
        /// <param name="closer"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Generate<T, TResult>(this Func<T> opener, Func<T, Option<TResult>> generator, Action<T> closer)
        {
            T openerResult = opener();

            while (true)
            {
                Option<TResult> res = generator(openerResult);
                if (res.IsNone)
                {
                    closer(openerResult);
                    yield break;
                }

                yield return res.Value;
            }
        }

        public static IEnumerable<TResult> GenerateUsing<T, TResult>(this Func<T> opener, Func<T, Option<TResult>> generator) where T : IDisposable
        {
            return opener.Generate(generator, x => x.Dispose());
        }

        public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
        {
            var prop = expression.Body as MemberExpression;

            if (prop != null)
            {
                var info = prop.Member as PropertyInfo;
                if (info != null)
                    return info;
            }

            throw new ArgumentException("The expression target is not a Property");
        }
    }
}