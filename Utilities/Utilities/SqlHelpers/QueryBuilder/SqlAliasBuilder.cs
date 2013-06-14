namespace Utilities.QueryBuilder
{
    public static class SqlAliasBuilder
    {
        public static string Alias(string columnName, string aliasName = null)
        {
            return columnName + " as " + aliasName;
        }
    }
}