using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Utilities.SqlHelpers.Repository
{
    public static class Query
    {
        private static readonly Lazy<IRepository> Repository = new Lazy<IRepository>(() => new Repository(Throw));

        public static ICollection<T> Execute<T>(string connectionString, string commandText, params object[] parameters)
        {
            return Repository.Value.Execute<T>(connectionString, commandText, parameters);
        }

        public static ICollection<T> Execute<T>(string connectionString, string commandText,
            Func<SqlDataReader, T> mapper, params object[] parameters)
        {
            return Repository.Value.Execute(connectionString, commandText, mapper, parameters);
        }

        public static T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters)
        {
            return Repository.Value.ExecuteOne<T>(connectionString, commandText, parameters);
        }

        public static ICollection<T> ExecuteAndFlatten<T>(string connectionString, string commandText,
            Func<SqlDataReader, ICollection<T>> mapper, params object[] parameters)
        {
            return Repository.Value.ExecuteAndFlatten(connectionString, commandText, mapper, parameters);
        }

        public static ICollection<T> FindAll<T>(string connectionString, string table)
        {
            return Repository.Value.FindAll<T>(connectionString, table);
        }

        public static T FindOne<T>(string connectionString, string table)
        {
            return Repository.Value.FindOne<T>(connectionString, table);
        }

        public static ICollection<T> FindAllBy<T>(string connectionString, string table,
            IDictionary<string, object> properties)
        {
            return Repository.Value.FindAllBy<T>(connectionString, table, properties);
        }

        public static T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            return Repository.Value.FindOneBy<T>(connectionString, table, properties);
        }

        public static ICollection<IDictionary<string, object>> ExecuteDictionary(string connectionString,
            string commandText, params object[] parameters)
        {
            return Repository.Value.ExecuteDictionary(connectionString, commandText, parameters);
        }

        public static IDictionary<string, object> ExecuteOneDictionary(string connectionString, string commandText,
            params object[] parameters)
        {
            return Repository.Value.ExecuteOneDictionary(connectionString, commandText, parameters);
        }

        public static ICollection<dynamic> ExecuteDynamic(string connectionString, string commandText,
            params object[] parameters)
        {
            return Repository.Value.ExecuteDynamic(connectionString, commandText, parameters);
        }

        public static dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters)
        {
            return Repository.Value.ExecuteOneDynamic(connectionString, commandText, parameters);
        }

        public static ICollection<dynamic> FindAllDynamic(string connectionString, string table)
        {
            return Repository.Value.FindAllDynamic(connectionString, table);
        }

        public static dynamic FindOneDynamic(string connectionString, string table)
        {
            return Repository.Value.FindOneDynamic(connectionString, table);
        }

        public static ICollection<dynamic> FindAllByDynamic(string connectionString, string table, string where,
            params object[] parameters)
        {
            return Repository.Value.FindAllByDynamic(connectionString, table, where, parameters);
        }

        public static dynamic FindOneByDynamic(string connectionString, string table, string where,
            params object[] parameters)
        {
            return Repository.Value.FindOneByDynamic(connectionString, table, where, parameters);
        }

        public static ICollection<dynamic> FindAllByDynamic(string connectionString, string table,
            IDictionary<string, object> properties)
        {
            return Repository.Value.FindAllByDynamic(connectionString, table, properties);
        }

        public static dynamic FindOneByDynamic(string connectionString, string table,
            IDictionary<string, object> properties)
        {
            return Repository.Value.FindOneByDynamic(connectionString, table, properties);
        }

        public static ICollection<T> Execute<T>(string connectionString, string commandText,
            Func<SqlDataReader, IEnumerable<T>> funcMapper, params object[] parameters)
        {
            return Repository.Value.Execute(connectionString, commandText, funcMapper, parameters);
        }

        public static T ExecuteScalar<T>(string connectionString, string commandText, params object[] parameters)
        {
            return Repository.Value.ExecuteScalar<T>(connectionString, commandText, parameters);
        }

        public static DataSet ExecuteDataSet(string connectionString, string commandText, DataSet dataSet,
            string tablename, params object[] parameters)
        {
            return Repository.Value.ExecuteDataSet(connectionString, commandText, dataSet, tablename, parameters);
        }

        public static DataSet ExecuteDataSet(string connectionString, string commandText, string tablename,
            params object[] parameters)
        {
            return Repository.Value.ExecuteDataSet(connectionString, commandText, tablename, parameters);
        }

        public static DataTable ExecuteDataTable(string connectionString, string commandText, string tablename,
            params object[] parameters)
        {
            return Repository.Value.ExecuteDataTable(connectionString, commandText, tablename, parameters);
        }

        public static IDataReader ExecuteReader(string connectionString, string commandText, params object[] parameters)
        {
            return Repository.Value.ExecuteReader(connectionString, commandText, parameters);
        }

        public static int ExecuteNonQuery(string connectionString, string commandText, params object[] parameters)
        {
            return Repository.Value.ExecuteNonQuery(connectionString, commandText, parameters);
        }

        public static T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper,
            params object[] parameters)
        {
            return Repository.Value.ExecuteOneRow(connectionString, commandText, funcMapper, parameters);
        }

        public static dynamic ExecuteDynamicDataReader(string connectionString, string commandText,
            params object[] parameters)
        {
            return Repository.Value.ExecuteDynamicDataReader(connectionString, commandText, parameters);
        }

        public static int Empty(string connectionString, string table)
        {
            return Repository.Value.Empty(connectionString, table);
        }

        private static void Throw(string message, Exception exception)
        {
            throw exception;
        }
    }
}