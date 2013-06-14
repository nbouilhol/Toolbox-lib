using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<string, Func<object, object, object, object>> cacheInstanceCreation
            = new ConcurrentDictionary<string, Func<object, object, object, object>>();

        public static TResult GetInstance<TResult>()
        {
            return (TResult) typeof (TResult).GetInstance();
        }

        public static TResult GetInstance<TArg, TResult>(TArg argument)
        {
            return (TResult) typeof (TResult).GetInstance(argument);
        }

        public static TResult GetInstance<TArg1, TArg2, TResult>(TArg1 argument1, TArg2 argument2)
        {
            return (TResult) typeof (TResult).GetInstance(argument1, argument2);
        }

        public static TResult GetInstance<TArg1, TArg2, TArg3, TResult>(TArg1 argument1, TArg2 argument2,
            TArg3 argument3)
        {
            return (TResult) typeof (TResult).GetInstance(argument1, argument2, argument3);
        }

        public static object GetInstance(this Type type)
        {
            return GetInstance<TypeToIgnore>(type, null);
        }

        public static object GetInstance<TArg>(this Type type, TArg argument)
        {
            return GetInstance<TArg, TypeToIgnore>(type, argument, null);
        }

        public static object GetInstance<TArg1, TArg2>(this Type type, TArg1 argument1, TArg2 argument2)
        {
            return GetInstance<TArg1, TArg2, TypeToIgnore>(type, argument1, argument2, null);
        }

        public static object GetInstance<TArg1, TArg2, TArg3>(this Type type, TArg1 argument1, TArg2 argument2,
            TArg3 argument3)
        {
            Type[] argumentTypes = {typeof (TArg1), typeof (TArg2), typeof (TArg3)};
            Type[] constructorArgumentTypes = argumentTypes.Where(t => t != typeof (TypeToIgnore)).ToArray();
            string constructorSignatureKey = GetConstructorSignatureKey(type, constructorArgumentTypes);

            if (!IsInCache(constructorSignatureKey))
                CacheInstanceCreationMethodIfRequired(type, argumentTypes, constructorSignatureKey,
                    constructorArgumentTypes);

            return cacheInstanceCreation.TryGetValue(constructorSignatureKey)(argument1, argument2, argument3);
        }

        private static bool IsInCache(string constructorSignatureKey)
        {
            return cacheInstanceCreation.ContainsKey(constructorSignatureKey);
        }

        private static void CacheInstanceCreationMethodIfRequired(Type type, Type[] argumentTypes,
            string constructorSignatureKey, Type[] constructorArgumentTypes)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                CallingConventions.HasThis, constructorArgumentTypes, new ParameterModifier[0]);
            ParameterExpression[] lamdaParameterExpressions = GetLambdaParameterExpressions(argumentTypes).ToArray();
            UnaryExpression[] constructorParameterExpressions =
                GetConstructorParameterExpressions(lamdaParameterExpressions, constructorArgumentTypes).ToArray();
            NewExpression constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);
            Func<object, object, object, object> constructorCallingLambda =
                Expression.Lambda<Func<object, object, object, object>>(constructorCallExpression,
                    lamdaParameterExpressions).Compile();

            cacheInstanceCreation.TryAdd(constructorSignatureKey, constructorCallingLambda);
        }

        private static IEnumerable<ParameterExpression> GetLambdaParameterExpressions(Type[] argumentTypes)
        {
            for (int i = 0; i < argumentTypes.Length; i++)
                yield return Expression.Parameter(typeof (object), string.Concat("param", i));
        }

        private static IEnumerable<UnaryExpression> GetConstructorParameterExpressions(
            ParameterExpression[] lamdaParameterExpressions, Type[] constructorArgumentTypes)
        {
            for (int i = 0; i < constructorArgumentTypes.Length; i++)
                yield return Expression.Convert(lamdaParameterExpressions[i], constructorArgumentTypes[i]);
        }

        private static string GetConstructorSignatureKey(Type type, Type[] argumentTypes)
        {
            return string.Concat(type.FullName, " (", string.Join(", ", argumentTypes.Select(at => at.FullName)), ")");
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInfoWithInterfaces(this Type type,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            var comparer = new PropertyInfoComparer();
            return type.GetProperties(flags).Concat(type.GetPropertiesInfoFromInterface()).Distinct(comparer);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInfoFromInterface(this Type type,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return type.GetInterfaces().SelectMany(@interface => @interface.GetProperties(flags));
        }

        private class TypeToIgnore
        {
        }
    }

    public class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        #region IEqualityComparer<PropertyInfo> Members

        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(PropertyInfo obj)
        {
            if (obj == null) return 0;
            return obj.Name.GetHashCode();
        }

        #endregion IEqualityComparer<PropertyInfo> Members
    }
}