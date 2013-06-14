using System;
using System.Data;
using System.Text;

namespace Utilities.QueryBuilder
{
    public class SqlComparisonOperator
    {
        public SqlComparisonOperator()
        {
            querySqlComparison = new StringBuilder();
        }

        public SqlComparisonOperator(StringBuilder querySqlComparison)
        {
            this.querySqlComparison = querySqlComparison;
        }

        public SqlComparisonBuilder Equals(string toTableName, string toColumnName, string fromTableName, string fromColumnName)
        {
            querySqlComparison.Append(FormatColumnTableName(toTableName, toColumnName) + " = " + FormatColumnTableName(fromTableName, fromColumnName));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder Equals(string tableName, string fieldName, object value, DbType? dbType = null)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} = {2} ", tableName, fieldName, FormatSqlValue(value, dbType));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder In(string tableName, string fieldName, object[] values, DbType? dbType = null)
        {
            if (values == null || values.Length == 0) return new SqlComparisonBuilder(querySqlComparison);

            string valuesWithComma = FormatSqlValues(values, dbType);
            querySqlComparison.Append(tableName + "." + fieldName + " IN (" + valuesWithComma + ")");
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder NotEquals(string tableName, string fieldName, object value, DbType? dbType = null)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} <> {2} ", tableName, fieldName, FormatSqlValue(value, dbType));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder Like(string tableName, string fieldName, object value)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} LIKE '{2}' ", tableName, fieldName, value);
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder NotLike(string tableName, string fieldName, object value)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" NOT {0}.{1} LIKE '{2}' ", tableName, fieldName, value);
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder GreaterThan(string tableName, string fieldName, object value, DbType? dbType = null)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} > {2} ", tableName, fieldName, FormatSqlValue(value, dbType));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder GreaterOrEquals(string tableName, string fieldName, object value, DbType? dbType = null)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} >= {2} ", tableName, fieldName, FormatSqlValue(value, dbType));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder LessThan(string tableName, string fieldName, object value, DbType? dbType = null)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} < {2} ", tableName, fieldName, FormatSqlValue(value, dbType));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        public SqlComparisonBuilder LessOrEquals(string tableName, string fieldName, object value, DbType? dbType = null)
        {
            if (value != null)
                querySqlComparison.AppendFormat(" {0}.{1} <= {2}", tableName, fieldName, FormatSqlValue(value, dbType));
            return new SqlComparisonBuilder(querySqlComparison);
        }

        private static string FormatSqlValues(object[] values, DbType? dbType = null)
        {
            string valuesWithComma = null;
            foreach (var value in values)
            {
                if (!string.IsNullOrEmpty(valuesWithComma)) valuesWithComma += ",";
                valuesWithComma += FormatSqlValue(value, dbType);
            }
            return valuesWithComma;
        }

        private static object FormatSqlValue(object value, DbType? dbType)
        {
            if (value == null)
                return "NULL";
            if (dbType == DbType.Boolean)
                return (bool)value ? "1" : "0";
            if (dbType == DbType.DateTime)
                return String.Format("'{0:yyyy/MM/dd hh:mm:ss}'", (DateTime)value);
            if (dbType == DbType.String)
                return String.Format("'{0}'", ((string)value).Replace("'", "''"));
            return value;
        }

        private static string FormatColumnTableName(string tableName, string columnName)
        {
            if (string.IsNullOrEmpty(tableName)) return columnName;
            return tableName + "." + columnName;
        }

        private readonly StringBuilder querySqlComparison;
    }
}