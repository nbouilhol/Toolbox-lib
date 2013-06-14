namespace Utilities.QueryBuilder
{
    public static class SqlSelectQueryBuilder
    {
        public static SqlQueryBuilder From(string fromTableName, string aliasNameFrom = null)
        {
            return new SqlQueryBuilder(new SqlQueryRequest() { From = fromTableName, FromAlias = aliasNameFrom });
        }

        public static SqlQueryBuilder SubQueryFrom(string subQueryFrom, string aliasNameFrom)
        {
            return new SqlQueryBuilder(new SqlQueryRequest() { FromAlias = aliasNameFrom, FromSubQuery = subQueryFrom });
        }
    }
}