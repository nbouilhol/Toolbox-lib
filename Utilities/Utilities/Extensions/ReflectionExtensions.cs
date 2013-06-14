using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities.Extensions
{
    public static class ReflectionExtensions
    {
        private const BindingFlags getPropertyFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        public static PropertyInfo GetLastProperty(this Type baseType, string propertyName)
        {
            string[] parts = propertyName.Split('.');

            if (parts.Length > 1)
            {
                PropertyInfo propertyInfo = baseType.GetProperty(parts.FirstOrDefault(), getPropertyFlags);
                if (propertyInfo == null) return null;

                Type nextType = propertyInfo.PropertyType;
                string nextPropertyName = parts.Skip(1).Aggregate((a, i) => string.Concat(a, ".", i));

                return nextType.GetLastProperty(nextPropertyName) ??
                       nextType.GetLastPropertyFromInterface(nextPropertyName);
            }

            return baseType.GetProperty(propertyName, getPropertyFlags);
        }

        public static Expression GetProperty(this Expression parameterExpression, string propertyName)
        {
            return propertyName.Split('.')
                .Aggregate(parameterExpression,
                    (current, property) => Expression.Property(current, GetOrSearchProperty(current.Type, property)));
        }

        private static PropertyInfo GetLastPropertyFromInterface(this Type baseType, string propertyName)
        {
            return
                baseType.GetInterfaces()
                    .Select(@interface => @interface.GetLastProperty(propertyName))
                    .FirstOrDefault(result => result != null);
        }

        private static PropertyInfo GetOrSearchProperty(this Type baseType, string propertyName)
        {
            PropertyInfo propertyInfo = baseType.GetProperty(propertyName);

            if (propertyInfo != null) return propertyInfo;

            IEnumerable<Type> types = baseType.GetInterfaces();
            if (baseType.BaseType != null) types = types.Concat(new List<Type> {baseType.BaseType});

            return types.Select(type => type.GetOrSearchProperty(propertyName)).FirstOrDefault(prop => prop != null);
        }
    }
}