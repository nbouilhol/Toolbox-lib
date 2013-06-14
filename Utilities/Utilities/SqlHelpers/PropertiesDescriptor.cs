using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.Extensions;
using Utilities.SqlHelpers.Mapper;

namespace Utilities.SqlHelpers
{
    public class PropertiesDescriptor<TData>
    {
        private static readonly Lazy<PropertiesDescriptor<TData>> instanceCache = new Lazy<PropertiesDescriptor<TData>>(() => new PropertiesDescriptor<TData>());

        public PropertiesDescriptor()
        {
            Type type = typeof(TData);

            PropertiesInfo = type.GetPropertiesInfoWithInterfaces(BindingFlags.Instance | BindingFlags.Public).Where(property => Filter(property)).ToList();
            Accessors = PropertiesInfo.ToDictionary(property => property.Name, property => CreatePropertyAccessor(type, property));
        }

        public IDictionary<string, Func<TData, object>> Accessors { get; private set; }

        public IEnumerable<PropertyInfo> PropertiesInfo { get; private set; }

        public static PropertiesDescriptor<TData> Create()
        {
            return instanceCache.Value;
        }

        private static Func<TData, object> CreatePropertyAccessor(Type type, PropertyInfo propertyInfo)
        {
            Contract.Requires(type != null);
            Contract.Requires(propertyInfo != null);

            ParameterExpression parameter = ParameterExpression.Parameter(type);
            Expression expression = GetProperty(parameter, propertyInfo.Name);

            if (propertyInfo.PropertyType.IsValueType) expression = UnaryExpression.Convert(expression, typeof(object));

            return Expression.Lambda<Func<TData, object>>(expression, parameter).Compile();
        }

        private static bool Filter(PropertyInfo property)
        {
            return property.CanRead && (NotMappedAttribute)Attribute.GetCustomAttribute(property, typeof(NotMappedAttribute)) == null && property.PropertyType.Namespace.StartsWith("System");
        }

        public static Expression GetProperty(Expression parameterExpression, string propertyName)
        {
            return MemberExpression.Property(parameterExpression, GetOrSearchProperty(parameterExpression.Type, propertyName));
        }

        private static PropertyInfo GetOrSearchProperty(Type baseType, string propertyName)
        {
            PropertyInfo propertyInfo = baseType.GetProperty(propertyName);

            if (propertyInfo != null) return propertyInfo;

            IEnumerable<Type> types = baseType.GetInterfaces();
            if (baseType.BaseType != null) types = types.Concat(new List<Type> { baseType.BaseType });

            return types.Select(type => GetOrSearchProperty(type, propertyName)).FirstOrDefault(prop => prop != null);
        }
    }
}