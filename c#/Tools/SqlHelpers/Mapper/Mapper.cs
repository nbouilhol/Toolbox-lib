using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BouilholLib.Helper
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

        private IDictionary<string, PropertyMetadata<T>> BuildPropertiesDictionary()
        {
            if (typeof(T).IsPrimitive)
                return null;

            return GetProperties()
                .Select(property => new PropertyMetadata<T>(property))
                .ToDictionary(property => property.PropertyName, StringComparer.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<PropertyInfo> GetProperties()
        {
            if (typeof(T).IsPrimitive)
                return null;

            return typeof(T).GetProperties()
                .Where(property => property.CanWrite && (NotMappedAttribute)Attribute.GetCustomAttribute(property, typeof(NotMappedAttribute)) == null);
        }

        public IEnumerable<T> MapToList(SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    T result = Map(reader);
                    if (result != null)
                        yield return result;
                }

                reader.NextResult();
            }
        }

        public T MapOne(SqlDataReader reader)
        {
            reader.Read();
            return Map(reader);
        }

        public T Map(SqlDataReader record)
        {
            T instance = factory();

            for (int column = 0; column != record.FieldCount; column++)
            {
                PropertyMetadata<T> property;

                if (!record.IsDBNull(column))
                {
                    try
                    {
                        if (typeof(T).IsPrimitive)
                            instance = (T)record[column];
                        else if (properties.TryGetValue(record.GetName(column), out property))
                            property.SetValue(instance, record[column]);
                    }
                    catch (InvalidCastException e)
                    {
                        throw MappingException.InvalidCast(record.GetName(column), e);
                    }
                }
            }

            return instance;
        }

        private static Func<T> CreateActivatorDelegate()
        {
            if (typeof(T).IsPrimitive)
                return CreateActivatorDelegateForPrimitive();

            ConstructorInfo constructor = typeof(T).GetConstructor(Type.EmptyTypes);

            if (constructor == null)
                return () => { throw MappingException.NoParameterlessConstructor(typeof(T)); };

            return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
        }

        private static Func<T> CreateActivatorDelegateForPrimitive()
        {
            return Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        }
    }
}
