using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Nebula.Utils.Collections.Paging
{
    public sealed class ObservableFilterPager<T> : FilterPager<T>
    {
        public ObservableFilterPager(IList<T> list, int currentPage = 1, int pageSize = 20) : base(list, currentPage, pageSize)
        {
            BindSource(list);
            UpdateObservablePage();
        }

        public ObservableCollection<T> ObservablePage { get; } = new();

        public override void SetSource(IList<T> source)
        {
            BindSource(source);
            base.SetSource(source);
            UpdateObservablePage();
        }

        private void ObservableCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add when e.NewItems != null:
                {
                    foreach (T newItem in e.NewItems)
                        ObservablePage.Add(newItem);
                    break;
                }
                case NotifyCollectionChangedAction.Remove when e.OldItems != null:
                {
                    foreach (T oldItem in e.OldItems)
                        ObservablePage.Remove(oldItem);
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems != null)
                    {
                        foreach (T oldItem in e.OldItems)
                            ObservablePage.Remove(oldItem);
                    }

                    if (e.NewItems != null)
                    {
                        foreach (T oldItem in e.NewItems)
                            ObservablePage.Add(oldItem);
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                {
                    ObservablePage.Clear();
                    foreach (T element in FilteredPager.Source != null ? FilteredPager.GetPageElements() : GetPageElements())
                        ObservablePage.Add(element);
                    break;
                }
            }
        }

        private void BindSource(IList<T> source)
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
            foreach (T element in FilteredPager.Source != null ? FilteredPager.GetPageElements() : GetPageElements())
                ObservablePage.Add(element);
        }


        public override void RaiseFilterChanged()
        {
            base.RaiseFilterChanged();
            UpdateObservablePage();
        }

        protected override void RaisePageChanged()
        {
            base.RaisePageChanged();
            UpdateObservablePage();
        }
    }
}