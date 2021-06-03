using System;
using System.Collections.Generic;

// ReSharper disable PossibleUnintendedReferenceComparison

namespace Nebula.Utils.Collections.Paging
{
    public class FilterPager<T> : Pager<T>
    {
        public FilterPager(IList<T> list, Predicate<T> filter = null, int currentPage = 1, int pageSize = 20) : base(list, currentPage, pageSize)
        {
            OriginalSource = list;
            Filter = filter;
        }

        public    Predicate<T> Filter           { get; set; }
        protected IList<T>     OriginalSource   { get; private set; }
        protected List<T>      FilteredElements { get; } = new();

        public void ApplyFilter(Predicate<T> filter)
        {
            Filter = filter;
            ApplyFilter();
        }

        public virtual void ApplyFilter()
        {
            if (Filter == null)
                return;
            ResetFilter();
            foreach (T element in Source)
            {
                if (Filter(element))
                    FilteredElements.Add(element);
            }

            if (Source != FilteredElements)
                OriginalSource = Source;
            SetSource(FilteredElements);
        }

        public virtual void ResetFilter()
        {
            if (OriginalSource == null)
                return;
            SetSource(OriginalSource);
            FilteredElements.Clear();
        }
    }
}