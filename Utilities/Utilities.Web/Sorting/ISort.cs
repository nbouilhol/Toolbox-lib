namespace Mvc.Helper.Sorting
{
    public interface ISort
    {
        string Column { get; }

        SortDirection Direction { get; }

        SortDirection? GetSortDataFor(string column);
    }
}