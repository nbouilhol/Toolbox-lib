using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities.SqlHelpers.Mapper;

namespace Utilities.SqlHelpers
{
    public class Repository : IRepository
    {
        private readonly Action<string, Exception> logger;

        public Repository(Action<string, Exception> logger)
        {
            Contract.Requires(logger != null);

            this.logger = logger;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.logger != null);
        }

        public virtual ICollection<T> Execute<T>(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            Mapper<T> mapper = Mapper<T>.Create();
            return Execute<T>(connectionString, commandText, reader => mapper.MapToList(reader), parameters);
        }

        public virtual ICollection<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, T> mapper, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute<T>(connectionString, commandText, reader => QueryMapping.Map<T>(reader, mapper), parameters);
        }

        public virtual T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            Mapper<T> mapper = Mapper<T>.Create();

            return ExecuteOneRow<T>(connectionString, commandText, reader => mapper.MapOne(reader), parameters);
        }

        public virtual ICollection<T> ExecuteAndFlatten<T>(string connectionString, string commandText, Func<SqlDataReader, ICollection<T>> mapper, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute<T>(connectionString, commandText, reader => QueryMapping.MapForFlatten<T>(reader, mapper).SelectMany(result => result), parameters);
        }

        public virtual ICollection<T> FindAll<T>(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return Execute<T>(connectionString, query);
        }

        public virtual T FindOne<T>(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return ExecuteOne<T>(connectionString, query);
        }

        public virtual ICollection<T> FindAllBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return Execute<T>(connectionString, query, properties.Values.ToArray());
        }

        public virtual T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return ExecuteOne<T>(connectionString, query, properties.Values.ToArray());
        }

        public virtual ICollection<IDictionary<string, object>> ExecuteDictionary(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute<IDictionary<string, object>>(connectionString, commandText, reader => reader.ToDictionary(), parameters);
        }

        public virtual IDictionary<string, object> ExecuteOneDictionary(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return ExecuteOneRow<IDictionary<string, object>>(connectionString, commandText, reader => reader.ToOneDictionary(), parameters);
        }

        public virtual ICollection<dynamic> ExecuteDynamic(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute<dynamic>(connectionString, commandText, reader => reader.ToDynamic(), parameters);
        }

        public virtual dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return ExecuteOneRow<dynamic>(connectionString, commandText, reader => reader.ToOneDynamic(), parameters);
        }

        public virtual ICollection<dynamic> FindAllDynamic(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return ExecuteDynamic(connectionString, query);
        }

        public virtual dynamic FindOneDynamic(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return ExecuteDynamic(connectionString, query);
        }

        public virtual ICollection<dynamic> FindAllByDynamic(string connectionString, string table, string where, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, where);
            return ExecuteDynamic(connectionString, query, parameters);
        }

        public virtual dynamic FindOneByDynamic(string connectionString, string table, string where, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, where);
            return ExecuteOneDynamic(connectionString, query, parameters);
        }

        public virtual ICollection<dynamic> FindAllByDynamic(string connectionString, string table, IDictionary<string, object> properties)
        {
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return ExecuteDynamic(connectionString, query, properties.Values.ToArray());
        }

        public virtual dynamic FindOneByDynamic(string connectionString, string table, IDictionary<string, object> properties)
        {
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return ExecuteOneDynamic(connectionString, query, properties.Values.ToArray());
        }

        public virtual ICollection<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, IEnumerable<T>> funcMapper, params object[] parameters)
        {
            Contract.Requires(funcMapper != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    using (SqlDataReader reader = sqlCommande.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        IEnumerable<T> result = funcMapper(reader);
                        return result != null ? result.ToList() : null;
                    }
                }
            }
            catch (Exception e)
            {
                logger(commandText, e);
            }

            return null;
        }

        public virtual TResult ExecuteScalar<TResult>(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    var result = sqlCommande.ExecuteScalar();
                    return result != null ? (TResult)result : default(TResult);
                }
            }
            catch (Exception e)
            {
                logger(commandText, e);
            }

            return default(TResult);
        }

        public virtual DataSet ExecuteDataSet(string connectionString, string commandText, DataSet dataSet, string tablename, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            if (dataSet == null) dataSet = new DataSet();
            if (string.IsNullOrEmpty(tablename)) tablename = Guid.NewGuid().ToString();

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommande))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                    if (dataSet.Tables.Contains(tablename))
                        dataSet.Tables.Remove(tablename);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    sqlDataAdapter.Fill(dataSet, tablename);

                    return dataSet;
                }
            }
            catch (Exception e)
            {
                logger(commandText, e);
            }

            return null;
        }

        public virtual DataSet ExecuteDataSet(string connectionString, string commandText, string tablename, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return ExecuteDataSet(connectionString, commandText, null, tablename, parameters);
        }

        public virtual DataTable ExecuteDataTable(string connectionString, string commandText, string tablename, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            if (string.IsNullOrEmpty(tablename)) tablename = Guid.NewGuid().ToString();

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommande))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    DataTable dataTable = new DataTable(tablename);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    sqlDataAdapter.Fill(dataTable);

                    return dataTable;
                }
            }
            catch (Exception e)
            {
                logger(commandText, e);
            }

            return null;
        }

        public virtual IDataReader ExecuteReader(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    return sqlCommande.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception e)
            {
                logger(commandText, e);
            }

            return null;
        }

        public virtual int ExecuteNonQuery(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            int recordCount = 0;

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    recordCount = sqlCommande.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                recordCount = -1;
                logger(commandText, e);
            }

            return recordCount;
        }

        public virtual T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper, params object[] parameters)
        {
            Contract.Requires(funcMapper != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0 && sqlCommande.Parameters != null)
                        sqlCommande.Parameters.AddRange(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    using (SqlDataReader reader = sqlCommande.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        reader.Read();
                        return funcMapper(reader);
                    }
                }

            }
            catch (Exception e)
            {
                logger(commandText, e);
            }

            return default(T);
        }

        public virtual dynamic ExecuteDynamicDataReader(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return new DynamicDataReader(ExecuteReader(connectionString, commandText, parameters));
        }

        public virtual int Empty(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            return ExecuteNonQuery(connectionString, QueryRequest.Truncate(table));
        }

        public static string CleanUpRequest(string source)
        {
            return Regex.Replace(source, @"\s+", " ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
    }
}
