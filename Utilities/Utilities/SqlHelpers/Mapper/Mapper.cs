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
        private readonly Func<T> factory;
        private readonly IDictionary<string, PropertyMetadata<T>> properties;
        private static readonly Lazy<Mapper<T>> instanceCache = new Lazy<Mapper<T>>(() => new Mapper<T>());

        public static Mapper<T> Create()
        {
            return instanceCache.Value;
        }

        private Mapper()
        {
            this.factory = CreateActivatorDelegate();
            this.properties = BuildPropertiesDictionary();
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.factory != null);
            Contract.Invariant(this.properties != null);
        }

        private static IDictionary<string, PropertyMetadata<T>> BuildPropertiesDictionary()
        {
            if (typeof(T).IsValueType) return null;

            IEnumerable<PropertyInfo> properties = GetProperties();
            return properties != null
                ? properties.Select(property => new PropertyMetadata<T>(property)).ToDictionary(property => property.PropertyName, StringComparer.InvariantCultureIgnoreCase)
                : null;
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
            if (typeof(T).IsValueType) return null;

            return typeof(T).GetPropertiesInfoWithInterfaces().Where(property => property.CanWrite && (NotMappedAttribute)Attribute.GetCustomAttribute(property, typeof(NotMappedAttribute)) == null);
        }

        public IEnumerable<T> MapToList(SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    T result = Map(reader);
                    if (result != null) yield return result;
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

            T instance = factory();

            for (int column = 0; column < record.FieldCount; column++)
            {
                PropertyMetadata<T> property;

                if (!record.IsDBNull(column))
                {
                    try
                    {
                        if (typeof(T).IsValueType) instance = record[column] != null ? (T)record[column] : default(T);
                        else if (properties.TryGetValue(record.GetName(column), out property)) if (property != null) property.SetValue(instance, record[column]);
                    }
                    catch (InvalidCastException e)
                    {
                        throw MappingException.InvalidCast(string.Format("{0} should be {1}", record.GetName(column), record[column].GetType().Name), e);
                    }
                }
            }

            return instance;
        }

        private static Func<T> CreateActivatorDelegate()
        {
            if (typeof(T).IsValueType) return CreateActivatorDelegateForPrimitive();

            ConstructorInfo constructor = typeof(T).GetConstructor(Type.EmptyTypes);

            if (constructor == null) return () => { throw MappingException.NoParameterlessConstructor(typeof(T)); };

            return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
        }

        private static Func<T> CreateActivatorDelegateForPrimitive()
        {
            return Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        }
    }
}