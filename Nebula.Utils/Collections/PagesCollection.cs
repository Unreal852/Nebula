using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Nebula.Utils.Collections.Events;


namespace Nebula.Utils.Collections
{
    public class PagesCollection<T> : IEnumerable<T>
    {
        private int _maxElementsPerPage;

        public PagesCollection(ICollection<T> elements = null, int maxElementsPerPage = 10)
        {
            Elements = elements is {Count: > 0} ? new ObservableCollection<T>(elements) : new ObservableCollection<T>();
            Elements.CollectionChanged += OnElementsCollectionChanged;
            MaxElementsPerPage = maxElementsPerPage;
            Refresh();
        }

        public int                     TotalPages                          { get; private set; }
        public int                     CurrentPage                         { get; private set; }
        public bool                    AutoRefresh                         { get; set; } = true;
        public bool                    InfinitePagesCycle                  { get; set; } = true;
        public bool                    ReturnPageElementsInsteadOfElements { get; set; } = true;
        public ObservableCollection<T> Elements                            { get; private set; }
        public ObservableCollection<T> PageElements                        { get; } = new();
        public int                     Count                               => Elements.Count;
        public int                     CurrentPageCount                    => PageElements.Count;

        public int MaxElementsPerPage
        {
            get => _maxElementsPerPage;
            set
            {
                _maxElementsPerPage = value;
                Refresh();
            }
        }

        public event EventHandler<PageChangedEventArgs> PageChanged;

        public T this[int index] => Elements[index];
        public void             Add(T element)               => Elements.Add(element);
        public void             Remove(T element)            => Elements.Remove(element);
        public void             Insert(T element, int index) => Elements.Insert(index, element);
        public void             Clear()                      => Elements.Clear();
        public IEnumerator<T>   GetEnumerator()              => ReturnPageElementsInsteadOfElements ? PageElements.GetEnumerator() : Elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()              => GetEnumerator();

        private void Refresh()
        {
            CalculateTotalPages();
            RefreshPage();
        }

        private void RefreshPage()
        {
            PageElements.Clear();
            foreach (T element in GetElementsFromPage())
                PageElements.Add(element);
        }

        private void CalculateTotalPages()
        {
            TotalPages = (int) Math.Ceiling((double) Elements.Count / MaxElementsPerPage);
            if (TotalPages == 0)
                TotalPages = 1;
        }

        public void SetPage(int page)
        {
            int oldPage = CurrentPage;
            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages - 1;
            else if (CurrentPage < 1)
                CurrentPage = 0;
            if (oldPage != CurrentPage)
            {
                RefreshPage();
                PageChanged?.Invoke(this, new PageChangedEventArgs(CurrentPage, TotalPages, MaxElementsPerPage));
            }
        }

        public void NextPage()
        {
            if (Elements.Count == 0)
                return;
            if (CurrentPage == TotalPages - 1)
            {
                if (!InfinitePagesCycle)
                    return;
                SetPage(0);
            }
            else
                SetPage(CurrentPage++);
        }

        public void PreviousPage()
        {
            if (Elements.Count == 0)
                return;
            if (CurrentPage == 0)
            {
                if (!InfinitePagesCycle)
                    return;
                SetPage(TotalPages - 1);
            }
            else
                SetPage(CurrentPage--);
        }

        public IEnumerable<T> GetElementsFromPage(int pageIndex = -1)
        {
            if (pageIndex < 0)
                pageIndex = CurrentPage;
            int startIndex = pageIndex * MaxElementsPerPage;
            int dif = Elements.Count - startIndex;
            int endIndex = startIndex + (dif < MaxElementsPerPage ? dif : MaxElementsPerPage);
            for (int i = startIndex; i != endIndex; i++)
                yield return Elements[i];
        }

        public void AddRange(params T[] elements)
        {
            foreach (T element in elements)
                Elements.Add(element);
        }

        public void RemoveRange(params T[] elements)
        {
            foreach (T element in elements)
                Elements.Remove(element);
        }

        public void SetElements(ICollection<T> collection)
        {
            Elements.CollectionChanged -= OnElementsCollectionChanged;
            Elements = new ObservableCollection<T>(collection);
            Elements.CollectionChanged += OnElementsCollectionChanged;
            if (AutoRefresh)
                Refresh();
        }

        private void OnElementsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (AutoRefresh)
                Refresh();
        }
    }
}