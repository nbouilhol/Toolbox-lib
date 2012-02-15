using System.Collections.Generic;

namespace Utilities.QueryBuilder
{
    public class SqlQueryRequest
    {
        public bool Distinct { get; set; }
        public int Top { get; set; }
        public string From { get; set; }
        public string FromAlias { get; set; }
        public string FromSubQuery { get; set; }

        public ICollection<string> Joins { get; set; }
        public string Wheres { get; set; }
        public ICollection<string> Columns { get; set; }
        public ICollection<string> SortColumns { get; set; }
        public ICollection<string> GroupByColumns { get; set; }

        public string MaxAlias { get; set; }
        public string MaxColumnName { get; set; }
        public string MaxTableName { get; set; }

        public string MinTableName { get; set; }
        public string MinColumnName { get; set; }
        public string MinAlias { get; set; }

        public string SumAlias { get; set; }
        public string SumColumnName { get; set; }
        public string SumTableName { get; set; }

        public string AvgAlias { get; set; }
        public string AvgColumnName { get; set; }
        public string AvgTableName { get; set; }

        public string CountAlias { get; set; }
        public string CountColumnName { get; set; }
        public string CountTableName { get; set; }
    }
}
