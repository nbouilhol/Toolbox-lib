using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.Contracts;
using System.Data.SqlClient;
using System.Data;

namespace BouilholLib.Helper
{
    public class Query
    {
        public static IList<T> Execute<T>(string connectionString, string commandText, params object[] parameters)
        {
            Mapper<T> mapper = Mapper<T>.Create();

            return Execute<T>(connectionString, commandText, reader => mapper.MapToList(reader), parameters);
        }

        public static IList<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, T> mapper, params object[] parameters)
        {
            return MoteurCGRP.Helper.SqlHelpers.Query.Execute<T>(connectionString, commandText, reader => Map<T>(reader, mapper), parameters);
        }

        public static T ExecuteOne<T>(string connectionString, string commandText, params object[] parameters)
        {
            Mapper<T> mapper = Mapper<T>.Create();

            return ExecuteOneRow<T>(connectionString, commandText, reader => mapper.MapOne(reader), parameters);
        }

        public static IList<T> ExecuteAndFlatten<T>(string connectionString, string commandText, Func<SqlDataReader, IList<T>> mapper, params object[] parameters)
        {
            return MoteurCGRP.Helper.SqlHelpers.Query.Execute<T>(connectionString, commandText, reader => MapForFlatten<T>(reader, mapper).SelectMany(result => result), parameters);
        }

        public static IList<T> FindAll<T>(string connectionString, string table)
        {
            var query = FindAllQuery(table);
            return Execute<T>(connectionString, query);
        }

        public static T FindOne<T>(string connectionString, string table)
        {
            var query = FindAllQuery(table);
            return ExecuteOne<T>(connectionString, query);
        }

        public static IList<T> FindAllBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            var query = FindAllByQuery(table, properties.Keys);
            return Execute<T>(connectionString, query, properties.Values.ToArray());
        }

        public static T FindOneBy<T>(string connectionString, string table, IDictionary<string, object> properties)
        {
            var query = FindAllByQuery(table, properties.Keys);
            return ExecuteOne<T>(connectionString, query, properties.Values.ToArray());
        }

        public static IList<dynamic> ExecuteDynamic(string connectionString, string commandText, params object[] parameters)
        {
            return Execute<dynamic>(connectionString, commandText, reader => reader.ToDynamic(), parameters);
        }

        public static dynamic ExecuteOneDynamic(string connectionString, string commandText, params object[] parameters)
        {
            return ExecuteOneRow<dynamic>(connectionString, commandText, reader => reader.ToOneDynamic(), parameters);
        }

        public static IList<dynamic> FindAllDynamic(string connectionString, string table)
        {
            var query = FindAllQuery(table);
            return ExecuteDynamic(connectionString, query);
        }

        public static dynamic FindOneDynamic(string connectionString, string table)
        {
            var query = FindAllQuery(table);
            return ExecuteDynamic(connectionString, query);
        }

        public static IList<dynamic> FindAllByDynamic(string connectionString, string table, IDictionary<string, object> properties)
        {
            var query = FindAllByQuery(table, properties.Keys);
            return ExecuteDynamic(connectionString, query, properties.Values.ToArray());
        }

        public static dynamic FindOneByDynamic(string connectionString, string table, IDictionary<string, object> properties)
        {
            var query = FindAllByQuery(table, properties.Keys);
            return ExecuteOneDynamic(connectionString, query, properties.Values.ToArray());
        }

        public static IList<T> Execute<T>(string connectionString, string commandText, Func<SqlDataReader, IEnumerable<T>> funcMapper, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(commandText, sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
                        sqlCommande.AddParams(parameters);

                    if (sqlConnexion.State == ConnectionState.Closed)
                        sqlConnexion.Open();

                    using (SqlDataReader reader = sqlCommande.ExecuteReader(CommandBehavior.CloseConnection))
                        return funcMapper(reader).ToList();
                }
            }
            catch (Exception e)
            {
                Guard.LogAndThrow(e);
            }

            return null;
        }

        public static T ExecuteOneRow<T>(string connectionString, string commandText, Func<SqlDataReader, T> funcMapper, params object[] parameters)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            try
            {
                using (SqlConnection sqlConnexion = new SqlConnection(connectionString))
                using (SqlCommand sqlCommande = new SqlCommand(commandText, sqlConnexion))
                {
                    sqlCommande.CommandType = CommandType.Text;
                    sqlCommande.CommandTimeout = 500;
                    if (parameters != null && parameters.Length > 0)
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
                Guard.LogAndThrow(e);
            }

            return default(T);
        }

        private static string FindAllQuery(string table)
        {
            return string.Format("SELECT * FROM {0}", table);
        }

        private static string FindAllByQuery(string table, IEnumerable<string> parameters)
        {
            string where = string.Join(" AND ", parameters.Select((param, i) => string.Format("{0}=@{1}", param, i)));
            return string.Format("SELECT * FROM {0} WHERE {1}", table, where);
        }

        private static IEnumerable<IEnumerable<T>> MapForFlatten<T>(SqlDataReader reader, Func<SqlDataReader, IList<T>> mapper)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    IList<T> result = mapper(reader);
                    if (result != null)
                        yield return result;
                }
                reader.NextResult();
            }
        }

        private static IEnumerable<T> Map<T>(SqlDataReader reader, Func<SqlDataReader, T> mapper)
        {
            while (reader.HasRows)
            {
                while (reader.Read())
                {
                    T result = mapper(reader);
                    if (result != null)
                        yield return result;
                }
                reader.NextResult();
            }
        }
    }
}
