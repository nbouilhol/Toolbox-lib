using System.Collections.Generic;
using System.Data.SqlClient;

namespace Utilities.QueryBuilder
{
    public class SqlSelectBuilder
    {
        private readonly SqlQueryRequest _sqlQueryRequest;

        public SqlSelectBuilder(SqlQueryRequest sqlQueryRequest)
        {
            _sqlQueryRequest = sqlQueryRequest;
        }

        public void Distinct()
        {
            _sqlQueryRequest.Distinct = true;
        }

        public void Top(int topValue)
        {
            _sqlQueryRequest.Top = topValue;
        }

        public void Max(string tableName, string columnName, string aliasMax = null)
        {
            _sqlQueryRequest.MaxTableName = tableName;
            _sqlQueryRequest.MaxColumnName = columnName;
            _sqlQueryRequest.MaxAlias = aliasMax;
        }

        public void Min(string tableName, string columnName, string aliasMin = null)
        {
            _sqlQueryRequest.MinTableName = tableName;
            _sqlQueryRequest.MinColumnName = columnName;
            _sqlQueryRequest.MinAlias = aliasMin;
        }

        public void Sum(string tableName, string columnName, string aliasSum = null)
        {
            _sqlQueryRequest.SumTableName = tableName;
            _sqlQueryRequest.SumColumnName = columnName;
            _sqlQueryRequest.SumAlias = aliasSum;
        }

        public void Avg(string tableName, string columnName, string aliasAvg = null)
        {
            _sqlQueryRequest.AvgTableName = tableName;
            _sqlQueryRequest.AvgColumnName = columnName;
            _sqlQueryRequest.AvgAlias = aliasAvg;
        }

        public void Count(string tableName, string columnName, string aliasCount = null)
        {
            _sqlQueryRequest.CountTableName = tableName;
            _sqlQueryRequest.CountColumnName = columnName;
            _sqlQueryRequest.CountAlias = aliasCount;
        }

        public void Count()
        {
            _sqlQueryRequest.CountTableName = null;
            _sqlQueryRequest.CountColumnName = "*";
            _sqlQueryRequest.CountAlias = null;
        }

        public void Select(string tableName, params string[] newColumnsName)
        {
            if (_sqlQueryRequest.Columns == null) _sqlQueryRequest.Columns = new List<string>();

            if (string.IsNullOrEmpty(tableName))
            {
                _sqlQueryRequest.Columns.AddRange(newColumnsName);
                return;
            }

            if (newColumnsName == null || newColumnsName.Length == 0)
            {
                _sqlQueryRequest.Columns.Add(tableName + ".*");
                return;
            }
            foreach (string columnName in newColumnsName)
                _sqlQueryRequest.Columns.Add(tableName + "." + columnName);
        }

        public void SelectSqlLiteral(params string[] sqlLiteralColumns)
        {
            if (_sqlQueryRequest.Columns == null) _sqlQueryRequest.Columns = new List<string>();
            _sqlQueryRequest.Columns.AddRange(sqlLiteralColumns);
        }

        public void SelectAll(string nomTable)
        {
            Select(nomTable);
        }

        public void SelectAll()
        {
            if (_sqlQueryRequest.Columns != null) _sqlQueryRequest.Columns.Clear();
        }

        public void OrderBy(string tableName, string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending)
        {
            if (sortColumnsName == null || sortColumnsName.Length == 0)
                return;

            string sortOrderSql = ConvertSortOrderToSql(sortOrder);

            if (_sqlQueryRequest.SortColumns == null)
                _sqlQueryRequest.SortColumns = new List<string>();

            foreach (string columnName in sortColumnsName)
            {
                if (!string.IsNullOrEmpty(tableName))
                    _sqlQueryRequest.SortColumns.Add(tableName + "." + columnName + sortOrderSql);
                else
                    _sqlQueryRequest.SortColumns.Add(columnName + sortOrderSql);
            }
        }

        public void GroupBy(string tableName, params string[] grouppedColumnsName)
        {
            if (grouppedColumnsName == null || grouppedColumnsName.Length == 0)
                return;

            if (_sqlQueryRequest.GroupByColumns == null)
                _sqlQueryRequest.GroupByColumns = new List<string>();

            if (!string.IsNullOrEmpty(tableName))
            {
                foreach (string columnName in grouppedColumnsName)
                    _sqlQueryRequest.GroupByColumns.Add(tableName + "." + columnName);
            }
            else
                _sqlQueryRequest.GroupByColumns.AddRange(grouppedColumnsName);
        }

        public void SelectSubQuery(string subQuery, string aliasSubQuery)
        {
            if (string.IsNullOrEmpty(subQuery)) return;
            subQuery = "(" + subQuery + ")";
            if (!string.IsNullOrEmpty(aliasSubQuery))
                subQuery += " as " + aliasSubQuery;
            SelectSqlLiteral(subQuery);
        }

        #region Private methods

        private static string ConvertSortOrderToSql(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return " ASC ";
                case SortOrder.Descending:
                    return " DESC ";
                default:
                    return "";
            }
        }

        #endregion Private methods
    }
}