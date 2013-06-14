using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace Utilities.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            yield return value;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T value, params T[] values)
        {
            return values != null ? value.AsEnumerable().Concat(values) : value.AsEnumerable();
        }

        public static DateTime ToDateTime(this object source)
        {
            if (source is DateTime) return (DateTime)source;
            if (source is string) return (source as string).ToDateTime();

            try
            {
                return Convert.ToDateTime(source);
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }

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

        public static int ToInt(this object source)
        {
            if (source is int) return (int)source;
            if (source is string) return (source as string).ToInt();

            try
            {
                return Convert.ToInt32(source);
            }
            catch (Exception)
            {
                return default(int);
            }
        }

        public static bool ToBool(this object source)
        {
            if (source is bool) return (bool)source;
            if (source is string) return (source as string).ToBool();

            try
            {
                return Convert.ToBoolean(source);
            }
            catch (Exception)
            {
                return default(bool);
            }
        }

        public static TimeSpan? ToTimeSpan(this object value)
        {
            if (value is TimeSpan) return (TimeSpan)value;
            if (value is string) return (value as string).ToTimeSpan();

            return null;
        }
    }
}