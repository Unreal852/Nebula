using System;
using System.Collections;
using System.Collections.Generic;
using Nebula.Utils.Collections.Paging.Events;

namespace Nebula.Utils.Collections.Paging
{
    public class Pager<T> : IEnumerable<T>
    {
        public static Pager<T> CreateFor(IList<T> targetCollection, int currentPage = 1, int pageSize = 20) => new(targetCollection, currentPage, pageSize);

        private IList<T> _source;
        private int      _pageSize;
        private int      _currentPage;
        private int      _totalPages;

        public Pager(IList<T> source, int currentPage = 1, int pageSize = 20)
        {
            _source = source;
            _currentPage = currentPage;
            _pageSize = pageSize;
            Update();
        }

        public IList<T> Source
        {
            get => _source;
            private set
            {
                _source = value;
                Update();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                Update();
                RaisePageChanged();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set
            {
                _totalPages = value;
                RaiseTotalPagesChanged();
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                Update();
            }
        }

        public IEnumerable<int> Pages
        {
            get
            {
                for (int i = 1; i <= TotalPages; i++)
                    yield return i;
            }
        }

        public IEnumerable<T> Elements => GetPageElements(CurrentPage);

        public event EventHandler<PageChangedEventArgs>       PageChanged;
        public event EventHandler<TotalPagesChangedEventArgs> TotalPagesChanged;

        public IEnumerator<T>   GetEnumerator() => Elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual void RaisePageChanged()         => PageChanged?.Invoke(this, new PageChangedEventArgs(CurrentPage, TotalPages, PageSize));
        protected virtual void RaiseTotalPagesChanged()   => TotalPagesChanged?.Invoke(this, new TotalPagesChangedEventArgs(CurrentPage, TotalPages));
        public virtual    void SetSource(IList<T> source) => Source = source;

        protected void Update()
        {
            if (Source is not {Count: > 0})
                return;
            TotalPages = (int) Math.Ceiling((decimal) Source.Count / PageSize);

            if (CurrentPage < 1)
                CurrentPage = 1;
            else if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;
        }

        public IEnumerable<T> GetPageElements(int page = -1)
        {
            if (Source is {Count: > 0})
            {
                if (page < 0)
                    page = CurrentPage;
                int startIndex = (page - 1) * PageSize;
                int endIndex = Math.Min(startIndex + PageSize, Source.Count - 1);
                for (int i = startIndex; i <= endIndex; i++)
                    yield return Source[i];
            }
        }
    }
}