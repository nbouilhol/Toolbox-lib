using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.Extensions;

namespace Utilities.SqlHelpers.Mapper
{
    public class Mapper<T>
    {
        private static readonly Lazy<Mapper<T>> InstanceCache = new Lazy<Mapper<T>>(() => new Mapper<T>());
        private readonly Func<T> _factory;
        private readonly IDictionary<string, PropertyMetadata<T>> _properties;

        private Mapper()
        {
            _factory = CreateActivatorDelegate();
            _properties = BuildPropertiesDictionary();
        }

        public static Mapper<T> Create()
        {
            return InstanceCache.Value;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_factory != null);
            Contract.Invariant(_properties != null);
        }

        private static IDictionary<string, PropertyMetadata<T>> BuildPropertiesDictionary()
        {
            if (typeof (T).IsValueType) return null;

            IEnumerable<PropertyInfo> properties = GetProperties();
            return properties != null
                ? properties.Select(property => new PropertyMetadata<T>(property))
                    .ToDictionary(property => property.PropertyName, StringComparer.InvariantCultureIgnoreCase)
                : null;
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
            if (typeof (T).IsValueType) return null;

            return
                typeof (T).GetPropertiesInfoWithInterfaces()
                    .Where(
                        property =>
                            property.CanWrite &&
                            (NotMappedAttribute) Attribute.GetCustomAttribute(property, typeof (NotMappedAttribute)) ==
                            null);
        }

        public IEnumerable<T> MapToList(SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    T result = Map(reader);
                    if (!EqualityComparer<T>.Default.Equals(result, default(T))) yield return result;
                }

                reader.NextResult();
            }
        }

        public T MapOne(SqlDataReader reader)
        {
            Contract.Requires(reader != null);

            return Map(reader);
        }

        public T Map(SqlDataReader record)
        {
            Contract.Requires(record != null);

            T instance = _factory();

            for (int column = 0; column < record.FieldCount; column++)
            {
                if (!record.IsDBNull(column))
                {
                    try
                    {
                        if (typeof (T).IsValueType) instance = record[column] != null ? (T) record[column] : default(T);
                        else
                        {
                            PropertyMetadata<T> property;
                            if (_properties.TryGetValue(record.GetName(column), out property))
                                if (property != null) property.SetValue(instance, record[column]);
                        }
                    }
                    catch (InvalidCastException e)
                    {
                        throw MappingException.InvalidCast(
                            string.Format("{0} should be {1}", record.GetName(column), record[column].GetType().Name), e);
                    }
                }
            }

            return instance;
        }

        private static Func<T> CreateActivatorDelegate()
        {
            if (typeof (T).IsValueType) return CreateActivatorDelegateForPrimitive();

            ConstructorInfo constructor = typeof (T).GetConstructor(Type.EmptyTypes);

            if (constructor == null) return () => { throw MappingException.NoParameterlessConstructor(typeof (T)); };

            return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
        }

        private static Func<T> CreateActivatorDelegateForPrimitive()
        {
            return Expression.Lambda<Func<T>>(Expression.New(typeof (T))).Compile();
        }
    }
}