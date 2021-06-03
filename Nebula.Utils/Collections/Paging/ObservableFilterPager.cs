using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Nebula.Utils.Collections.Paging
{
    public sealed class ObservableFilterPager<T> : FilterPager<T>
    {
        public ObservableFilterPager(IList<T> list, int currentPage = 1, int pageSize = 20) : base(list, null, currentPage, pageSize)
        {
            BindSources(list);
            UpdateObservablePage();
        }

        public ObservableCollection<T> ObservablePage { get; } = new();

        public override void SetSource(IList<T> source)
        {
            BindSources(source);
            base.SetSource(source);
            UpdateObservablePage();
        }

        private void ObservableCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => UpdateObservablePage();

        private void BindSources(IList<T> source)
        {
            if (Source is ObservableCollection<T> oldObservableCollection)
                oldObservableCollection.CollectionChanged -= ObservableCollectionOnCollectionChanged;
            if (source is ObservableCollection<T> observableCollection)
                observableCollection.CollectionChanged += ObservableCollectionOnCollectionChanged;
        }

        private void UpdateObservablePage()
        {
            Update();
            ObservablePage.Clear();
            foreach (T element in GetPageElements())
                ObservablePage.Add(element);
        }

        public override void ApplyFilter()
        {
            base.ApplyFilter();
            UpdateObservablePage();
        }

        public override void ResetFilter()
        {
            base.ResetFilter();
            UpdateObservablePage();
        }

        protected override void OnPageChanged()
        {
            base.OnPageChanged();
            UpdateObservablePage();
        }
    }
}