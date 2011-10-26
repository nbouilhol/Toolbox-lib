using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace Mvc.Helper.Sorting
{
    public interface ISort
    {
        string Column { get; }
        SortDirection Direction { get; }
        SortDirection? GetSortDataFor(string column);
    }
}