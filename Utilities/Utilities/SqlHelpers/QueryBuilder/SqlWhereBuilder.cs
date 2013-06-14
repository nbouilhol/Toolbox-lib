namespace Utilities.QueryBuilder
{
    public class SqlWhereBuilder
    {
        private SqlQueryRequest _sqlQueryRequest;

        public SqlWhereBuilder(SqlQueryRequest _sqlQueryRequest)
        {
            this._sqlQueryRequest = _sqlQueryRequest;
        }

        public virtual SqlWhereOperator Where(SqlComparisonBuilder sqlComparison)
        {
            if (sqlComparison == null || sqlComparison.QuerySqlComparison == null || sqlComparison.QuerySqlComparison.Length == 0)
                return new SqlWhereOperator(_sqlQueryRequest);
            if (string.IsNullOrEmpty(_sqlQueryRequest.Wheres)) _sqlQueryRequest.Wheres = " WHERE ";
            _sqlQueryRequest.Wheres += " (" + sqlComparison.QuerySqlComparison + ")";
            return new SqlWhereOperator(_sqlQueryRequest);
        }

        public virtual SqlWhereOperator WhereSqlLiteral(string sqlComparisonSqlLiteral)
        {
            if (string.IsNullOrEmpty(sqlComparisonSqlLiteral))
                return new SqlWhereOperator(_sqlQueryRequest);
            if (string.IsNullOrEmpty(_sqlQueryRequest.Wheres)) _sqlQueryRequest.Wheres = " WHERE ";
            _sqlQueryRequest.Wheres += " (" + sqlComparisonSqlLiteral + ")";
            return new SqlWhereOperator(_sqlQueryRequest);
        }
    }
}