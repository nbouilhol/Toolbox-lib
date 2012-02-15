using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace Utilities
{
    public static class ObjectExtensions
    {
        public static string GetMethodName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> selector) where T : class
        {
            Contract.Requires(selector != null);

            return ((MethodCallExpression)(selector.Body is UnaryExpression ? ((UnaryExpression)selector.Body).Operand : selector.Body)).Method.Name;
        }

        public static string GetMethodName<T>(this T owner, Expression<Action<T>> selector) where T : class
        {
            Contract.Requires(selector != null);

            return ((MethodCallExpression)(selector.Body is UnaryExpression ? ((UnaryExpression)selector.Body).Operand : selector.Body)).Method.Name;
        }

        public static string GetPropertyName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> property) where T : class
        {
            Contract.Requires(property != null);

            return ((MemberExpression)(property.Body is UnaryExpression ? ((UnaryExpression)property.Body).Operand : property.Body)).Member.Name;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T value) { yield return value; }
        public static IEnumerable<T> AsEnumerable<T>(this T value, params T[] values) { return values != null ? value.AsEnumerable().Concat(values) : value.AsEnumerable(); }
    }
}
