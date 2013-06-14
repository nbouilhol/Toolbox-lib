namespace Mvc.Helper.Pagination
{
    public interface IPagination
    {
        int PageNumber { get; }

        int PageSize { get; }

        int TotalItems { get; }

        int TotalPages { get; }

        int FirstItem { get; }

        int LastItem { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }
    }
}