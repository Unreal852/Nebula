using System.ComponentModel;

namespace Nebula.Utils.Collections.Paging.Events
{
    public class PageChangedEventArgs : CancelEventArgs
    {
        public PageChangedEventArgs(int newPage, int totalPages, int pageSize)
        {
            NewPage = newPage;
            TotalPages = totalPages;
            PageSize = pageSize;
        }

        public int NewPage    { get; }
        public int TotalPages { get; }
        public int PageSize   { get; }
    }
}