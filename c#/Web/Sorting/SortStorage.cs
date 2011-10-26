using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Mvc.Helper.Grid;
using System.Collections;

namespace Mvc.Helper.Sorting
{
    [Serializable]
    public class SortStorage : ISort
    {
        public string Column { get; private set; }
        public SortDirection Direction { get; private set; }

        public SortStorage(string column, SortDirection? direction)
        {
            Column = column;
            Direction = direction ?? SortDirection.Descending;
        }

        public SortDirection? GetSortDataFor(string column)
        {
            if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(Column) && column == Column)
                return Direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            return null;
        }
    }
}