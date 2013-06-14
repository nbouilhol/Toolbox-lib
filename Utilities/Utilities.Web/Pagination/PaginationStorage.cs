using System;

namespace Mvc.Helper.Pagination
{
    [Serializable]
    public class PaginationStorage : IPagination
    {
        private int totalPages;

        public PaginationStorage(int pageNumber, int pageSize, int totalItems)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        public PaginationStorage(int pageNumber, int pageSize, int totalItems, int totalPages)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int PageSize { get; private set; }

        public int PageNumber { get; private set; }

        public int TotalItems { get; private set; }

        public int TotalPages
        {
            get
            {
                if (totalPages == 0)
                    return (int) Math.Ceiling(((double) TotalItems)/PageSize);
                return totalPages;
            }
            private set { totalPages = value; }
        }

        public int FirstItem
        {
            get { return ((PageNumber - 1)*PageSize) + 1; }
        }

        public int LastItem
        {
            get
            {
                int lastItem = FirstItem + PageSize - 1;
                return lastItem <= TotalItems ? lastItem : TotalItems;
            }
        }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool HasNextPage
        {
            get { return PageNumber < TotalPages; }
        }
    }
}