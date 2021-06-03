using System;

namespace Nebula.Utils.Collections.Events
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