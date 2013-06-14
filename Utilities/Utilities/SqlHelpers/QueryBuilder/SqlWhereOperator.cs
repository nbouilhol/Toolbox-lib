using System.Data.SqlClient;
using System.Text;

namespace Utilities.QueryBuilder
{
    public class SqlWhereOperator : ISqlQueryBuilder
    {
        private SqlQueryRequest _sqlQueryRequest;
        private SqlQueryBuilder _sqlQueryBuilder;

        public SqlWhereOperator(SqlQueryRequest sqlQueryRequest)
        {
            _sqlQueryRequest = sqlQueryRequest;
            _sqlQueryBuilder = new SqlQueryBuilder(sqlQueryRequest);
        }

        public SqlWhereBuilder And()
        {
            if (_sqlQueryRequest.Wheres != null && !_sqlQueryRequest.Wheres.TrimEnd().EndsWith("AND"))
                _sqlQueryRequest.Wheres += " AND ";
            return new SqlWhereBuilder(_sqlQueryRequest);
        }

        public SqlWhereBuilder Or()
        {
            if (_sqlQueryRequest.Wheres != null && !_sqlQueryRequest.Wheres.TrimEnd().EndsWith("OR"))
                _sqlQueryRequest.Wheres += " OR ";
            return new SqlWhereBuilder(_sqlQueryRequest);
        }

        #region Implementation of ISqlQueryBuilder

        public override string ToString()
        {
            return _sqlQueryBuilder.ToString();
        }

        public StringBuilder ToStringBuilder()
        {
            return _sqlQueryBuilder.ToStringBuilder();
        }

        public SqlQueryBuilder Distinct()
        {
            return _sqlQueryBuilder.Distinct();
        }

        public SqlQueryBuilder InnerJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            return _sqlQueryBuilder.InnerJoin(toTableName, sqlComaraison);
        }

        public SqlQueryBuilder LeftJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            return _sqlQueryBuilder.LeftJoin(toTableName, sqlComaraison);
        }

        public SqlQueryBuilder OuterJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            return _sqlQueryBuilder.OuterJoin(toTableName, sqlComaraison);
        }

        public SqlQueryBuilder RightJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            return _sqlQueryBuilder.RightJoin(toTableName, sqlComaraison);
        }

        public SqlQueryBuilder Select(string tableName, params string[] newColumnsName)
        {
            return _sqlQueryBuilder.Select(tableName, newColumnsName);
        }

        public SqlQueryBuilder SelectAll()
        {
            return _sqlQueryBuilder.SelectAll();
        }

        public SqlQueryBuilder SelectAll(string nomTable)
        {
            return _sqlQueryBuilder.SelectAll(nomTable);
        }

        public SqlQueryBuilder SelectSqlLiteral(params string[] sqlLiteralColumns)
        {
            return _sqlQueryBuilder.SelectSqlLiteral(sqlLiteralColumns);
        }

        public SqlQueryBuilder Top(int topValue)
        {
            return _sqlQueryBuilder.Top(topValue);
        }

        #endregion Implementation of ISqlQueryBuilder

        public SqlQueryBuilder OrderBy(string tableName, string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _sqlQueryBuilder.OrderBy(tableName, sortColumnsName, sortOrder);
        }

        public SqlQueryBuilder GroupBy(string tableName, params string[] grouppedColumnsName)
        {
            return _sqlQueryBuilder.GroupBy(tableName, grouppedColumnsName);
        }

        public SqlQueryBuilder OrderBy(string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _sqlQueryBuilder.OrderBy(sortColumnsName, sortOrder);
        }

        public SqlQueryBuilder OrderBy(string sortColumnName, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _sqlQueryBuilder.OrderBy(sortColumnName, sortOrder);
        }

        public SqlQueryBuilder Max(string tableName, string columnName, string aliasMax = null)
        {
            return _sqlQueryBuilder.Max(tableName, columnName, aliasMax);
        }

        public SqlQueryBuilder Min(string tableName, string columnName, string aliasMin = null)
        {
            return _sqlQueryBuilder.Min(tableName, columnName, aliasMin);
        }

        public SqlQueryBuilder Sum(string tableName, string columnName, string aliasSum = null)
        {
            return _sqlQueryBuilder.Sum(tableName, columnName, aliasSum);
        }

        public SqlQueryBuilder Avg(string tableName, string columnName, string aliasAvg = null)
        {
            return _sqlQueryBuilder.Avg(tableName, columnName, aliasAvg);
        }

        public SqlQueryBuilder Count(string tableName, string columnName, string aliasCount = null)
        {
            return _sqlQueryBuilder.Count(tableName, columnName, aliasCount);
        }

        public SqlQueryBuilder SelectSubQuery(string subQuery, string aliasSubQuery = null)
        {
            return _sqlQueryBuilder.SelectSubQuery(subQuery, aliasSubQuery);
        }

        public SqlQueryBuilder Count()
        {
            return _sqlQueryBuilder.Count();
        }
    }
}