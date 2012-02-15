using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utilities.SqlHelpers
{
    public static class QueryRequest
    {
        [Pure]
        public static string FindAllQuery(string table)
        {
            Contract.Requires(table != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format("SELECT * FROM {0}", table) ?? string.Empty;
        }

        [Pure]
        public static string FindAllWhere(string table, IEnumerable<string> parameters)
        {
            Contract.Requires(table != null);
            Contract.Requires(parameters != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string where = string.Join(" AND ", parameters.Select((param, i) => string.Format("{0}=@{1}", param, i)));
            return string.Format("SELECT * FROM {0} WHERE {1}", table, where) ?? string.Empty;
        }

        [Pure]
        public static string FindAllWhere(string table, string where)
        {
            Contract.Requires(table != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format("SELECT * FROM {0} WHERE {1}", table, where) ?? string.Empty;
        }

        [Pure]
        public static string ApplyPagination(string query, string idColumnName, int from, int to)
        {
            Contract.Requires(query != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format("SELECT * FROM ({0}) AS WP WHERE Row BETWEEN {1} AND {2}",
                query.Replace("SELECT", string.Format("SELECT ROW_NUMBER() OVER (ORDER BY {0}) AS Row", idColumnName)),
                from,
                to) ?? string.Empty;
        }

        [Pure]
        public static string Truncate(string table)
        {
            Contract.Requires(table != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format("TRUNCATE TABLE {0}", table);
        }
    }
}
