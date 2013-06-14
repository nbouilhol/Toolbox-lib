using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.Extensions;

namespace Utilities.SqlHelpers.Mapper
{
    public class PropertyAccessor<TData>
    {
        private readonly Func<TData, object>[] _accessors;
        private readonly IDictionary<string, int> _ordinalLookup;
        private static readonly Lazy<PropertyAccessor<TData>> InstanceCache = new Lazy<PropertyAccessor<TData>>(() => new PropertyAccessor<TData>());

        public Func<TData, object>[] Accessors { get { return _accessors; } }

        public IDictionary<string, int> OrdinalLookup { get { return _ordinalLookup; } }

        public PropertyAccessor()
        {
            var propertyAccessors = typeof(TData).GetPropertiesInfoWithInterfaces()
                .Where(property => property.CanRead && (NotMappedAttribute)Attribute.GetCustomAttribute(property, typeof(NotMappedAttribute)) == null)
                .Select((p, i) => new
                {
                    Index = i,
                    Property = p,
                    Accessor = CreatePropertyAccessor(p)
                }).ToArray();

            _accessors = propertyAccessors.Select(p => p.Accessor).ToArray();
            _ordinalLookup = propertyAccessors.ToDictionary(p => MapPropertyName(p.Property), p => p.Index, StringComparer.OrdinalIgnoreCase);
        }

        public static PropertyAccessor<TData> Create()
        {
            return InstanceCache.Value;
        }

        private static Func<TData, object> CreatePropertyAccessor(PropertyInfo propertyInfo)
        {
            Contract.Requires(propertyInfo != null);

            ParameterExpression parameter = Expression.Parameter(typeof(TData), "input");
            Expression expression = Expression.PropertyOrField(parameter, propertyInfo.Name);

            if (propertyInfo.PropertyType.IsValueType) expression = Expression.Convert(expression, typeof(object));

            return Expression.Lambda<Func<TData, object>>(expression, parameter).Compile();
        }

        private static string MapPropertyName(PropertyInfo property)
        {
            var attribute = (ColumnAttribute)Attribute.GetCustomAttribute(property, typeof(ColumnAttribute));

            if (attribute != null) return attribute.Name;
            if (property != null) return property.Name;
            return null;
        }
    }
}