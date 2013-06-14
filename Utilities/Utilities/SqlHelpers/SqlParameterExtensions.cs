using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Utilities.Extensions;

namespace Utilities.SqlHelpers
{
    public static class SqlParameterExtensions
    {
        private const string FormatNameForAddedProperty = "{0}.{1}";

        public static void AddToParameters<TParent, T>(this List<List<SqlParameter>> parameters,
            string property,
            IEnumerable<TParent> data,
            Func<TParent, T> func,
            params string[] propertiesNameToTake)
            where T : class
        {
            PropertiesDescriptor<T> mappedProperties = PropertiesDescriptor<T>.Create();
            IEnumerable<PropertyInfo> propertiesInfos = FilterPropertiesInfos(mappedProperties, propertiesNameToTake);
            IDictionary<string, SqlDbType> cachedProperties = BuildPropertiesCache(propertiesInfos);

            int cpt = 0;
            foreach (TParent item in data.WhereNotNull())
                parameters[cpt++].AddRange(AddToParameters(cachedProperties,
                    FilterAccessors(mappedProperties, propertiesNameToTake), func, property, item));
        }

        private static List<SqlParameter> AddToParameters<TParent, T>(IDictionary<string, SqlDbType> cachedProperties,
            IEnumerable<KeyValuePair<string, Func<T, object>>> accessors, Func<TParent, T> func, string property,
            TParent item)
            where T : class
        {
            return
                accessors.Select(accessor => AddToParameter(cachedProperties, accessor, property, func(item))).ToList();
        }

        private static SqlParameter AddToParameter<T>(IDictionary<string, SqlDbType> cachedProperties,
            KeyValuePair<string, Func<T, object>> accessor, string property, T propertyValue)
            where T : class
        {
            var sqlParameter =
                new SqlParameter(
                    FormatParametername(string.Format(FormatNameForAddedProperty, property, accessor.Key)),
                    cachedProperties.TryGetValue(accessor.Key));
            sqlParameter.Value = propertyValue != null ? accessor.Value(propertyValue) ?? DBNull.Value : DBNull.Value;

            return sqlParameter;
        }

        public static List<List<SqlParameter>> ToParameters<T>(this IEnumerable<T> data,
            params string[] propertiesNameToTake)
            where T : class
        {
            return GetParameters(data, PropertiesDescriptor<T>.Create(), propertiesNameToTake);
        }

        private static Dictionary<string, SqlDbType> BuildPropertiesCache(IEnumerable<PropertyInfo> propertiesInfos)
        {
            return propertiesInfos.ToDictionary(propertyInfo => propertyInfo.Name,
                propertyInfo =>
                    (Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType).ToSqlType());
        }

        private static IEnumerable<KeyValuePair<string, Func<T, object>>> FilterAccessors<T>(
            PropertiesDescriptor<T> mappedProperties, params string[] propertiesNameToTake) where T : class
        {
            return propertiesNameToTake != null && propertiesNameToTake.Length > 0
                ? mappedProperties.Accessors.Where(accessor => propertiesNameToTake.Contains(accessor.Key))
                : mappedProperties.Accessors;
        }

        private static IEnumerable<PropertyInfo> FilterPropertiesInfos<T>(PropertiesDescriptor<T> mappedProperties,
            params string[] propertiesNameToTake) where T : class
        {
            return propertiesNameToTake != null && propertiesNameToTake.Length > 0
                ? mappedProperties.PropertiesInfo.Where(
                    mappedProperty => propertiesNameToTake.Contains(mappedProperty.Name))
                : mappedProperties.PropertiesInfo;
        }

        private static string FormatParametername(string name)
        {
            return string.Format("@{0}", name.Replace(".", null));
        }

        private static SqlParameter GetParameter<T>(IDictionary<string, SqlDbType> cachedProperties,
            KeyValuePair<string, Func<T, object>> accessor, T item)
            where T : class
        {
            var sqlParameter = new SqlParameter(FormatParametername(accessor.Key),
                cachedProperties.TryGetValue(accessor.Key));
            sqlParameter.Value = accessor.Value(item) ?? DBNull.Value;

            return sqlParameter;
        }

        private static List<List<SqlParameter>> GetParameters<T>(IEnumerable<T> data,
            PropertiesDescriptor<T> mappedProperties, params string[] propertiesNameToTake)
            where T : class
        {
            IEnumerable<PropertyInfo> propertiesInfos = FilterPropertiesInfos(mappedProperties, propertiesNameToTake);
            IDictionary<string, SqlDbType> cachedProperties = BuildPropertiesCache(propertiesInfos);

            return
                data.WhereNotNull()
                    .Select(
                        item =>
                            GetParameters(cachedProperties, FilterAccessors(mappedProperties, propertiesNameToTake),
                                item))
                    .ToList();
        }

        private static List<SqlParameter> GetParameters<T>(IDictionary<string, SqlDbType> cachedProperties,
            IEnumerable<KeyValuePair<string, Func<T, object>>> accessors, T item)
            where T : class
        {
            return accessors.Select(accessor => GetParameter(cachedProperties, accessor, item)).ToList();
        }
    }
}