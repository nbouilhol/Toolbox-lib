using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace Utilities.QueryBuilder
{
    public class SqlQueryBuilder : ISqlWhereBuilder, ISqlQueryBuilder
    {
        private SqlJoinBuilder _sqlJoinBuilder;
        private SqlQueryRequest _sqlQueryRequest;
        private SqlSelectBuilder _sqlSelectBuilder;
        private SqlWhereBuilder _sqlWhereBuilder;
        private StringBuilder query;

        public SqlQueryBuilder(SqlQueryRequest sqlQueryRequest)
        {
            this._sqlQueryRequest = sqlQueryRequest;
            this._sqlSelectBuilder = new SqlSelectBuilder(sqlQueryRequest);
            this._sqlJoinBuilder = new SqlJoinBuilder(_sqlQueryRequest);
            this._sqlWhereBuilder = new SqlWhereBuilder(_sqlQueryRequest);
        }

        public override string ToString()
        {
            return BuildSelectQuery().ToString();
        }

        public StringBuilder ToStringBuilder()
        {
            return BuildSelectQuery();
        }

        #region SqlSelectBuilder

        public SqlQueryBuilder Distinct()
        {
            _sqlSelectBuilder.Distinct();
            return this;
        }

        public SqlQueryBuilder Top(int topValue)
        {
            _sqlSelectBuilder.Top(topValue);
            return this;
        }

        public SqlQueryBuilder Max(string tableName, string columnName, string aliasMax = null)
        {
            _sqlSelectBuilder.Max(tableName, columnName, aliasMax);
            return this;
        }

        public SqlQueryBuilder Min(string tableName, string columnName, string aliasMin = null)
        {
            _sqlSelectBuilder.Min(tableName, columnName, aliasMin);
            return this;
        }

        public SqlQueryBuilder Select(string tableName, params string[] newColumnsName)
        {
            _sqlSelectBuilder.Select(tableName, newColumnsName);
            return this;
        }

        public SqlQueryBuilder SelectColumns(params string[] newColumnsName)
        {
            _sqlSelectBuilder.Select(null, newColumnsName);
            return this;
        }

        public SqlQueryBuilder SelectSqlLiteral(params string[] sqlLiteralColumns)
        {
            _sqlSelectBuilder.SelectSqlLiteral(sqlLiteralColumns);
            return this;
        }

        public SqlQueryBuilder SelectAll(string nomTable)
        {
            _sqlSelectBuilder.SelectAll(nomTable);
            return this;
        }

        public SqlQueryBuilder SelectAll()
        {
            _sqlSelectBuilder.SelectAll();
            return this;
        }

        #endregion SqlSelectBuilder

        #region SqlJoinBuilder

        public SqlQueryBuilder InnerJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            _sqlJoinBuilder.InnerJoin(toTableName, sqlComaraison);
            return this;
        }

        public SqlQueryBuilder LeftJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            _sqlJoinBuilder.LeftJoin(toTableName, sqlComaraison);
            return this;
        }

        public SqlQueryBuilder RightJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            _sqlJoinBuilder.RightJoin(toTableName, sqlComaraison);
            return this;
        }

        public SqlQueryBuilder OuterJoin(string toTableName, SqlComparisonBuilder sqlComaraison)
        {
            _sqlJoinBuilder.OuterJoin(toTableName, sqlComaraison);
            return this;
        }

        #endregion SqlJoinBuilder

        #region Implementation of ISqlWhereBuilder

        public SqlWhereOperator Where(SqlComparisonBuilder sqlComparison)
        {
            return _sqlWhereBuilder.Where(sqlComparison);
        }

        public SqlWhereOperator WhereSqlLiteral(string sqlComparisonSqlLiteral)
        {
            return _sqlWhereBuilder.WhereSqlLiteral(sqlComparisonSqlLiteral);
        }

        #endregion Implementation of ISqlWhereBuilder

        #region OrderBy

        public SqlQueryBuilder OrderBy(string tableName, string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending)
        {
            _sqlSelectBuilder.OrderBy(tableName, sortColumnsName, sortOrder);
            return this;
        }

        public SqlQueryBuilder OrderBy(string[] sortColumnsName, SortOrder sortOrder = SortOrder.Ascending)
        {
            _sqlSelectBuilder.OrderBy(null, sortColumnsName, sortOrder);
            return this;
        }

        public SqlQueryBuilder OrderBy(string sortColumnName, SortOrder sortOrder = SortOrder.Ascending)
        {
            _sqlSelectBuilder.OrderBy(null, new string[] { sortColumnName }, sortOrder);
            return this;
        }

        #endregion OrderBy

        public SqlQueryBuilder GroupBy(string tableName, params string[] grouppedColumnsName)
        {
            _sqlSelectBuilder.GroupBy(tableName, grouppedColumnsName);
            return this;
        }

        #region Private Methods

        /// <summary>
        /// fabrique requete SELECT
        /// </summary>
        /// <returns></returns>
        private StringBuilder BuildSelectQuery()
        {
            if (string.IsNullOrEmpty(_sqlQueryRequest.From)) return null;
            query = new StringBuilder(" SELECT ");
            BuildDistinct();
            BuildTop();
            BuildMax();
            BuildMin();
            BuildSum();
            BuildAvg();
            BuildCount();
            BuildSelectedColumns();
            BuildFrom();
            BuildJoin();
            BuildWhere();
            BuildGroupBy();
            BuildOrderBy();
            return query;
        }

        private void BuildWhere()
        {
            if (_sqlQueryRequest.Wheres != null)
            {
                if (_sqlQueryRequest.Wheres.TrimEnd().EndsWith("AND"))
                    _sqlQueryRequest.Wheres = _sqlQueryRequest.Wheres.Substring(0, _sqlQueryRequest.Wheres.LastIndexOf("AND"));
                if (_sqlQueryRequest.Wheres.TrimEnd().EndsWith("OR"))
                    _sqlQueryRequest.Wheres = _sqlQueryRequest.Wheres.Substring(0, _sqlQueryRequest.Wheres.LastIndexOf("OR"));
            }

            query.AppendLine(_sqlQueryRequest.Wheres);
        }

        private void BuildJoin()
        {
            if (_sqlQueryRequest.Joins != null && _sqlQueryRequest.Joins.Count > 0)
            {
                query.AppendLine(string.Join("", _sqlQueryRequest.Joins.ToArray()));
            }
        }

        private void BuildFrom()
        {
            string fromQuery = null;
            if (string.IsNullOrEmpty(_sqlQueryRequest.FromSubQuery) && string.IsNullOrEmpty(_sqlQueryRequest.From)) return;

            if (string.IsNullOrEmpty(_sqlQueryRequest.FromSubQuery))
                fromQuery = _sqlQueryRequest.From;
            else
                fromQuery = "(" + _sqlQueryRequest.FromSubQuery + ")";

            if (!string.IsNullOrEmpty(_sqlQueryRequest.FromAlias))
                fromQuery += " as " + _sqlQueryRequest.FromAlias;

            query.AppendLine(" FROM " + fromQuery);
        }

        private void BuildTop()
        {
            if (_sqlQueryRequest.Top > 0) query.Append(string.Format(" TOP {0} ", _sqlQueryRequest.Top));
        }

        private void BuildMax()
        {
            if (string.IsNullOrEmpty(_sqlQueryRequest.MaxColumnName)) return;
            string queryMax = string.IsNullOrEmpty(_sqlQueryRequest.MaxTableName)
                ? string.Format(" MAX({0}) ", _sqlQueryRequest.MaxColumnName)
                : string.Format(" MAX({0}.{1}) ", _sqlQueryRequest.MaxTableName, _sqlQueryRequest.MaxColumnName);
            if (!string.IsNullOrEmpty(_sqlQueryRequest.MaxAlias)) queryMax += string.Format(" AS {0} ", _sqlQueryRequest.MaxAlias);
            query.Append(queryMax);
        }

        private void BuildMin()
        {
            if (string.IsNullOrEmpty(_sqlQueryRequest.MinColumnName)) return;
            string queryMin = string.IsNullOrEmpty(_sqlQueryRequest.MinTableName)
                ? string.Format(" MIN({0}) ", _sqlQueryRequest.MinColumnName)
                : string.Format(" MIN({0}.{1}) ", _sqlQueryRequest.MinTableName, _sqlQueryRequest.MinColumnName);
            if (!string.IsNullOrEmpty(_sqlQueryRequest.MinAlias)) queryMin += string.Format(" AS {0} ", _sqlQueryRequest.MinAlias);
            query.Append(queryMin);
        }

        private void BuildSum()
        {
            if (string.IsNullOrEmpty(_sqlQueryRequest.SumColumnName)) return;
            string querySum = string.IsNullOrEmpty(_sqlQueryRequest.SumTableName)
                ? string.Format(" Sum({0}) ", _sqlQueryRequest.SumColumnName)
                : string.Format(" Sum({0}.{1}) ", _sqlQueryRequest.SumTableName, _sqlQueryRequest.SumColumnName);
            if (!string.IsNullOrEmpty(_sqlQueryRequest.SumAlias)) querySum += string.Format(" AS {0} ", _sqlQueryRequest.SumAlias);
            query.Append(querySum);
        }

        private void BuildAvg()
        {
            if (string.IsNullOrEmpty(_sqlQueryRequest.AvgColumnName)) return;
            string queryAvg = string.IsNullOrEmpty(_sqlQueryRequest.AvgTableName)
                ? string.Format(" Avg({0}) ", _sqlQueryRequest.AvgColumnName)
                : string.Format(" Avg({0}.{1}) ", _sqlQueryRequest.AvgTableName, _sqlQueryRequest.AvgColumnName);
            if (!string.IsNullOrEmpty(_sqlQueryRequest.AvgAlias)) queryAvg += string.Format(" AS {0} ", _sqlQueryRequest.AvgAlias);
            query.Append(queryAvg);
        }

        private void BuildCount()
        {
            if (string.IsNullOrEmpty(_sqlQueryRequest.CountColumnName)) return;
            string queryCount = string.IsNullOrEmpty(_sqlQueryRequest.CountTableName)
                ? string.Format(" Count({0}) ", _sqlQueryRequest.CountColumnName)
                : string.Format(" Count({0}.{1}) ", _sqlQueryRequest.CountTableName, _sqlQueryRequest.CountColumnName);
            if (!string.IsNullOrEmpty(_sqlQueryRequest.CountAlias)) queryCount += string.Format(" AS {0} ", _sqlQueryRequest.CountAlias);
            query.Append(queryCount);
        }

        private void BuildDistinct()
        {
            if (_sqlQueryRequest.Distinct) query.Append(" DISTINCT ");
        }

        private void BuildSelectedColumns()
        {
            if ((_sqlQueryRequest.Columns == null || _sqlQueryRequest.Columns.Count == 0)
                    && (string.IsNullOrEmpty(_sqlQueryRequest.MaxColumnName) && string.IsNullOrEmpty(_sqlQueryRequest.MinColumnName)
                    && string.IsNullOrEmpty(_sqlQueryRequest.SumColumnName) && string.IsNullOrEmpty(_sqlQueryRequest.CountColumnName))
                )
                query.Append(" * ");
            if (_sqlQueryRequest.Columns != null && _sqlQueryRequest.Columns.Count > 0)
                query.Append(string.Join(", ", _sqlQueryRequest.Columns.ToArray()));
        }

        private void BuildGroupBy()
        {
            if (_sqlQueryRequest.GroupByColumns != null && _sqlQueryRequest.GroupByColumns.Count > 0)
            {
                query.AppendLine(" GROUP BY ");
                query.Append(string.Join(", ", _sqlQueryRequest.GroupByColumns.ToArray()));
            }
        }

        private void BuildOrderBy()
        {
            if (_sqlQueryRequest.SortColumns != null && _sqlQueryRequest.SortColumns.Count > 0)
            {
                query.AppendLine(" ORDER BY ");
                query.Append(string.Join(", ", _sqlQueryRequest.SortColumns.ToArray()));
            }
        }

        #endregion Private Methods

        public SqlQueryBuilder Sum(string tableName, string columnName, string aliasSum = null)
        {
            _sqlSelectBuilder.Sum(tableName, columnName, aliasSum);
            return this;
        }

        public SqlQueryBuilder Avg(string tableName, string columnName, string aliasAvg = null)
        {
            _sqlSelectBuilder.Avg(tableName, columnName, aliasAvg);
            return this;
        }

        public SqlQueryBuilder Count(string tableName, string columnName, string aliasCount = null)
        {
            _sqlSelectBuilder.Count(tableName, columnName, aliasCount);
            return this;
        }

        public SqlQueryBuilder Count()
        {
            _sqlSelectBuilder.Count();
            return this;
        }

        public SqlQueryBuilder SelectSubQuery(string subQuery, string aliasSubQuery = null)
        {
            _sqlSelectBuilder.SelectSubQuery(subQuery, aliasSubQuery);
            return this;
        }
    }
}