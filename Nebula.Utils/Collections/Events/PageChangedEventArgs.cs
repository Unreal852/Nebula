using System.ComponentModel;

namespace Nebula.Utils.Collections.Events
{
    public class PageChangedEventArgs : CancelEventArgs
    {
        public PageChangedEventArgs(int newPage, int totalPages, int maxElementsPerPage)
        {
            NewPage = newPage;
            TotalPages = totalPages;
            MaxElementsPerPage = maxElementsPerPage;
        }

        public int NewPage            { get; }
        public int TotalPages         { get; }
        public int MaxElementsPerPage { get; }
    }
}