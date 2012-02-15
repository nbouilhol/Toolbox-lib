using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Utilities.SqlHelpers
{
    public interface IRepository
    {
        int Empty(string connectionString, string table);
        ICollection<T> Execute<T>(string connectionString, string commandText, params object[] parameters);
        ICollection<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, IEnumerable<T>> funcMapper, params object[] parameters);
        ICollection<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, T> mapper, params object[] parameters);
        ICollection<T> ExecuteAndFlatten<T>(string connectionString, string commandText, Func<SqlDataReader, ICollection<T>> mapper, params object[] parameters);
        DataSet ExecuteDataSet(string connectionString, string commandText, DataSet dataSet, string tablename, params object[] parameters);
        DataSet ExecuteDataSet(string connectionString, string commandText, string tablename, params object[] parameters);
        DataTable ExecuteDataTable(string connectionString, string commandText, string tablename, params object[] parameters);
        ICollection<IDictionary<string, object>> ExecuteDictionary(string connectionString, string commandText, params object[] parameters);
        ICollection<dynamic> ExecuteDynamic(string connectionString, string commandText, params object[] parameters);
        dynamic ExecuteDynamicDataReader(string connectionString, string commandText, params object[] parameters);
        int ExecuteNonQuery(string connectionString, string commandText, params object[] parameters);
        T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters);
        IDictionary<string, object> ExecuteOneDictionary(string connectionString, string commandText, params object[] parameters);
        dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters);
        T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper, params object[] parameters);
        IDataReader ExecuteReader(string connectionString, string commandText, params object[] parameters);
        TResult ExecuteScalar<TResult>(string connectionString, string commandText, params object[] parameters);
        ICollection<T> FindAll<T>(string connectionString, string table);
        ICollection<T> FindAllBy<T>(string connectionString, string table, IDictionary<string, object> properties);
        ICollection<dynamic> FindAllByDynamic(string connectionString, string table, IDictionary<string, object> properties);
        ICollection<dynamic> FindAllByDynamic(string connectionString, string table, string where, params object[] parameters);
        ICollection<dynamic> FindAllDynamic(string connectionString, string table);
        T FindOne<T>(string connectionString, string table);
        T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties);
        dynamic FindOneByDynamic(string connectionString, string table, IDictionary<string, object> properties);
        dynamic FindOneByDynamic(string connectionString, string table, string where, params object[] parameters);
        dynamic FindOneDynamic(string connectionString, string table);
    }
}
