using System;
using System.Collections.Generic;

// ReSharper disable PossibleUnintendedReferenceComparison

namespace Nebula.Utils.Collections.Paging
{
    public class FilterPager<T> : Pager<T>
    {
        public FilterPager(IList<T> list, int currentPage = 1, int pageSize = 20) : base(list, currentPage, pageSize)
        {
        }

        public Pager<T> FilteredPager { get; } = new(null);

        public event EventHandler FilterChanged;

        public virtual void RaiseFilterChanged() => FilterChanged?.Invoke(this, new EventArgs());

        public void ApplyFilter(Predicate<T> filter)
        {
            if (filter == null)
                return;
            ApplyFilter(source =>
            {
                List<T> filteredElements = new List<T>();
                foreach (T element in source)
                {
                    if (filter(element))
                        filteredElements.Add(element);
                }

                return filteredElements;
            });
        }

        public void ApplyFilter(Func<IList<T>, IList<T>> filterFunc)
        {
            if (filterFunc == null)
                return;
            FilteredPager.SetSource(filterFunc(Source));
            RaiseFilterChanged();
        }

        public void ResetFilter()
        {
            if (FilteredPager.Source == null)
                return;
            FilteredPager.Source.Clear();
            FilteredPager.SetSource(null);
            RaiseFilterChanged();
        }
    }
}