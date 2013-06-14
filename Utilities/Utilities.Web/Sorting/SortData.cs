using System;

namespace Mvc.Helper.Sorting
{
    [Serializable]
    public class SortData
    {
        public string Column { get; set; }

        public SortDirection? Direction { get; set; }

        public SortData()
        { }

        public SortData(string column)
        {
            Column = column;
            Direction = SortDirection.Descending;
        }

        public SortData(string column, SortDirection? direction)
        {
            Column = column;
            Direction = direction;
        }
    }
}