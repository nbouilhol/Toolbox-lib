using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using Utilities.Extensions;

namespace Utilities.SqlHelpers
{
    public static class SqlExtensions
    {
        public static void AddParam(this SqlCommand command, object item)
        {
            Contract.Requires(command != null);

            SqlParameter parameter = item is SqlParameter ? item as SqlParameter : BuildParameter(command, item);
            command.Parameters.Add(parameter);
        }

        public static void AddParams(this SqlCommand cmd, params object[] args)
        {
            Contract.Requires(args != null);
            Contract.Requires(cmd != null);

            foreach (object item in args)
                AddParam(cmd, item);
        }

        public static string In<TValue>(this string source, string property, IEnumerable<TValue> parameters)
        {
            return In(source, property, parameters, false);
        }

        public static string In<TValue>(this string source, string property, IEnumerable<TValue> parameters,
            bool isNotIn)
        {
            Contract.Requires(source != null);
            Contract.Requires(property != null);

            const string empty = "''";
            string clause = string.Format("{{IN:{0}}}", property);
            IEnumerable<TValue> enumerable = parameters as IList<TValue> ?? parameters.ToList();
            string newClauseParameter = !isNotIn && parameters != null && enumerable.Count() != 0
                ? string.Join(",", enumerable)
                : empty;
            string newClause = string.Format("{0}IN ({1})", isNotIn ? "NOT " : "", newClauseParameter);
            return source.Replace(clause, newClause);
        }

        public static string In(this string source, string property, IEnumerable<string> parameters, bool isNotIn)
        {
            return In<string>(source, property,
                parameters != null ? parameters.Select(p => string.Format("'{0}'", p)) : null, isNotIn);
        }

        public static string InIgnoreCase(this string source, string property, IEnumerable<string> parameters,
            bool isNotIn)
        {
            return In<string>(source, property,
                parameters != null ? parameters.Select(p => string.Format("'{0}'", p.ToLower())) : null, isNotIn);
        }

        public static string NotIn<TValue>(this string source, string property, IEnumerable<TValue> parameters)
        {
            return In(source, property, parameters, true);
        }

        [Pure]
        public static IEnumerable<IDictionary<string, object>> ToDictionary(this SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                IEnumerable<string> fieldNames = reader.GetFieldNames();
                IEnumerable<string> enumerable = fieldNames as IList<string> ?? fieldNames.ToList();

                while (reader.Read())
                {
                    yield return reader.ToDictionary(enumerable);
                }

                reader.NextResult();
            }
        }

        [Pure]
        public static IDictionary<string, object> ToDictionary(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dataRecord.GetFieldNames()
                .Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v))
                .ToList()
                .ForEach(value => dictionary.Add(value.Key, value.Value));
            return dictionary;
        }

        [Pure]
        public static IEnumerable<dynamic> ToDynamic(this SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                IEnumerable<string> fieldNames = reader.GetFieldNames();
                IEnumerable<string> enumerable = fieldNames as IList<string> ?? fieldNames.ToList();

                while (reader.Read())
                {
                    yield return reader.ToDynamic(enumerable);
                }

                reader.NextResult();
            }
        }

        [Pure]
        public static dynamic ToDynamic(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            dynamic expando = new ExpandoObject();
            var dictionary = expando as IDictionary<string, object>;
            dataRecord.GetFieldNames()
                .Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v))
                .ToList()
                .ForEach(value => dictionary.Add(value.Key, value.Value));
            return expando;
        }

        [Pure]
        public static IDictionary<string, object> ToOneDictionary(this SqlDataReader reader)
        {
            Contract.Requires(reader != null && reader.FieldCount >= 0);

            IEnumerable<string> fieldNames = reader.GetFieldNames();
            reader.Read();
            return reader.FieldCount >= 0 ? reader.ToDictionary(fieldNames) : null;
        }

        [Pure]
        public static dynamic ToOneDynamic(this SqlDataReader reader)
        {
            Contract.Requires(reader != null && reader.FieldCount >= 0);

            IEnumerable<string> fieldNames = reader.GetFieldNames();
            reader.Read();
            return reader.FieldCount >= 0 ? reader.ToDynamic(fieldNames) : null;
        }

        private static SqlParameter BuildParameter(SqlCommand command, object item)
        {
            SqlParameter parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", command.Parameters.Count);

            if (item == null)
                parameter.Value = DBNull.Value;
            else if (item is Guid)
            {
                parameter.Value = item;
                parameter.DbType = DbType.Guid;
            }
            else if (item is ExpandoObject || item is IDictionary<string, object>)
            {
                var dictioanry = (IDictionary<string, object>) item;
                parameter.Value = dictioanry.Values.FirstOrDefault();
            }
            else
            {
                var s = item as string;
                if (s != null)
                {
                    parameter.Size = s.Length;
                    parameter.Value = item;
                }
                else if (item is DateTime)
                {
                    parameter.SqlDbType = SqlDbType.DateTime;
                    parameter.Value = item;
                }
                else
                    parameter.Value = item;
            }

            return parameter;
        }

        [Pure]
        private static IEnumerable<string> GetFieldNames(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            return Enumerable.Range(0, dataRecord.FieldCount).Select(dataRecord.GetName);
        }

        [Pure]
        private static IEnumerable<object> GetValues(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            var values = new object[dataRecord.FieldCount];
            dataRecord.GetValues(values);
            return values.Replace(DBNull.Value, null);
        }

        [Pure]
        private static IDictionary<string, object> ToDictionary(this IDataRecord dataRecord,
            IEnumerable<string> fieldNames)
        {
            Contract.Requires(fieldNames != null);
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            fieldNames.Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v))
                .ToList()
                .ForEach(value => dictionary.Add(value.Key, value.Value));
            return dictionary;
        }

        [Pure]
        private static dynamic ToDynamic(this IDataRecord dataRecord, IEnumerable<string> fieldNames)
        {
            Contract.Requires(fieldNames != null);
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            dynamic expando = new ExpandoObject();
            var dictionary = expando as IDictionary<string, object>;
            fieldNames.Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v))
                .ToList()
                .ForEach(value => dictionary.Add(value.Key, value.Value));
            return expando;
        }

        public static short GetInt16(this SqlDataReader reader, ref int column)
        {
            short value = !reader.IsDBNull(column) ? reader.GetInt16(column) : default(short);
            column++;
            return value;
        }

        public static int GetInt32(this SqlDataReader reader, ref int column)
        {
            int value = !reader.IsDBNull(column) ? reader.GetInt32(column) : 0;
            column++;
            return value;
        }

        public static long GetInt64(this SqlDataReader reader, ref int column)
        {
            long value = !reader.IsDBNull(column) ? reader.GetInt64(column) : 0;
            column++;
            return value;
        }

        public static bool? GetNullableBoolean(this SqlDataReader reader, ref int column)
        {
            bool? value;

            if (!reader.IsDBNull(column)) value = reader.GetBoolean(column);
            else value = null;

            column++;
            return value;
        }

        public static double? GetNullableDouble(this SqlDataReader reader, ref int column)
        {
            double? value;

            if (!reader.IsDBNull(column)) value = reader.GetDouble(column);
            else value = null;

            column++;
            return value;
        }

        public static int? GetNullableInt32(this SqlDataReader reader, ref int column)
        {
            int? value;

            if (!reader.IsDBNull(column)) value = reader.GetInt32(column);
            else value = null;

            column++;
            return value;
        }

        public static string GetString(this SqlDataReader reader, ref int column)
        {
            string value = !reader.IsDBNull(column) ? reader.GetString(column) : null;
            column++;
            return value;
        }
    }
}