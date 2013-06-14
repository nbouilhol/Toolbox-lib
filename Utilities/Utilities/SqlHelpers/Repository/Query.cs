using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Utilities.SqlHelpers
{
    public static class Query
    {
        private static readonly Lazy<IRepository> repository = new Lazy<IRepository>(() => new Repository(Throw));

        public static ICollection<T> Execute<T>(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.Execute<T>(connectionString, commandText, parameters);
        }

        public static ICollection<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, T> mapper, params object[] parameters)
        {
            return repository.Value.Execute<T>(connectionString, commandText, mapper, parameters);
        }

        public static T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteOne<T>(connectionString, commandText, parameters);
        }

        public static ICollection<T> ExecuteAndFlatten<T>(string connectionString, string commandText, Func<SqlDataReader, ICollection<T>> mapper, params object[] parameters)
        {
            return repository.Value.ExecuteAndFlatten<T>(connectionString, commandText, mapper, parameters);
        }

        public static ICollection<T> FindAll<T>(string connectionString, string table)
        {
            return repository.Value.FindAll<T>(connectionString, table);
        }

        public static T FindOne<T>(string connectionString, string table)
        {
            return repository.Value.FindOne<T>(connectionString, table);
        }

        public static ICollection<T> FindAllBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            return repository.Value.FindAllBy<T>(connectionString, table, properties);
        }

        public static T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            return repository.Value.FindOneBy<T>(connectionString, table, properties);
        }

        public static ICollection<IDictionary<string, object>> ExecuteDictionary(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteDictionary(connectionString, commandText, parameters);
        }

        public static IDictionary<string, object> ExecuteOneDictionary(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteOneDictionary(connectionString, commandText, parameters);
        }

        public static ICollection<dynamic> ExecuteDynamic(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteDynamic(connectionString, commandText, parameters);
        }

        public static dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteOneDynamic(connectionString, commandText, parameters);
        }

        public static ICollection<dynamic> FindAllDynamic(string connectionString, string table)
        {
            return repository.Value.FindAllDynamic(connectionString, table);
        }

        public static dynamic FindOneDynamic(string connectionString, string table)
        {
            return repository.Value.FindOneDynamic(connectionString, table);
        }

        public static ICollection<dynamic> FindAllByDynamic(string connectionString, string table, string where, params object[] parameters)
        {
            return repository.Value.FindAllByDynamic(connectionString, table, where, parameters);
        }

        public static dynamic FindOneByDynamic(string connectionString, string table, string where, params object[] parameters)
        {
            return repository.Value.FindOneByDynamic(connectionString, table, where, parameters);
        }

        public static ICollection<dynamic> FindAllByDynamic(string connectionString, string table, IDictionary<string, object> properties)
        {
            return repository.Value.FindAllByDynamic(connectionString, table, properties);
        }

        public static dynamic FindOneByDynamic(string connectionString, string table, IDictionary<string, object> properties)
        {
            return repository.Value.FindOneByDynamic(connectionString, table, properties);
        }

        public static ICollection<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, IEnumerable<T>> funcMapper, params object[] parameters)
        {
            return repository.Value.Execute<T>(connectionString, commandText, funcMapper, parameters);
        }

        public static T ExecuteScalar<T>(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteScalar<T>(connectionString, commandText, parameters);
        }

        public static DataSet ExecuteDataSet(string connectionString, string commandText, DataSet dataSet, string tablename, params object[] parameters)
        {
            return repository.Value.ExecuteDataSet(connectionString, commandText, dataSet, tablename, parameters);
        }

        public static DataSet ExecuteDataSet(string connectionString, string commandText, string tablename, params object[] parameters)
        {
            return repository.Value.ExecuteDataSet(connectionString, commandText, tablename, parameters);
        }

        public static DataTable ExecuteDataTable(string connectionString, string commandText, string tablename, params object[] parameters)
        {
            return repository.Value.ExecuteDataTable(connectionString, commandText, tablename, parameters);
        }

        public static IDataReader ExecuteReader(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteReader(connectionString, commandText, parameters);
        }

        public static int ExecuteNonQuery(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteNonQuery(connectionString, commandText, parameters);
        }

        public static T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper, params object[] parameters)
        {
            return repository.Value.ExecuteOneRow<T>(connectionString, commandText, funcMapper, parameters);
        }

        public static dynamic ExecuteDynamicDataReader(string connectionString, string commandText, params object[] parameters)
        {
            return repository.Value.ExecuteDynamicDataReader(connectionString, commandText, parameters);
        }

        public static int Empty(string connectionString, string table)
        {
            return repository.Value.Empty(connectionString, table);
        }

        private static void Throw(string message, Exception exception)
        {
            throw exception;
        }
    }
}