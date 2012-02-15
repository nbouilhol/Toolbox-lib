using System.Data.SqlClient;
using System.Text;
namespace Utilities.QueryBuilder
{
    public interface ISqlQueryBuilder
    {
        StringBuilder ToStringBuilder();
        SqlQueryBuilder Distinct();
        SqlQueryBuilder InnerJoin(string toTableName, SqlComparisonBuilder sqlComaraison);
        SqlQueryBuilder LeftJoin(string toTableName, SqlComparisonBuilder sqlComaraison);
        SqlQueryBuilder OuterJoin(string toTableName, SqlComparisonBuilder sqlComaraison);
        SqlQueryBuilder RightJoin(string toTableName, SqlComparisonBuilder sqlComaraison);
        SqlQueryBuilder Select(string tableName, params string[] newColumnsName);
        SqlQueryBuilder SelectAll();
        SqlQueryBuilder SelectAll(string nomTable);
        SqlQueryBuilder SelectSqlLiteral(params string[] sqlLiteralColumns);
        SqlQueryBuilder Top(int topValue);

        SqlQueryBuilder Max(string tableName, string columnName, string aliasMax = null);
        SqlQueryBuilder Min(string tableName, string columnName, string aliasMin = null);        
        SqlQueryBuilder Sum(string tableName, string columnName, string aliasSum = null);
        SqlQueryBuilder Avg(string tableName, string columnName, string aliasAvg = null);
        SqlQueryBuilder Count(string tableName, string columnName, string aliasCount = null);
        SqlQueryBuilder Count();
        
        SqlQueryBuilder GroupBy(string tableName, params string[] grouppedColumnsName);       

        SqlQueryBuilder OrderBy(string tableName, string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending);
        SqlQueryBuilder OrderBy(string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending);
        SqlQueryBuilder OrderBy(string sortColumnName, SortOrder sortOrder = SortOrder.Ascending);
        
        SqlQueryBuilder SelectSubQuery(string subQuery, string aliasSubQuery = null);
    }
}
