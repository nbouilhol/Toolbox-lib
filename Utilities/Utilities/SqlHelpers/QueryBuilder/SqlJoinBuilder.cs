using System.Collections.Generic;

namespace Utilities.QueryBuilder
{
    public class SqlJoinBuilder
    {
        private SqlQueryRequest _sqlQueryRequest;

        public SqlJoinBuilder(SqlQueryRequest sqlQueryRequest)
        {
            _sqlQueryRequest = sqlQueryRequest;
        }

        private void JoinFactory(string jointType, string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            if (_sqlQueryRequest.Joins == null) _sqlQueryRequest.Joins = new List<string>();
            _sqlQueryRequest.Joins.Add(jointType + toTableName + " ON " + sqlComaraison.QuerySqlComparison);
        }

        public void InnerJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            JoinFactory(" INNER JOIN ", toTableName, sqlComaraison);
        }

        public void LeftJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            JoinFactory(" LEFT JOIN ", toTableName, sqlComaraison);
        }

        public void RightJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            JoinFactory(" RIGHT JOIN ", toTableName, sqlComaraison);
        }

        public void OuterJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            JoinFactory(" OUTER JOIN ", toTableName, sqlComaraison);
        }
    }
}