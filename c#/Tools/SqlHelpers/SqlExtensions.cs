using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Dynamic;
using System.Data;
using System.Collections.Specialized;
using System.Reflection;
using System.Data.SqlClient;

namespace BouilholLib.Helper
{
    public static partial class SqlExtensions
    {
        public static void AddParams(this SqlCommand cmd, params object[] args)
        {
            foreach (object item in args)
                AddParam(cmd, item);
        }

        public static void AddParam(this SqlCommand command, object item)
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
            command.Parameters.Add(parameter);
        }

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

        public static dynamic ToOneDynamic(this SqlDataReader reader)
        {
            IEnumerable<string> fieldNames = reader.GetFieldNames();
            reader.Read();
            return reader.ToDynamic(fieldNames);
        }

        private static dynamic ToDynamic(this IDataRecord dataRecord, IEnumerable<string> fieldNames)
        {
            dynamic expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando as IDictionary<string, object>;
            fieldNames.Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v)).ToList().ForEach(value => dictionary.Add(value.Key, value.Value));
            return expando;
        }

        public static dynamic ToDynamic(this IDataReader dataRecord)
        {
            dynamic expando = new ExpandoObject();
            var dictionary = expando as IDictionary<string, object>;
            dataRecord.GetFieldNames().Zip(dataRecord.GetValues(), (f, v) => new KeyValuePair<string, object>(f, v)).ToList().ForEach(value => dictionary.Add(value.Key, value.Value));
            return expando;
        }

        private static IEnumerable<string> GetFieldNames(this IDataRecord dataRecord)
        {
            return Enumerable.Range(0, dataRecord.FieldCount).Select(i => dataRecord.GetName(i));
        }

        private static IEnumerable<object> GetValues(this IDataRecord dataRecord)
        {
            object[] values = new object[dataRecord.FieldCount];
            dataRecord.GetValues(values);
            return values.Replace(DBNull.Value, null);
        }
    }
}
