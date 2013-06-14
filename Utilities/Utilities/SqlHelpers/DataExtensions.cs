using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Utilities.Extensions;

namespace Utilities.SqlHelpers
{
    public static class DataExtensions
    {
        private const string FormatNameForAddedProperty = "{0}.{1}";

        public static DataTable AddToDataTable<TParent, T>(this DataTable table, string property, IEnumerable<TParent> data, Func<TParent, T> func, params string[] propertiesNameToTake)
            where T : class
        {
            PropertiesDescriptor<T> mappedProperties = PropertiesDescriptor<T>.Create();
            IEnumerable<PropertyInfo> propertiesInfos = FilterPropertiesInfos<T>(mappedProperties, propertiesNameToTake);

            foreach (PropertyInfo propertyInfo in propertiesInfos)
                table.Columns.Add(string.Format(FormatNameForAddedProperty, property, propertyInfo.Name), Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);

            int cpt = 0;
            foreach (TParent item in data.WhereNotNull())
            {
                DataRow row = table.Rows[cpt++];

                IEnumerable<KeyValuePair<string, Func<T, object>>> accessors = FilterAccessors<T>(mappedProperties, propertiesNameToTake);

                foreach (KeyValuePair<string, Func<T, object>> accessor in accessors)
                {
                    T value = func(item);
                    row[string.Format(FormatNameForAddedProperty, property, accessor.Key)] = value != null ? accessor.Value(value) ?? DBNull.Value : DBNull.Value;
                }
            }

            return table;
        }

        private static DataTable GetDataTable<T>(this IEnumerable<T> data, IEnumerable<PropertyDescriptor> mappedProperties, params string[] propertiesNameToTake)
        {
            DataTable table = new DataTable();

            mappedProperties = FilterPropertiesDescriptor(mappedProperties, propertiesNameToTake).ToList();

            foreach (PropertyDescriptor prop in mappedProperties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data.WhereNotNull())
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in mappedProperties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }

        private static DataTable GetDataTable<T>(this IEnumerable<T> data, PropertiesDescriptor<T> mappedProperties, params string[] propertiesNameToTake) where T : class
        {
            DataTable table = new DataTable();
            IEnumerable<PropertyInfo> propertiesInfos = FilterPropertiesInfos<T>(mappedProperties, propertiesNameToTake);

            foreach (PropertyInfo propertyInfo in propertiesInfos)
                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);

            foreach (T item in data.WhereNotNull())
            {
                DataRow row = table.NewRow();
                IEnumerable<KeyValuePair<string, Func<T, object>>> accessors = FilterAccessors<T>(mappedProperties, propertiesNameToTake);

                foreach (KeyValuePair<string, Func<T, object>> accessor in accessors)
                    row[accessor.Key] = accessor.Value(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable GetOneColumnDataTable<T>(this IEnumerable<T> data, string columnName)
        {
            DataTable table = new DataTable();
            table.Columns.Add(columnName, typeof(T));
            foreach (T item in data) table.Rows.Add(item);
            return table;
        }

        public static DataTable ToOneColumnDataTable<T>(this IEnumerable<T> data, string columnName)
        {
            return GetOneColumnDataTable(data, columnName);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data, params string[] propertiesNameToTake) where T : class
        {
            //IEnumerable<PropertyDescriptor> properties = TypeDescriptor.GetProperties(typeof(T)).Cast<PropertyDescriptor>().Where(propertyInfo => IsBasicType(propertyInfo.PropertyType));
            return GetDataTable(data, PropertiesDescriptor<T>.Create(), propertiesNameToTake);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data, Func<PropertyDescriptor, bool> expression, params string[] propertiesNameToTake) where T : class
        {
            IEnumerable<PropertyDescriptor> properties = typeof(T).GetPropertiesInfoWithInterfaces().Cast<PropertyDescriptor>().Where(expression);
            return GetDataTable(data, properties, propertiesNameToTake);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data, IEnumerable<PropertyDescriptor> properties, params string[] propertiesNameToTake) where T : class
        {
            return GetDataTable(data, properties, propertiesNameToTake);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data, PropertiesDescriptor<T> properties, params string[] propertiesNameToTake) where T : class
        {
            return GetDataTable(data, properties, propertiesNameToTake);
        }

        private static IEnumerable<KeyValuePair<string, Func<T, object>>> FilterAccessors<T>(PropertiesDescriptor<T> mappedProperties, params string[] propertiesNameToTake) where T : class
        {
            return propertiesNameToTake != null && propertiesNameToTake.Length > 0
                ? mappedProperties.Accessors.Where(accessor => propertiesNameToTake.Contains(accessor.Key))
                 : mappedProperties.Accessors;
        }

        private static IEnumerable<PropertyDescriptor> FilterPropertiesDescriptor(IEnumerable<PropertyDescriptor> mappedProperties, params string[] propertiesNameToTake)
        {
            return propertiesNameToTake != null && propertiesNameToTake.Length > 0
                ? mappedProperties.Where(mappedProperty => propertiesNameToTake.Contains(mappedProperty.Name))
                : mappedProperties;
        }

        private static IEnumerable<PropertyInfo> FilterPropertiesInfos<T>(PropertiesDescriptor<T> mappedProperties, params string[] propertiesNameToTake) where T : class
        {
            return propertiesNameToTake != null && propertiesNameToTake.Length > 0
                ? mappedProperties.PropertiesInfo.Where(mappedProperty => propertiesNameToTake.Contains(mappedProperty.Name))
                : mappedProperties.PropertiesInfo;
        }

        private static bool IsBasicType(Type type)
        {
            return type.Namespace.StartsWith("System");
        }
    }
}