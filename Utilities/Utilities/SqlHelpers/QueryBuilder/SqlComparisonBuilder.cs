using System.Data;
using System.Text;

namespace Utilities.QueryBuilder
{
    public class SqlComparisonBuilder
    {
        private readonly StringBuilder querySqlComparison;

        public SqlComparisonBuilder(StringBuilder querySqlComparison)
        {
            this.querySqlComparison = querySqlComparison;
        }

        public StringBuilder QuerySqlComparison
        {
            get { return querySqlComparison; }
        }

        public static SqlComparisonBuilder Equals(string toTableName, string toColumnName, string fromTableName,
            string fromColumnName)
        {
            return new SqlComparisonOperator().Equals(toTableName, toColumnName, fromTableName, fromColumnName);
        }

        public static SqlComparisonBuilder Equals(string tableName, string fieldName, object value,
            DbType? dbType = null)
        {
            return new SqlComparisonOperator().Equals(tableName, fieldName, value, dbType);
        }

        public static SqlComparisonBuilder In(string tableName, string fieldName, object[] values, DbType? dbType = null)
        {
            return new SqlComparisonOperator().In(tableName, fieldName, values, dbType);
        }

        public static SqlComparisonBuilder LessOrEquals(string tableName, string fieldName, object value,
            DbType? dbType = null)
        {
            return new SqlComparisonOperator().LessOrEquals(tableName, fieldName, value, dbType);
        }

        public static SqlComparisonBuilder LessThan(string tableName, string fieldName, object value,
            DbType? dbType = null)
        {
            return new SqlComparisonOperator().LessThan(tableName, fieldName, value, dbType);
        }

        public static SqlComparisonBuilder GreaterOrEquals(string tableName, string fieldName, object value,
            DbType? dbType = null)
        {
            return new SqlComparisonOperator().GreaterOrEquals(tableName, fieldName, value, dbType);
        }

        public static SqlComparisonBuilder GreaterThan(string tableName, string fieldName, object value,
            DbType? dbType = null)
        {
            return new SqlComparisonOperator().GreaterThan(tableName, fieldName, value, dbType);
        }

        public static SqlComparisonBuilder NotLike(string tableName, string fieldName, object value)
        {
            return new SqlComparisonOperator().NotLike(tableName, fieldName, value);
        }

        public static SqlComparisonBuilder Like(string tableName, string fieldName, object value)
        {
            return new SqlComparisonOperator().Like(tableName, fieldName, value);
        }

        public static SqlComparisonBuilder NotEquals(string tableName, string fieldName, object value,
            DbType? dbType = null)
        {
            return new SqlComparisonOperator().NotEquals(tableName, fieldName, value, dbType);
        }

        public SqlComparisonOperator And()
        {
            if (QuerySqlComparison != null)
                querySqlComparison.Append(" AND ");
            return new SqlComparisonOperator(querySqlComparison);
        }

        public SqlComparisonOperator Or()
        {
            if (QuerySqlComparison != null)
                querySqlComparison.Append(" OR ");

            return new SqlComparisonOperator(querySqlComparison);
        }
    }
}