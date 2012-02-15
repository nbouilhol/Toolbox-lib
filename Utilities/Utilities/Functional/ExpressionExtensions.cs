using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace Utilities.Functional
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T1, TCovariantResult>> ToCovariant<T1, TResult, TCovariantResult>(this Expression<Func<T1, TResult>> expr)
            where TResult : TCovariantResult
        {
            Contract.Requires(expr != null);

            return Expression.Lambda<Func<T1, TCovariantResult>>(expr.Body, expr.Parameters);
        }

        public static Func<A, Func<B, C>> Curry<A, B, C>(this Func<A, B, C> f)
        {
            return a => b => f(a, b);
        }

        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(this Func<A, B, C, D> f)
        {
            return a => b => c => f(a, b, c);
        }

        public static Func<A, C> Compose<A, B, C>(this Func<B, C> f, Func<A, B> g)
        {
            Contract.Requires(g != null);

            return a => f(g(a));
        }

        public static Func<A, Func<B>> Promote<A, B>(this Func<A, B> f)
        {
            return a => () => f(a);
        }

        public static Func<B> Select<A, B>(this Func<A> a, Func<A, B> f)
        {
            Contract.Requires(f != null);

            return () => f(a());
        }

        public static Func<B> SelectMany<A, B>(this Func<A> a, Func<A, Func<B>> f)
        {
            Contract.Requires(a != null);
            Contract.Requires(f != null);

            return f(a());
        }

        public static Func<C> SelectMany<A, B, C>(this Func<A> k, Func<A, Func<B>> p, Func<A, B, C> f)
        {
            Contract.Requires(f != null);
            Contract.Requires(p != null);

            return SelectMany<A, C>(k, a => Select<B, C>(p(a), b => f(a, b)));
        }

        public static Func<A, C> Select<A, B, C>(this Func<A, B> f, Func<B, C> g)
        {
            Contract.Requires(g != null);

            return a => g(f(a));
        }

        public static Func<C, B> SelectMany<A, B, C>(this Func<C, A> f, Func<A, Func<C, B>> g)
        {
            Contract.Requires(g != null);

            return a => g(f(a))(a);
        }

        public static Func<C, D> SelectMany<A, B, C, D>(this Func<C, A> f, Func<A, Func<C, B>> p, Func<A, B, D> k)
        {
            Contract.Requires(p != null);
            Contract.Requires(k != null);

            return SelectMany<A, D, C>(f, b => Select<C, B, D>(p(b), x => k(b, x)));
        }

        public static Func<B, A, C> Flip<A, B, C>(this Func<A, B, C> f)
        {
            return (b, a) => f(a, b);
        }
    }
}
