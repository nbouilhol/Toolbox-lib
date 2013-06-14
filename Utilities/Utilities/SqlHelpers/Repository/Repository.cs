using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities.SqlHelpers.Mapper;

namespace Utilities.SqlHelpers.Repository
{
    public class Repository : IRepository
    {
        private const int CommandTimeout = 30;

        private readonly Action<string, Exception> _logger;

        public Repository(Action<string, Exception> logger)
        {
            Contract.Requires(logger != null);

            _logger = logger;
        }

        public virtual int BulkDelete(string connectionString, string commandText, DataTable dataTable,
            params SqlParameter[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            int recordCount;

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (var sqlDataAdapter = new SqlDataAdapter())
            using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
            {
                sqlCommande.CommandType = CommandType.Text;
                sqlCommande.Parameters.AddRange(parameters != null && parameters.Length > 0
                    ? parameters
                    : BuildSqlParameterByDataTable(dataTable));
                sqlCommande.UpdatedRowSource = UpdateRowSource.None;

                sqlDataAdapter.AcceptChangesDuringUpdate = false;
                sqlDataAdapter.UpdateBatchSize = 5000;
                sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.Add;
                sqlDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;

                sqlDataAdapter.InsertCommand = sqlCommande;
                sqlDataAdapter.UpdateCommand = sqlCommande;
                sqlDataAdapter.DeleteCommand = sqlCommande;

                if (sqlConnexion.State == ConnectionState.Closed) sqlConnexion.Open();
                using (SqlTransaction sqlTransaction = sqlConnexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    sqlCommande.Transaction = sqlTransaction;

                    try
                    {
                        recordCount = sqlDataAdapter.Update(dataTable);
                        sqlTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        sqlTransaction.Rollback();
                        recordCount = -1;
                        _logger(commandText, e);
                    }
                }
            }

            return recordCount;
        }

        public virtual int BulkUpdate(string connectionString, string commandText, DataTable dataTable,
            params SqlParameter[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            int recordCount;

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (var sqlDataAdapter = new SqlDataAdapter())
            using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
            {
                sqlCommande.CommandType = CommandType.Text;
                sqlCommande.CommandTimeout = 900;
                sqlCommande.Parameters.AddRange(parameters != null && parameters.Length > 0
                    ? parameters
                    : BuildSqlParameterByDataTable(dataTable));
                sqlCommande.UpdatedRowSource = UpdateRowSource.None;

                sqlDataAdapter.AcceptChangesDuringUpdate = false;
                sqlDataAdapter.UpdateBatchSize = 5000;
                sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.Add;
                sqlDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;

                sqlDataAdapter.InsertCommand = sqlCommande;
                sqlDataAdapter.UpdateCommand = sqlCommande;

                if (sqlConnexion.State == ConnectionState.Closed) sqlConnexion.Open();
                using (SqlTransaction sqlTransaction = sqlConnexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    sqlCommande.Transaction = sqlTransaction;

                    try
                    {
                        recordCount = sqlDataAdapter.Update(dataTable);
                        sqlTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        sqlTransaction.Rollback();
                        recordCount = -1;
                        _logger(commandText, e);
                    }
                }
            }

            return recordCount;
        }

        public virtual int Empty(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            return ExecuteNonQuery(connectionString, QueryRequest.Truncate(table));
        }

        public virtual IList<T> Execute<T>(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            Mapper<T> mapper = Mapper<T>.Create();
            return Execute(connectionString, commandText, mapper.MapToList, parameters);
        }

        public virtual IList<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, T> mapper,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute(connectionString, commandText, reader => QueryMapping.Map(reader, mapper), parameters);
        }

        public virtual IList<T> Execute<T>(string connectionString, string commandText,
            Func<SqlDataReader, IEnumerable<T>> funcMapper, params object[] parameters)
        {
            Contract.Requires(funcMapper != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (var sqlConnexion = new SqlConnection(connectionString))
                using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
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
                _logger(
                    parameters != null
                        ? string.Concat(commandText, " ", string.Concat(parameters.Select(x => x.ToString())))
                        : commandText, e);
            }

            return null;
        }

        public virtual IList<T> ExecuteAndFlatten<T>(string connectionString, string commandText,
            Func<SqlDataReader, ICollection<T>> mapper, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute(connectionString, commandText,
                reader => QueryMapping.MapForFlatten(reader, mapper).SelectMany(result => result), parameters);
        }

        public virtual IEnumerable<T> ExecuteAsStream<T>(string connectionString, string commandText,
            Func<SqlDataReader, T> funcMapper, params object[] parameters)
        {
            return ExecuteAsStream(connectionString, commandText, funcMapper,
                sqlCommand => AddParams(sqlCommand, parameters));
        }

        public virtual IEnumerable<T> ExecuteAsStream<T>(string connectionString, string commandText,
            Func<SqlDataReader, T> funcMapper, Action<SqlCommand> funcAddParams)
        {
            Contract.Requires(funcMapper != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
            {
                sqlCommande.CommandType = CommandType.Text;
                sqlCommande.CommandTimeout = CommandTimeout;
                funcAddParams(sqlCommande);

                if (sqlConnexion.State == ConnectionState.Closed)
                    sqlConnexion.Open();

                using (SqlDataReader reader = sqlCommande.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read()) yield return funcMapper(reader);
                        reader.NextResult();
                    }
                }
            }
        }

        public virtual DataSet ExecuteDataSet(string connectionString, string commandText, DataSet dataSet,
            string tablename, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            if (dataSet == null) dataSet = new DataSet();
            if (string.IsNullOrEmpty(tablename)) tablename = Guid.NewGuid().ToString();

            try
            {
                using (var sqlConnexion = new SqlConnection(connectionString))
                using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                using (var sqlDataAdapter = new SqlDataAdapter(sqlCommande))
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
                _logger(commandText, e);
            }

            return null;
        }

        public virtual DataSet ExecuteDataSet(string connectionString, string commandText, string tablename,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return ExecuteDataSet(connectionString, commandText, null, tablename, parameters);
        }

        public virtual DataTable ExecuteDataTable(string connectionString, string commandText, string tablename,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            if (string.IsNullOrEmpty(tablename)) tablename = Guid.NewGuid().ToString();

            try
            {
                using (var sqlConnexion = new SqlConnection(connectionString))
                using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                using (var sqlDataAdapter = new SqlDataAdapter(sqlCommande))
                {
                    sqlCommande.CommandType = CommandType.Text;

                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    sqlDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    var dataTable = new DataTable(tablename);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    sqlDataAdapter.Fill(dataTable);

                    return dataTable;
                }
            }
            catch (Exception e)
            {
                _logger(commandText, e);
            }

            return null;
        }

        public virtual IList<IDictionary<string, object>> ExecuteDictionary(string connectionString, string commandText,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute(connectionString, commandText, reader => reader.ToDictionary(), parameters);
        }

        public virtual IList<dynamic> ExecuteDynamic(string connectionString, string commandText,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return Execute(connectionString, commandText, reader => reader.ToDynamic(), parameters);
        }

        public virtual dynamic ExecuteDynamicDataReader(string connectionString, string commandText,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return new DynamicDataReader(ExecuteReader(connectionString, commandText, parameters));
        }

        public virtual int ExecuteNonQuery(string connectionString, string commandText,
            IEnumerable<IEnumerable<SqlParameter>> parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            int recordCount = 0;

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
            {
                sqlCommande.CommandType = CommandType.Text;
                sqlCommande.UpdatedRowSource = UpdateRowSource.None;

                if (sqlConnexion.State == ConnectionState.Closed) sqlConnexion.Open();
                using (SqlTransaction sqlTransaction = sqlConnexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    sqlCommande.Transaction = sqlTransaction;

                    try
                    {
                        foreach (var parameter in parameters)
                        {
                            sqlCommande.Parameters.Clear();
                            sqlCommande.Parameters.AddRange(parameter.ToArray());

                            recordCount += sqlCommande.ExecuteNonQuery();
                        }

                        sqlTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        sqlTransaction.Rollback();
                        recordCount = -1;
                        _logger(commandText, e);
                    }
                }
            }

            return recordCount;
        }

        public virtual int ExecuteNonQuery(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            int recordCount;

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
            {
                sqlCommande.CommandType = CommandType.Text;
                sqlCommande.UpdatedRowSource = UpdateRowSource.None;
                if (parameters != null && parameters.Length > 0) sqlCommande.AddParams(parameters);

                if (sqlConnexion.State == ConnectionState.Closed) sqlConnexion.Open();

                using (SqlTransaction sqlTransaction = sqlConnexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    sqlCommande.Transaction = sqlTransaction;

                    try
                    {
                        recordCount = sqlCommande.ExecuteNonQuery();
                        sqlTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        sqlTransaction.Rollback();
                        recordCount = -1;
                        _logger(commandText, e);
                    }
                }
            }

            return recordCount;
        }

        public virtual TResult ExecuteNonQueryWithIdentity<TResult>(string connectionString, string commandText,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            using (var sqlConnexion = new SqlConnection(connectionString))
            using (
                var sqlCommande = new SqlCommand(
                    CleanUpRequest(string.Concat(commandText, ";set @ID=SCOPE_IDENTITY()")), sqlConnexion))
            {
                sqlCommande.CommandType = CommandType.Text;
                sqlCommande.UpdatedRowSource = UpdateRowSource.OutputParameters;

                if (parameters != null && parameters.Length > 0)
                    sqlCommande.AddParams(parameters);

                sqlCommande.Parameters.Add("@ID", typeof (TResult).ToSqlType(), 0, "ID");
                sqlCommande.Parameters["@ID"].Direction = ParameterDirection.Output;

                if (sqlConnexion.State == ConnectionState.Closed)
                    sqlConnexion.Open();

                using (SqlTransaction sqlTransaction = sqlConnexion.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    sqlCommande.Transaction = sqlTransaction;

                    try
                    {
                        sqlCommande.ExecuteNonQuery();
                        sqlTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        sqlTransaction.Rollback();
                        _logger(commandText, e);
                    }
                }

                return (TResult) sqlCommande.Parameters["@ID"].Value;
            }
        }

        public virtual T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            Mapper<T> mapper = Mapper<T>.Create();

            return ExecuteOneRow(connectionString, commandText, mapper.MapOne, parameters);
        }

        public virtual IDictionary<string, object> ExecuteOneDictionary(string connectionString, string commandText,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return ExecuteOneRow(connectionString, commandText, reader => reader.ToOneDictionary(), parameters);
        }

        public virtual dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            return ExecuteOneRow(connectionString, commandText, reader => reader.ToOneDynamic(), parameters);
        }

        public virtual T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper,
            params object[] parameters)
        {
            Contract.Requires(funcMapper != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (var sqlConnexion = new SqlConnection(connectionString))
                using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;

                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    using (SqlDataReader reader = sqlCommande.ExecuteReader(CommandBehavior.SingleRow))
                        if (reader.HasRows && reader.Read()) return funcMapper(reader);
                }
            }
            catch (Exception e)
            {
                _logger(
                    parameters != null
                        ? string.Concat(commandText, " ", string.Concat(parameters.Select(x => x.ToString())))
                        : commandText, e);
            }

            return default(T);
        }

        public virtual IDataReader ExecuteReader(string connectionString, string commandText, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (var sqlConnexion = new SqlConnection(connectionString))
                using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
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
                _logger(commandText, e);
            }

            return null;
        }

        public virtual TResult ExecuteScalar<TResult>(string connectionString, string commandText,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(commandText != null);

            try
            {
                using (var sqlConnexion = new SqlConnection(connectionString))
                using (var sqlCommande = new SqlCommand(CleanUpRequest(commandText), sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;

                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    object result = sqlCommande.ExecuteScalar();
                    return result != null ? (TResult) result : default(TResult);
                }
            }
            catch (Exception e)
            {
                _logger(commandText, e);
            }

            return default(TResult);
        }

        public virtual IList<T> FindAll<T>(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return Execute<T>(connectionString, query);
        }

        public virtual IList<T> FindAllBy<T>(string connectionString, string table,
            IDictionary<string, object> properties)
        {
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return Execute<T>(connectionString, query, properties.Values.ToArray());
        }

        public virtual IList<dynamic> FindAllByDynamic(string connectionString, string table, string where,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, where);
            return ExecuteDynamic(connectionString, query, parameters);
        }

        public virtual IList<dynamic> FindAllByDynamic(string connectionString, string table,
            IDictionary<string, object> properties)
        {
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return ExecuteDynamic(connectionString, query, properties.Values.ToArray());
        }

        public virtual IList<dynamic> FindAllDynamic(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return ExecuteDynamic(connectionString, query);
        }

        public virtual T FindOne<T>(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return ExecuteOne<T>(connectionString, query);
        }

        public virtual T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return ExecuteOne<T>(connectionString, query, properties.Values.ToArray());
        }

        public virtual dynamic FindOneByDynamic(string connectionString, string table, string where,
            params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, where);
            return ExecuteOneDynamic(connectionString, query, parameters);
        }

        public virtual dynamic FindOneByDynamic(string connectionString, string table,
            IDictionary<string, object> properties)
        {
            Contract.Requires(properties != null);
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllWhere(table, properties.Keys);
            return ExecuteOneDynamic(connectionString, query, properties.Values.ToArray());
        }

        public virtual dynamic FindOneDynamic(string connectionString, string table)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(table));

            string query = QueryRequest.FindAllQuery(table);
            return ExecuteDynamic(connectionString, query);
        }

        public static string CleanUpRequest(string source)
        {
            string rawText = source;

            rawText = SimpleRequestCleanUp(rawText);
            rawText = RemoveExtraSpacing(rawText);

            return rawText.Trim();
            //return Regex.Replace(source, @"\s+", " ", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        private static void AddParams(SqlCommand sqlCommande, params object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
                sqlCommande.AddParams(parameters);
        }

        private static SqlParameter[] BuildSqlParameterByDataTable(DataTable dataTable)
        {
            return dataTable.Columns.Cast<DataColumn>().Select(column =>
            {
                var sqlParameter = new SqlParameter(FormatParametername(column.ColumnName), column.DataType.ToSqlType()) { SourceColumn = column.ColumnName };
                return sqlParameter;
            }).ToArray();
        }

        private static string FormatParametername(string name)
        {
            return string.Format("@{0}", name.Replace(".", null));
        }

        private static string RemoveExtraSpacing(string source)
        {
            const string pattern = @"'([^']|'')*'|[ ]{2,}";
            MatchCollection matches = Regex.Matches(source, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            int skip = 0;
            foreach (
                Match matche in
                    matches.Cast<Match>().Where(match => !match.Value.StartsWith("'") && !match.Value.EndsWith("'")))
            {
                int index = matche.Index - skip;
                source = string.Concat(source.Substring(0, index), " ", source.Substring(index + matche.Length));
                skip += matche.Length - 1;
            }

            return source;
        }

        private static string SimpleRequestCleanUp(string source)
        {
            //string pattern = @"('(''|[^'])*')|([\r|\n][\s| ]*[\r|\n])|(--[^\r\n]*)|(/\*[\w\W]*?(?=\*/)\*/)";
            const string pattern = @"('(''|[^'])*')|[\t\r\n]|(--[^\r\n]*)|(/\*[\w\W]*?(?=\*/)\*/)";
            MatchCollection matches = Regex.Matches(source, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            int skip = 0;
            foreach (
                Match matche in
                    matches.Cast<Match>().Where(match => !match.Value.StartsWith("'") && !match.Value.EndsWith("'")))
            {
                int index = matche.Index - skip;
                source = string.Concat(source.Substring(0, index), " ", source.Substring(index + matche.Length));
                skip += matche.Length - 1;
            }

            return source;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(_logger != null);
        }
    }
}