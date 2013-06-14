using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Utilities.SqlHelpers
{
    public interface IRepository
    {
        int BulkDelete(string connectionString, string commandText, DataTable dataTable,
            params SqlParameter[] parameters);

        int BulkUpdate(string connectionString, string commandText, DataTable dataTable,
            params SqlParameter[] parameters);

        int Empty(string connectionString, string table);

        IList<T> Execute<T>(string connectionString, string commandText, params object[] parameters);

        IList<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, IEnumerable<T>> funcMapper,
            params object[] parameters);

        IList<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, T> mapper,
            params object[] parameters);

        IList<T> ExecuteAndFlatten<T>(string connectionString, string commandText,
            Func<SqlDataReader, ICollection<T>> mapper, params object[] parameters);

        IEnumerable<T> ExecuteAsStream<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper,
            params object[] parameters);

        IEnumerable<T> ExecuteAsStream<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper,
            Action<SqlCommand> funcAddParams);

        DataSet ExecuteDataSet(string connectionString, string commandText, DataSet dataSet, string tablename,
            params object[] parameters);

        DataSet ExecuteDataSet(string connectionString, string commandText, string tablename, params object[] parameters);

        DataTable ExecuteDataTable(string connectionString, string commandText, string tablename,
            params object[] parameters);

        IList<IDictionary<string, object>> ExecuteDictionary(string connectionString, string commandText,
            params object[] parameters);

        IList<dynamic> ExecuteDynamic(string connectionString, string commandText, params object[] parameters);

        dynamic ExecuteDynamicDataReader(string connectionString, string commandText, params object[] parameters);

        int ExecuteNonQuery(string connectionString, string commandText,
            IEnumerable<IEnumerable<SqlParameter>> parameters);

        int ExecuteNonQuery(string connectionString, string commandText, params object[] parameters);

        TResult ExecuteNonQueryWithIdentity<TResult>(string connectionString, string commandText,
            params object[] parameters);

        T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters);

        IDictionary<string, object> ExecuteOneDictionary(string connectionString, string commandText,
            params object[] parameters);

        dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters);

        T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper,
            params object[] parameters);

        IDataReader ExecuteReader(string connectionString, string commandText, params object[] parameters);

        TResult ExecuteScalar<TResult>(string connectionString, string commandText, params object[] parameters);

        IList<T> FindAll<T>(string connectionString, string table);

        IList<T> FindAllBy<T>(string connectionString, string table, IDictionary<string, object> properties);

        IList<dynamic> FindAllByDynamic(string connectionString, string table, IDictionary<string, object> properties);

        IList<dynamic> FindAllByDynamic(string connectionString, string table, string where, params object[] parameters);

        IList<dynamic> FindAllDynamic(string connectionString, string table);

        T FindOne<T>(string connectionString, string table);

        T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties);

        dynamic FindOneByDynamic(string connectionString, string table, IDictionary<string, object> properties);

        dynamic FindOneByDynamic(string connectionString, string table, string where, params object[] parameters);

        dynamic FindOneDynamic(string connectionString, string table);
    }
}