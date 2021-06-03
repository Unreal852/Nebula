using System;

namespace Nebula.Utils.Collections.Paging.Events
{
    public class TotalPagesChangedEventArgs : EventArgs
    {
        public TotalPagesChangedEventArgs(int currentPage, int totalPages)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; }
        public int TotalPages  { get; }
    }
}