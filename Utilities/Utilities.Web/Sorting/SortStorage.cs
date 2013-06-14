using System;

namespace Mvc.Helper.Sorting
{
    [Serializable]
    public class SortStorage : ISort
    {
        public SortStorage(string column, SortDirection? direction)
        {
            Column = column;
            Direction = direction ?? SortDirection.Descending;
        }

        public string Column { get; private set; }

        public SortDirection Direction { get; private set; }

        public SortDirection? GetSortDataFor(string column)
        {
            if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(Column) && column == Column)
                return Direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            return null;
        }
    }
}