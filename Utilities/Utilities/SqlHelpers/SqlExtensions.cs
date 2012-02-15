using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;

namespace Utilities.SqlHelpers
{
    public static partial class SqlExtensions
    {
        [Pure]
        public static void AddParams(this SqlCommand cmd, params object[] args)
        {
            Contract.Requires(args != null);
            Contract.Requires(cmd != null);

            foreach (object item in args)
                AddParam(cmd, item);
        }

        [Pure]
        public static void AddParam(this SqlCommand command, object item)
        {
            Contract.Requires(command != null);

            SqlParameter parameter = command.CreateParameter();
            parameter.ParameterName = string.Format("@{0}", command.Parameters != null ? command.Parameters.Count : 0);

            if (item == null)
                parameter.Value = DBNull.Value;
            else if (item is Guid)
            {
                parameter.Value = item;
                parameter.DbType = DbType.Guid;
            }
            else if (item is ExpandoObject || item is IDictionary<string, object>)
            {
                var dictioanry = (IDictionary<string, object>)item;
                parameter.Value = dictioanry.Values.FirstOrDefault();
            }
            else if (item is string)
            {
                parameter.Size = ((string)item).Length;
                parameter.Value = item;
            }
            else
                parameter.Value = item;

            if (command.Parameters != null) command.Parameters.Add(parameter);
        }

        [Pure]
        public static IEnumerable<dynamic> ToDynamic(this SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                IEnumerable<string> fieldNames = reader.GetFieldNames();

                while (reader.Read())
                    yield return reader.ToDynamic(fieldNames);

                reader.NextResult();
            }
        }

        [Pure]
        public static dynamic ToOneDynamic(this SqlDataReader reader)
        {
            Contract.Requires(reader != null && reader.FieldCount >= 0);

            IEnumerable<string> fieldNames = reader.GetFieldNames();
            reader.Read();
            return reader != null && reader.FieldCount >= 0 ? reader.ToDynamic(fieldNames) : null;
        }

        [Pure]
        public static IEnumerable<IDictionary<string, object>> ToDictionary(this SqlDataReader reader)
        {
            while (reader.HasRows)
            {
                IEnumerable<string> fieldNames = reader.GetFieldNames();

                while (reader.Read())
                    yield return reader.ToDictionary(fieldNames);

                reader.NextResult();
            }
        }

        [Pure]
        public static IDictionary<string, object> ToOneDictionary(this SqlDataReader reader)
        {
            Contract.Requires(reader != null && reader.FieldCount >= 0);

            IEnumerable<string> fieldNames = reader.GetFieldNames();
            reader.Read();
            return reader != null && reader.FieldCount >= 0 ? reader.ToDictionary(fieldNames) : null;
        }

        [Pure]
        public static IDictionary<string, object> ToDictionary(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dataRecord.GetFieldNames().Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v)).ToList().ForEach(value => dictionary.Add(value.Key, value.Value));
            return dictionary;
        }

        [Pure]
        public static dynamic ToDynamic(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            dynamic expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            dataRecord.GetFieldNames().Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v)).ToList().ForEach(value => dictionary.Add(value.Key, value.Value));
            return expando;
        }

        [Pure]
        private static IDictionary<string, object> ToDictionary(this IDataRecord dataRecord, IEnumerable<string> fieldNames)
        {
            Contract.Requires(fieldNames != null);
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            fieldNames.Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v)).ToList().ForEach(value => dictionary.Add(value.Key, value.Value));
            return dictionary;
        }

        [Pure]
        private static dynamic ToDynamic(this IDataRecord dataRecord, IEnumerable<string> fieldNames)
        {
            Contract.Requires(fieldNames != null);
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            dynamic expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            fieldNames.Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v)).ToList().ForEach(value => dictionary.Add(value.Key, value.Value));
            return expando;
        }

        [Pure]
        private static IEnumerable<string> GetFieldNames(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            return Enumerable.Range(0, dataRecord.FieldCount).Select(i => dataRecord.GetName(i));
        }

        [Pure]
        private static IEnumerable<object> GetValues(this IDataRecord dataRecord)
        {
            Contract.Requires(dataRecord != null && dataRecord.FieldCount >= 0);

            object[] values = new object[dataRecord.FieldCount];
            dataRecord.GetValues(values);
            return values.Replace(DBNull.Value, null);
        }

        public static string In<TValue>(this string source, string property, IEnumerable<TValue> parameters)
        {
            return In(source, property, parameters, false);
        }

        public static string NotIn<TValue>(this string source, string property, IEnumerable<TValue> parameters)
        {
            return In(source, property, parameters, true);
        }

        public static string In<TValue>(this string source, string property, IEnumerable<TValue> parameters, bool isNotIn)
        {
            Contract.Requires(source != null);
            Contract.Requires(property != null);

            string empty = "''";
            string clause = string.Format("{{IN:{0}}}", property);
            string newClauseParameter = !isNotIn && parameters != null && parameters.Count() != 0 ? string.Join(",", parameters) : empty;
            string newClause = string.Format("{0}IN ({1})", isNotIn ? "NOT " : "", newClauseParameter);
            return source.Replace(clause, newClause);
        }
    }
}
