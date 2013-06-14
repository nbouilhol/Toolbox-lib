namespace Utilities.QueryBuilder
{
    public interface ISqlWhereBuilder
    {
        SqlWhereOperator Where(SqlComparisonBuilder sqlComparison);

        SqlWhereOperator WhereSqlLiteral(string sqlComparisonSqlLiteral);
    }
}